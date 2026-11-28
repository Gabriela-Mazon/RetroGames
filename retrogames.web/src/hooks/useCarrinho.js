import { createContext, useContext } from 'react'

export const CarrinhoContext = createContext(null)
export const useCarrinho = () => useContext(CarrinhoContext)
