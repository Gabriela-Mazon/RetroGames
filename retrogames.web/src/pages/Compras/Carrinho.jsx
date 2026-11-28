import { useRef, useState } from 'react'
import { pedidoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import { Link } from 'react-router-dom'
import { EstadoVazio } from '../../components/MensagensEstado'
import { useCarrinho } from '../../hooks/useCarrinho'
import { formatarPreco } from '../../lib/formato'

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
    <section className="card p-4 mx-auto recibo" role="status">
      <h1>Compra registrada</h1>
      <p>Pedido #{recibo.id} · Pendente</p>
      <p>Total do pedido <strong>{formatarPreco(recibo.total)}</strong></p>
      <Link className="btn btn-primary align-self-start" to="/">Voltar ao catálogo</Link>
    </section>
  )

  return (
    <section>
      <h1 className="mb-4">Carrinho</h1>
      {itens.length === 0 ? (
        <EstadoVazio titulo="Seu carrinho está vazio">
          <Link className="btn btn-primary mt-2" to="/">Explorar catálogo</Link>
        </EstadoVazio>
      ) : (
        <div className="row g-4">
          <div className="col-12 col-lg-8">
            <ul className="list-group">
              {itens.map(item => (
                <li className="list-group-item p-3 p-md-4" key={item.idJogo}>
                  <div className="d-flex justify-content-between align-items-start gap-3 flex-wrap">
                    <div>
                      <Link className="h5 d-inline-block" to={'/jogos/' + item.idJogo}>{item.nome}</Link>
                      <p className="text-muted mb-2">{formatarPreco(item.preco)} por unidade</p>
                    </div>
                    <strong>{formatarPreco(item.preco * item.quantidade)}</strong>
                  </div>
                  <div className="d-flex align-items-center gap-3 flex-wrap">
                    <label htmlFor={'qt-' + item.idJogo}>Quantidade</label>
                    <select id={'qt-' + item.idJogo} className="form-select w-auto"
                      disabled={enviando} value={item.quantidade} onChange={e => alterar(item.idJogo, Number(e.target.value))}>
                      {quantidades.map(n => <option key={n}>{n}</option>)}
                    </select>
                    <button className="btn btn-outline-danger ms-auto" type="button"
                      disabled={enviando} aria-label={'Remover ' + item.nome} onClick={() => remover(item.idJogo)}>Remover</button>
                  </div>
                </li>
              ))}
            </ul>
          </div>
          <div className="col-12 col-lg-4">
            <aside className="card p-4">
              <h2 className="h4">Resumo da compra</h2>
              <p className="text-muted">{quantidade} {quantidade === 1 ? 'unidade' : 'unidades'}</p>
              <div className="d-flex justify-content-between border-top pt-3 mb-3">
                <span>Total estimado</span><strong>{formatarPreco(total)}</strong>
              </div>
              {erro && <p className="alert alert-danger" role="alert">{erro}</p>}
              <button className="btn btn-primary mb-2" type="button" onClick={finalizar} disabled={enviando}>
                {enviando ? 'Finalizando…' : 'Finalizar compra'}
              </button>
              <Link className="btn btn-outline-secondary" to="/">Continuar comprando</Link>
            </aside>
          </div>
        </div>
      )}
    </section>
  )
}
