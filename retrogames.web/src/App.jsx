
import { Routes, Route } from 'react-router-dom'

import Layout from './components/Layout'
import Catalogo from './pages/Catalogo/Catalogo'
import DetalheJogo from './pages/Catalogo/DetalheJogo'
import NaoEncontrada from './pages/NaoEncontrada'
import Login from './pages/Login/Login'
import RotaProtegida from './components/RotaProtegida'

function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        <Route index element={<Catalogo />} />
        <Route path="jogos/:id" element={<DetalheJogo />} />
        <Route path="login" element={<Login />} />
        <Route path="admin" element={<RotaProtegida somenteAdministrador />}>
  <Route
    path="jogos"
    element={<h1>Administração de jogos</h1>}
  />
  <Route
    path="jogos/novo"
    element={<h1>Novo jogo</h1>}
  />
  <Route
    path="jogos/:id/editar"
    element={<h1>Editar jogo</h1>}
  />
  <Route
    path="plataformas"
    element={<h1>Administração de plataformas</h1>}
  />
</Route>
        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  )
}

export default App
