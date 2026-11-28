# Especificação Técnica — Módulo de Compras (Versão Básica C01 a C05)

Este documento complementa a especificação base [`Especificacao-Arquitetura-e-Stack.pdf`](./Especificacao-Arquitetura-e-Stack.pdf) com tudo o que foi construído no **Desafio: Módulo de Compras (Versão Básica — histórias C01 a C05)** pela dupla **Gabriela-Mazon** e **LuthorW**, mantendo integralmente a arquitetura em camadas, o envelope padrão e os requisitos de segurança definidos para o projeto.

---

## 1. Escopo implementado (Versão Básica)

| Código | Funcionalidade | Resumo |
| --- | --- | --- |
| **C01** | Cadastro de cliente | Visitante cria conta ativa com perfil `Cliente` (`co_perfil = 2`), senha em hash (`PasswordHasher`) e sessão iniciada imediatamente. |
| **C02** | Cliente logado | Barra de navegação exibe `Carrinho` (com contador de itens) e `Meus pedidos` para o cliente autenticado; o menu `Administração` fica visível apenas para `Administrador`. |
| **C03** | Carrinho | Adição a partir do detalhe do jogo (bloqueada para jogo esgotado ou inativo), ajuste de quantidade (1 a 10), remoção de itens, cálculo de subtotal/total e persistência no `localStorage` do navegador (separado por conta/visitante). |
| **C04** | Finalização da compra | Criação do pedido autenticado em `TransactionScope`, validando jogos ativos e estoque com `FOR UPDATE`, usando preço do banco, calculando o total no `Service`, gravando pedido (`Pendente`), itens e baixando estoque na mesma transação. |
| **C05** | Meus pedidos | Listagem exclusiva dos pedidos do cliente logado, ordenados do mais recente para o mais antigo, exibindo situação, data/hora formatada, total e itens com quantidade e preço pago na compra. |

> **Observação sobre identidade visual:** A estilização customizada (paleta própria, tipografia, logotipo e imagens) foi mantida **pendente** por decisão da dupla, utilizando o **Bootstrap 5 padrão** com melhorias funcionais de responsividade, usabilidade e acessibilidade.

---

## 2. Modelo de dados atualizado (`database/04_modulo_compras.sql`)

Além das 7 tabelas do módulo de Catálogo e Administração (`tb_condicao_jogo`, `tb_perfil`, `tb_plataforma`, `tb_genero`, `tb_jogo`, `tb_jogo_genero`, `tb_usuario`), o Módulo de Compras adiciona 3 tabelas seguindo estritamente a convenção `tb_`, `co_seq_`, `co_`, `ds_`, `dt_`, `qt_`, `vl_` e sem triggers, functions, `CHECK` ou `ON DELETE CASCADE`/`SET NULL`:

| Tabela | Tipo | Colunas | Restrições estruturais |
| --- | --- | --- | --- |
| `tb_situacao_pedido` | Domínio | `co_situacao_pedido`, `ds_situacao_pedido` | `PRIMARY KEY (co_situacao_pedido)`, `UNIQUE (ds_situacao_pedido)`. Dados fixos: `1 Pendente`, `2 Pago`, `3 Enviado`, `4 Cancelado`. |
| `tb_pedido` | Principal | `co_seq_pedido`, `dt_pedido`, `vl_total`, `co_usuario`, `co_situacao_pedido` | `PRIMARY KEY (co_seq_pedido)`, índice `(co_usuario, dt_pedido, co_seq_pedido)`, FKs para `tb_usuario` e `tb_situacao_pedido`. `dt_pedido` em UTC (`DATETIME(6)`). |
| `tb_item_pedido` | Associativa / Itens | `co_seq_item_pedido`, `qt_item`, `vl_unitario`, `co_pedido`, `co_jogo` | `PRIMARY KEY (co_seq_item_pedido)`, `UNIQUE (co_pedido, co_jogo)`, FKs para `tb_pedido` e `tb_jogo`. `vl_unitario` armazena o preço vigente no momento da compra. |

