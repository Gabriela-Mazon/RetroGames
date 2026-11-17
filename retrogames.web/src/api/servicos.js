
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
}

// Consultas de plataformas.
export const plataformaApi = {
  async listar() {
    const resposta = await api.get('/Plataforma')

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
