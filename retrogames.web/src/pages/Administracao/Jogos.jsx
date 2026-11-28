
import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'

import { jogoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import { formatarPreco } from '../../lib/formato'

import Paginacao from '../../components/Paginacao'

import {
  Carregando,
  MensagemErro,
  EstadoVazio,
} from '../../components/MensagensEstado'

function Jogos() {
  const [buscaDigitada, setBuscaDigitada] = useState('')
  const [busca, setBusca] = useState('')
  const [pagina, setPagina] = useState(1)
  const [tentativa, setTentativa] = useState(0)
  const [resultado, setResultado] = useState(null)
  const [idDesativando, setIdDesativando] = useState(null)
const [erroDesativacao, setErroDesativacao] = useState('')

  const chaveConsulta = `${busca}|${pagina}|${tentativa}`
  const atualizando = resultado?.chave !== chaveConsulta
  const carregandoInicial = !resultado || (!resultado.dados && atualizando)

  // Consultar os jogos, incluindo os inativos.
  useEffect(() => {
    let ativo = true

    async function carregarJogos() {
      try {
        const dados = await jogoApi.listar({
          busca,
          pagina,
          tamanhoPagina: 10,
          incluirInativos: true,
        })

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
  }, [busca, pagina, chaveConsulta])

  // Pesquisar pelo nome do jogo.
  function pesquisar(evento) {
    evento.preventDefault()

    setPagina(1)
    setBusca(buscaDigitada.trim())
  }

  // Desativar um jogo após confirmação.
  async function desativarJogo(jogo) {
    const confirmado = window.confirm(
      `Deseja realmente desativar o jogo "${jogo.nome}"?`
    )

    if (!confirmado) {
      return
    }

    setErroDesativacao('')
    setIdDesativando(jogo.id)

    try {
      await jogoApi.desativar(jogo.id)

      // Atualizar a lista após a desativação.
      setTentativa((valor) => valor + 1)
    } catch (falha) {
      setErroDesativacao(obterMensagemErro(falha))
    } finally {
      setIdDesativando(null)
    }
  }

  const dados = resultado?.dados ?? null
  const erro = !atualizando ? (resultado?.erro || '') : ''

  return (
    <section>
      <div className="d-flex justify-content-between align-items-center flex-wrap gap-2 mb-4">
        <h1 className="mb-0">Administração de jogos</h1>

        <Link
          to="/admin/jogos/novo"
          className="btn btn-primary"
        >
          Novo jogo
        </Link>
      </div>

      <form className="rg-filter-card row g-3 mb-4" onSubmit={pesquisar}>
        <div className="col-12 col-md-8">
          <label htmlFor="buscaJogo" className="form-label">
            Pesquisar jogo
          </label>

          <input
            id="buscaJogo"
            type="text"
            className="form-control"
            placeholder="Digite o nome do jogo"
            value={buscaDigitada}
            onChange={(evento) =>
              setBuscaDigitada(evento.target.value)
            }
          />
        </div>

        <div className="col-12 col-md-4 d-flex align-items-end">
          <button type="submit" className="btn btn-primary w-100">
            Buscar
          </button>
        </div>
      </form>

{erroDesativacao && (
  <MensagemErro mensagem={erroDesativacao} />
)}
      {carregandoInicial ? (
        <Carregando texto="Carregando jogos..." />
      ) : erro ? (
        <MensagemErro
          mensagem={erro}
          aoTentarNovamente={() =>
            setTentativa((valor) => valor + 1)
          }
        />
      ) : dados.itens.length === 0 ? (
        <EstadoVazio titulo="Nenhum jogo encontrado">
          Tente alterar a pesquisa.
        </EstadoVazio>
      ) : (
        <div className={atualizando ? 'rg-atualizando' : ''} aria-busy={atualizando}>
          <p className="text-muted">
            {dados.totalItens} jogo(s) encontrado(s).
            {atualizando && (
              <span className="ms-2 small text-info" role="status">
                Atualizando…
              </span>
            )}
          </p>

          <div className="table-responsive">
            <table className="table table-striped align-middle">
              <thead>
                <tr>
                  <th>Nome</th>
                  <th>Plataforma</th>
                  <th>Ano</th>
                  <th>Preço</th>
                  <th>Estoque</th>
                  <th>Situação</th>
                  <th>Ações</th>
                </tr>
              </thead>

              <tbody>
                {dados.itens.map((jogo) => (
                  <tr key={jogo.id}>
                    <td>{jogo.nome}</td>
                    <td>{jogo.nomePlataforma}</td>
                    <td>{jogo.anoLancamento}</td>
                    <td>{formatarPreco(jogo.preco)}</td>
                    <td>{jogo.estoque}</td>

                    <td>
                      <span
                        className={`badge ${
                          jogo.ativo
                            ? 'rg-badge-estoque'
                            : 'rg-badge-esgotado'
                        }`}
                      >
                        {jogo.ativo ? 'Ativo' : 'Inativo'}
                      </span>
                    </td>

                    
<td>
  <div className="d-flex gap-2">
    <Link
      to={`/admin/jogos/${jogo.id}/editar`}
      className="btn btn-sm btn-outline-primary"
    >
      Editar
    </Link>

    {jogo.ativo && (
      <button
        type="button"
        className="btn btn-sm btn-outline-danger"
        onClick={() => desativarJogo(jogo)}
        disabled={idDesativando !== null}
      >
        {idDesativando === jogo.id
          ? 'Desativando...'
          : 'Desativar'}
      </button>
    )}
  </div>
</td>

                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <Paginacao
            pagina={dados.pagina}
            totalPaginas={dados.totalPaginas}
            aoMudar={setPagina}
          />
        </div>
      )}
    </section>
  )
}

export default Jogos
