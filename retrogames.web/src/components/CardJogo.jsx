
import { Link } from 'react-router-dom'
import { formatarPreco } from '../lib/formato'

function CardJogo({ jogo }) {
  const esgotado = jogo.estoque === 0

  return (
    <article className="card h-100 shadow-sm">
      <div className="card-body d-flex flex-column">
        <p className="text-muted small mb-2">
          {jogo.nomePlataforma}
        </p>

        <h2 className="card-title h5">
          {jogo.nome}
        </h2>

        <p className="card-text mb-2">
          <span className="text-muted">Ano:</span>{' '}
          {jogo.anoLancamento}
        </p>

        <p className="card-text mb-2">
          <span className="text-muted">Condição:</span>{' '}
          {jogo.descricaoCondicao}
        </p>

        <div className="mb-3">
          {jogo.generos.map((genero) => (
            <span
              key={genero}
              className="badge bg-secondary me-1 mb-1"
            >
              {genero}
            </span>
          ))}
        </div>

        <div className="mt-auto">
          <p className="fs-5 fw-bold mb-2">
            {formatarPreco(jogo.preco)}
          </p>

          {esgotado && (
            <p className="text-danger fw-semibold mb-2">
              Esgotado
            </p>
          )}

          <Link
            to={`/jogos/${jogo.id}`}
            className="btn btn-primary w-100"
          >
            Ver detalhes
          </Link>
        </div>
      </div>
    </article>
  )
}

export default CardJogo
