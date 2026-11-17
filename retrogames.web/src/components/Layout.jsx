
import { NavLink, Outlet } from 'react-router-dom'

function Layout() {
  return (
    <div className="d-flex flex-column min-vh-100">
      <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
        <div className="container">
          <NavLink className="navbar-brand" to="/">
            RetroGames
          </NavLink>

          <button
            className="navbar-toggler"
            type="button"
            data-bs-toggle="collapse"
            data-bs-target="#menuPrincipal"
            aria-controls="menuPrincipal"
            aria-expanded="false"
            aria-label="Abrir menu"
          >
            <span className="navbar-toggler-icon"></span>
          </button>

          <div className="collapse navbar-collapse" id="menuPrincipal">
            <div className="navbar-nav">
              <NavLink className="nav-link" to="/">
                Catálogo
              </NavLink>
            </div>
          </div>
        </div>
      </nav>

      <main className="container py-4 flex-grow-1">
        <Outlet />
      </main>

      <footer className="bg-dark text-white text-center py-3">
        <div className="container">
          RetroGames — Loja de jogos retrô
        </div>
      </footer>
    </div>
  )
}

export default Layout
