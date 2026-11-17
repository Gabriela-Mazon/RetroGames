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