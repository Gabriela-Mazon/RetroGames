
export function Carregando({ texto = 'Carregando...' }) {
  return (
    <div className="text-center py-4" role="status">
      <div
        className="spinner-border text-primary"
        aria-hidden="true"
      ></div>

      <p className="mt-2">{texto}</p>
    </div>
  )
}

export function MensagemErro({ mensagem, aoTentarNovamente }) {
  return (
    <div className="alert alert-danger" role="alert">
      <p className="mb-2">{mensagem}</p>

      {aoTentarNovamente && (
        <button
          type="button"
          className="btn btn-outline-danger"
          onClick={aoTentarNovamente}
        >
          Tentar novamente
        </button>
      )}
    </div>
  )
}

export function EstadoVazio({ titulo, children }) {
  return (
    <div className="text-center py-5">
      <h2 className="h4">{titulo}</h2>

      {children && (
        <div className="text-muted mt-2">
          {children}
        </div>
      )}
    </div>
  )
}
