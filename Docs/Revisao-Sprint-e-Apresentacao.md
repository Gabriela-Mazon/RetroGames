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
| **Entrega — Interface, identidade visual, documentação e revisão** | **Concluído** | Identidade visual **Arcade Neon Dark** aplicada (`#0F0A1F` / `#7C3AED` / `#22D3EE` / `#EC4899`, fontes `Chakra Petch` + `Inter`, favicon e logo SVG de joystick, banner `hero-arcade.png`), responsividade validada em `1366×768` e `360×800`, heurísticas de Nielsen aplicadas, testes unitários/E2E aprovados e especificação atualizada. |

---

## 2. Itens em Aberto Pós-Entrega

1. ~~**Contribuição própria em commits da integrante `LuthorW`**~~ — **resolvido em 28/11/2026:**
   - A integrante `LuthorW` registrou um commit de revisão da documentação (`README.md` e este documento), com o próprio nome no histórico da branch `main`.
2. **Apresentação do projeto (01/12/2026):**
   - Aguardando a data da apresentação em aula (roteiro na seção 4 abaixo).

---

## 3. Verificações Executadas (28/11/2026)

- **Testes unitários (`dotnet test RetroGames.sln --nologo`):** **35 aprovados, 0 falhas, 0 ignorados** (xUnit v3 + Moq, sem banco real).
- **Lint e build do front-end (`npm run lint` e `npm run build` em `retrogames.web`):** **aprovados sem erros**, gerando o bundle com `hero-arcade.png` e estilos do tema escuro.
- **Testes E2E da API + MySQL (banco isolado de QA):**
  - `POST /api/Pedido` e `GET /api/Pedido/meus` sem sessão → HTTP `401`, `responseCode: 4`.
  - Cadastro inválido → HTTP `400`, `responseCode: 3` com erros por campo; cadastro válido → HTTP `201`, perfil `Cliente`, cookie `HttpOnly` + `SameSite=Strict`; e-mail repetido → HTTP `409`, `responseCode: 106`.
  - Cliente tentando rota administrativa (`POST /api/Plataforma`) → HTTP `403`, `responseCode: 5`.
  - Jogo inexistente (`POST /api/Pedido`) → HTTP `400`, `responseCode: 110`.
  - **Rollback da transação por estoque insuficiente:** pedido com 2 jogos onde o 2º não tem estoque suficiente retornou HTTP `409`, `responseCode: 111`, e a consulta direta ao MySQL comprovou que o estoque do 1º jogo permaneceu inalterado e nenhum registro foi inserido em `tb_pedido` ou `tb_item_pedido`.
  - **Compra válida + isolamento:** pedido gravado com situação `Pendente` (`1`), total calculado pelo preço do banco (`R$ 179,80` para 2× `R$ 89,90`), estoque reduzido em 2 unidades e outro cliente cadastrado não visualizou o pedido (`[]`).
- **Testes E2E visuais e funcionais no navegador com a identidade Arcade Neon Dark (`1366×768` e `360×800`):**
  - **Fluxo público e de compras:** `/` (`hero-arcade.png` carregado com `1280×714`, fundo `#0F0A1F`) → `/jogos/1` → `/cadastro` → `/carrinho` → finalização → `/meus-pedidos`.
  - **Fluxo administrativo:** login de administrador → `/admin/jogos` e `/admin/plataformas` com tabelas, formulários, badges e botões legíveis em tema escuro.
  - **Celular (`360×800`):** menu hambúrguer abre/fecha automaticamente na navegação; todas as páginas públicas, de compras e administrativas renderizaram sem cortes e com **0 erros de console** e **0 overflow horizontal**.
- **Segurança do repositório:** `.env` e `appsettings.Development.json` não estão versionados (`git ls-files` conferido); apenas `.env.example` sem senhas está no Git.

---

## 4. Guia Rápido para a Apresentação (01/12/2026)

1. **Sprint (Planejado × Entregue):** Mostrar o quadro no GitHub Projects (`C01` a `C05` e `Entrega` concluídos na entrega de 29/11).
2. **Demonstração de compra no computador (`1366×768`) e celular (`360×800`, F12 → `Ctrl+Shift+M`):**
   - Mostrar o catálogo com a identidade visual **Arcade Neon Dark** (banner, paleta roxa com ciano/magenta, tipografia `Chakra Petch` + `Inter`).
   - Cadastrar um cliente em `/cadastro` (mostrar menu mudando para `Carrinho 0` e `Meus pedidos`, sem `Administração`).
   - Adicionar um jogo ao carrinho em `/jogos/{id}`, atualizar quantidade em `/carrinho`, finalizar a compra, ver o recibo e conferir o pedido em `/meus-pedidos`.
   - Em `360 px`, abrir o menu hambúrguer e mostrar a navegação e o carrinho sem rolagem horizontal.
3. **5+ Heurísticas de Nielsen nas telas:**
   - **Visibilidade do status:** contador do carrinho em destaque ciano no menu, estados `Carregando...` e `"Finalizando…"` desabilitado.
   - **Correspondência com o mundo real:** `"Esgotado"`, `"Pendente"`, preços em `R$` e datas `dd/mm/aaaa hh:mm`.
   - **Controle e liberdade:** botão `Limpar` nos filtros, `Remover` item no carrinho e `Voltar ao catálogo`.
   - **Prevenção de erros:** quantidade limitada de 1 a 10 e botão desabilitado para jogo esgotado.
   - **Reconhecer, diagnosticar e corrigir erros:** erros abaixo de cada campo no cadastro e mensagem clara de estoque insuficiente no carrinho sem apagar os itens.
4. **Regra de negócio e seu teste unitário:**
   - Explicar a **RN10 (Atomicidade e estoque na finalização da compra)** em `RetroGames.Services/PedidoService.cs` (`FinalizarAsync`): leitura dos jogos com `FOR UPDATE` em ordem de `IdJogo`, conferência de ativo/estoque, cálculo do total pelos preços do banco e `TransactionScope` envolvendo pedido, itens e baixa de estoque.
   - Mostrar o teste correspondente em `RetroGames.WebApi.UnitTests/Services/PedidoServiceTests.cs`: **`Finalizar_SegundoJogoSemEstoque_AbortaTransacaoDaPrimeiraBaixa`**, que simula os repositórios com Moq, verifica que o erro `BusinessErrors.ESTOQUE_INSUFICIENTE` (`111`) é lançado e comprova que o `TransactionStatus` da transação é `Aborted` sem chamar `Pedido.InserirAsync` nem `ItemPedido.InserirAsync`.
