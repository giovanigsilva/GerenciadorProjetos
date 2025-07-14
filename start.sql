CREATE DATABASE projetos_dev;
CREATE DATABASE projetos_uat;
CREATE DATABASE projetos_prod;

USE projetos_dev;
GO


CREATE TABLE Usuarios (
    UsuarioID INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    SenhaHash NVARCHAR(255) NOT NULL,
    Funcao NVARCHAR(50) NOT NULL DEFAULT 'usuario',
    DataCriacao DATETIME2 DEFAULT SYSUTCDATETIME()
);
GO

CREATE TABLE Projetos (
    ProjetoID INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioID INT NOT NULL,
    Nome NVARCHAR(150) NOT NULL,
    Descricao NVARCHAR(MAX) NULL,
    DataCriacao DATETIME2 DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Projetos_Usuarios FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID)
);
GO

CREATE TABLE Tarefas (
    TarefaID INT IDENTITY(1,1) PRIMARY KEY,
    ProjetoID INT NOT NULL,
    Titulo NVARCHAR(200) NOT NULL,
    Descricao NVARCHAR(MAX) NULL,
    DataVencimento DATE NULL,
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('pendente', 'em andamento', 'concluída')),
    Prioridade NVARCHAR(10) NOT NULL CHECK (Prioridade IN ('baixa', 'média', 'alta')),
    DataCriacao DATETIME2 DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Tarefas_Projetos FOREIGN KEY (ProjetoID) REFERENCES Projetos(ProjetoID)
);
GO

CREATE TABLE HistoricoTarefas (
    HistoricoID INT IDENTITY(1,1) PRIMARY KEY,
    TarefaID INT NOT NULL,
    UsuarioID INT NOT NULL,
    DataAlteracao DATETIME2 DEFAULT SYSUTCDATETIME(),
    DescricaoAlteracao NVARCHAR(MAX) NOT NULL,


    StatusAntigo NVARCHAR(50) NULL,
    StatusNovo NVARCHAR(50) NULL,
    PrioridadeAntiga NVARCHAR(50) NULL,
    PrioridadeNova NVARCHAR(50) NULL,

    
    CONSTRAINT FK_HistoricoTarefas_Tarefas 
        FOREIGN KEY (TarefaID) REFERENCES Tarefas(TarefaID)
        ON DELETE CASCADE,

   
    CONSTRAINT FK_HistoricoTarefas_Usuarios 
        FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID)
        ON DELETE NO ACTION  -- ou ON DELETE RESTRICT (equivalente)
);
GO

CREATE TRIGGER trg_Tarefas_NaoAlterarPrioridade
ON Tarefas
INSTEAD OF UPDATE
AS
BEGIN
    IF UPDATE(Prioridade)
    BEGIN
        IF EXISTS (
            SELECT 1 FROM inserted i
            JOIN deleted d ON i.TarefaID = d.TarefaID
            WHERE i.Prioridade <> d.Prioridade
        )
        BEGIN
            RAISERROR('A prioridade da tarefa não pode ser alterada após a criação.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END
    END

    UPDATE Tarefas
    SET 
        Titulo = i.Titulo,
        Descricao = i.Descricao,
        DataVencimento = i.DataVencimento,
        Status = i.Status
    FROM Tarefas t
    JOIN inserted i ON t.TarefaID = i.TarefaID;
END;
GO


CREATE TRIGGER trg_Projetos_NaoExcluirSeTarefasPendentes
ON Projetos
INSTEAD OF DELETE
AS
BEGIN
    IF EXISTS (
        SELECT 1 FROM Tarefas t
        JOIN deleted d ON t.ProjetoID = d.ProjetoID
        WHERE t.Status = 'pendente'
    )
    BEGIN
        RAISERROR('Não é permitido excluir projeto que possua tarefas pendentes. Finalize ou exclua as tarefas antes.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    DELETE FROM Projetos WHERE ProjetoID IN (SELECT ProjetoID FROM deleted);
END;
GO

CREATE PROCEDURE sp_AdicionarComentario
    @TarefaID INT,
    @UsuarioID INT,
    @Comentario NVARCHAR(MAX)
AS
BEGIN
    INSERT INTO HistoricoTarefas (TarefaID, UsuarioID, DescricaoAlteracao)
    VALUES (@TarefaID, @UsuarioID, CONCAT('Comentário adicionado: ', @Comentario));
END;
GO

CREATE TRIGGER trg_HistoricoTarefas_Atualizacao
ON Tarefas
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UsuarioID INT = NULL;

    INSERT INTO HistoricoTarefas (TarefaID, UsuarioID, DescricaoAlteracao)
    SELECT 
        i.TarefaID,
        @UsuarioID,
        CONCAT(
            'Tarefa atualizada. ',
            CASE WHEN i.Status <> d.Status THEN CONCAT('Status: ', d.Status, ' -> ', i.Status, '. ') ELSE '' END,
            CASE WHEN i.Titulo <> d.Titulo THEN CONCAT('Título: ', d.Titulo, ' -> ', i.Titulo, '. ') ELSE '' END,
            CASE WHEN ISNULL(i.Descricao,'') <> ISNULL(d.Descricao,'') THEN 'Descrição alterada. ' ELSE '' END,
            CASE WHEN i.DataVencimento <> d.DataVencimento THEN CONCAT('Data Vencimento: ', CONVERT(VARCHAR, d.DataVencimento, 23), ' -> ', CONVERT(VARCHAR, i.DataVencimento, 23), '. ') ELSE '' END
        )
    FROM inserted i
    JOIN deleted d ON i.TarefaID = d.TarefaID;
END;
GO
ALTER TABLE Usuarios
ADD DataExpiracaoToken DATETIME NULL,
    TokenRecuperacaoSenha NVARCHAR(255) NULL;
GO

CREATE OR ALTER TRIGGER trg_HistoricoTarefas_Atualizacao
ON dbo.Tarefas
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @usuarioId INT = TRY_CONVERT(INT, SESSION_CONTEXT(N'usuario_id'));

    IF @usuarioId IS NULL
        THROW 51000, 'Usuário não informado no contexto da sessão.', 1;

    INSERT INTO dbo.HistoricoTarefas (
        TarefaID,
        UsuarioID,
        DataAlteracao,
        DescricaoAlteracao,
        StatusAntigo,
        StatusNovo,
        PrioridadeAntiga,
        PrioridadeNova
    )
    SELECT
        i.TarefaID,
        @usuarioId,
        SYSUTCDATETIME(),
        'Alteração no status e/ou prioridade da tarefa.',
        d.Status,
        i.Status,
        d.Prioridade,
        i.Prioridade
    FROM inserted i
    INNER JOIN deleted d ON i.TarefaID = d.TarefaID;
END;
GO

ALTER TABLE Tarefas
ADD UsuarioID INT NOT NULL;


ALTER TABLE Tarefas
ADD CONSTRAINT FK_Tarefas_Usuarios
FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID);

GO

