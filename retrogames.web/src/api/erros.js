export function obterMensagemErro(erro) {
  // Quando não existe resposta do servidor.
  if (!erro.response) {
    return 'Não foi possível conectar ao servidor.'
  }

  // Utilizar a mensagem enviada pela API.
  return (
    erro.response.data?.errorMessage ||
    'Não foi possível concluir a operação.'
  )
}

export function obterErrosDosCampos(erro) {
  const resposta = erro.response?.data

  // Apenas erros de validação possuem código 3.
  if (resposta?.responseCode !== 3 || !resposta.data) {
    return {}
  }

  // Transformar as mensagens em um objeto por campo.
  return Object.fromEntries(
    Object.entries(resposta.data).map(([campo, mensagens]) => [
      campo,
      Array.isArray(mensagens)
        ? mensagens.join(' ')
        : String(mensagens),
    ]),
  )
}
