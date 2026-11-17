
import { Link } from 'react-router-dom'

function NaoEncontrada() {
  return (
    <section className="text-center py-5">
      <h1 className="display-4">404</h1>

      <h2 className="h4 mt-3">Página não encontrada</h2>

      <p className="text-muted">
        O endereço que você tentou acessar não existe.
      </p>

      <Link to="/" className="btn btn-primary mt-3">
        Voltar ao catálogo
      </Link>
    </section>
  )
}

export default NaoEncontrada
