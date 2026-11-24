import { useEffect, useState } from 'react'

import { autenticacaoApi } from '../api/servicos'
import { AutenticacaoContext } from './autenticacaoContext'

function AutenticacaoProvider({ children: filhos }) {
  const [usuario, setUsuario] = useState(null)
  const [carregando, setCarregando] = useState(true)

  useEffect(() => {
    let ativo = true

    async function carregarSessao() {
      try {
        const usuarioAtual = await autenticacaoApi.usuarioAtual()

        if (ativo) {
          setUsuario(usuarioAtual)
        }
      } catch (falha) {
        if (ativo) {
          setUsuario(null)

          if (falha.response?.status !== 401) {
            console.error(
              'Não foi possível consultar a sessão:',
              falha.message,
            )
          }
        }
      } finally {
        if (ativo) {
          setCarregando(false)
        }
      }
    }

    function tratarSessaoExpirada() {
      setUsuario(null)
    }

    window.addEventListener(
      'sessao:expirada',
      tratarSessaoExpirada,
    )

    carregarSessao()

    return () => {
      ativo = false

      window.removeEventListener(
        'sessao:expirada',
        tratarSessaoExpirada,
      )
    }
  }, [])

  async function entrar(email, senha) {
    const usuarioAutenticado = await autenticacaoApi.entrar(
      email,
      senha,
    )

    setUsuario(usuarioAutenticado)

    return usuarioAutenticado
  }

  async function sair() {
    await autenticacaoApi.sair()

    setUsuario(null)
  }

  const valor = {
    usuario,
    carregando,
    estaAutenticado: Boolean(usuario),
    ehAdministrador: usuario?.perfil === 'Administrador',
    entrar,
    sair,
  }

  return (
    <AutenticacaoContext.Provider value={valor}>
      {filhos}
    </AutenticacaoContext.Provider>
  )
}

export default AutenticacaoProvider