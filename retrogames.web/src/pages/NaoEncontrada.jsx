
import { Link } from 'react-router-dom'

function NaoEncontrada() {
  return (
    <section className="rg-state-box my-4 mx-auto" style={{ maxWidth: 560 }}>
      <h1 className="display-4 mb-2">404</h1>

      <h2 className="h4 mt-2">Página não encontrada</h2>

      <p className="text-muted mt-2">
        O endereço que você tentou acessar não existe.
      </p>

      <Link to="/" className="btn btn-primary mt-3">
        Voltar ao catálogo
      </Link>
    </section>
  )
}

export default NaoEncontrada
