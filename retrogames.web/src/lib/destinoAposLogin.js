export function destinoAposLogin(parametros) {
  const caminho = parametros.get('voltarPara')
  if (!caminho?.startsWith('/') || caminho.startsWith('//') || caminho.includes('\\')) {
    return '/'
  }
  return caminho
}
