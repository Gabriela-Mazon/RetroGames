-- Aplique uma vez no mesmo banco que já contém o catálogo.
CREATE TABLE tb_situacao_pedido (
    co_situacao_pedido INT NOT NULL PRIMARY KEY,
    ds_situacao_pedido VARCHAR(100) NOT NULL UNIQUE
);

INSERT INTO tb_situacao_pedido (co_situacao_pedido, ds_situacao_pedido)
VALUES (1, 'Pendente'), (2, 'Pago'), (3, 'Enviado'), (4, 'Cancelado');

CREATE TABLE tb_pedido (
    co_seq_pedido INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    dt_pedido DATETIME(6) NOT NULL,
    vl_total DECIMAL(20, 2) NOT NULL,
    co_usuario INT NOT NULL,
    co_situacao_pedido INT NOT NULL,
    INDEX idx_pedido_usuario_data (co_usuario, dt_pedido, co_seq_pedido),
    FOREIGN KEY (co_usuario) REFERENCES tb_usuario (co_seq_usuario),
    FOREIGN KEY (co_situacao_pedido) REFERENCES tb_situacao_pedido (co_situacao_pedido)
);

CREATE TABLE tb_item_pedido (
    co_seq_item_pedido INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    qt_item INT NOT NULL,
    vl_unitario DECIMAL(10, 2) NOT NULL,
    co_pedido INT NOT NULL,
    co_jogo INT NOT NULL,
    UNIQUE (co_pedido, co_jogo),
    FOREIGN KEY (co_pedido) REFERENCES tb_pedido (co_seq_pedido),
    FOREIGN KEY (co_jogo) REFERENCES tb_jogo (co_seq_jogo)
);
