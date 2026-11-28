import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'

import { useCarrinho } from '../../hooks/useCarrinho'
import { jogoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import { formatarPreco } from '../../lib/formato'

import {
  Carregando,
  MensagemErro,
} from '../../components/MensagensEstado'

function DetalheJogo() {
  const { id } = useParams()
  const { itens, adicionar } = useCarrinho()
  const [jogoAdicionado, setJogoAdicionado] = useState(null)
  const noCarrinho = itens.find(item => item.idJogo === Number(id))?.quantidade || 0

  const [resultado, setResultado] = useState(null)
  const [tentativa, setTentativa] = useState(0)

  const chaveConsulta = `${id}|${tentativa}`
  const carregando = resultado?.chave !== chaveConsulta

  // Buscar os dados do jogo pelo ID informado na URL.
  useEffect(() => {
    let ativo = true

    async function carregarJogo() {
      try {
        const dados = await jogoApi.buscarPorId(id)

        if (ativo) {
          setResultado({
            chave: chaveConsulta,
            jogo: dados,
            erro: '',
          })
        }
      } catch (falha) {
        if (ativo) {
          setResultado({
            chave: chaveConsulta,
            jogo: null,
            erro: obterMensagemErro(falha),
          })
        }
      }
    }

    carregarJogo()

    return () => {
      ativo = false
    }
  }, [id, chaveConsulta])

  const jogo = carregando ? null : resultado.jogo
  const erro = carregando ? '' : resultado.erro
  const indisponivel = jogo ? !jogo.ativo || jogo.estoque < 1 : false

  return (
    <section>
      <Link to="/" className="btn btn-outline-secondary mb-4">
        Voltar ao catálogo
      </Link>

      {carregando ? (
        <Carregando texto="Carregando detalhes do jogo..." />
      ) : erro ? (
        <MensagemErro
          mensagem={erro}
          aoTentarNovamente={() =>
            setTentativa((valor) => valor + 1)
          }
        />
      ) : (
        <div className="row g-4">
          <div className="col-12 col-lg-8">
            <div className="card shadow-sm h-100">
              <div className="card-body p-4">
                <div className="d-flex flex-wrap align-items-center gap-2 mb-3">
                  <span className="rg-platform-pill">{jogo.nomePlataforma}</span>
                  <span className="text-muted small fw-semibold">
                    Lançado em {jogo.anoLancamento}
                  </span>
                </div>

                <h1 className="mb-3">{jogo.nome}</h1>

                <p className="mb-3">
                  <strong>Condição:</strong>{' '}
                  {jogo.descricaoCondicao}
                </p>

                <div className="mb-4">
                  <strong>Gêneros:</strong>

                  <div className="d-flex flex-wrap gap-2 mt-2">
                    {jogo.generos.map((genero) => (
                      <span
                        key={genero.id}
                        className="rg-genre-chip"
                      >
                        {genero.nome}
                      </span>
                    ))}
                  </div>
                </div>

                {jogo.descricao?.trim() && (
                  <div className="border-top pt-3">
                    <h2 className="h5 mb-2">Descrição</h2>
                    <p className="mb-0">{jogo.descricao}</p>
                  </div>
                )}
              </div>
            </div>
          </div>

          <div className="col-12 col-lg-4">
            <aside className="card shadow-sm p-4">
              <p className="text-muted small mb-1">Valor unitário</p>
              <p className="fs-2 fw-bold mb-2 rg-price">
                {formatarPreco(jogo.preco)}
              </p>

              {jogo.estoque > 0 && jogo.ativo ? (
                <p className="mb-3">
                  <span className="badge rg-badge-estoque">
                    Disponível — {jogo.estoque} unidade(s)
                  </span>
                </p>
              ) : (
                <p className="mb-3">
                  <span className="badge rg-badge-esgotado">
                    Esgotado
                  </span>
                </p>
              )}

              <div className="d-grid gap-2">
                <button
                  type="button"
                  className="btn btn-primary"
                  disabled={indisponivel || noCarrinho >= 10}
                  onClick={() => { adicionar(jogo); setJogoAdicionado(jogo.id) }}
                >
                  {indisponivel
                    ? 'Jogo esgotado'
                    : noCarrinho >= 10
                      ? 'Limite de 10 por jogo'
                      : 'Adicionar ao carrinho'}
                </button>
                <Link className="btn btn-outline-secondary" to="/carrinho">
                  Ver carrinho{noCarrinho > 0 ? ` (${noCarrinho})` : ''}
                </Link>
              </div>

              <div role="status" className="mt-3">
                {jogoAdicionado === jogo.id && noCarrinho > 0 && (
                  <div className="alert alert-success py-2 px-3 mb-0 small">
                    Adicionado ao carrinho · {noCarrinho} unidade(s) de {jogo.nome}.
                  </div>
                )}
              </div>
            </aside>
          </div>
        </div>
      )}
    </section>
  )
}

export default DetalheJogo
