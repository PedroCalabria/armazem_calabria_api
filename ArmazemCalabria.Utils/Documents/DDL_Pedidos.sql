/*
    DDL_Pedidos.sql
    Cria tabelas de pedidos no schema armazem.

    Pré-requisitos:
    1. Schemas armazem e usuario existentes
    2. Tabelas usuario.tb_usuarios e armazem.tb_pisos já criadas
*/

USE armazem_calabria_db;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables t
               INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
               WHERE s.name = 'armazem' AND t.name = 'tb_pedidos')
BEGIN
    CREATE TABLE armazem.tb_pedidos (
        id_pedido              INT IDENTITY(1,1) NOT NULL,
        id_usuario_solicitante INT               NOT NULL,
        id_status              TINYINT           NOT NULL,
        motivo_rejeicao        NVARCHAR(500)     NULL,
        data_criacao           DATETIME2         NOT NULL CONSTRAINT DF_tb_pedidos_data_criacao DEFAULT GETDATE(),
        data_alteracao         DATETIME2         NULL,
        id_usuario_aprovador   INT               NULL,
        data_aprovacao         DATETIME2         NULL,
        data_rejeicao          DATETIME2         NULL,
        CONSTRAINT PK_tb_pedidos PRIMARY KEY (id_pedido),
        CONSTRAINT FK_tb_pedidos_usuario_solicitante
            FOREIGN KEY (id_usuario_solicitante) REFERENCES usuario.tb_usuarios (id_usuario),
        CONSTRAINT FK_tb_pedidos_usuario_aprovador
            FOREIGN KEY (id_usuario_aprovador) REFERENCES usuario.tb_usuarios (id_usuario),
        CONSTRAINT CK_tb_pedidos_status CHECK (id_status IN (1, 2, 3))
    );

    CREATE INDEX IX_tb_pedidos_usuario_solicitante ON armazem.tb_pedidos (id_usuario_solicitante);
    CREATE INDEX IX_tb_pedidos_status ON armazem.tb_pedidos (id_status);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables t
               INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
               WHERE s.name = 'armazem' AND t.name = 'tb_pedidos_itens')
BEGIN
    CREATE TABLE armazem.tb_pedidos_itens (
        id_pedido_item INT IDENTITY(1,1) NOT NULL,
        id_pedido      INT               NOT NULL,
        id_piso        INT               NOT NULL,
        quantidade     INT               NOT NULL,
        CONSTRAINT PK_tb_pedidos_itens PRIMARY KEY (id_pedido_item),
        CONSTRAINT FK_tb_pedidos_itens_pedido
            FOREIGN KEY (id_pedido) REFERENCES armazem.tb_pedidos (id_pedido) ON DELETE CASCADE,
        CONSTRAINT FK_tb_pedidos_itens_piso
            FOREIGN KEY (id_piso) REFERENCES armazem.tb_pisos (id_piso),
        CONSTRAINT CK_tb_pedidos_itens_quantidade CHECK (quantidade > 0)
    );

    CREATE INDEX IX_tb_pedidos_itens_pedido ON armazem.tb_pedidos_itens (id_pedido);
END
GO
