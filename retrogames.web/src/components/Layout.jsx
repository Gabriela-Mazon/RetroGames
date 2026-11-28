import { useEffect, useRef, useState } from 'react'
import { NavLink, Outlet, useLocation, useNavigate } from 'react-router-dom'

import { obterMensagemErro } from '../api/erros'
import { useAutenticacao } from '../hooks/useAutenticacao'
import { useCarrinho } from '../hooks/useCarrinho'

function Layout() {
  const {
    usuario,
    carregando,
    estaAutenticado,
    ehAdministrador,
    sair,
  } = useAutenticacao()
  const { quantidade } = useCarrinho()

  const navegar = useNavigate()
  const localizacao = useLocation()
  const menuRef = useRef(null)
  const botaoMenuRef = useRef(null)
  const [saindo, setSaindo] = useState(false)
  const [erroSaida, setErroSaida] = useState('')

  useEffect(() => {
    if (menuRef.current?.classList.contains('show')) {
      menuRef.current.classList.remove('show')
      botaoMenuRef.current?.setAttribute('aria-expanded', 'false')
      botaoMenuRef.current?.classList.add('collapsed')
    }
  }, [localizacao.pathname, localizacao.search])

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
      <nav className="navbar navbar-expand-lg navbar-dark bg-dark" aria-label="Navegação principal">
        <div className="container">
          <NavLink className="navbar-brand" to="/">
            RetroGames
          </NavLink>

          <button
            ref={botaoMenuRef}
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

          <div ref={menuRef} className="collapse navbar-collapse" id="menuPrincipal">
            <div className="navbar-nav me-auto">
              <NavLink className="nav-link" to="/" end>
                Catálogo
              </NavLink>

              {!carregando && estaAutenticado && (
                <>
                  <NavLink className="nav-link" to="/carrinho">
                    Carrinho
                    <span
                      className="badge text-bg-light ms-1"
                      aria-label={`${quantidade} ${quantidade === 1 ? 'item' : 'itens'} no carrinho`}
                    >
                      {quantidade}
                    </span>
                  </NavLink>

                  <NavLink className="nav-link" to="/meus-pedidos">
                    Meus pedidos
                  </NavLink>
                </>
              )}

              {!carregando && ehAdministrador && (
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
                <div className="d-flex flex-column flex-lg-row align-items-stretch align-items-lg-center gap-2 gap-lg-3 mt-3 mt-lg-0">
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
                <div className="d-flex flex-column flex-lg-row align-items-stretch align-items-lg-center gap-2 mt-3 mt-lg-0">
                  <NavLink className="btn btn-outline-light" to="/cadastro">
                    Criar conta
                  </NavLink>
                  <NavLink className="btn btn-light" to="/login">
                    Entrar
                  </NavLink>
                </div>
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
