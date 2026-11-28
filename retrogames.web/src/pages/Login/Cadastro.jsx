import { useState } from 'react'
import { Link, useNavigate, useSearchParams } from 'react-router-dom'
import { obterErrosDosCampos, obterMensagemErro } from '../../api/erros'
import { useAutenticacao } from '../../hooks/useAutenticacao'
import { destinoAposLogin } from '../../lib/destinoAposLogin'

export default function Cadastro() {
  const { cadastrar } = useAutenticacao()
  const [parametros] = useSearchParams()
  const navegar = useNavigate()
  const [enviando, setEnviando] = useState(false)
  const [erro, setErro] = useState('')
  const [campos, setCampos] = useState({})

  async function enviar(evento) {
    evento.preventDefault()
    if (enviando) return
    const dados = new FormData(evento.currentTarget)
    setEnviando(true)
    setErro('')
    setCampos({})
    try {
      await cadastrar(dados.get('nome').trim(), dados.get('email').trim(), dados.get('senha'))
      navegar(destinoAposLogin(parametros), { replace: true })
    } catch (falha) {
      setErro(obterMensagemErro(falha))
      const erros = obterErrosDosCampos(falha)
      if (falha.response?.data?.responseCode === 106) erros.email = obterMensagemErro(falha)
      setCampos(erros)
    } finally {
      setEnviando(false)
    }
  }

  return (
    <section className="row justify-content-center">
      <div className="col-12 col-md-8 col-lg-5">
        <h1 className="mb-4">Criar conta</h1>
        <form onSubmit={enviar} className="card shadow-sm">
          <div className="card-body p-4">
            {erro && <div className="alert alert-danger" role="alert">{erro}</div>}
            {[
              ['nome', 'Nome', 'text', 'name', 150],
              ['email', 'E-mail', 'email', 'email', 255],
              ['senha', 'Senha', 'password', 'new-password', undefined],
            ].map(([nome, rotulo, tipo, completar, tamanho]) => (
              <div className="mb-3" key={nome}>
                <label htmlFor={nome} className="form-label">{rotulo}</label>
                <input id={nome} name={nome} type={tipo} className="form-control"
                  autoComplete={completar} maxLength={tamanho} required
                  aria-invalid={Boolean(campos[nome])}
                  aria-describedby={campos[nome] ? nome + '-erro' : undefined} />
                {campos[nome] && <div id={nome + '-erro'} className="text-danger">{campos[nome]}</div>}
              </div>
            ))}
            <button type="submit" disabled={enviando} className="btn btn-primary w-100">
              {enviando ? 'Criando conta…' : 'Criar conta'}
            </button>
            <p className="mt-3 mb-0 text-center">
              Já tem conta? <Link to={{ pathname: '/login', search: parametros.toString() }}>Entrar</Link>
            </p>
          </div>
        </form>
      </div>
    </section>
  )
}