---

## 3. Contrato da API atualizado

### 3.1 Novos endpoints (C01 a C05)

| Método | Rota | Acesso | Descrição |
| --- | --- | --- | --- |
| `POST` | `/api/Autenticacao/cadastrar` | Público (`[AllowAnonymous]`) | Cadastra novo usuário ativo com perfil `Cliente` (`co_perfil = 2`) e inicia sessão por cookie. |
| `POST` | `/api/Pedido` | Usuário logado (`FallbackPolicy`) | Finaliza o carrinho, valida jogos e estoque, grava pedido + itens e baixa estoque em transação única. Retorna HTTP `201`. |
| `GET` | `/api/Pedido/meus` | Usuário logado (`FallbackPolicy`) | Lista os pedidos do usuário autenticado do mais recente ao mais antigo. Retorna HTTP `200`. |

### 3.2 Níveis de acesso completos da API

- **Público (`[AllowAnonymous]`):** `POST /api/Autenticacao/login`, `POST /api/Autenticacao/cadastrar`, `GET /api/Jogo`, `GET /api/Jogo/{id}`, `GET /api/Plataforma`, `GET /api/Plataforma/{id}`, `GET /api/Genero`, `GET /api/Genero/{id}`, `GET /api/CondicaoJogo` e OpenAPI/Swagger em desenvolvimento.
- **Usuário logado (`FallbackPolicy` padrão):** `POST /api/Autenticacao/logout`, `GET /api/Autenticacao/eu`, `POST /api/Pedido`, `GET /api/Pedido/meus`.
- **Administrador (`[Authorize(Policy = Politicas.Administrador)]`):** `POST`, `PUT` e `DELETE` de `/api/Plataforma` e `/api/Jogo`.

### 3.3 Novos códigos de erro em `BusinessErrors`

| Código · Status | Constante | Mensagem padrão |
| --- | --- | --- |
| `106 · 409 Conflict` | `EMAIL_JA_CADASTRADO` | Já existe uma conta com este e-mail. |
| `110 · 400 Bad Request` | `JOGO_INDISPONIVEL` | Um ou mais jogos não estão disponíveis para venda. |
| `111 · 409 Conflict` | `ESTOQUE_INSUFICIENTE` | Estoque insuficiente. |

### 3.4 Exemplos de requisição e resposta no envelope

#### Cadastro (`POST /api/Autenticacao/cadastrar`)
- **Requisição:** `{ "nome": "Maria Teste", "email": "maria@exemplo.com", "senha": "..." }`
- **HTTP 201 Created:**
```json
{
  "responseCode": 0,
  "data": { "id": 2, "nome": "Maria Teste", "email": "maria@exemplo.com", "perfil": "Cliente" },
  "errorMessage": null
}
```
- **HTTP 409 Conflict (e-mail duplicado):**
```json
{ "responseCode": 106, "errorMessage": "Já existe uma conta com este e-mail." }
```

#### Finalização do pedido (`POST /api/Pedido`)
- **Requisição:** `{ "itens": [{ "idJogo": 1, "quantidade": 2 }] }`
- **HTTP 201 Created:**
```json
{
  "responseCode": 0,
  "data": { "id": 1, "data": "2026-11-28T15:01:00Z", "total": 179.80, "idSituacaoPedido": 1 },
  "errorMessage": null
}
```
- **HTTP 409 Conflict (estoque insuficiente — transação revertida):**
```json
{ "responseCode": 111, "errorMessage": "Estoque insuficiente." }
```

---

## 4. Segurança (RS01 a RS06)

