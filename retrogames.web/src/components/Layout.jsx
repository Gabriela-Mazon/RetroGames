import { useEffect, useRef, useState } from 'react'
import { NavLink, Outlet, useLocation, useNavigate } from 'react-router-dom'

import { obterMensagemErro } from '../api/erros'
import { useAutenticacao } from '../hooks/useAutenticacao'
import { useCarrinho } from '../hooks/useCarrinho'

function IconeJoystick() {
  return (
    <svg width="26" height="26" viewBox="0 0 64 64" fill="none" aria-hidden="true">
      <rect width="64" height="64" rx="16" fill="#0F0A1F" />
      <rect x="3" y="3" width="58" height="58" rx="13" stroke="#22D3EE" strokeWidth="3" />
      <rect x="16" y="40" width="32" height="10" rx="4" fill="#8B5CF6" />
      <path d="M32 23v17" stroke="#F8FAFC" strokeWidth="4" strokeLinecap="round" />
      <circle cx="32" cy="19" r="7" fill="#EC4899" stroke="#22D3EE" strokeWidth="2.5" />
      <circle cx="41" cy="45" r="2.5" fill="#22D3EE" />
    </svg>
  )
}

function IconeCarrinho() {
  return (
    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" aria-hidden="true">
      <path
        d="M3 4h2l2.2 10.2a2 2 0 0 0 2 1.6h8.7a2 2 0 0 0 2-1.5L21 7H6.2"
        stroke="currentColor"
        strokeWidth="2"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <circle cx="10" cy="20" r="1.6" fill="currentColor" />
      <circle cx="18" cy="20" r="1.6" fill="currentColor" />
    </svg>
  )
}

function IconePedidos() {
  return (
    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" aria-hidden="true">
      <path
        d="M6 3h12a1 1 0 0 1 1 1v17l-3-2-2 2-2-2-2 2-2-2-3 2V4a1 1 0 0 1 1-1Z"
        stroke="currentColor"
        strokeWidth="2"
        strokeLinejoin="round"
      />
      <path d="M9 8h6M9 12h6M9 16h4" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
    </svg>
  )
}

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
      <header>
        <nav className="navbar navbar-expand-lg navbar-dark rg-navbar" aria-label="Navegação principal">
          <div className="container">
            <NavLink className="navbar-brand d-inline-flex align-items-center gap-2 fw-bold" to="/">
              <IconeJoystick />
              <span>RetroGames</span>
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
              <div className="navbar-nav me-auto gap-lg-1 mt-2 mt-lg-0">
                <NavLink className="nav-link" to="/" end>
                  Catálogo
                </NavLink>

                {!carregando && estaAutenticado && (
                  <>
                    <NavLink className="nav-link d-inline-flex align-items-center gap-2" to="/carrinho">
                      <IconeCarrinho />
                      <span>Carrinho</span>
                      <span
                        className="rg-cart-badge"
                        aria-label={`${quantidade} ${quantidade === 1 ? 'item' : 'itens'} no carrinho`}
                      >
                        {quantidade}
                      </span>
                    </NavLink>

                    <NavLink className="nav-link d-inline-flex align-items-center gap-2" to="/meus-pedidos">
                      <IconePedidos />
                      <span>Meus pedidos</span>
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
                    <span className="rg-user-greeting">
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
                    <NavLink className="btn rg-btn-accent" to="/login">
                      Entrar
                    </NavLink>
                  </div>
                )
              )}
            </div>
          </div>
        </nav>
      </header>

      <main className="container py-4 flex-grow-1">
        {erroSaida && (
          <div className="alert alert-danger" role="alert">
            {erroSaida}
          </div>
        )}

        <Outlet />
      </main>

      <footer className="rg-footer text-white py-4 mt-4">
        <div className="container d-flex flex-column flex-md-row justify-content-between align-items-center gap-2 text-center text-md-start">
          <div className="d-inline-flex align-items-center gap-2 fw-semibold">
            <IconeJoystick />
            <span>RetroGames — Loja de jogos retrô</span>
          </div>
          <small className="rg-footer-note">
            Clássicos preservados com compra segura e estoque em tempo real
          </small>
        </div>
      </footer>
    </div>
  )
}

export default Layout
