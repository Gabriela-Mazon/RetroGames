import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { pedidoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado'
import { formatarDataHora, formatarPreco } from '../../lib/formato'

function classeSituacao(situacao) {
  switch ((situacao || '').toLowerCase()) {
    case 'pago':
      return 'rg-status-pago'
    case 'enviado':
      return 'rg-status-enviado'
    case 'cancelado':
      return 'rg-status-cancelado'
    default:
      return 'rg-status-pendente'
  }
}

export default function MeusPedidos() {
  const [tentativa, setTentativa] = useState(0)
  const [resultado, setResultado] = useState(null)

  useEffect(() => {
    let ativo = true
    pedidoApi.meus().then(pedidos => {
      if (ativo) setResultado({ tentativa, pedidos })
    }).catch(erro => {
      if (ativo) setResultado({ tentativa, erro: obterMensagemErro(erro) })
    })
    return () => { ativo = false }
  }, [tentativa])

  return (
    <section>
      <div className="d-flex justify-content-between align-items-center flex-wrap gap-2 mb-4">
        <h1 className="mb-0">Meus pedidos</h1>
        <Link className="btn btn-outline-secondary" to="/">Voltar ao catálogo</Link>
      </div>

      {resultado?.tentativa !== tentativa ? <Carregando texto="Buscando seus pedidos…" />
        : resultado.erro ? <MensagemErro mensagem={resultado.erro} aoTentarNovamente={() => setTentativa(n => n + 1)} />
        : resultado.pedidos.length === 0 ? (
          <EstadoVazio titulo="Você ainda não fez pedidos">
            <p className="mb-3">Quando você finalizar uma compra, seus pedidos aparecerão aqui.</p>
            <Link className="btn btn-primary" to="/">Explorar catálogo</Link>
          </EstadoVazio>
        ) : resultado.pedidos.map(pedido => (
          <article key={pedido.id} className="card rg-order-card p-3 p-md-4 mb-3">
            <header className="d-flex justify-content-between align-items-start flex-wrap gap-2 pb-2 border-bottom">
              <div>
                <h2 className="h4 mb-1">Pedido #{pedido.id}</h2>
                <time className="text-muted small" dateTime={pedido.data}>
                  {formatarDataHora(pedido.data)}
                </time>
              </div>
              <span className={`badge situacao-pedido ${classeSituacao(pedido.situacao)}`}>
                {pedido.situacao}
              </span>
            </header>
            <ul className="list-group list-group-flush my-3">
              {pedido.itens.map(item => (
                <li key={item.idJogo} className="list-group-item px-0 d-flex justify-content-between align-items-center gap-3 flex-wrap">
                  <span>{item.nomeJogo}</span>
                  <span className="text-muted">
                    {item.quantidade} × {formatarPreco(item.valorUnitario)}
                    <strong className="text-body ms-2">{formatarPreco(item.quantidade * item.valorUnitario)}</strong>
                  </span>
                </li>
              ))}
            </ul>
            <p className="mb-0 text-end fs-5">
              Total <strong className="ms-2 rg-price">{formatarPreco(pedido.total)}</strong>
            </p>
          </article>
        ))}
    </section>
  )
}
