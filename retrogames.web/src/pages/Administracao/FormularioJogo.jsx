
import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'

import {
  jogoApi,
  plataformaApi,
  generoApi,
  condicaoJogoApi,
} from '../../api/servicos'

import {
  obterMensagemErro,
  obterErrosDosCampos,
} from '../../api/erros'

import {
  Carregando,
  MensagemErro,
} from '../../components/MensagensEstado'

const formularioVazio = {
  nome: '',
  descricao: '',
  anoLancamento: '',
  preco: '',
  estoque: '',
  idPlataforma: '',
  idCondicaoJogo: '',
  idsGeneros: [],
  ativo: true,
}

function FormularioJogo() {
  const { id } = useParams()
  const navegar = useNavigate()
  const ehEdicao = id !== undefined

  const [formulario, setFormulario] = useState(formularioVazio)
  const [resultado, setResultado] = useState(null)
  const [tentativa, setTentativa] = useState(0)

  const [salvando, setSalvando] = useState(false)
  const [erroFormulario, setErroFormulario] = useState('')
  const [errosCampos, setErrosCampos] = useState({})

  const chaveConsulta = `${id ?? 'novo'}|${tentativa}`
  const carregando = resultado?.chave !== chaveConsulta

  // Carregar os dados necessários para o formulário.
  useEffect(() => {
    let ativo = true

    async function carregarDados() {
      try {
        const [plataformas, generos, condicoes, jogo] =
          await Promise.all([
            plataformaApi.listar(),
            generoApi.listar(),
            condicaoJogoApi.listar(),
            ehEdicao
              ? jogoApi.buscarPorId(id)
              : Promise.resolve(null),
          ])

        if (ativo) {
          setResultado({
            chave: chaveConsulta,
            plataformas,
            generos,
            condicoes,
            erro: '',
          })

          setFormulario(
            jogo
              ? {
                  nome: jogo.nome,
                  descricao: jogo.descricao ?? '',
                  anoLancamento: String(jogo.anoLancamento),
                  preco: String(jogo.preco),
                  estoque: String(jogo.estoque),
                  idPlataforma: String(jogo.idPlataforma),
                  idCondicaoJogo: String(jogo.idCondicaoJogo),
                  idsGeneros: jogo.generos.map((genero) => genero.id),
                  ativo: jogo.ativo,
                }
              : { ...formularioVazio, idsGeneros: [] },
          )
        }
      } catch (falha) {
        if (ativo) {
          setResultado({
            chave: chaveConsulta,
            erro: obterMensagemErro(falha),
          })
        }
      }
    }

    carregarDados()

    return () => {
      ativo = false
    }
  }, [id, ehEdicao, chaveConsulta])

  // Alterar campos simples.
  function alterarCampo(campo, valor) {
    setFormulario((atual) => ({
      ...atual,
      [campo]: valor,
    }))

    setErrosCampos((atual) => ({
      ...atual,
      [campo]: undefined,
    }))
  }

  // Marcar ou desmarcar um gênero.
  function alterarGenero(idGenero, marcado) {
    setFormulario((atual) => ({
      ...atual,
      idsGeneros: marcado
        ? [...atual.idsGeneros, idGenero]
        : atual.idsGeneros.filter((idAtual) => idAtual !== idGenero),
    }))

    setErrosCampos((atual) => ({
      ...atual,
      idsGeneros: undefined,
    }))
  }

  // Cadastrar ou atualizar um jogo.
  async function salvarJogo(evento) {
    evento.preventDefault()

    setSalvando(true)
    setErroFormulario('')
    setErrosCampos({})

    const dados = {
      nome: formulario.nome.trim(),
      descricao: formulario.descricao.trim() || null,
      anoLancamento: Number(formulario.anoLancamento),
      preco: Number(formulario.preco),
      estoque: Number(formulario.estoque),
      idPlataforma: Number(formulario.idPlataforma),
      idCondicaoJogo: Number(formulario.idCondicaoJogo),
      idsGeneros: formulario.idsGeneros,
    }

    try {
      if (ehEdicao) {
        await jogoApi.alterar(id, {
          ...dados,
          ativo: formulario.ativo,
        })
      } else {
        await jogoApi.inserir(dados)
      }

      navegar('/admin/jogos')
    } catch (falha) {
      setErroFormulario(obterMensagemErro(falha))
      setErrosCampos(obterErrosDosCampos(falha))
    } finally {
      setSalvando(false)
    }
  }

  const erroCarregamento = carregando ? '' : resultado.erro

  if (carregando) {
    return <Carregando texto="Carregando formulário..." />
  }

  if (erroCarregamento) {
    return (
      <MensagemErro
        mensagem={erroCarregamento}
        aoTentarNovamente={() =>
          setTentativa((valor) => valor + 1)
        }
      />
    )
  }

  const { plataformas, generos, condicoes } = resultado

  return (
    <section>
      <Link
        to="/admin/jogos"
        className="btn btn-outline-secondary mb-4"
      >
        Voltar para jogos
      </Link>

      <h1 className="mb-4">
        {ehEdicao ? 'Editar jogo' : 'Cadastrar jogo'}
      </h1>

      <div className="card shadow-sm">
        <div className="card-body p-4">
          {erroFormulario && (
            <div className="alert alert-danger" role="alert">
              {erroFormulario}
            </div>
          )}

          <form onSubmit={salvarJogo} noValidate>
            <div className="row g-3">
              <div className="col-12">
                <label htmlFor="nome" className="form-label">
                  Nome
                </label>
                <input
                  id="nome"
                  className={`form-control ${
                    errosCampos.nome ? 'is-invalid' : ''
                  }`}
                  value={formulario.nome}
                  onChange={(evento) =>
                    alterarCampo('nome', evento.target.value)
                  }
                />
                {errosCampos.nome && (
                  <div className="invalid-feedback">
                    {errosCampos.nome}
                  </div>
                )}
              </div>

              <div className="col-12">
                <label htmlFor="descricao" className="form-label">
                  Descrição
                </label>
                <textarea
                  id="descricao"
                  rows="3"
                  className={`form-control ${
                    errosCampos.descricao ? 'is-invalid' : ''
                  }`}
                  value={formulario.descricao}
                  onChange={(evento) =>
                    alterarCampo('descricao', evento.target.value)
                  }
                />
                {errosCampos.descricao && (
                  <div className="invalid-feedback">
                    {errosCampos.descricao}
                  </div>
                )}
              </div>

              <div className="col-12 col-md-4">
                <label htmlFor="anoLancamento" className="form-label">
                  Ano de lançamento
                </label>
                <input
                  id="anoLancamento"
                  type="number"
                  className={`form-control ${
                    errosCampos.anoLancamento ? 'is-invalid' : ''
                  }`}
                  value={formulario.anoLancamento}
                  onChange={(evento) =>
                    alterarCampo('anoLancamento', evento.target.value)
                  }
                />
                {errosCampos.anoLancamento && (
                  <div className="invalid-feedback">
                    {errosCampos.anoLancamento}
                  </div>
                )}
              </div>

              <div className="col-12 col-md-4">
                <label htmlFor="preco" className="form-label">
                  Preço (R$)
                </label>
                <input
                  id="preco"
                  type="number"
                  step="0.01"
                  className={`form-control ${
                    errosCampos.preco ? 'is-invalid' : ''
                  }`}
                  value={formulario.preco}
                  onChange={(evento) =>
                    alterarCampo('preco', evento.target.value)
                  }
                />
                {errosCampos.preco && (
                  <div className="invalid-feedback">
                    {errosCampos.preco}
                  </div>
                )}
              </div>

              <div className="col-12 col-md-4">
                <label htmlFor="estoque" className="form-label">
                  Estoque
                </label>
                <input
                  id="estoque"
                  type="number"
                  className={`form-control ${
                    errosCampos.estoque ? 'is-invalid' : ''
                  }`}
                  value={formulario.estoque}
                  onChange={(evento) =>
                    alterarCampo('estoque', evento.target.value)
                  }
                />
                {errosCampos.estoque && (
                  <div className="invalid-feedback">
                    {errosCampos.estoque}
                  </div>
                )}
              </div>

              <div className="col-12 col-md-6">
                <label htmlFor="idPlataforma" className="form-label">
                  Plataforma
                </label>
                <select
                  id="idPlataforma"
                  className={`form-select ${
                    errosCampos.idPlataforma ? 'is-invalid' : ''
                  }`}
                  value={formulario.idPlataforma}
                  onChange={(evento) =>
                    alterarCampo('idPlataforma', evento.target.value)
                  }
                >
                  <option value="">Selecione a plataforma</option>
                  {plataformas.map((plataforma) => (
                    <option key={plataforma.id} value={plataforma.id}>
                      {plataforma.nome}
                    </option>
                  ))}
                </select>
                {errosCampos.idPlataforma && (
                  <div className="invalid-feedback">
                    {errosCampos.idPlataforma}
                  </div>
                )}
              </div>

              <div className="col-12 col-md-6">
                <label htmlFor="idCondicaoJogo" className="form-label">
                  Condição do jogo
                </label>
                <select
                  id="idCondicaoJogo"
                  className={`form-select ${
                    errosCampos.idCondicaoJogo ? 'is-invalid' : ''
                  }`}
                  value={formulario.idCondicaoJogo}
                  onChange={(evento) =>
                    alterarCampo('idCondicaoJogo', evento.target.value)
                  }
                >
                  <option value="">Selecione a condição</option>
                  {condicoes.map((condicao) => (
                    <option key={condicao.id} value={condicao.id}>
                      {condicao.descricao}
                    </option>
                  ))}
                </select>
                {errosCampos.idCondicaoJogo && (
                  <div className="invalid-feedback">
                    {errosCampos.idCondicaoJogo}
                  </div>
                )}
              </div>

              <div className="col-12">
                <fieldset>
                  <legend className="fs-6 fw-semibold">
                    Gêneros
                  </legend>

                  <div
                    className={`d-flex flex-wrap gap-3 p-3 border rounded ${
                      errosCampos.idsGeneros ? 'border-danger' : ''
                    }`}
                  >
                    {generos.map((genero) => (
                      <div className="form-check" key={genero.id}>
                        <input
                          className="form-check-input"
                          type="checkbox"
                          id={`genero-${genero.id}`}
                          checked={formulario.idsGeneros.includes(genero.id)}
                          onChange={(evento) =>
                            alterarGenero(genero.id, evento.target.checked)
                          }
                        />
                        <label
                          className="form-check-label"
                          htmlFor={`genero-${genero.id}`}
                        >
                          {genero.nome}
                        </label>
                      </div>
                    ))}
                  </div>

                  {errosCampos.idsGeneros && (
                    <div className="text-danger small mt-2">
                      {errosCampos.idsGeneros}
                    </div>
                  )}
                </fieldset>
              </div>
            </div>

            <div className="d-flex gap-2 mt-4">
              <button
                type="submit"
                className="btn btn-primary"
                disabled={salvando}
              >
                {salvando
                  ? 'Salvando...'
                  : ehEdicao
                    ? 'Salvar alterações'
                    : 'Cadastrar jogo'}
              </button>

              <Link
                to="/admin/jogos"
                className="btn btn-outline-secondary"
              >
                Cancelar
              </Link>
            </div>
          </form>
        </div>
      </div>
    </section>
  )
}

export default FormularioJogo
