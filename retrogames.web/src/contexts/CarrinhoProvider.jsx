import { useEffect, useState } from 'react'
import { Carregando } from '../components/MensagensEstado'
import { useAutenticacao } from '../hooks/useAutenticacao'
import { CarrinhoContext } from '../hooks/useCarrinho'

const chaveVisitante = 'retrogames.carrinho.visitante'

function lerItens(chave) {
  try {
    const itens = JSON.parse(localStorage.getItem(chave) || '[]')
    return Array.isArray(itens) ? itens.filter(item =>
      item && Number.isSafeInteger(item.idJogo) && item.idJogo > 0 &&
      typeof item.nome === 'string' && Number.isFinite(item.preco) && item.preco >= 0 &&
      Number.isInteger(item.quantidade) && item.quantidade >= 1 && item.quantidade <= 10
    ) : []
  } catch {
    return []
  }
}

function incluir(itens, novo) {
  if (!itens.some(item => item.idJogo === novo.idJogo)) return [...itens, novo]
  return itens.map(item => item.idJogo === novo.idJogo
    ? { ...novo, quantidade: Math.min(10, item.quantidade + novo.quantidade) }
    : item)
}

function CarrinhoDaConta({ chave, children }) {
  const [itens, setItens] = useState(() => {
    const salvos = lerItens(chave)
    if (chave === chaveVisitante) return salvos
    return lerItens(chaveVisitante).reduce(incluir, salvos)
  })

  useEffect(() => {
    try {
      localStorage.setItem(chave, JSON.stringify(itens))
      if (chave !== chaveVisitante) localStorage.removeItem(chaveVisitante)
    } catch {
      // O carrinho continua disponível nesta aba se não houver armazenamento.
    }
  }, [chave, itens])

  function adicionar(jogo) {
    if (!jogo.ativo || jogo.estoque <= 0) return
    const novo = { idJogo: jogo.id, nome: jogo.nome, preco: jogo.preco, quantidade: 1 }
    setItens(atuais => incluir(atuais, novo))
  }

  function alterar(idJogo, quantidade) {
    if (!Number.isInteger(quantidade) || quantidade < 1 || quantidade > 10) return
    setItens(atuais => atuais.map(item => item.idJogo === idJogo ? { ...item, quantidade } : item))
  }

  function remover(idJogo) {
    setItens(atuais => atuais.filter(item => item.idJogo !== idJogo))
  }

  const quantidade = itens.reduce((soma, item) => soma + item.quantidade, 0)
  const total = itens.reduce((centavos, item) =>
    centavos + Math.round(item.preco * 100) * item.quantidade, 0) / 100

  return (
    <CarrinhoContext.Provider value={{ itens, quantidade, total, adicionar, alterar, remover, limpar: () => setItens([]) }}>
      {children}
    </CarrinhoContext.Provider>
  )
}

export default function CarrinhoProvider({ children }) {
  const { usuario, carregando } = useAutenticacao()
  if (carregando) return <Carregando texto="Abrindo a loja…" />
  const chave = usuario ? 'retrogames.carrinho.' + usuario.id : chaveVisitante
  return <CarrinhoDaConta chave={chave} key={chave}>{children}</CarrinhoDaConta>
}
