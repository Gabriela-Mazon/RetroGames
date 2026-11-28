
import { useEffect, useState } from 'react'
import { useSearchParams } from 'react-router-dom'

import { jogoApi, plataformaApi, generoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'

import CardJogo from '../../components/CardJogo'
import Paginacao from '../../components/Paginacao'

import {
  Carregando,
  MensagemErro,
  EstadoVazio,
} from '../../components/MensagensEstado'

function Catalogo() {
  const [parametros, setParametros] = useSearchParams()

  const [plataformas, setPlataformas] = useState([])
  const [generos, setGeneros] = useState([])
  const [erroFiltros, setErroFiltros] = useState('')

  const [resultado, setResultado] = useState(null)
  const [tentativa, setTentativa] = useState(0)

  const consulta = parametros.toString()
  const chaveConsulta = `${consulta}|${tentativa}`

  const busca = parametros.get('busca') || ''
  const idPlataforma = parametros.get('idPlataforma') || ''
  const idGenero = parametros.get('idGenero') || ''
  const temFiltrosAtivos = Boolean(busca || idPlataforma || idGenero)

  const carregando = resultado?.chave !== chaveConsulta

  // Carregar as opções de filtros ao abrir o catálogo.
  useEffect(() => {
    let ativo = true

    async function carregarFiltros() {
      try {
        const [listaPlataformas, listaGeneros] = await Promise.all([
          plataformaApi.listar(),
          generoApi.listar(),
        ])

        if (ativo) {
          setPlataformas(listaPlataformas)
          setGeneros(listaGeneros)
        }
      } catch (falha) {
        if (ativo) {
          setErroFiltros(obterMensagemErro(falha))
        }
      }
    }

    carregarFiltros()

    return () => {
      ativo = false
    }
  }, [])

  // Buscar os jogos sempre que a URL mudar.
  useEffect(() => {
    let ativo = true

    async function carregarJogos() {
      const parametrosConsulta = new URLSearchParams(consulta)

      const filtros = {
        pagina: Math.max(
          1,
          Number(parametrosConsulta.get('pagina')) || 1
        ),
        tamanhoPagina: 12,
      }

      if (parametrosConsulta.get('busca')) {
        filtros.busca = parametrosConsulta.get('busca')
      }

      if (parametrosConsulta.get('idPlataforma')) {
        filtros.idPlataforma = parametrosConsulta.get('idPlataforma')
      }

      if (parametrosConsulta.get('idGenero')) {
        filtros.idGenero = parametrosConsulta.get('idGenero')
      }

      try {
        const dados = await jogoApi.listar(filtros)

        if (ativo) {
          setResultado({
            chave: chaveConsulta,
            dados,
            erro: '',
          })
        }
      } catch (falha) {
        if (ativo) {
          setResultado({
            chave: chaveConsulta,
            dados: null,
            erro: obterMensagemErro(falha),
          })
        }
      }
    }

    carregarJogos()

    return () => {
      ativo = false
    }
  }, [consulta, chaveConsulta])

  function alterarFiltro(nome, valor) {
    const novos = new URLSearchParams(parametros)

    if (valor) {
      novos.set(nome, valor)
    } else {
      novos.delete(nome)
    }

    // Ao alterar um filtro, voltar para a primeira página.
    novos.delete('pagina')

    setParametros(novos)
  }

  function pesquisar(evento) {
    evento.preventDefault()

    const formulario = new FormData(evento.currentTarget)
    const texto = String(formulario.get('busca') || '').trim()

    alterarFiltro('busca', texto)
  }

  function limparFiltros() {
    setParametros(new URLSearchParams())
  }

  function mudarPagina(novaPagina) {
    const novos = new URLSearchParams(parametros)

    novos.set('pagina', String(novaPagina))

    setParametros(novos)
  }

  const dados = carregando ? null : resultado.dados
  const erroJogos = carregando ? '' : resultado.erro

  return (
    <section>
      <h1 className="mb-4">Catálogo de jogos</h1>

      <form className="row g-3 mb-4" onSubmit={pesquisar}>
        <div className="col-12 col-md-4">
          <label htmlFor="busca" className="form-label">
            Pesquisar jogo
          </label>

          <input
            key={busca}
            id="busca"
            name="busca"
            className="form-control"
            placeholder="Digite o nome do jogo"
            defaultValue={busca}
          />
        </div>

        <div className="col-12 col-md-3">
          <label htmlFor="plataforma" className="form-label">
            Plataforma
          </label>

          <select
            id="plataforma"
            className="form-select"
            value={idPlataforma}
            onChange={(evento) =>
              alterarFiltro('idPlataforma', evento.target.value)
            }
          >
            <option value="">Todas</option>

            {plataformas.map((plataforma) => (
              <option key={plataforma.id} value={plataforma.id}>
                {plataforma.nome}
              </option>
            ))}
          </select>
        </div>

        <div className="col-12 col-md-3">
          <label htmlFor="genero" className="form-label">
            Gênero
          </label>

          <select
            id="genero"
            className="form-select"
            value={idGenero}
            onChange={(evento) =>
              alterarFiltro('idGenero', evento.target.value)
            }
          >
            <option value="">Todos</option>

            {generos.map((genero) => (
              <option key={genero.id} value={genero.id}>
                {genero.nome}
              </option>
            ))}
          </select>
        </div>

        <div className="col-12 col-md-2 d-flex align-items-end gap-2">
          <button type="submit" className="btn btn-primary flex-grow-1">
            Buscar
          </button>
          {temFiltrosAtivos && (
            <button
              type="button"
              className="btn btn-outline-secondary"
              onClick={limparFiltros}
              aria-label="Limpar filtros"
              title="Limpar filtros"
            >
              Limpar
            </button>
          )}
        </div>
      </form>

      {erroFiltros && (
        <MensagemErro mensagem={erroFiltros} />
      )}

      {carregando ? (
        <Carregando texto="Carregando jogos..." />
      ) : erroJogos ? (
        <MensagemErro
          mensagem={erroJogos}
          aoTentarNovamente={() => setTentativa((valor) => valor + 1)}
        />
      ) : dados.itens.length === 0 ? (
        <EstadoVazio titulo="Nenhum jogo encontrado">
          <p className="mb-2">Tente alterar a pesquisa ou os filtros.</p>
          {temFiltrosAtivos && (
            <button type="button" className="btn btn-outline-secondary" onClick={limparFiltros}>
              Limpar filtros
            </button>
          )}
        </EstadoVazio>
      ) : (
        <>
          <p className="text-muted">
            {dados.totalItens} jogo(s) encontrado(s).
          </p>

          <div className="row g-4">
            {dados.itens.map((jogo) => (
              <div key={jogo.id} className="col-12 col-md-6 col-lg-4">
                <CardJogo jogo={jogo} />
              </div>
            ))}
          </div>

          <Paginacao
            pagina={dados.pagina}
            totalPaginas={dados.totalPaginas}
            aoMudar={mudarPagina}
          />
        </>
      )}
    </section>
  )
}

export default Catalogo
