
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
        <div className="card shadow-sm">
          <div className="card-body p-4">
            <p className="text-muted mb-2">
              {jogo.nomePlataforma}
            </p>

            <h1 className="mb-4">{jogo.nome}</h1>

            <p>
              <strong>Ano de lançamento:</strong>{' '}
              {jogo.anoLancamento}
            </p>

            <p>
              <strong>Condição:</strong>{' '}
              {jogo.descricaoCondicao}
            </p>

            <div className="mb-3">
              <strong>Gêneros:</strong>

              <div className="mt-2">
                {jogo.generos.map((genero) => (
                  <span
                    key={genero.id}
                    className="badge bg-secondary me-2 mb-2"
                  >
                    {genero.nome}
                  </span>
                ))}
              </div>
            </div>

            {jogo.descricao?.trim() && (
              <div className="mb-4">
                <h2 className="h5">Descrição</h2>
                <p>{jogo.descricao}</p>
              </div>
            )}

            <hr />

            <p className="fs-3 fw-bold mb-2">
              {formatarPreco(jogo.preco)}
            </p>

            {jogo.estoque > 0 ? (
              <p className="text-success fw-semibold">
                Disponível — {jogo.estoque} unidade(s)
              </p>
            ) : (
              <p className="text-danger fw-semibold">
                Esgotado
              </p>
            )}
            <div className="d-flex flex-wrap gap-2">
              <button type="button" className="btn btn-primary"
                disabled={!jogo.ativo || jogo.estoque < 1 || noCarrinho >= 10}
                onClick={() => { adicionar(jogo); setJogoAdicionado(jogo.id) }}>
                {noCarrinho >= 10 ? 'Limite de 10 por jogo' : 'Adicionar ao carrinho'}
              </button>
              <Link className="btn btn-outline-secondary" to="/carrinho">Ver carrinho</Link>
            </div>
            <p role="status" className="mt-2 mb-0">
              {jogoAdicionado === jogo.id && noCarrinho > 0 && `Adicionado ao carrinho · ${noCarrinho} de ${jogo.nome}`}
            </p>
          </div>
        </div>
      )}
    </section>
  )
}

export default DetalheJogo
