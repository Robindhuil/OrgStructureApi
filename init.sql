IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Employees] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Phone] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251220080945_InitialCreate', N'7.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Employees]') AND [c].[name] = N'Title');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Employees] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Employees] ALTER COLUMN [Title] nvarchar(20) NOT NULL;
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Employees]') AND [c].[name] = N'LastName');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Employees] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Employees] ALTER COLUMN [LastName] nvarchar(50) NOT NULL;
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Employees]') AND [c].[name] = N'FirstName');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Employees] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [Employees] ALTER COLUMN [FirstName] nvarchar(50) NOT NULL;
GO

ALTER TABLE [Employees] ADD [CompanyId] int NULL;
GO

CREATE TABLE [Companies] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Code] nvarchar(20) NOT NULL,
    [DirectorId] int NULL,
    CONSTRAINT [PK_Companies] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Companies_Employees_DirectorId] FOREIGN KEY ([DirectorId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Divisions] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Code] nvarchar(20) NOT NULL,
    [CompanyId] int NOT NULL,
    [LeaderId] int NULL,
    CONSTRAINT [PK_Divisions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Divisions_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Divisions_Employees_LeaderId] FOREIGN KEY ([LeaderId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Projects] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Code] nvarchar(20) NOT NULL,
    [DivisionId] int NOT NULL,
    [LeaderId] int NULL,
    CONSTRAINT [PK_Projects] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Projects_Divisions_DivisionId] FOREIGN KEY ([DivisionId]) REFERENCES [Divisions] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Projects_Employees_LeaderId] FOREIGN KEY ([LeaderId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Departments] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Code] nvarchar(20) NOT NULL,
    [ProjectId] int NOT NULL,
    [LeaderId] int NULL,
    CONSTRAINT [PK_Departments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Departments_Employees_LeaderId] FOREIGN KEY ([LeaderId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Departments_Projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Employees_CompanyId] ON [Employees] ([CompanyId]);
GO

CREATE INDEX [IX_Companies_DirectorId] ON [Companies] ([DirectorId]);
GO

CREATE INDEX [IX_Departments_LeaderId] ON [Departments] ([LeaderId]);
GO

CREATE INDEX [IX_Departments_ProjectId] ON [Departments] ([ProjectId]);
GO

CREATE INDEX [IX_Divisions_CompanyId] ON [Divisions] ([CompanyId]);
GO

CREATE INDEX [IX_Divisions_LeaderId] ON [Divisions] ([LeaderId]);
GO

CREATE INDEX [IX_Projects_DivisionId] ON [Projects] ([DivisionId]);
GO

CREATE INDEX [IX_Projects_LeaderId] ON [Projects] ([LeaderId]);
GO

ALTER TABLE [Employees] ADD CONSTRAINT [FK_Employees_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251221144822_EmployeeSingleCompany', N'7.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Employees] ADD [Role] int NOT NULL DEFAULT 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251221152721_EmployeeRoleEnum', N'7.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Employees] DROP CONSTRAINT [FK_Employees_Companies_CompanyId];
GO

DROP INDEX [IX_Employees_CompanyId] ON [Employees];
DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Employees]') AND [c].[name] = N'CompanyId');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Employees] DROP CONSTRAINT [' + @var3 + '];');
UPDATE [Employees] SET [CompanyId] = 0 WHERE [CompanyId] IS NULL;
ALTER TABLE [Employees] ALTER COLUMN [CompanyId] int NOT NULL;
ALTER TABLE [Employees] ADD DEFAULT 0 FOR [CompanyId];
CREATE INDEX [IX_Employees_CompanyId] ON [Employees] ([CompanyId]);
GO

ALTER TABLE [Employees] ADD CONSTRAINT [FK_Employees_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251222080005_EmployeeCompanyIdRequired', N'7.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Employees] DROP CONSTRAINT [FK_Employees_Companies_CompanyId];
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Employees]') AND [c].[name] = N'CompanyId');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Employees] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [Employees] ALTER COLUMN [CompanyId] int NULL;
GO

ALTER TABLE [Employees] ADD CONSTRAINT [FK_Employees_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251222102626_EmployeeCompanyIdNullable', N'7.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Employees]') AND [c].[name] = N'Role');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Employees] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [Employees] DROP COLUMN [Role];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251222112523_RemoveEmployeeRole', N'7.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Employees] DROP CONSTRAINT [FK_Employees_Companies_CompanyId];
GO

DROP INDEX [IX_Projects_DivisionId] ON [Projects];
GO

DROP INDEX [IX_Divisions_CompanyId] ON [Divisions];
GO

DROP INDEX [IX_Departments_ProjectId] ON [Departments];
GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Employees]') AND [c].[name] = N'Phone');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Employees] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [Employees] ALTER COLUMN [Phone] nvarchar(450) NOT NULL;
GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Employees]') AND [c].[name] = N'Email');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Employees] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [Employees] ALTER COLUMN [Email] nvarchar(450) NOT NULL;
GO

CREATE UNIQUE INDEX [IX_Projects_DivisionId_Code] ON [Projects] ([DivisionId], [Code]);
GO


WITH cte AS (
    SELECT Id, Email, ROW_NUMBER() OVER (PARTITION BY Email ORDER BY Id) AS rn
    FROM Employees
    WHERE Email IS NOT NULL
)
UPDATE e
SET Email = CONCAT(LEFT(e.Email, CHARINDEX('@', e.Email) - 1), '+dup', e.Id, SUBSTRING(e.Email, CHARINDEX('@', e.Email), 8000))
FROM Employees e
INNER JOIN cte c ON e.Id = c.Id
WHERE c.rn > 1;

GO

CREATE UNIQUE INDEX [IX_Employees_Email] ON [Employees] ([Email]);
GO


WITH cte AS (
    SELECT Id, Phone, ROW_NUMBER() OVER (PARTITION BY Phone ORDER BY Id) AS rn
    FROM Employees
    WHERE Phone IS NOT NULL
)
UPDATE e
SET Phone = CONCAT(e.Phone, '-', e.Id)
FROM Employees e
INNER JOIN cte c ON e.Id = c.Id
WHERE c.rn > 1;

GO

CREATE UNIQUE INDEX [IX_Employees_Phone] ON [Employees] ([Phone]);
GO

CREATE UNIQUE INDEX [IX_Divisions_CompanyId_Code] ON [Divisions] ([CompanyId], [Code]);
GO

CREATE UNIQUE INDEX [IX_Departments_ProjectId_Code] ON [Departments] ([ProjectId], [Code]);
GO

CREATE UNIQUE INDEX [IX_Companies_Code] ON [Companies] ([Code]);
GO

ALTER TABLE [Employees] ADD CONSTRAINT [FK_Employees_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251222135832_AddUniqueIndexesAndRestrictEmployeeCompany', N'7.0.8');
GO

COMMIT;
GO

