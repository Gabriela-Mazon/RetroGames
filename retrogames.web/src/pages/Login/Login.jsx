
import { useState } from 'react'
import { Link, useNavigate, useSearchParams } from 'react-router-dom'

import { destinoAposLogin } from '../../lib/destinoAposLogin'
import { obterMensagemErro } from '../../api/erros'
import { useAutenticacao } from '../../hooks/useAutenticacao'

function Login() {
  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [erro, setErro] = useState('')
  const [enviando, setEnviando] = useState(false)

  const { entrar } = useAutenticacao()
  const navegar = useNavigate()
  const [parametros] = useSearchParams()

  async function enviar(evento) {
    evento.preventDefault()
    setErro('')
    setEnviando(true)

    try {
      await entrar(email.trim(), senha)

      navegar(destinoAposLogin(parametros), { replace: true })
    } catch (falha) {
      setErro(obterMensagemErro(falha))
    } finally {
      setEnviando(false)
    }
  }

  return (
    <section className="row justify-content-center">
      <div className="col-12 col-md-6">
        <h1 className="mb-4">Entrar</h1>

        <form onSubmit={enviar} className="card shadow-sm">
          <div className="card-body p-4">
            {erro && (
              <div className="alert alert-danger" role="alert">
                {erro}
              </div>
            )}

            <div className="mb-3">
              <label htmlFor="email" className="form-label">
                E-mail
              </label>

              <input
                id="email"
                type="email"
                className="form-control"
                value={email}
                onChange={(evento) =>
                  setEmail(evento.target.value)
                }
                autoComplete="username"
                required
              />
            </div>

            <div className="mb-3">
              <label htmlFor="senha" className="form-label">
                Senha
              </label>

              <input
                id="senha"
                type="password"
                className="form-control"
                value={senha}
                onChange={(evento) =>
                  setSenha(evento.target.value)
                }
                autoComplete="current-password"
                required
              />
            </div>

            <button
              type="submit"
              className="btn btn-primary w-100"
              disabled={enviando}
            >
              {enviando ? 'Entrando...' : 'Entrar'}
            </button>
            <p className="mt-3 mb-0 text-center">
              Primeira compra? <Link to={{ pathname: '/cadastro', search: parametros.toString() }}>Criar conta</Link>
            </p>
          </div>
        </form>
      </div>
    </section>
  )
}

export default Login
