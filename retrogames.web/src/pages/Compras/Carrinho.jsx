import { useRef, useState } from 'react'
import { pedidoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import { Link } from 'react-router-dom'
import { EstadoVazio, MensagemErro } from '../../components/MensagensEstado'
import { useCarrinho } from '../../hooks/useCarrinho'
import { formatarDataHora, formatarPreco } from '../../lib/formato'

const quantidades = Array.from({ length: 10 }, (_, indice) => indice + 1)

export default function Carrinho() {
  const { itens, quantidade, total, alterar, remover, limpar } = useCarrinho()
  const [enviando, setEnviando] = useState(false)
  const [erro, setErro] = useState('')
  const [recibo, setRecibo] = useState(null)
  const emCurso = useRef(false)

  async function finalizar() {
    if (emCurso.current || !itens.length) return
    emCurso.current = true
    setEnviando(true)
    setErro('')
    try {
      const pedido = await pedidoApi.finalizar(itens)
      setRecibo(pedido)
      limpar()
    } catch (falha) {
      const codigo = falha.response?.data?.responseCode
      setErro(codigo === 111 ? 'Estoque insuficiente. Confira as unidades disponíveis no detalhe do jogo e ajuste o carrinho.'
        : codigo === 110 ? 'Um jogo ficou indisponível. Confira os itens e remova o jogo antes de finalizar.'
        : obterMensagemErro(falha))
    } finally {
      emCurso.current = false
      setEnviando(false)
    }
  }

  if (recibo) return (
    <section className="card rg-receipt-card p-4 p-md-5 mx-auto recibo" role="status">
      <div className="d-flex justify-content-between align-items-center flex-wrap gap-2 mb-3">
        <h1 className="h2 mb-0">Compra registrada</h1>
        <span className="badge situacao-pedido rg-status-pendente">Pendente</span>
      </div>
      <p className="text-muted mb-2">
        Pedido #{recibo.id}{recibo.data ? ` · ${formatarDataHora(recibo.data)}` : ''}
      </p>
      <p className="fs-5 mb-4">
        Total do pedido <strong className="rg-price ms-1">{formatarPreco(recibo.total)}</strong>
      </p>
      <div className="d-flex gap-2 flex-wrap">
        <Link className="btn btn-primary" to="/meus-pedidos">Ver meus pedidos</Link>
        <Link className="btn btn-outline-secondary" to="/">Voltar ao catálogo</Link>
      </div>
    </section>
  )

  return (
    <section>
      <div className="d-flex justify-content-between align-items-center flex-wrap gap-2 mb-4">
        <h1 className="mb-0">Carrinho</h1>
        {itens.length > 0 && (
          <Link className="btn btn-outline-secondary" to="/">Continuar comprando</Link>
        )}
      </div>

      {itens.length === 0 ? (
        <EstadoVazio titulo="Seu carrinho está vazio">
          <p className="mb-3">Escolha seus jogos favoritos no catálogo para montar o pedido.</p>
          <Link className="btn btn-primary" to="/">Explorar catálogo</Link>
        </EstadoVazio>
      ) : (
        <div className="row g-4">
          <div className="col-12 col-lg-8">
            <ul className="list-group shadow-sm">
              {itens.map(item => (
                <li className="list-group-item p-3 p-md-4" key={item.idJogo}>
                  <div className="d-flex justify-content-between align-items-start gap-3 flex-wrap mb-3">
                    <div>
                      <Link className="h5 d-inline-block text-decoration-none mb-1" to={'/jogos/' + item.idJogo}>
                        {item.nome}
                      </Link>
                      <p className="text-muted small mb-0">{formatarPreco(item.preco)} por unidade</p>
                    </div>
                    <strong className="fs-5 rg-price">{formatarPreco(item.preco * item.quantidade)}</strong>
                  </div>
                  <div className="d-flex align-items-center gap-3 flex-wrap">
                    <label htmlFor={'qt-' + item.idJogo} className="fw-medium mb-0">Quantidade</label>
                    <select id={'qt-' + item.idJogo} className="form-select w-auto"
                      disabled={enviando} value={item.quantidade} onChange={e => alterar(item.idJogo, Number(e.target.value))}>
                      {quantidades.map(n => <option key={n} value={n}>{n}</option>)}
                    </select>
                    <button className="btn btn-outline-danger ms-auto" type="button"
                      disabled={enviando} aria-label={'Remover ' + item.nome} onClick={() => remover(item.idJogo)}>
                      Remover
                    </button>
                  </div>
                </li>
              ))}
            </ul>
          </div>
          <div className="col-12 col-lg-4">
            <aside className="card shadow-sm p-4">
              <h2 className="h4 mb-2">Resumo da compra</h2>
              <p className="text-muted mb-3">{quantidade} {quantidade === 1 ? 'unidade' : 'unidades'}</p>
              <div className="d-flex justify-content-between align-items-baseline border-top pt-3 mb-3">
                <span>Total estimado</span>
                <strong className="fs-4 rg-price">{formatarPreco(total)}</strong>
              </div>
              {erro && <MensagemErro mensagem={erro} />}
              <div className="d-grid gap-2">
                <button className="btn btn-primary" type="button" onClick={finalizar} disabled={enviando}>
                  {enviando ? 'Finalizando…' : 'Finalizar compra'}
                </button>
                <Link className="btn btn-outline-secondary" to="/">Continuar comprando</Link>
              </div>
            </aside>
          </div>
        </div>
      )}
    </section>
  )
}
