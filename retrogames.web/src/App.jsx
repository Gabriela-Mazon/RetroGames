
import { Routes, Route } from 'react-router-dom'

import Layout from './components/Layout'
import Catalogo from './pages/Catalogo/Catalogo'
import DetalheJogo from './pages/Catalogo/DetalheJogo'
import NaoEncontrada from './pages/NaoEncontrada'
import Login from './pages/Login/Login'
import MeusPedidos from './pages/Compras/MeusPedidos'
import Carrinho from './pages/Compras/Carrinho'
import Cadastro from './pages/Login/Cadastro'
import RotaProtegida from './components/RotaProtegida'
import Plataforma from './pages/Auxiliares/Plataforma'
import Jogos from './pages/Administracao/Jogos'
import FormularioJogo from './pages/Administracao/FormularioJogo'

function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        <Route index element={<Catalogo />} />
        <Route path="jogos/:id" element={<DetalheJogo />} />
        <Route path="login" element={<Login />} />
        <Route path="cadastro" element={<Cadastro />} />
        <Route element={<RotaProtegida />}>
          <Route path="carrinho" element={<Carrinho />} />
          <Route path="meus-pedidos" element={<MeusPedidos />} />
        </Route>
        <Route path="admin" element={<RotaProtegida somenteAdministrador />}>
 <Route
  path="jogos"
  element={<Jogos />}
/>
  <Route
    path="jogos/novo"
    element={<FormularioJogo />}
  />
  <Route
    path="jogos/:id/editar"
    element={<FormularioJogo />}
  />
 <Route
  path="plataformas"
  element={<Plataforma />}
/>
</Route>
        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  )
}

export default App
