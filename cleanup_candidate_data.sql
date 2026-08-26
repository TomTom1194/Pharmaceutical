-- Wipes all candidate/recruitment data and every UserAccount except
-- admin@pharma.com. Deletes in child-to-parent order so it works whether or
-- not FK constraints are actually enforced in your DB.
--
-- !! THIS IS DESTRUCTIVE AND CANNOT BE UNDONE. Back up your DB first, e.g.:
--    docker exec <container> /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P '<pwd>' -C \
--      -Q "BACKUP DATABASE [YourDbName] TO DISK = N'/var/opt/mssql/backup/before_cleanup.bak'"
--
-- Run with sqlcmd, SSMS, or Azure Data Studio against your DB, e.g.:
--    sqlcmd -S <server> -d <db> -i cleanup_candidate_data.sql

BEGIN TRANSACTION;

DELETE FROM [ApplicationLog];
DELETE FROM [InterviewInvitation];
DELETE FROM [Application];
DELETE FROM [ResumeFile];
DELETE FROM [EducationRecord];
DELETE FROM [WorkExperience];
DELETE FROM [CandidateProfile];
DELETE FROM [UserAccount] WHERE [email] <> 'admin@pharma.com';

-- Reset IDENTITY counters on the tables that are now fully empty, so new
-- rows start back at 1. CandidateProfile isn't an IDENTITY column (its id is
-- the same as UserAccount.user_id), and UserAccount still has a row left, so
-- neither is reseeded here.
DBCC CHECKIDENT ('ApplicationLog', RESEED, 0);
DBCC CHECKIDENT ('InterviewInvitation', RESEED, 0);
DBCC CHECKIDENT ('Application', RESEED, 0);
DBCC CHECKIDENT ('ResumeFile', RESEED, 0);
DBCC CHECKIDENT ('EducationRecord', RESEED, 0);
DBCC CHECKIDENT ('WorkExperience', RESEED, 0);

COMMIT;

PRINT 'Done. Remaining UserAccount rows:';
SELECT [user_id], [email], [role] FROM [UserAccount];
