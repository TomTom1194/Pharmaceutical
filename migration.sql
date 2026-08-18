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
CREATE TABLE [CandidateProfile] (
    [candidate_id] int NOT NULL,
    [full_name] nvarchar(255) NULL,
    [phone] nvarchar(20) NULL,
    [address] nvarchar(255) NULL,
    [summary] nvarchar(max) NULL,
    [created_at] datetime2 NULL,
    [profile_image] nvarchar(255) NULL,
    CONSTRAINT [PK_CandidateProfile] PRIMARY KEY ([candidate_id])
);

CREATE TABLE [CapsuleSpecification] (
    [product_id] int NOT NULL,
    [output] nvarchar(100) NULL,
    [capsule_size_mm] decimal(10,2) NULL,
    [machine_dimension] nvarchar(100) NULL,
    [shipping_weight_kg] decimal(10,2) NULL,
    CONSTRAINT [PK_CapsuleSpecification] PRIMARY KEY ([product_id])
);

CREATE TABLE [ContentPage] (
    [page_id] int NOT NULL IDENTITY,
    [slug] nvarchar(100) NOT NULL,
    [title] nvarchar(255) NULL,
    [body] nvarchar(max) NULL,
    [banner_image_url] nvarchar(500) NULL,
    [status] nvarchar(20) NULL,
    [updated_by] int NULL,
    [updated_at] datetime2 NULL,
    CONSTRAINT [PK_ContentPage] PRIMARY KEY ([page_id])
);

CREATE TABLE [EducationRecord] (
    [education_id] int NOT NULL IDENTITY,
    [candidate_id] int NULL,
    [institution] nvarchar(255) NULL,
    [qualification] nvarchar(255) NULL,
    [field] nvarchar(255) NULL,
    [start_date] date NULL,
    [end_date] date NULL,
    CONSTRAINT [PK_EducationRecord] PRIMARY KEY ([education_id])
);

CREATE TABLE [InterviewInvitation] (
    [invitation_id] int NOT NULL IDENTITY,
    [candidate_id] int NULL,
    [sent_by] int NULL,
    [subject] nvarchar(255) NULL,
    [body] nvarchar(max) NULL,
    [status] nvarchar(20) NULL,
    [sent_at] datetime2 NULL,
    CONSTRAINT [PK_InterviewInvitation] PRIMARY KEY ([invitation_id])
);

CREATE TABLE [LiquidFillingSpecification] (
    [product_id] int NOT NULL,
    [air_pressure] decimal(10,2) NULL,
    [air_volume] decimal(10,2) NULL,
    [filling_speed] decimal(10,2) NULL,
    [filling_range_ml] decimal(10,2) NULL,
    CONSTRAINT [PK_LiquidFillingSpecification] PRIMARY KEY ([product_id])
);

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

CREATE TABLE [Product] (
    [product_id] int NOT NULL IDENTITY,
    [category_id] int NULL,
    [model_name] nvarchar(255) NOT NULL,
    [summary] nvarchar(255) NULL,
    [description] nvarchar(max) NULL,
    [output_label] nvarchar(100) NULL,
    [is_published] bit NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY ([product_id])
);

CREATE TABLE [ProductCategory] (
    [category_id] int NOT NULL IDENTITY,
    [name] nvarchar(100) NOT NULL,
    [description] nvarchar(max) NULL,
    [is_active] bit NULL,
    CONSTRAINT [PK_ProductCategory] PRIMARY KEY ([category_id])
);

CREATE TABLE [QuoteRequest] (
    [quote_id] int NOT NULL IDENTITY,
    [full_name] nvarchar(255) NOT NULL,
    [company_name] nvarchar(255) NULL,
    [address] nvarchar(255) NULL,
    [city] nvarchar(100) NULL,
    [state] nvarchar(100) NULL,
    [postal_code] nvarchar(20) NULL,
    [country] nvarchar(100) NULL,
    [email] nvarchar(255) NOT NULL,
    [phone] nvarchar(20) NULL,
    [comments] nvarchar(max) NULL,
    [status] nvarchar(20) NULL,
    [submitted_at] datetime2 NULL,
    [handled_by] int NULL,
    CONSTRAINT [PK_QuoteRequest] PRIMARY KEY ([quote_id])
);

CREATE TABLE [ResumeFile] (
    [resume_id] int NOT NULL IDENTITY,
    [candidate_id] int NULL,
    [storage_key] nvarchar(255) NOT NULL,
    [original_name] nvarchar(255) NULL,
    [mime_type] nvarchar(100) NULL,
    [size] int NULL,
    [uploaded_at] datetime2 NULL,
    [is_current] bit NULL,
    CONSTRAINT [PK_ResumeFile] PRIMARY KEY ([resume_id])
);

CREATE TABLE [TabletSpecification] (
    [product_id] int NOT NULL,
    [model_number] nvarchar(100) NULL,
    [dies] int NULL,
    [max_pressure] decimal(10,2) NULL,
    [max_diameter_mm] decimal(10,2) NULL,
    [max_depth_fill_mm] decimal(10,2) NULL,
    [production_capacity] decimal(10,2) NULL,
    [machine_size] nvarchar(100) NULL,
    [net_weight_kg] decimal(10,2) NULL,
    CONSTRAINT [PK_TabletSpecification] PRIMARY KEY ([product_id])
);

CREATE TABLE [UserAccount] (
    [user_id] int NOT NULL IDENTITY,
    [email] nvarchar(255) NOT NULL,
    [password_hash] nvarchar(255) NOT NULL,
    [role] nvarchar(20) NOT NULL,
    [status] nvarchar(20) NOT NULL,
    [last_login_at] datetime2 NULL,
    CONSTRAINT [PK_UserAccount] PRIMARY KEY ([user_id])
);

CREATE TABLE [WorkExperience] (
    [experience_id] int NOT NULL IDENTITY,
    [candidate_id] int NULL,
    [employer] nvarchar(255) NULL,
    [title] nvarchar(255) NULL,
    [start_date] date NULL,
    [end_date] date NULL,
    [description] nvarchar(max) NULL,
    CONSTRAINT [PK_WorkExperience] PRIMARY KEY ([experience_id])
);

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

CREATE TABLE [ImageProduct] (
    [image_id] int NOT NULL IDENTITY,
    [product_id] int NULL,
    [url] nvarchar(500) NOT NULL,
    [display_order] int NULL,
    [is_thumbnail] bit NULL,
    CONSTRAINT [PK_ImageProduct] PRIMARY KEY ([image_id]),
    CONSTRAINT [FK_ImageProduct_Product_product_id] FOREIGN KEY ([product_id]) REFERENCES [Product] ([product_id])
);

CREATE INDEX [IX_Application_candidate_id] ON [Application] ([candidate_id]);

CREATE INDEX [IX_Application_PositionId] ON [Application] ([PositionId]);

CREATE INDEX [IX_ImageProduct_product_id] ON [ImageProduct] ([product_id]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260818131220_InitialApplication', N'10.0.8');

COMMIT;
GO

