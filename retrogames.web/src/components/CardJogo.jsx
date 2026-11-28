import { Link } from 'react-router-dom'
import { formatarPreco } from '../lib/formato'

function CardJogo({ jogo }) {
  const esgotado = jogo.estoque === 0

  return (
    <article className="card rg-game-card h-100">
      <div className="card-body d-flex flex-column p-4">
        <div className="d-flex justify-content-between align-items-center gap-2 mb-2 flex-wrap">
          <span className="rg-platform-pill">{jogo.nomePlataforma}</span>
          <span className="text-muted small fw-semibold">{jogo.anoLancamento}</span>
        </div>

        <h2 className="card-title h5 mb-2">
          {jogo.nome}
        </h2>

        <p className="card-text small text-muted mb-3">
          Condição: <strong className="text-body">{jogo.descricaoCondicao}</strong>
        </p>

        <div className="d-flex flex-wrap gap-1 mb-3">
          {jogo.generos.map((genero) => (
            <span
              key={genero}
              className="rg-genre-chip"
            >
              {genero}
            </span>
          ))}
        </div>

        <div className="mt-auto pt-3 border-top">
          <div className="d-flex justify-content-between align-items-center gap-2 mb-3 flex-wrap">
            <p className="fs-4 fw-bold mb-0 rg-price">
              {formatarPreco(jogo.preco)}
            </p>

            {esgotado ? (
              <span className="badge rg-badge-esgotado">
                Esgotado
              </span>
            ) : (
              <span className="badge rg-badge-estoque">
                Em estoque ({jogo.estoque})
              </span>
            )}
          </div>

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
