# Revisão da Sprint e Guia de Apresentação — Módulo de Compras

## 1. Resumo da Sprint (Planejado × Entregue)

- **Objetivo da sprint:** *"O cliente se cadastra, monta o carrinho, finaliza a compra e acompanha seus pedidos."*
- **Período:** 25/11/2026 a 29/11/2026
- **Versão:** Básica (`C01` a `C05` — grupo de 2 estudantes: `Gabriela-Mazon` e `LuthorW`)
- **Quadro de tarefas:** [GitHub Projects #3](https://github.com/users/Gabriela-Mazon/projects/3)

| Item Planejado | Situação | Observações |
| --- | --- | --- |
| **C01 — Cadastro de cliente** | **Concluído** | `POST /api/Autenticacao/cadastrar`, hash com `PasswordHasher`, perfil `Cliente` (`2`), sessão automática via cookie `HttpOnly` + `SameSite=Strict`, erro `106` (`409`) e tela `/cadastro`. |
| **C02 — Cliente logado** | **Concluído** | Menu exibe `Carrinho` (com contador) e `Meus pedidos` ao cliente autenticado, oculta `Administração` e preserva `?voltarPara=` nas rotas protegidas. |
| **C03 — Carrinho de compras** | **Concluído** | Adição pelo detalhe do jogo (bloqueada se esgotado), ajuste de quantidade (1–10), remoção, total em `R$` e persistência no `localStorage`. |
| **C04 — Finalização da compra** | **Concluído** | `POST /api/Pedido` autenticado (`401`/`4` sem sessão), preço lido do banco, total calculado no `Service`, gravação de pedido (`Pendente`), itens e baixa de estoque em `TransactionScope` (`110`/`400`, `111`/`409`). |
| **C05 — Meus pedidos** | **Concluído** | `GET /api/Pedido/meus` ordenado do mais recente ao mais antigo, isolado por cliente, exibindo situação, data formatada, itens com preço pago e total na tela `/meus-pedidos`. |
| **Entrega — Responsividade, heurísticas e especificação** | **Parcialmente concluído** | Responsividade (1366 px e 360 px), heurísticas de Nielsen, testes unitários/E2E e especificação atualizada estão prontos. **Pendências abaixo.** |

---

## 2. Itens Pendentes (e Motivos)

1. **Identidade visual própria (cores, tipografia, logotipo e imagens):**
   - **Motivo:** A dupla optou por manter o layout no **Bootstrap 5 padrão** nesta etapa e ainda não definiu a identidade visual final do projeto.
2. **Contribuição própria em commits da integrante `LuthorW`:**
   - **Motivo:** Até esta revisão (28/11/2026), todos os commits na branch `main` foram registrados pela conta `Gabriela-Mazon`. Caso a integrante `LuthorW` conclua a identidade visual antes da entrega/apresentação, seu commit entrará diretamente no histórico.
3. **Apresentação do projeto (01/12/2026):**
   - Aguardando a data da apresentação em aula.

---

## 3. Verificações Executadas (28/11/2026)

- **Testes unitários (`dotnet test RetroGames.sln --nologo`):** **35 aprovados, 0 falhas, 0 ignorados** (xUnit v3 + Moq, sem banco real).
- **Lint e build do front-end (`npm run lint` e `npm run build` em `retrogames.web`):** **aprovados sem erros**.
- **Testes E2E da API + MySQL (banco isolado de QA):**
  - `POST /api/Pedido` e `GET /api/Pedido/meus` sem sessão → HTTP `401`, `responseCode: 4`.
  - Cadastro inválido → HTTP `400`, `responseCode: 3` com erros por campo; cadastro válido → HTTP `201`, perfil `Cliente`, cookie `HttpOnly` + `SameSite=Strict`; e-mail repetido → HTTP `409`, `responseCode: 106`.
  - Cliente tentando rota administrativa (`POST /api/Plataforma`) → HTTP `403`, `responseCode: 5`.
  - Jogo inexistente (`POST /api/Pedido`) → HTTP `400`, `responseCode: 110`.
  - **Rollback da transação por estoque insuficiente:** pedido com 2 jogos onde o 2º não tem estoque suficiente retornou HTTP `409`, `responseCode: 111`, e a consulta direta ao MySQL comprovou que o estoque do 1º jogo permaneceu inalterado e nenhum registro foi inserido em `tb_pedido` ou `tb_item_pedido`.
  - **Compra válida + isolamento:** pedido gravado com situação `Pendente` (`1`), total calculado pelo preço do banco (`R$ 179,80` para 2× `R$ 89,90`), estoque reduzido em 2 unidades e outro cliente cadastrado não visualizou o pedido (`[]`).
- **Testes E2E no navegador (desktop `1366×768` e celular `360×800`):**
  - Fluxo completo: catálogo/filtros/limpar → cadastro → menu do cliente → adicionar ao carrinho → recarregamento da página preservando `localStorage` → ajuste de quantidade no carrinho → finalização → recibo → `/meus-pedidos` → logout → redirecionamento `/login?voltarPara=%2Fmeus-pedidos` → novo login retornando para `/meus-pedidos`.
  - **0 erros de console/página** e **0 overflow horizontal** em todas as telas testadas em `1366×768` e `360×800`.
- **Segurança do repositório:** `.env` e `appsettings.Development.json` não estão versionados (`git ls-files` conferido); apenas `.env.example` sem senhas está no Git.

---

## 4. Guia Rápido para a Apresentação (01/12/2026)

1. **Sprint (Planejado × Entregue):** Mostrar o quadro no GitHub Projects (`C01` a `C05` concluídos, `Entrega` em andamento devido à identidade visual pendente).
2. **Demonstração de compra no computador (`1366×768`) e celular (`360×800`, F12 → `Ctrl+Shift+M`):**
   - Cadastrar um cliente em `/cadastro` (mostrar menu mudando para `Carrinho 0` e `Meus pedidos`, sem `Administração`).
   - Adicionar um jogo ao carrinho em `/jogos/{id}`, atualizar quantidade em `/carrinho`, finalizar a compra, ver o recibo e conferir o pedido em `/meus-pedidos`.
   - Em `360 px`, abrir o menu hambúrguer e mostrar a navegação e o carrinho sem rolagem horizontal.
3. **5+ Heurísticas de Nielsen nas telas:**
   - **Visibilidade do status:** contador do carrinho no menu e `"Finalizando…"` desabilitado.
   - **Correspondência com o mundo real:** `"Esgotado"`, `"Pendente"`, preços em `R$` e datas `dd/mm/aaaa hh:mm`.
   - **Controle e liberdade:** botão `Limpar` nos filtros, `Remover` item no carrinho e `Voltar ao catálogo`.
   - **Prevenção de erros:** quantidade limitada de 1 a 10 e botão desabilitado para jogo esgotado.
   - **Reconhecer, diagnosticar e corrigir erros:** erros abaixo de cada campo no cadastro e mensagem clara de estoque insuficiente no carrinho sem apagar os itens.
4. **Regra de negócio e seu teste unitário:**
   - Explicar a **RN10 (Atomicidade e estoque na finalização da compra)** em `RetroGames.Services/PedidoService.cs` (`FinalizarAsync`): leitura dos jogos com `FOR UPDATE` em ordem de `IdJogo`, conferência de ativo/estoque, cálculo do total pelos preços do banco e `TransactionScope` envolvendo pedido, itens e baixa de estoque.
   - Mostrar o teste correspondente em `RetroGames.WebApi.UnitTests/Services/PedidoServiceTests.cs`: **`Finalizar_SegundoJogoSemEstoque_AbortaTransacaoDaPrimeiraBaixa`**, que simula os repositórios com Moq, verifica que o erro `BusinessErrors.ESTOQUE_INSUFICIENTE` (`111`) é lançado e comprova que o `TransactionStatus` da transação é `Aborted` sem chamar `Pedido.InserirAsync` nem `ItemPedido.InserirAsync`.
