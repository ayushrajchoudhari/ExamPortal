USE ExamPortalDb;
GO

DECLARE @TenantId UNIQUEIDENTIFIER = NEWID();
DECLARE @AdminId UNIQUEIDENTIFIER = NEWID();
DECLARE @Student1 UNIQUEIDENTIFIER = NEWID();

DECLARE @Exam1 UNIQUEIDENTIFIER = NEWID(); 
DECLARE @Exam2 UNIQUEIDENTIFIER = NEWID(); 
DECLARE @Exam3 UNIQUEIDENTIFIER = NEWID(); 
DECLARE @Exam4 UNIQUEIDENTIFIER = NEWID(); 
DECLARE @Exam5 UNIQUEIDENTIFIER = NEWID(); 

DECLARE @E1Q1 UNIQUEIDENTIFIER = NEWID(), @E1Q2 UNIQUEIDENTIFIER = NEWID();
DECLARE @E1Q1_Correct UNIQUEIDENTIFIER = NEWID(), @E1Q2_Correct UNIQUEIDENTIFIER = NEWID();

INSERT INTO Tenants (Id, Name, Domain, IsActive) VALUES (@TenantId, 'Global Tech University', 'globaltech.edu', 1);

INSERT INTO Users (Id, TenantId, Role, Email, PasswordHash) VALUES 
(@AdminId, @TenantId, 'Admin', 'admin@globaltech.edu', 'hashed_pw_admin'),
(@Student1, @TenantId, 'User', 'student1@globaltech.edu', 'hashed_pw_stu1');

INSERT INTO ExamSets (Id, TenantId, Title, DurationMinutes, PassingScore, IsActive, CreatedByUserId, IsPublic, SecretToken) VALUES 
(@Exam1, @TenantId, 'C# and.NET 8 Advanced Architecture', 60, 50.00, 1, @AdminId, 1, NULL),
(@Exam2, @TenantId, 'Angular 20 & Signal State Management', 45, 60.00, 1, @AdminId, 1, NULL),
(@Exam3, @TenantId, 'Entity Framework Core Data Access', 30, 70.00, 1, @AdminId, 1, NULL),
(@Exam4, @TenantId, 'MS SQL Server Database Design', 90, 65.00, 1, @AdminId, 1, NULL),
(@Exam5, @TenantId, 'Azure DevOps & CI/CD Pipelines (PRIVATE)', 60, 50.00, 1, @AdminId, 0, 'SECRET-DEV-2026');

INSERT INTO InstructionSets (Id, ExamSetId, Content, AgreementRequired) VALUES 
(NEWID(), @Exam1, 'No pausing allowed. Window focus will be monitored.', 1),
(NEWID(), @Exam5, 'Questions cover Azure, Docker, and GitHub Actions.', 1);

INSERT INTO QuestionSets (Id, ExamSetId, QuestionText, Points, DisplayOrder) VALUES 
(@E1Q1, @Exam1, 'Which DI lifetime creates a new instance per HTTP request?', 10.00, 1),
(@E1Q2, @Exam1, 'What is the correct order for Auth middleware in Program.cs?', 10.00, 2);

INSERT INTO AnswerOptions (Id, QuestionSetId, OptionText, IsCorrect) VALUES 
(@E1Q1_Correct, @E1Q1, 'Scoped', 1), (NEWID(), @E1Q1, 'Transient', 0), (NEWID(), @E1Q1, 'Singleton', 0), (NEWID(), @E1Q1, 'Static', 0),
(NEWID(), @E1Q2, 'UseAuthorization() then UseAuthentication()', 0), (@E1Q2_Correct, @E1Q2, 'UseAuthentication() then UseAuthorization()', 1), (NEWID(), @E1Q2, 'Order does not matter', 0);
GO