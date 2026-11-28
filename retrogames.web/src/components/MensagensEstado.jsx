export function Carregando({ texto = 'Carregando...' }) {
  return (
    <div className="rg-state-box my-3" role="status" aria-live="polite">
      <div
        className="spinner-border text-info"
        aria-hidden="true"
      ></div>

      <p className="mt-3 mb-0 fw-medium text-muted">{texto}</p>
    </div>
  )
}

export function MensagemErro({ mensagem, aoTentarNovamente }) {
  return (
    <div className="alert alert-danger rg-error-box d-flex flex-column flex-sm-row align-items-sm-center justify-content-between gap-3" role="alert">
      <div className="d-flex align-items-start gap-2">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" className="flex-shrink-0 mt-1" aria-hidden="true">
          <circle cx="12" cy="12" r="9" stroke="currentColor" strokeWidth="2" />
          <path d="M12 8v5M12 16.2h.01" stroke="currentColor" strokeWidth="2.2" strokeLinecap="round" />
        </svg>
        <p className="mb-0">{mensagem}</p>
      </div>

      {aoTentarNovamente && (
        <button
          type="button"
          className="btn btn-outline-danger flex-shrink-0"
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
    <div className="rg-state-box my-3">
      <div className="rg-state-icon" aria-hidden="true">
        <svg width="24" height="24" viewBox="0 0 24 24" fill="none">
          <rect x="4" y="15" width="16" height="5" rx="2" stroke="currentColor" strokeWidth="2" />
          <path d="M12 7v8" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
          <circle cx="12" cy="5.5" r="2.5" stroke="currentColor" strokeWidth="2" />
        </svg>
      </div>

      <h2 className="h4 mb-1">{titulo}</h2>

      {children && (
        <div className="text-muted mt-2">
          {children}
        </div>
      )}
    </div>
  )
}
