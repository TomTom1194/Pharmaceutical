-- Adds the ApplicationLog table: a frozen snapshot of the candidate's
-- profile (CandidateProfile fields, Education, WorkExperience, current
-- ResumeFile) taken at the moment they applied to a position. One row per
-- Application, written once by CandidateController.ApplyToPosition and
-- never updated afterward — so editing your profile after applying no
-- longer rewrites what a recruiter sees for an already-submitted application.
--
-- Run this once against your database (e.g. in SSMS / Azure Data Studio,
-- or `sqlcmd -S <server> -d <db> -i add_application_log_table.sql`).

IF OBJECT_ID(N'[ApplicationLog]') IS NULL
BEGIN
    CREATE TABLE [ApplicationLog] (
        [log_id] int NOT NULL IDENTITY,
        [application_id] int NOT NULL,
        [full_name] nvarchar(255) NULL,
        [phone] nvarchar(50) NULL,
        [address] nvarchar(255) NULL,
        [summary] nvarchar(max) NULL,
        [profile_image] nvarchar(255) NULL,
        [educations_json] nvarchar(max) NULL,
        [work_experiences_json] nvarchar(max) NULL,
        [resume_original_name] nvarchar(255) NULL,
        [resume_storage_key] nvarchar(255) NULL,
        [resume_mime_type] nvarchar(100) NULL,
        [resume_size] int NULL,
        [resume_uploaded_at] datetime2 NULL,
        [logged_at] datetime2 NOT NULL,
        CONSTRAINT [PK_ApplicationLog] PRIMARY KEY ([log_id]),
        CONSTRAINT [FK_ApplicationLog_Application_application_id] FOREIGN KEY ([application_id]) REFERENCES [Application] ([application_id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_ApplicationLog_application_id] ON [ApplicationLog] ([application_id]);
END
