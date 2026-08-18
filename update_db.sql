IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

IF OBJECT_ID(N'[Positions]') IS NULL
BEGIN
    CREATE TABLE [Positions] (
        [PositionId] int NOT NULL IDENTITY,
        [Title] nvarchar(255) NOT NULL,
        [Department] nvarchar(100) NOT NULL,
        [Type] nvarchar(50) NOT NULL,
        [SalaryRange] nvarchar(100) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [Requirements] nvarchar(max) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Positions] PRIMARY KEY ([PositionId])
    );
END

IF OBJECT_ID(N'[Application]') IS NULL
BEGIN
    CREATE TABLE [Application] (
        [application_id] int NOT NULL IDENTITY,
        [candidate_id] int NOT NULL,
        [PositionId] int NOT NULL,
        [AppliedDate] datetime2 NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Application] PRIMARY KEY ([application_id]),
        CONSTRAINT [FK_Application_CandidateProfile_candidate_id] FOREIGN KEY ([candidate_id]) REFERENCES [CandidateProfile] ([candidate_id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Application_Positions_PositionId] FOREIGN KEY ([PositionId]) REFERENCES [Positions] ([PositionId]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_Application_candidate_id] ON [Application] ([candidate_id]);
    CREATE INDEX [IX_Application_PositionId] ON [Application] ([PositionId]);
END

IF OBJECT_ID(N'[ImageProduct]') IS NULL
BEGIN
    CREATE TABLE [ImageProduct] (
        [image_id] int NOT NULL IDENTITY,
        [product_id] int NULL,
        [url] nvarchar(500) NOT NULL,
        [display_order] int NULL,
        [is_thumbnail] bit NULL,
        CONSTRAINT [PK_ImageProduct] PRIMARY KEY ([image_id]),
        CONSTRAINT [FK_ImageProduct_Product_product_id] FOREIGN KEY ([product_id]) REFERENCES [Product] ([product_id])
    );
    CREATE INDEX [IX_ImageProduct_product_id] ON [ImageProduct] ([product_id]);
END

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260818131220_InitialApplication', N'10.0.8');
