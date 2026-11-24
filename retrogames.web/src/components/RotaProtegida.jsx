
import { Navigate, Outlet, useLocation } from 'react-router-dom'

import { useAutenticacao } from '../hooks/useAutenticacao'

function RotaProtegida({ somenteAdministrador = false }) {
  const {
    carregando,
    estaAutenticado,
    ehAdministrador,
  } = useAutenticacao()

  const localizacao = useLocation()

  // Aguardar a consulta da sessão.
  if (carregando) {
    return <p>Verificando sessão...</p>
  }

  // Enviar visitantes ao login, preservando o destino.
  if (!estaAutenticado) {
    const voltarPara =
      localizacao.pathname + localizacao.search

    return (
      <Navigate
        to={`/login?voltarPara=${encodeURIComponent(voltarPara)}`}
        replace
      />
    )
  }

  // Impedir acesso de usuários sem perfil de administrador.
  if (somenteAdministrador && !ehAdministrador) {
    return <Navigate to="/" replace />
  }

  // Permitir acesso à página solicitada.
  return <Outlet />
}

export default RotaProtegida
