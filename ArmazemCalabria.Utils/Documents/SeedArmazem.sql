/*
    SeedArmazem.sql
    Popula tabelas de domínio e 7 pisos de exemplo (um por tipo TPT).

    Pré-requisitos:
    1. DDL das tabelas armazem.* já executado
    2. Coluna quantidade_disponivel em armazem.tb_pisos
    3. Pelo menos 1 usuário em usuario.tb_usuarios (FK id_usuario_criacao)
    4. Schemas armazem e usuario existentes

    IDs de domínio alinhados aos enums em ArmazemCalabria.Entity/Enum/
*/

use armazem_calabria_db

SET NOCOUNT ON;

DECLARE @IdUsuario INT = (SELECT TOP 1 id_usuario FROM usuario.tb_usuarios ORDER BY id_usuario);
DECLARE @IdPiso INT;

IF @IdUsuario IS NULL
    THROW 50001, 'Nenhum usuário encontrado. Cadastre um usuário antes de executar o seed.', 1;

BEGIN TRY
    BEGIN TRAN;

    -- Limpeza (reexecução segura em dev)
    DELETE FROM armazem.tb_pisos;

    DELETE FROM armazem.tb_acabamentos;
    DELETE FROM armazem.tb_ambientes;
    DELETE FROM armazem.tb_niveis_resistencia;
    DELETE FROM armazem.tb_marcas;
    DELETE FROM armazem.tb_tipos_piso;

    -- Domínio: tipos de piso
    SET IDENTITY_INSERT armazem.tb_tipos_piso ON;
    INSERT INTO armazem.tb_tipos_piso (id_tipo_piso, descricao) VALUES
        (1, 'Cerâmica'),
        (2, 'Porcelanato'),
        (3, 'Vinílico'),
        (4, 'Laminado'),
        (5, 'Madeira'),
        (6, 'Pedra Natural'),
        (7, 'Cimento Queimado');
    SET IDENTITY_INSERT armazem.tb_tipos_piso OFF;

    -- Domínio: marcas
    SET IDENTITY_INSERT armazem.tb_marcas ON;
    INSERT INTO armazem.tb_marcas (id_marca, nome) VALUES
        (1, 'Portobello'),
        (2, 'Eliane'),
        (3, 'Incopisos'),
        (4, 'Cecafi'),
        (5, 'Durafloor');
    SET IDENTITY_INSERT armazem.tb_marcas OFF;

    -- Domínio: níveis de resistência
    SET IDENTITY_INSERT armazem.tb_niveis_resistencia ON;
    INSERT INTO armazem.tb_niveis_resistencia (id_nivel_resistencia, descricao) VALUES
        (1, 'Alta'),
        (2, 'Média'),
        (3, 'Baixa'),
        (4, 'Tráfego Comercial'),
        (5, 'Tráfego Residencial');
    SET IDENTITY_INSERT armazem.tb_niveis_resistencia OFF;

    -- Domínio: acabamentos
    SET IDENTITY_INSERT armazem.tb_acabamentos ON;
    INSERT INTO armazem.tb_acabamentos (id_acabamento, descricao) VALUES
        (1, 'Acetinado'),
        (2, 'Polido'),
        (3, 'Fosco');
    SET IDENTITY_INSERT armazem.tb_acabamentos OFF;

    -- Domínio: ambientes
    SET IDENTITY_INSERT armazem.tb_ambientes ON;
    INSERT INTO armazem.tb_ambientes (id_ambiente, descricao) VALUES
        (1, 'Externo'),
        (2, 'Interno');
    SET IDENTITY_INSERT armazem.tb_ambientes OFF;

    -- 1. Cerâmica
    INSERT INTO armazem.tb_pisos (
        nome, cor, preco, largura, comprimento, espessura, peso,
        fl_resistente_agua, fl_antiderrapante, quantidade_disponivel,
        id_tipo_piso, id_marca, id_nivel_resistencia, id_acabamento, id_ambiente,
        id_usuario_criacao
    ) VALUES (
        N'Cerâmica Bege Classic', N'Bege', 45.90, 45.00, 45.00, 8.00, 12.50,
        1, 1, 120,
        1, 1, 2, 1, 2,
        @IdUsuario
    );
    SET @IdPiso = SCOPE_IDENTITY();
    INSERT INTO armazem.tb_pisos_ceramica (id_piso, classe_pei) VALUES (@IdPiso, 4);

    -- 2. Porcelanato
    INSERT INTO armazem.tb_pisos (
        nome, cor, preco, largura, comprimento, espessura, peso,
        fl_resistente_agua, fl_antiderrapante, quantidade_disponivel,
        id_tipo_piso, id_marca, id_nivel_resistencia, id_acabamento, id_ambiente,
        id_usuario_criacao
    ) VALUES (
        N'Porcelanato Marmo Bianco', N'Branco', 89.50, 60.00, 60.00, 9.00, 18.00,
        1, 0, 85,
        2, 2, 1, 2, 2,
        @IdUsuario
    );
    SET @IdPiso = SCOPE_IDENTITY();
    INSERT INTO armazem.tb_pisos_porcelanato (id_piso, fl_retificado, tipo_porcelanato)
    VALUES (@IdPiso, 1, 'Polido');

    -- 3. Vinílico
    INSERT INTO armazem.tb_pisos (
        nome, cor, preco, largura, comprimento, espessura, peso,
        fl_resistente_agua, fl_antiderrapante, quantidade_disponivel,
        id_tipo_piso, id_marca, id_nivel_resistencia, id_acabamento, id_ambiente,
        id_usuario_criacao
    ) VALUES (
        N'Vinílico Carvalho Acústico', N'Carvalho', 62.00, 15.00, 120.00, 3.00, 8.50,
        1, 0, 200,
        3, 3, 5, 3, 2,
        @IdUsuario
    );
    SET @IdPiso = SCOPE_IDENTITY();
    INSERT INTO armazem.tb_pisos_vinilico (id_piso, fl_acustico, tipo_instalacao)
    VALUES (@IdPiso, 1, 'Clicado');

    -- 4. Laminado
    INSERT INTO armazem.tb_pisos (
        nome, cor, preco, largura, comprimento, espessura, peso,
        fl_resistente_agua, fl_antiderrapante, quantidade_disponivel,
        id_tipo_piso, id_marca, id_nivel_resistencia, id_acabamento, id_ambiente,
        id_usuario_criacao
    ) VALUES (
        N'Laminado Freijó', N'Freijó', 38.75, 19.00, 135.00, 7.00, 10.20,
        0, 0, 150,
        4, 4, 3, 1, 2,
        @IdUsuario
    );
    SET @IdPiso = SCOPE_IDENTITY();
    INSERT INTO armazem.tb_pisos_laminado (id_piso, fl_resistente_cupim) VALUES (@IdPiso, 1);

    -- 5. Madeira
    INSERT INTO armazem.tb_pisos (
        nome, cor, preco, largura, comprimento, espessura, peso,
        fl_resistente_agua, fl_antiderrapante, quantidade_disponivel,
        id_tipo_piso, id_marca, id_nivel_resistencia, id_acabamento, id_ambiente,
        id_usuario_criacao
    ) VALUES (
        N'Madeira Ipê Nobre', N'Ipê', 125.00, 10.00, 100.00, 2.00, 14.00,
        0, 1, 60,
        5, 5, 1, 2, 2,
        @IdUsuario
    );
    SET @IdPiso = SCOPE_IDENTITY();
    INSERT INTO armazem.tb_pisos_madeira (id_piso, tipo_madeira, fl_madeira_nobre)
    VALUES (@IdPiso, N'Ipê', 1);

    -- 6. Pedra Natural
    INSERT INTO armazem.tb_pisos (
        nome, cor, preco, largura, comprimento, espessura, peso,
        fl_resistente_agua, fl_antiderrapante, quantidade_disponivel,
        id_tipo_piso, id_marca, id_nivel_resistencia, id_acabamento, id_ambiente,
        id_usuario_criacao
    ) VALUES (
        N'Pedra Mármore Carrara', N'Carrara', 210.00, 60.00, 60.00, 2.00, 25.00,
        0, 0, 40,
        6, 1, 4, 2, 1,
        @IdUsuario
    );
    SET @IdPiso = SCOPE_IDENTITY();
    INSERT INTO armazem.tb_pisos_pedra_natural (id_piso, tipo_pedra, fl_porosidade, fl_necessita_impermeabilizacao)
    VALUES (@IdPiso, N'Mármore', 1, 1);

    -- 7. Cimento Queimado
    INSERT INTO armazem.tb_pisos (
        nome, cor, preco, largura, comprimento, espessura, peso,
        fl_resistente_agua, fl_antiderrapante, quantidade_disponivel,
        id_tipo_piso, id_marca, id_nivel_resistencia, id_acabamento, id_ambiente,
        id_usuario_criacao
    ) VALUES (
        N'Cimento Queimado Cinza', N'Cinza', 55.00, 50.00, 50.00, 3.00, 15.00,
        1, 1, 95,
        7, 2, 2, 3, 2,
        @IdUsuario
    );
    SET @IdPiso = SCOPE_IDENTITY();
    INSERT INTO armazem.tb_pisos_cimento_queimado (id_piso) VALUES (@IdPiso);

    COMMIT TRAN;

    PRINT 'Seed armazém concluído com sucesso.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRAN;

    THROW;