- **RS01:** Sessão mantida exclusivamente no cookie `retrogames.sessao` (`HttpOnly`, `SameSite=Strict`, expiração deslizante de 2 horas). O `localStorage` armazena apenas os itens do carrinho (dados não sensíveis de seleção de jogos), nunca tokens de sessão.
- **RS02:** Proteção contra CSRF garantida pelo cookie `SameSite=Strict` em requisições da mesma origem via proxy do Vite (`/api` → `http://localhost:5080`).
- **RS03:** `FallbackPolicy` exige usuário autenticado por padrão. Apenas `POST /api/Autenticacao/cadastrar` recebeu `[AllowAnonymous]` entre as rotas novas; `POST /api/Pedido` e `GET /api/Pedido/meus` dependem da política global e respondem `401` (`responseCode: 4`) sem sessão.
- **RS04:** Senhas gravadas exclusivamente como hash PBKDF2 gerado pelo `PasswordHasher<Usuario>`.
- **RS05:** Credenciais e connection string permanecem fora do Git (`.env` e `appsettings.Development.json` no `.gitignore`); apenas `.env.example` é versionado.
- **RS06:** Todas as consultas e comandos nos repositórios usam parâmetros SQL (`@nome`), inclusive listas dinâmicas de IDs em `ItemPedidoRepository.ListarPorPedidosAsync` (`@pedido0`, `@pedido1`, ...).

---

## 5. Regras de negócio do Módulo de Compras

| ID | Regra | Implementação |
| --- | --- | --- |
| **RN07** | Unicidade de e-mail no cadastro (`HTTP 409`, código `106`). Conferência prévia no `Service` + tratamento de `RegistroDuplicadoException` (`UNIQUE` em `tb_usuario.ds_email`). Usuário nasce ativo (`st_ativo = true`) e com perfil `Cliente` (`co_perfil = 2`). | `AutenticacaoService.CadastrarAsync` e `UsuarioRepository` |
| **RN08** | Limite de itens e quantidades no pedido (`HTTP 400`, código `3` na API; `ArgumentException` como guarda no `Service`): ao menos 1 item, jogos distintos e quantidade de `1` a `10` por jogo. | `InserirPedidoRequestValidator` e `PedidoService.FinalizarAsync` |
| **RN09** | Disponibilidade do jogo (`HTTP 400`, código `110`): jogo deve existir e estar ativo (`st_ativo = true`). | `PedidoService.FinalizarAsync` e `JogoRepository.BuscarParaCompraAsync` |
| **RN10** | Estoque suficiente e atomicidade (`HTTP 409`, código `111`): leitura com bloqueio (`FOR UPDATE`) em ordem determinística de `IdJogo`, preço unitário lido do banco, total calculado no `Service`, pedido criado com situação `Pendente` (`1`) e data UTC, gravação do pedido, itens e baixa de estoque em `TransactionScope` (`ReadCommitted`). Qualquer falha desfaz todas as gravações e baixas anteriores. | `PedidoService.FinalizarAsync`, `JogoRepository`, `PedidoRepository`, `ItemPedidoRepository` |
| **RN11** | Isolamento e fidelidade do histórico: `GET /api/Pedido/meus` filtra pelo `idUsuario` da sessão, ordena de forma decrescente por data/ID e preserva o `vl_unitario` pago no item mesmo se o jogo for alterado ou desativado posteriormente. | `PedidoService.ListarMeusAsync`, `PedidoRepository`, `ItemPedidoRepository`, `SituacaoPedidoRepository`, `JogoRepository` |

---

## 6. Rastreabilidade completa (Seção 7.1 atualizada)

