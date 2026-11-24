
import { useEffect, useState } from 'react'

import { plataformaApi } from '../../api/servicos'
import {
  obterMensagemErro,
  obterErrosDosCampos,
} from '../../api/erros'

import {
  Carregando,
  MensagemErro,
  EstadoVazio,
} from '../../components/MensagensEstado'

const formularioVazio = {
  nome: '',
  fabricante: '',
  anoLancamento: '',
}

function Plataforma() {
  const [resultado, setResultado] = useState(null)
  const [tentativa, setTentativa] = useState(0)

  const [formulario, setFormulario] = useState(formularioVazio)
  const [idEmEdicao, setIdEmEdicao] = useState(null)
  const [salvando, setSalvando] = useState(false)
  const [erroFormulario, setErroFormulario] = useState('')
  const [errosCampos, setErrosCampos] = useState({})
  const [excluindoId, setExcluindoId] = useState(null)
  const [erroExclusao, setErroExclusao] = useState('')

  const carregando = resultado?.chave !== tentativa

  // Carregar a lista de plataformas.
  useEffect(() => {
    let ativo = true

    async function carregarPlataformas() {
      try {
        const dados = await plataformaApi.listar()

        if (ativo) {
          setResultado({
            chave: tentativa,
            plataformas: dados,
            erro: '',
          })
        }
      } catch (falha) {
        if (ativo) {
          setResultado({
            chave: tentativa,
            plataformas: [],
            erro: obterMensagemErro(falha),
          })
        }
      }
    }

    carregarPlataformas()

    return () => {
      ativo = false
    }
  }, [tentativa])

  // Atualizar um campo do formulário.
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

  // Preencher o formulário para editar.
  function editarPlataforma(plataforma) {
    setIdEmEdicao(plataforma.id)

    setFormulario({
      nome: plataforma.nome,
      fabricante: plataforma.fabricante,
      anoLancamento: String(plataforma.anoLancamento),
    })

    setErroFormulario('')
    setErrosCampos({})
  }

  // Voltar ao formulário de cadastro.
  function limparFormulario() {
    setFormulario(formularioVazio)
    setIdEmEdicao(null)
    setErroFormulario('')
    setErrosCampos({})
  }

  // Inserir ou alterar uma plataforma.
  async function salvarPlataforma(evento) {
    evento.preventDefault()

    setSalvando(true)
    setErroFormulario('')
    setErrosCampos({})

    const dados = {
      nome: formulario.nome.trim(),
      fabricante: formulario.fabricante.trim(),
      anoLancamento: Number(formulario.anoLancamento),
    }

    try {
      if (idEmEdicao !== null) {
        await plataformaApi.alterar(idEmEdicao, dados)
      } else {
        await plataformaApi.inserir(dados)
      }

      limparFormulario()

      // Recarregar a lista após salvar.
      setTentativa((valor) => valor + 1)
    } catch (falha) {
      setErroFormulario(obterMensagemErro(falha))
      setErrosCampos(obterErrosDosCampos(falha))
    } finally {
      setSalvando(false)
    }
  }

  // Excluir uma plataforma após confirmação.
  async function excluirPlataforma(plataforma) {
    const confirmado = window.confirm(
      `Deseja realmente excluir a plataforma "${plataforma.nome}"?`
    )

    if (!confirmado) {
      return
    }

    setErroExclusao('')
    setExcluindoId(plataforma.id)

    try {
      await plataformaApi.excluir(plataforma.id)

      // Limpar a edição caso a plataforma excluída
      // esteja no formulário.
      if (idEmEdicao === plataforma.id) {
        limparFormulario()
      }

      // Atualizar a lista após excluir.
      setTentativa((valor) => valor + 1)
    } catch (falha) {
      setErroExclusao(obterMensagemErro(falha))
    } finally {
      setExcluindoId(null)
    }
  }

  const plataformas = carregando ? [] : resultado.plataformas
  const erro = carregando ? '' : resultado.erro

  return (
    <section>
      <h1 className="mb-4">Administração de plataformas</h1>

      <div className="card shadow-sm mb-4">
        <div className="card-body">
          <h2 className="h5 mb-3">
            {idEmEdicao !== null
              ? 'Editar plataforma'
              : 'Cadastrar plataforma'}
          </h2>

          {erroFormulario && (
            <div className="alert alert-danger" role="alert">
              {erroFormulario}
            </div>
          )}

          <form onSubmit={salvarPlataforma} noValidate>
            <div className="row g-3">
              <div className="col-12 col-md-4">
                <label htmlFor="nome" className="form-label">
                  Nome
                </label>

                <input
                  id="nome"
                  type="text"
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

              <div className="col-12 col-md-4">
                <label htmlFor="fabricante" className="form-label">
                  Fabricante
                </label>

                <input
                  id="fabricante"
                  type="text"
                  className={`form-control ${
                    errosCampos.fabricante ? 'is-invalid' : ''
                  }`}
                  value={formulario.fabricante}
                  onChange={(evento) =>
                    alterarCampo(
                      'fabricante',
                      evento.target.value,
                    )
                  }
                />

                {errosCampos.fabricante && (
                  <div className="invalid-feedback">
                    {errosCampos.fabricante}
                  </div>
                )}
              </div>

              <div className="col-12 col-md-4">
                <label
                  htmlFor="anoLancamento"
                  className="form-label"
                >
                  Ano de lançamento
                </label>

                <input
                  id="anoLancamento"
                  type="number"
                  className={`form-control ${
                    errosCampos.anoLancamento
                      ? 'is-invalid'
                      : ''
                  }`}
                  value={formulario.anoLancamento}
                  onChange={(evento) =>
                    alterarCampo(
                      'anoLancamento',
                      evento.target.value,
                    )
                  }
                />

                {errosCampos.anoLancamento && (
                  <div className="invalid-feedback">
                    {errosCampos.anoLancamento}
                  </div>
                )}
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
                  : idEmEdicao !== null
                    ? 'Salvar alterações'
                    : 'Cadastrar'}
              </button>

              {idEmEdicao !== null && (
                <button
                  type="button"
                  className="btn btn-outline-secondary"
                  onClick={limparFormulario}
                  disabled={salvando}
                >
                  Cancelar edição
                </button>
              )}
            </div>
          </form>
        </div>
      </div>

      <h2 className="h5 mb-3">Plataformas cadastradas</h2>
{erroExclusao && (
  <MensagemErro mensagem={erroExclusao} />
)}

      {carregando ? (
        <Carregando texto="Carregando plataformas..." />
      ) : erro ? (
        <MensagemErro
          mensagem={erro}
          aoTentarNovamente={() =>
            setTentativa((valor) => valor + 1)
          }
        />
      ) : plataformas.length === 0 ? (
        <EstadoVazio titulo="Nenhuma plataforma cadastrada">
          Cadastre uma plataforma para começar.
        </EstadoVazio>
      ) : (
        <div className="table-responsive">
          <table className="table table-striped align-middle">
            <thead>
              <tr>
                <th>Nome</th>
                <th>Fabricante</th>
                <th>Ano de lançamento</th>
                <th>Ações</th>
              </tr>
            </thead>

            <tbody>
              {plataformas.map((plataforma) => (
                <tr key={plataforma.id}>
                  <td>{plataforma.nome}</td>
                  <td>{plataforma.fabricante}</td>
                  <td>{plataforma.anoLancamento}</td>
            
<td>
  <div className="d-flex gap-2">
    <button
      type="button"
      className="btn btn-sm btn-outline-primary"
      onClick={() => editarPlataforma(plataforma)}
      disabled={salvando || excluindoId !== null}
    >
      Editar
    </button>

    <button
      type="button"
      className="btn btn-sm btn-outline-danger"
      onClick={() => excluirPlataforma(plataforma)}
      disabled={salvando || excluindoId !== null}
    >
      {excluindoId === plataforma.id
        ? 'Excluindo...'
        : 'Excluir'}
    </button>
  </div>
</td>

                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  )
}

export default Plataforma