END CATCH;

-- Verificação
SELECT 'tb_tipos_piso' AS tabela, COUNT(*) AS total FROM armazem.tb_tipos_piso
UNION ALL SELECT 'tb_marcas', COUNT(*) FROM armazem.tb_marcas
UNION ALL SELECT 'tb_niveis_resistencia', COUNT(*) FROM armazem.tb_niveis_resistencia
UNION ALL SELECT 'tb_acabamentos', COUNT(*) FROM armazem.tb_acabamentos
UNION ALL SELECT 'tb_ambientes', COUNT(*) FROM armazem.tb_ambientes
UNION ALL SELECT 'tb_pisos', COUNT(*) FROM armazem.tb_pisos
UNION ALL SELECT 'tb_pisos_ceramica', COUNT(*) FROM armazem.tb_pisos_ceramica
UNION ALL SELECT 'tb_pisos_porcelanato', COUNT(*) FROM armazem.tb_pisos_porcelanato
UNION ALL SELECT 'tb_pisos_vinilico', COUNT(*) FROM armazem.tb_pisos_vinilico
UNION ALL SELECT 'tb_pisos_laminado', COUNT(*) FROM armazem.tb_pisos_laminado
UNION ALL SELECT 'tb_pisos_madeira', COUNT(*) FROM armazem.tb_pisos_madeira
UNION ALL SELECT 'tb_pisos_pedra_natural', COUNT(*) FROM armazem.tb_pisos_pedra_natural
UNION ALL SELECT 'tb_pisos_cimento_queimado', COUNT(*) FROM armazem.tb_pisos_cimento_queimado;
