-- Execute este arquivo em um banco RetroGames vazio.
-- Primeiro, selecione o banco no MySQL Workbench.

CREATE TABLE tb_condicao_jogo (
    co_condicao_jogo INT NOT NULL,
    ds_condicao_jogo VARCHAR(100) NOT NULL,
    PRIMARY KEY (co_condicao_jogo),
    UNIQUE (ds_condicao_jogo)
);

CREATE TABLE tb_perfil (
    co_perfil INT NOT NULL,
    ds_perfil VARCHAR(100) NOT NULL,
    PRIMARY KEY (co_perfil),
    UNIQUE (ds_perfil)
);

CREATE TABLE tb_plataforma (
    co_seq_plataforma INT NOT NULL AUTO_INCREMENT,
    no_plataforma VARCHAR(150) NOT NULL,
    no_fabricante VARCHAR(150) NOT NULL,
    nu_ano_lancamento INT NOT NULL,
    PRIMARY KEY (co_seq_plataforma),
    UNIQUE (no_plataforma)
);

CREATE TABLE tb_genero (
    co_seq_genero INT NOT NULL AUTO_INCREMENT,
    no_genero VARCHAR(100) NOT NULL,
    PRIMARY KEY (co_seq_genero),
    UNIQUE (no_genero)
);

CREATE TABLE tb_jogo (
    co_seq_jogo INT NOT NULL AUTO_INCREMENT,
    no_jogo VARCHAR(200) NOT NULL,
    ds_jogo TEXT NULL,
    nu_ano_lancamento INT NOT NULL,
    vl_preco DECIMAL(10, 2) NOT NULL,
    qt_estoque INT NOT NULL,
    st_ativo BOOLEAN NOT NULL DEFAULT TRUE,
    dt_cadastro DATETIME NOT NULL,
    co_plataforma INT NOT NULL,
    co_condicao_jogo INT NOT NULL,

    PRIMARY KEY (co_seq_jogo),
    INDEX idx_jogo_nome (no_jogo),
    FOREIGN KEY (co_plataforma)
        REFERENCES tb_plataforma (co_seq_plataforma),
    FOREIGN KEY (co_condicao_jogo)
        REFERENCES tb_condicao_jogo (co_condicao_jogo)
);

CREATE TABLE tb_jogo_genero (
    co_jogo INT NOT NULL,
    co_genero INT NOT NULL,

    PRIMARY KEY (co_jogo, co_genero),
    FOREIGN KEY (co_jogo)
        REFERENCES tb_jogo (co_seq_jogo),
    FOREIGN KEY (co_genero)
        REFERENCES tb_genero (co_seq_genero)
);

CREATE TABLE tb_usuario (
    co_seq_usuario INT NOT NULL AUTO_INCREMENT,
    no_usuario VARCHAR(150) NOT NULL,
    ds_email VARCHAR(255) NOT NULL,
    ds_senha_hash VARCHAR(255) NOT NULL,
    st_ativo BOOLEAN NOT NULL DEFAULT TRUE,
    dt_cadastro DATETIME NOT NULL,
    co_perfil INT NOT NULL,

    PRIMARY KEY (co_seq_usuario),
    UNIQUE (ds_email),
    FOREIGN KEY (co_perfil)
        REFERENCES tb_perfil (co_perfil)
);