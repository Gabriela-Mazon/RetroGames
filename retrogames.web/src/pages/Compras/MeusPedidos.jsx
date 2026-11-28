import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { pedidoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado'
import { formatarDataHora, formatarPreco } from '../../lib/formato'

function classeSituacao(situacao) {
  switch ((situacao || '').toLowerCase()) {
    case 'pago':
      return 'bg-success'
    case 'enviado':
      return 'bg-info text-dark'
    case 'cancelado':
      return 'bg-danger'
    default:
      return 'bg-warning text-dark'
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
      <h1 className="mb-4">Meus pedidos</h1>
      {resultado?.tentativa !== tentativa ? <Carregando texto="Buscando seus pedidos…" />
        : resultado.erro ? <MensagemErro mensagem={resultado.erro} aoTentarNovamente={() => setTentativa(n => n + 1)} />
        : resultado.pedidos.length === 0 ? (
          <EstadoVazio titulo="Você ainda não fez pedidos">
            <Link className="btn btn-primary mt-2" to="/">Explorar catálogo</Link>
          </EstadoVazio>
        ) : resultado.pedidos.map(pedido => (
          <article key={pedido.id} className="card p-3 p-md-4 mb-3">
            <header className="d-flex justify-content-between align-items-start flex-wrap gap-2">
              <div><h2 className="h4">Pedido #{pedido.id}</h2>
                <time className="text-muted" dateTime={pedido.data}>{formatarDataHora(pedido.data)}</time>
              </div>
              <span className={`badge situacao-pedido ${classeSituacao(pedido.situacao)}`}>{pedido.situacao}</span>
            </header>
            <ul className="list-group list-group-flush my-3">
              {pedido.itens.map(item => (
                <li key={item.idJogo} className="list-group-item px-0 d-flex justify-content-between gap-3 flex-wrap">
                  <span>{item.nomeJogo}</span>
                  <span>
                    {item.quantidade} × {formatarPreco(item.valorUnitario)}{' '}
                    <strong className="ms-2">{formatarPreco(item.quantidade * item.valorUnitario)}</strong>
                  </span>
                </li>
              ))}
            </ul>
            <p className="mb-0 text-end">Total <strong className="ms-2">{formatarPreco(pedido.total)}</strong></p>
          </article>
        ))}
    </section>
  )
}
