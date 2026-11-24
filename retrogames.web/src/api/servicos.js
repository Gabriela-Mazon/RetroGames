
import api from './api'

// Consultas de jogos.

export const jogoApi = {
  async listar(filtros = {}) {
    const resposta = await api.get('/Jogo', {
      params: filtros,
    })

    return resposta.data.data
  },

  async buscarPorId(id) {
    const resposta = await api.get(`/Jogo/${id}`)

    return resposta.data.data
  },

  async inserir(jogo) {
    const resposta = await api.post('/Jogo', jogo)

    return resposta.data.data
  },

  async alterar(id, jogo) {
    const resposta = await api.put(`/Jogo/${id}`, jogo)

    return resposta.data.data
  },

  async desativar(id) {
    const resposta = await api.delete(`/Jogo/${id}`)

    return resposta.data.data
  },
}


// Consultas de plataformas.

export const plataformaApi = {
  async listar() {
    const resposta = await api.get('/Plataforma')

    return resposta.data.data
  },

  async inserir(plataforma) {
    const resposta = await api.post('/Plataforma', plataforma)

    return resposta.data.data
  },

  async alterar(id, plataforma) {
    const resposta = await api.put(`/Plataforma/${id}`, plataforma)

    return resposta.data.data
  },

  async excluir(id) {
    const resposta = await api.delete(`/Plataforma/${id}`)

    return resposta.data.data
  },
}


// Consultas de gêneros.
export const generoApi = {
  async listar() {
    const resposta = await api.get('/Genero')

    return resposta.data.data
  },
}

// Consultas das condições de conservação.
export const condicaoJogoApi = {
  async listar() {
    const resposta = await api.get('/CondicaoJogo')

    return resposta.data.data
  },
 
}
// Autenticação e sessão.
export const autenticacaoApi = {
  async entrar(email, senha) {
    const resposta = await api.post('/Autenticacao/login', {
      email,
      senha,
    })

    return resposta.data.data
  },

  async sair() {
    await api.post('/Autenticacao/logout')
  },

  async usuarioAtual() {
    const resposta = await api.get('/Autenticacao/eu')

    return resposta.data.data
  },
}
 