| Funcionalidade | Endpoints | Regras | Classes principais | Testes |
| --- | --- | --- | --- | --- |
| **Catálogo público de jogos** | `GET /api/Jogo` | RN04 | `JogoController`, `JogoService`, `JogoRepository` | `JogoServiceTests` |
| **Detalhe de jogo** | `GET /api/Jogo/{id}` | RN04 | `JogoController`, `JogoService` | `JogoServiceTests` |
| **Manutenção de plataformas** | `GET`, `POST`, `PUT`, `DELETE /api/Plataforma` | RN01, RN02 | `PlataformaController`, `PlataformaService`, `PlataformaRepository`, validators de plataforma | `PlataformaServiceTests` |
| **Manutenção de jogos** | `POST`, `PUT`, `DELETE /api/Jogo` | RN03, RN05, RN06 | `JogoController`, `JogoService`, `JogoRepository`, `JogoGeneroRepository`, validators de jogo | `JogoServiceTests` |
| **Gêneros e condições de jogo** | `GET /api/Genero`, `GET /api/CondicaoJogo` | — | `GeneroController`, `GeneroService`, `GeneroRepository`, `CondicaoJogoController`, `CondicaoJogoService`, `CondicaoJogoRepository` | `GeneroServiceTests`, `CondicaoJogoServiceTests` |
| **Autenticação e autorização** | `/api/Autenticacao` (`login`, `logout`, `eu`) | RS01 a RS06 | `AutenticacaoController`, `AutenticacaoService`, `UsuarioRepository` | — |
| **C01 — Cadastro de cliente** | `POST /api/Autenticacao/cadastrar` | RN07, RS01 a RS06 | `AutenticacaoController`, `AutenticacaoService`, `UsuarioRepository`, `CadastrarUsuarioRequestValidator`, `Cadastro.jsx`, `AutenticacaoProvider.jsx` | `CadastroServiceTests` |
| **C02 — Cliente logado** | `/api/Autenticacao/eu` (sessão) | RS01, RS03 | `Layout.jsx`, `RotaProtegida.jsx`, `AutenticacaoProvider.jsx` | Verificação E2E no navegador |
| **C03 — Carrinho de compras** | — (estado local em `localStorage`) | — | `CarrinhoProvider.jsx`, `useCarrinho.js`, `DetalheJogo.jsx`, `Carrinho.jsx` | Verificação E2E no navegador |
| **C04 — Finalização da compra** | `POST /api/Pedido` | RN08, RN09, RN10, RS01 a RS06 | `PedidoController`, `PedidoService`, `PedidoRepository`, `ItemPedidoRepository`, `JogoRepository`, `InserirPedidoRequestValidator`, `Carrinho.jsx` | `PedidoServiceTests`, `InserirPedidoValidatorTests` |
| **C05 — Meus pedidos** | `GET /api/Pedido/meus` | RN11, RS01, RS03, RS06 | `PedidoController`, `PedidoService`, `PedidoMappers`, `PedidoRepository`, `ItemPedidoRepository`, `SituacaoPedidoRepository`, `MeusPedidos.jsx` | `MeusPedidosServiceTests` |
| **Todas** | — | Envelope (5.3) | `GlobalExceptionHandler`, `FluentValidationFilter` | Verificação E2E API |

---

## 7. Front-end, responsividade e heurísticas de Nielsen

### 7.1 Novos arquivos e atualizações no front-end (`retrogames.web/src/`)

- `lib/formato.js`: adicionada `formatarDataHora` para datas em `pt-BR`.
- `api/servicos.js`: adicionados `autenticacaoApi.cadastrar`, `pedidoApi.finalizar` e `pedidoApi.meus`.
- `contexts/CarrinhoProvider.jsx` e `hooks/useCarrinho.js`: estado global do carrinho por usuário/visitante persistido no `localStorage`.
- `components/Layout.jsx`: menu responsivo com `Catálogo`, `Carrinho` (com contador), `Meus pedidos` para clientes autenticados, `Administração` apenas para administradores, `Criar conta` / `Entrar` para visitantes e recolhimento automático ao trocar de rota no celular.
- `components/RotaProtegida.jsx`: redireciona visitantes para `/login?voltarPara=...` e exibe estado de carregamento consistente.
- `pages/Login/Cadastro.jsx`: cadastro de cliente com validação por campo (`is-invalid` + `invalid-feedback`) e retorno automático à rota de origem.
- `pages/Catalogo/Catalogo.jsx`: adicionado botão `Limpar` para resetar busca e filtros ativos.
- `pages/Catalogo/DetalheJogo.jsx`: botão `Adicionar ao carrinho` (desabilitado se esgotado/inativo ou no limite de 10 unidades) e feedback imediato com link para `/carrinho`.
- `pages/Compras/Carrinho.jsx`: ajuste de quantidades (1 a 10), remoção de item, resumo da compra, finalização e recibo com link para `/meus-pedidos`.
- `pages/Compras/MeusPedidos.jsx`: listagem de pedidos do cliente com situação em badge contextual, data formatada, itens com preço unitário pago e total.

