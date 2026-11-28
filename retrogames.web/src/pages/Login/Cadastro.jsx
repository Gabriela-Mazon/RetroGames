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

  function limparErroCampo(nome) {
    if (campos[nome]) {
      setCampos(atuais => ({ ...atuais, [nome]: undefined }))
    }
  }

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
        <form onSubmit={enviar} className="card rg-auth-card">
          <div className="card-body p-4">
            {erro && <div className="alert alert-danger" role="alert">{erro}</div>}
            {[
              ['nome', 'Nome', 'text', 'name', 150],
              ['email', 'E-mail', 'email', 'email', 255],
              ['senha', 'Senha', 'password', 'new-password', undefined],
            ].map(([nome, rotulo, tipo, completar, tamanho]) => {
              const idsDescricao = [
                campos[nome] ? `${nome}-erro` : null,
                nome === 'senha' ? 'senha-ajuda' : null,
              ].filter(Boolean).join(' ') || undefined

              return (
                <div className="mb-3" key={nome}>
                  <label htmlFor={nome} className="form-label">{rotulo}</label>
                  <input
                    id={nome}
                    name={nome}
                    type={tipo}
                    className={`form-control ${campos[nome] ? 'is-invalid' : ''}`}
                    autoComplete={completar}
                    maxLength={tamanho}
                    required
                    aria-invalid={Boolean(campos[nome])}
                    aria-describedby={idsDescricao}
                    onChange={() => limparErroCampo(nome)}
                  />
                  {nome === 'senha' && (
                    <div id="senha-ajuda" className="form-text">
                      Informe uma senha para acessar sua conta e acompanhar seus pedidos.
                    </div>
                  )}
                  {campos[nome] && <div id={nome + '-erro'} className="invalid-feedback">{campos[nome]}</div>}
                </div>
              )
            })}
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
