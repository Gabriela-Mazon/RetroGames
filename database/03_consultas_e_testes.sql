SELECT
    j.no_jogo AS jogo,
    p.no_plataforma AS plataforma,
    c.ds_condicao_jogo AS condicao
FROM tb_jogo AS j
JOIN tb_plataforma AS p
    ON p.co_seq_plataforma = j.co_plataforma
JOIN tb_condicao_jogo AS c
    ON c.co_condicao_jogo = j.co_condicao_jogo
WHERE j.st_ativo = TRUE
ORDER BY j.no_jogo;
-- Consulta 2: gêneros de um jogo informado
SELECT
    j.no_jogo AS jogo,
    g.no_genero AS genero
FROM tb_jogo AS j
JOIN tb_jogo_genero AS jg
    ON jg.co_jogo = j.co_seq_jogo
JOIN tb_genero AS g
    ON g.co_seq_genero = jg.co_genero
WHERE j.no_jogo = 'Metroid Fusion'
ORDER BY g.no_genero;
-- Consulta 3: jogos de um gênero informado
SELECT
    g.no_genero AS genero,
    j.no_jogo AS jogo
FROM tb_genero AS g
JOIN tb_jogo_genero AS jg
    ON jg.co_genero = g.co_seq_genero
JOIN tb_jogo AS j
    ON j.co_seq_jogo = jg.co_jogo
WHERE g.no_genero = 'Corrida'
ORDER BY j.no_jogo;
-- Consulta 4: quantidade de jogos por plataforma, incluindo as sem jogos
SELECT
    p.no_plataforma AS plataforma,
    COUNT(j.co_seq_jogo) AS quantidade_jogos
FROM tb_plataforma AS p
LEFT JOIN tb_jogo AS j
    ON j.co_plataforma = p.co_seq_plataforma
GROUP BY p.co_seq_plataforma, p.no_plataforma
ORDER BY p.no_plataforma;
-- Consulta 5: busca por parte do nome, com 5 jogos por página

-- Página 1
SELECT no_jogo AS jogo
FROM tb_jogo
WHERE no_jogo LIKE '%Mario%'
ORDER BY no_jogo, co_seq_jogo
LIMIT 5 OFFSET 0;

-- Página 2
SELECT no_jogo AS jogo
FROM tb_jogo
WHERE no_jogo LIKE '%Mario%'
ORDER BY no_jogo, co_seq_jogo
LIMIT 5 OFFSET 5;
-- Teste 1: inserir um jogo com plataforma inexistente
-- Resultado esperado: erro 1452 por violação de chave estrangeira.
INSERT INTO tb_jogo (
    no_jogo, co_plataforma, co_condicao_jogo, nu_ano_lancamento,
    vl_preco, qt_estoque, st_ativo, dt_cadastro
)
VALUES (
    'Teste de plataforma inexistente', 999999, 2, 1995,
    49.90, 1, TRUE, UTC_TIMESTAMP()
);
-- Resultado obtido: erro 1452. O banco recusou o jogo porque a plataforma 999999 não existe.
-- Teste 2: inserir uma plataforma com nome já cadastrado
-- Resultado esperado: erro 1062 por nome duplicado.
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES ('Mega Drive', 'Sega', 1988);
-- Resultado obtido: erro 1062. A restrição UNIQUE impediu um segundo cadastro de Mega Drive.
-- Teste 3: excluir uma plataforma que possui jogos.
-- Resultado esperado: erro 1451 por violação de chave estrangeira.
START TRANSACTION;

DELETE FROM tb_plataforma
WHERE co_seq_plataforma = 1
  AND no_plataforma = 'Mega Drive';

ROLLBACK;
-- Resultado obtido: erro 1451. O banco impediu a exclusão do Mega Drive porque há jogos vinculados a ele.
-- Teste 4: inserir um jogo com nome nulo.
-- Resultado esperado: erro 1048, pois no_jogo é obrigatório.
INSERT INTO tb_jogo (
    no_jogo, co_plataforma, co_condicao_jogo, nu_ano_lancamento,
    vl_preco, qt_estoque, st_ativo, dt_cadastro
)
VALUES (
    NULL, 1, 2, 1995,
    49.90, 1, TRUE, UTC_TIMESTAMP()
);
-- Resultado obtido: erro 1048. O banco recusou o cadastro porque no_jogo recebeu NULL.
-- Teste 5: cadastrar um jogo com preço zero.
-- Resultado esperado: o banco aceita o valor. A regra RN06 cabe ao Service.
START TRANSACTION;

INSERT INTO tb_jogo (
    no_jogo, co_plataforma, co_condicao_jogo, nu_ano_lancamento,
    vl_preco, qt_estoque, st_ativo, dt_cadastro
)
VALUES (
    'Teste de preço zero', 1, 2, 1995,
    0.00, 1, TRUE, UTC_TIMESTAMP()
);

-- Conferir o jogo que acabou de ser inserido.
SELECT no_jogo, vl_preco
FROM tb_jogo
WHERE co_seq_jogo = LAST_INSERT_ID();
-- Resultado obtido: o banco aceitou o preço 0.00.
-- O ROLLBACK desfez a inserção, e o catálogo voltou a ter 20 jogos.

ROLLBACK;

-- Conferir o catálogo após desfazer o teste.
SELECT
    COUNT(*) AS total_jogos,
    COUNT(CASE WHEN no_jogo = 'Teste de preço zero' THEN 1 END) AS jogos_de_teste
FROM tb_jogo;