### 7.2 Responsividade e acessibilidade

- Grid responsivo do Bootstrap 5 (`container`, `row`, `col-12 col-md-6 col-lg-4`) em listagens, formulários e carrinho.
- Menu recolhível em telas estreitas com botão hambúrguer (`navbar-toggler`) e tabelas administrativas encapsuladas em `table-responsive`.
- Testado no navegador em **1366×768** (desktop) e **360×800** (celular) com **0 px de overflow horizontal**.
- **Pendência registrada:** identidade visual própria (paleta de cores, tipografia, logotipo e imagens) ainda não definida pela dupla.

### 7.3 Exemplos das 10 heurísticas de Nielsen no RetroGames

| Heurística | Aplicação concreta nas telas do RetroGames |
| --- | --- |
| **1. Visibilidade do status do sistema** | Indicador `"Carregando..."` / `"Buscando seus pedidos..."`, botão `"Finalizando…"` desabilitado durante o envio, contador de itens ao lado de `Carrinho` no menu e feedback imediato ao adicionar item em `DetalheJogo.jsx`. |
| **2. Correspondência com o mundo real** | Rótulos claros em português (`"Esgotado"`, `"Pendente"`, `"Disponível — 5 unidade(s)"`), valores em `R$` (`formatarPreco`) e datas em `dd/mm/aaaa hh:mm` (`formatarDataHora`); sem códigos técnicos crus. |
| **3. Controle e liberdade do usuário** | Botão `Limpar` nos filtros do catálogo, `Remover` item no carrinho, `Voltar ao catálogo` no detalhe/recibo e `Cancelar` nos formulários administrativos. |
| **4. Consistência e padrões** | Ações principais sempre em `btn-primary`, ações destrutivas (`Remover`, `Excluir`, `Desativar`) em `btn-outline-danger` e layout consistente entre `Login` e `Cadastro`. |
| **5. Prevenção de erros** | Quantidade restrita de `1` a `10`, botão de compra bloqueado quando `estoque < 1` ou `!ativo`, limite de 10 unidades no botão de adição e `window.confirm` antes de excluir/desativar. |
| **6. Reconhecer em vez de lembrar** | Selectboxes populados com plataformas e gêneros no catálogo, resumo da compra no carrinho antes da confirmação e checkboxes de gêneros no formulário de jogos. |
| **7. Flexibilidade e eficiência** | Filtros do catálogo refletidos na URL (`?busca=...&idPlataforma=...&idGenero=...`), envio de busca com `Enter` e preservação de `?voltarPara=...` ao redirecionar para `/login` ou `/cadastro`. |
| **8. Estética e design minimalista** | Cartões do catálogo exibem apenas plataforma, ano, título, condição, gêneros, preço e botão `Ver detalhes`; resumo de compra focado em quantidade e total. |
| **9. Reconhecer, diagnosticar e corrigir erros** | Mensagens de erro de campo (`invalid-feedback`) logo abaixo de cada input no cadastro e alerta amigável no carrinho quando ocorre estoque insuficiente (`111`) ou jogo indisponível (`110`), mantendo os itens para ajuste. |
| **10. Ajuda e documentação** | Texto auxiliar no campo de senha em `Cadastro.jsx`, estados vazios com orientação e botão direto para o próximo passo (`"Explorar catálogo"` em carrinho e pedidos vazios). |
