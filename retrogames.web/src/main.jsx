import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'

import 'bootstrap/dist/css/bootstrap.min.css'
import './index.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'

import App from './App.jsx'
import CarrinhoProvider from './contexts/CarrinhoProvider'
import AutenticacaoProvider from './contexts/AutenticacaoProvider.jsx'

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <BrowserRouter>
      <AutenticacaoProvider>
        <CarrinhoProvider><App /></CarrinhoProvider>
      </AutenticacaoProvider>
    </BrowserRouter>
  </StrictMode>,
)