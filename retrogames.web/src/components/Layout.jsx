
import { useState } from 'react'
import { NavLink, Outlet, useNavigate } from 'react-router-dom'

import { obterMensagemErro } from '../api/erros'
import { useAutenticacao } from '../hooks/useAutenticacao'

function Layout() {
  const {
    usuario,
    carregando,
    estaAutenticado,
    ehAdministrador,
    sair,
  } = useAutenticacao()

  const navegar = useNavigate()
  const [saindo, setSaindo] = useState(false)
  const [erroSaida, setErroSaida] = useState('')

  async function encerrarSessao() {
    setErroSaida('')
    setSaindo(true)

    try {
      await sair()
      navegar('/')
    } catch (falha) {
      setErroSaida(obterMensagemErro(falha))
    } finally {
      setSaindo(false)
    }
  }

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
            <div className="navbar-nav me-auto">
              <NavLink className="nav-link" to="/">
                Catálogo
              </NavLink>

              {ehAdministrador && (
                <div className="nav-item dropdown">
                  <button
                    type="button"
                    className="nav-link dropdown-toggle"
                    data-bs-toggle="dropdown"
                    aria-expanded="false"
                  >
                    Administração
                  </button>

                  <ul className="dropdown-menu dropdown-menu-dark">
                    <li>
                      <NavLink className="dropdown-item" to="/admin/jogos">
                        Jogos
                      </NavLink>
                    </li>
                    <li>
                      <NavLink className="dropdown-item" to="/admin/plataformas">
                        Plataformas
                      </NavLink>
                    </li>
                  </ul>
                </div>
              )}
            </div>

            {!carregando && (
              estaAutenticado ? (
                <div className="d-flex align-items-center gap-3">
                  <span className="text-white">
                    Olá, {usuario.nome}
                  </span>

                  <button
                    type="button"
                    className="btn btn-outline-light"
                    onClick={encerrarSessao}
                    disabled={saindo}
                  >
                    {saindo ? 'Saindo...' : 'Sair'}
                  </button>
                </div>
              ) : (
                <NavLink className="btn btn-outline-light" to="/login">
                  Entrar
                </NavLink>
              )
            )}
          </div>
        </div>
      </nav>

      <main className="container py-4 flex-grow-1">
        {erroSaida && (
          <div className="alert alert-danger" role="alert">
            {erroSaida}
          </div>
        )}

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
