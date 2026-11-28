# RetroGames

Projeto acadêmico de desenvolvimento web **RetroGames** — loja web de jogos retrô usados (Catálogo, Administração e Módulo de Compras `C01`–`C05`).

## Stack tecnológica

- **Back-end:** .NET 10 (ASP.NET Core Web API com Controllers), ADO.NET com `MySqlConnector`, `FluentValidation`, `PasswordHasher` (PBKDF2), `DotNetEnv`.
- **Banco de dados:** MySQL 8.
- **Testes unitários:** xUnit v3 + Moq.
- **Front-end:** React 19, Vite, Bootstrap 5, React Router 7, Axios.

## Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/)
- [Node.js 22 LTS](https://nodejs.org/)
- MySQL 8 em execução

## Configuração do banco e variáveis de ambiente

1. Crie o banco no MySQL (por exemplo, `retrogames` em uma instalação local) e execute os scripts da pasta `database/` em ordem:
   - `database/01_criar_tabelas.sql`
   - `database/02_dados_exemplo.sql`
   - `database/04_modulo_compras.sql`

   O script `database/03_consultas_e_testes.sql` é opcional: ele contém as consultas usadas para conferir os dados e não precisa ser executado para a aplicação funcionar.
2. Copie `.env.example` para `.env` na raiz do repositório e preencha `ConnectionStrings__Default`, `AdminInicial__Email` e `AdminInicial__Senha` (o `.env` está no `.gitignore` e nunca deve ser versionado).
   - O `.env.example` traz como modelo o servidor MySQL da instituição. Para usar um banco local, troque `server`, `database` e `user` na connection string (por exemplo, `server=localhost;port=3306;database=retrogames;user=root;password=SUA_SENHA`).

## Execução local

1. **API (.NET) — porta `5080`:**
   ```bash
   dotnet run --project RetroGames.WebApi
   ```
   Em desenvolvimento, o Swagger UI fica disponível em `http://localhost:5080/swagger`.

2. **Front-end (Vite + React) — porta `5173`:**
   ```bash
   cd retrogames.web
   npm install
   npm run dev
   ```
   Acesse `http://localhost:5173` (requisições `/api` são encaminhadas para `http://localhost:5080` via proxy do Vite).

## Testes e build

Na raiz do repositório:

```bash
dotnet test RetroGames.sln
```

No diretório `retrogames.web`:

```bash
npm run lint
npm run build
```

## Documentação

- [Especificação base (Arquitetura e Stack)](./Docs/Especificacao-Arquitetura-e-Stack.pdf)
- [Especificação complementar — Módulo de Compras (C01 a C05) e Rastreabilidade](./Docs/Especificacao-Modulo-Compras.md)
- [Revisão da Sprint e Guia de Apresentação](./Docs/Revisao-Sprint-e-Apresentacao.md)
- [Diretrizes do repositório](./AGENTS.md)

## Equipe

- Gabriela Mazon ([@Gabriela-Mazon](https://github.com/Gabriela-Mazon))
- LuthorW ([@LuthorW](https://github.com/LuthorW)) — revisão da documentação
