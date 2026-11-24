import axios from 'axios'

const api = axios.create({
  baseURL: '/api',
  withCredentials: true,
})

api.interceptors.response.use(
  (resposta) => resposta,

  (falha) => {
    const status = falha.response?.status
    const rota = falha.config?.url

    const consultaDeSessao = rota === '/Autenticacao/eu'
    const tentativaDeLogin = rota === '/Autenticacao/login'

    if (
      status === 401 &&
      !consultaDeSessao &&
      !tentativaDeLogin
    ) {
      window.dispatchEvent(new Event('sessao:expirada'))
    }

    return Promise.reject(falha)
  },
)

export default api