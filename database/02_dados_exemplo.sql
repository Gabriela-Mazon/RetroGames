INSERT INTO tb_condicao_jogo (co_condicao_jogo, ds_condicao_jogo)
VALUES
    (1, 'Novo'),
    (2, 'Usado'),
    (3, 'Recondicionado');
    INSERT INTO tb_perfil (co_perfil, ds_perfil)
VALUES
    (1, 'Administrador'),
    (2, 'Cliente');
    INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES
    ('Mega Drive', 'Sega', 1988),
    ('Game Boy', 'Nintendo', 1989),
    ('Super Nintendo', 'Nintendo', 1990),
    ('PlayStation', 'Sony', 1994),
    ('Nintendo 64', 'Nintendo', 1996),
    ('Dreamcast', 'Sega', 1998),
    ('Game Boy Advance', 'Nintendo', 2001);
    INSERT INTO tb_genero (no_genero)
VALUES
    ('Ação'),
    ('Aventura'),
    ('Plataforma'),
    ('RPG'),
    ('Corrida'),
    ('Luta'),
    ('Esporte'),
    ('Quebra-cabeça');
    INSERT INTO tb_jogo
    (no_jogo, co_plataforma, co_condicao_jogo, nu_ano_lancamento,
     vl_preco, qt_estoque, st_ativo, dt_cadastro)
VALUES
    -- Mega Drive
    ('Sonic the Hedgehog', 1, 2, 1991, 89.90, 3, TRUE, UTC_TIMESTAMP()),
    ('Sonic the Hedgehog 2', 1, 2, 1992, 99.90, 2, TRUE, UTC_TIMESTAMP()),
    ('Streets of Rage', 1, 3, 1991, 79.90, 1, FALSE, UTC_TIMESTAMP()),

    -- Game Boy
    ('Super Mario Land', 2, 2, 1989, 69.90, 4, TRUE, UTC_TIMESTAMP()),
    ('Super Mario Land 2: 6 Golden Coins', 2, 2, 1992, 89.90, 2, TRUE, UTC_TIMESTAMP()),
    ('Tetris', 2, 2, 1989, 59.90, 0, TRUE, UTC_TIMESTAMP()),

    -- Super Nintendo
    ('Super Mario World', 3, 2, 1990, 119.90, 3, TRUE, UTC_TIMESTAMP()),
    ('Super Mario Kart', 3, 2, 1992, 109.90, 2, TRUE, UTC_TIMESTAMP()),
    ('Donkey Kong Country', 3, 3, 1994, 99.90, 1, TRUE, UTC_TIMESTAMP()),
    ('F-ZERO', 3, 2, 1990, 79.90, 2, TRUE, UTC_TIMESTAMP()),
    ('The Legend of Zelda: A Link to the Past', 3, 2, 1991, 149.90, 1, TRUE, UTC_TIMESTAMP()),

    -- PlayStation
    ('Crash Bandicoot', 4, 2, 1996, 89.90, 3, TRUE, UTC_TIMESTAMP()),
    ('Final Fantasy VII', 4, 2, 1997, 139.90, 2, TRUE, UTC_TIMESTAMP()),
    ('Tekken 3', 4, 3, 1998, 79.90, 4, TRUE, UTC_TIMESTAMP()),
    ('Tony Hawk''s Pro Skater', 4, 2, 1999, 69.90, 2, TRUE, UTC_TIMESTAMP()),

    -- Nintendo 64
    ('Super Mario 64', 5, 2, 1996, 129.90, 2, TRUE, UTC_TIMESTAMP()),
    ('Mario Kart 64', 5, 2, 1996, 139.90, 3, TRUE, UTC_TIMESTAMP()),
    ('The Legend of Zelda: Ocarina of Time', 5, 2, 1998, 159.90, 1, TRUE, UTC_TIMESTAMP()),

    -- Game Boy Advance
    ('Mario Kart: Super Circuit', 7, 2, 2001, 89.90, 2, TRUE, UTC_TIMESTAMP()),
    ('Metroid Fusion', 7, 1, 2002, 149.90, 1, TRUE, UTC_TIMESTAMP());
    INSERT INTO tb_jogo_genero (co_jogo, co_genero)

-- 1 = Ação
SELECT co_seq_jogo, 1
FROM tb_jogo
WHERE no_jogo IN (
    'Sonic the Hedgehog', 'Sonic the Hedgehog 2', 'Streets of Rage',
    'Donkey Kong Country', 'Crash Bandicoot', 'Metroid Fusion',
    'The Legend of Zelda: A Link to the Past',
    'The Legend of Zelda: Ocarina of Time'
)

UNION ALL

-- 2 = Aventura
SELECT co_seq_jogo, 2
FROM tb_jogo
WHERE no_jogo IN (
    'The Legend of Zelda: A Link to the Past',
    'The Legend of Zelda: Ocarina of Time', 'Metroid Fusion'
)

UNION ALL

-- 3 = Plataforma
SELECT co_seq_jogo, 3
FROM tb_jogo
WHERE no_jogo IN (
    'Sonic the Hedgehog', 'Sonic the Hedgehog 2',
    'Super Mario Land', 'Super Mario Land 2: 6 Golden Coins',
    'Super Mario World', 'Donkey Kong Country', 'Crash Bandicoot',
    'Super Mario 64', 'Metroid Fusion'
)

UNION ALL

-- 4 = RPG
SELECT co_seq_jogo, 4
FROM tb_jogo
WHERE no_jogo = 'Final Fantasy VII'

UNION ALL

-- 5 = Corrida
SELECT co_seq_jogo, 5
FROM tb_jogo
WHERE no_jogo IN (
    'Super Mario Kart', 'F-ZERO', 'Mario Kart 64', 'Mario Kart: Super Circuit'
)

UNION ALL

-- 6 = Luta
SELECT co_seq_jogo, 6
FROM tb_jogo
WHERE no_jogo IN ('Streets of Rage', 'Tekken 3')

UNION ALL

-- 7 = Esporte
SELECT co_seq_jogo, 7
FROM tb_jogo
WHERE no_jogo = 'Tony Hawk''s Pro Skater'

UNION ALL

-- 8 = Quebra-cabeça
SELECT co_seq_jogo, 8
FROM tb_jogo
WHERE no_jogo = 'Tetris';