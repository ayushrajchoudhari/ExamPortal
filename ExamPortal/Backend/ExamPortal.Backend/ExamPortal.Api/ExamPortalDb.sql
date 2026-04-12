USE master;
GO

DROP DATABASE IF EXISTS ExamPortalDb;
GO

CREATE DATABASE ExamPortalDb;
GO

USE ExamPortalDb;
GO

-- 1. Tenants Table
CREATE TABLE Tenants (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Domain NVARCHAR(100) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- 2. Users Table
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    TenantId UNIQUEIDENTIFIER NOT NULL,
    Role NVARCHAR(20) NOT NULL, -- 'Admin' or 'User'
    Email NVARCHAR(150) NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    CONSTRAINT FK_Users_Tenants FOREIGN KEY (TenantId) REFERENCES Tenants(Id) ON DELETE CASCADE
);
CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Email_Tenant ON Users(Email, TenantId);

-- 3. ExamSets Table
CREATE TABLE ExamSets (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    TenantId UNIQUEIDENTIFIER NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    DurationMinutes INT NOT NULL,
    PassingScore DECIMAL(5,2) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_ExamSets_Tenants FOREIGN KEY (TenantId) REFERENCES Tenants(Id) ON DELETE CASCADE
);
CREATE NONCLUSTERED INDEX IX_ExamSets_TenantId ON ExamSets(TenantId);

-- 4. InstructionSets Table
CREATE TABLE InstructionSets (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    ExamSetId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    Content NVARCHAR(1000) NOT NULL,
    AgreementRequired BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_InstructionSets_ExamSets FOREIGN KEY (ExamSetId) REFERENCES ExamSets(Id) ON DELETE CASCADE
);

-- 5. QuestionSets Table
CREATE TABLE QuestionSets (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    ExamSetId UNIQUEIDENTIFIER NOT NULL,
    QuestionText NVARCHAR(800) NOT NULL,
    Points DECIMAL(5,2) NOT NULL DEFAULT 1.0,
    DisplayOrder INT NOT NULL,
    CONSTRAINT FK_QuestionSets_ExamSets FOREIGN KEY (ExamSetId) REFERENCES ExamSets(Id) ON DELETE CASCADE
);
CREATE NONCLUSTERED INDEX IX_QuestionSets_ExamSetId ON QuestionSets(ExamSetId);

-- 6. AnswerOptions Table
CREATE TABLE AnswerOptions (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    QuestionSetId UNIQUEIDENTIFIER NOT NULL,
    OptionText NVARCHAR(500) NOT NULL,
    IsCorrect BIT NOT NULL,
    CONSTRAINT FK_AnswerOptions_QuestionSets FOREIGN KEY (QuestionSetId) REFERENCES QuestionSets(Id) ON DELETE CASCADE
);
CREATE NONCLUSTERED INDEX IX_AnswerOptions_QuestionSetId ON AnswerOptions(QuestionSetId);

-- 7. ExamAttempts Table
CREATE TABLE ExamAttempts (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    ExamSetId UNIQUEIDENTIFIER NOT NULL,
    StartTime DATETIME2 NOT NULL,
    EndTime DATETIME2 NULL,
    TotalScore DECIMAL(5,2) NULL,
    CONSTRAINT FK_ExamAttempts_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_ExamAttempts_ExamSets FOREIGN KEY (ExamSetId) REFERENCES ExamSets(Id) ON DELETE NO ACTION
);

-- 8. UserAnswers Table
CREATE TABLE UserAnswers (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    AttemptId UNIQUEIDENTIFIER NOT NULL,
    QuestionId UNIQUEIDENTIFIER NOT NULL,
    SelectedOptionId UNIQUEIDENTIFIER NOT NULL,
    IsCorrect BIT NOT NULL,
    CONSTRAINT FK_UserAnswers_ExamAttempts FOREIGN KEY (AttemptId) REFERENCES ExamAttempts(Id) ON DELETE CASCADE,
    CONSTRAINT FK_UserAnswers_QuestionSets FOREIGN KEY (QuestionId) REFERENCES QuestionSets(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_UserAnswers_AnswerOptions FOREIGN KEY (SelectedOptionId) REFERENCES AnswerOptions(Id) ON DELETE NO ACTION
);

/*** Inserting Dummy Data ***/
USE ExamPortalDb;
GO

-- 1. Declare Tenant and Users
DECLARE @TenantId UNIQUEIDENTIFIER = NEWID();
DECLARE @AdminId UNIQUEIDENTIFIER = NEWID();
DECLARE @Student1 UNIQUEIDENTIFIER = NEWID();
DECLARE @Student2 UNIQUEIDENTIFIER = NEWID();
DECLARE @Student3 UNIQUEIDENTIFIER = NEWID();
DECLARE @Student4 UNIQUEIDENTIFIER = NEWID();

-- 2. Declare ExamSets
DECLARE @Exam1 UNIQUEIDENTIFIER = NEWID(); --.NET 8
DECLARE @Exam2 UNIQUEIDENTIFIER = NEWID(); -- Angular 20
DECLARE @Exam3 UNIQUEIDENTIFIER = NEWID(); -- EF Core
DECLARE @Exam4 UNIQUEIDENTIFIER = NEWID(); -- SQL Server
DECLARE @Exam5 UNIQUEIDENTIFIER = NEWID(); -- DevOps

-- 3. Declare QuestionSets (5 per Exam)
DECLARE @E1Q1 UNIQUEIDENTIFIER = NEWID(), @E1Q2 UNIQUEIDENTIFIER = NEWID(), @E1Q3 UNIQUEIDENTIFIER = NEWID(), @E1Q4 UNIQUEIDENTIFIER = NEWID(), @E1Q5 UNIQUEIDENTIFIER = NEWID();
DECLARE @E2Q1 UNIQUEIDENTIFIER = NEWID(), @E2Q2 UNIQUEIDENTIFIER = NEWID(), @E2Q3 UNIQUEIDENTIFIER = NEWID(), @E2Q4 UNIQUEIDENTIFIER = NEWID(), @E2Q5 UNIQUEIDENTIFIER = NEWID();
DECLARE @E3Q1 UNIQUEIDENTIFIER = NEWID(), @E3Q2 UNIQUEIDENTIFIER = NEWID(), @E3Q3 UNIQUEIDENTIFIER = NEWID(), @E3Q4 UNIQUEIDENTIFIER = NEWID(), @E3Q5 UNIQUEIDENTIFIER = NEWID();
DECLARE @E4Q1 UNIQUEIDENTIFIER = NEWID(), @E4Q2 UNIQUEIDENTIFIER = NEWID(), @E4Q3 UNIQUEIDENTIFIER = NEWID(), @E4Q4 UNIQUEIDENTIFIER = NEWID(), @E4Q5 UNIQUEIDENTIFIER = NEWID();
DECLARE @E5Q1 UNIQUEIDENTIFIER = NEWID(), @E5Q2 UNIQUEIDENTIFIER = NEWID(), @E5Q3 UNIQUEIDENTIFIER = NEWID(), @E5Q4 UNIQUEIDENTIFIER = NEWID(), @E5Q5 UNIQUEIDENTIFIER = NEWID();

-- 4. Declare Correct Answer Options for Exam 1 (Needed to simulate the student attempt)
DECLARE @E1Q1_Correct UNIQUEIDENTIFIER = NEWID();
DECLARE @E1Q2_Correct UNIQUEIDENTIFIER = NEWID();
DECLARE @E1Q3_Correct UNIQUEIDENTIFIER = NEWID();
DECLARE @E1Q4_Correct UNIQUEIDENTIFIER = NEWID();
DECLARE @E1Q5_Correct UNIQUEIDENTIFIER = NEWID();
DECLARE @AttemptId UNIQUEIDENTIFIER = NEWID();

-- ==========================================
-- INSERT DATA
-- ==========================================

-- Insert Tenant
INSERT INTO Tenants (Id, Name, Domain, IsActive)
VALUES (@TenantId, 'Global Tech University', 'globaltech.edu', 1);

-- Insert Users (1 Admin, 4 Students)
INSERT INTO Users (Id, TenantId, Role, Email, PasswordHash)
VALUES 
(@AdminId, @TenantId, 'Admin', 'admin@globaltech.edu', 'hashed_pw_admin'),
(@Student1, @TenantId, 'User', 'student1@globaltech.edu', 'hashed_pw_stu1'),
(@Student2, @TenantId, 'User', 'student2@globaltech.edu', 'hashed_pw_stu2'),
(@Student3, @TenantId, 'User', 'student3@globaltech.edu', 'hashed_pw_stu3'),
(@Student4, @TenantId, 'User', 'student4@globaltech.edu', 'hashed_pw_stu4');

-- Insert 5 ExamSets
INSERT INTO ExamSets (Id, TenantId, Title, DurationMinutes, PassingScore, IsActive)
VALUES 
(@Exam1, @TenantId, 'C# and.NET 8 Advanced Architecture', 60, 50.00, 1),
(@Exam2, @TenantId, 'Angular 20 & Signal State Management', 45, 60.00, 1),
(@Exam3, @TenantId, 'Entity Framework Core Data Access', 30, 70.00, 1),
(@Exam4, @TenantId, 'MS SQL Server Database Design', 90, 65.00, 1),
(@Exam5, @TenantId, 'Azure DevOps & CI/CD Pipelines', 60, 50.00, 1);

-- Insert 5 InstructionSets
INSERT INTO InstructionSets (Id, ExamSetId, Content, AgreementRequired)
VALUES 
(NEWID(), @Exam1, 'No pausing allowed. Window focus will be monitored.', 1),
(NEWID(), @Exam2, 'Ensure stable internet. Signals knowledge required.', 1),
(NEWID(), @Exam3, 'Read carefully. EF Core 8 specific syntax applies.', 1),
(NEWID(), @Exam4, 'Database diagramming tools are not permitted.', 1),
(NEWID(), @Exam5, 'Questions cover Azure, Docker, and GitHub Actions.', 1);

-- Insert 25 QuestionSets
INSERT INTO QuestionSets (Id, ExamSetId, QuestionText, Points, DisplayOrder)
VALUES 
-- Exam 1 (.NET 8)
(@E1Q1, @Exam1, 'Which DI lifetime creates a new instance per HTTP request?', 10.00, 1),
(@E1Q2, @Exam1, 'What is the correct order for Auth middleware in Program.cs?', 10.00, 2),
(@E1Q3, @Exam1, 'Which C# 12 feature is best for immutable DTOs?', 10.00, 3),
(@E1Q4, @Exam1, 'How do you map a controller route dynamically?', 10.00, 4),
(@E1Q5, @Exam1, 'Which layer holds business logic in Clean Architecture?', 10.00, 5),
-- Exam 2 (Angular)
(@E2Q1, @Exam2, 'What feature replaces NgModules in modern Angular?', 10.00, 1),
(@E2Q2, @Exam2, 'Which Angular API handles synchronous state reactivity?', 10.00, 2),
(@E2Q3, @Exam2, 'How do you remove Zone.js in Angular 20?', 10.00, 3),
(@E2Q4, @Exam2, 'Which control flow block replaces *ngIf?', 10.00, 4),
(@E2Q5, @Exam2, 'What hook triggers when a Signal changes?', 10.00, 5),
-- Exam 3 (EF Core)
(@E3Q1, @Exam3, 'Which method prevents EF Core from tracking fetched entities?', 10.00, 1),
(@E3Q2, @Exam3, 'How do you enforce Multi-Tenancy automatically on all queries?', 10.00, 2),
(@E3Q3, @Exam3, 'Which command generates models from an existing database?', 10.00, 3),
(@E3Q4, @Exam3, 'What is the default tracking behavior of DbContext?', 10.00, 4),
(@E3Q5, @Exam3, 'Which interface intercepts database saves in EF Core?', 10.00, 5),
-- Exam 4 (SQL Server)
(@E4Q1, @Exam4, 'Which index determines the physical sorting of a table?', 10.00, 1),
(@E4Q2, @Exam4, 'What is the purpose of Normalization?', 10.00, 2),
(@E4Q3, @Exam4, 'Which JOIN returns only matched rows from both tables?', 10.00, 3),
(@E4Q4, @Exam4, 'What command builds an execution plan without running the query?', 10.00, 4),
(@E4Q5, @Exam4, 'Which constraint prevents orphaned child records?', 10.00, 5),
-- Exam 5 (DevOps)
(@E5Q1, @Exam5, 'What does CI stand for in CI/CD?', 10.00, 1),
(@E5Q2, @Exam5, 'Which file defines a Docker image environment?', 10.00, 2),
(@E5Q3, @Exam5, 'What is a Blue/Green deployment?', 10.00, 3),
(@E5Q4, @Exam5, 'In GitHub Actions, what triggers a workflow?', 10.00, 4),
(@E5Q5, @Exam5, 'Where should sensitive connection strings be stored in Azure?', 10.00, 5);

-- Insert Answer Options for Exam 1 (Using predefined Correct IDs)
INSERT INTO AnswerOptions (Id, QuestionSetId, OptionText, IsCorrect) VALUES 
(@E1Q1_Correct, @E1Q1, 'Scoped', 1), (NEWID(), @E1Q1, 'Transient', 0), (NEWID(), @E1Q1, 'Singleton', 0), (NEWID(), @E1Q1, 'Static', 0),
(NEWID(), @E1Q2, 'UseAuthorization() then UseAuthentication()', 0), (@E1Q2_Correct, @E1Q2, 'UseAuthentication() then UseAuthorization()', 1), (NEWID(), @E1Q2, 'Order does not matter', 0), (NEWID(), @E1Q2, 'UseCors() then UseAuthorization()', 0),
(NEWID(), @E1Q3, 'Structs', 0), (NEWID(), @E1Q3, 'Classes', 0), (@E1Q3_Correct, @E1Q3, 'Records', 1), (NEWID(), @E1Q3, 'Tuples', 0),
(@E1Q4_Correct, @E1Q4, '")]', 1), (NEWID(), @E1Q4, '[ApiController]', 0), (NEWID(), @E1Q4, '[HttpGet]', 0), (NEWID(), @E1Q4, '', 0),
(NEWID(), @E1Q5, 'Infrastructure', 0), (NEWID(), @E1Q5, 'API', 0), (@E1Q5_Correct, @E1Q5, 'Domain/Application', 1), (NEWID(), @E1Q5, 'Database', 0);

-- Insert Answer Options for Exam 2
INSERT INTO AnswerOptions (Id, QuestionSetId, OptionText, IsCorrect) VALUES 
(NEWID(), @E2Q1, 'Standalone Components', 1), (NEWID(), @E2Q1, 'Directives', 0), (NEWID(), @E2Q1, 'Pipes', 0), (NEWID(), @E2Q1, 'Services', 0),
(NEWID(), @E2Q2, 'RxJS', 0), (NEWID(), @E2Q2, 'Promises', 0), (NEWID(), @E2Q2, 'Signals', 1), (NEWID(), @E2Q2, 'EventEmitters', 0),
(NEWID(), @E2Q3, 'provideZonelessChangeDetection()', 1), (NEWID(), @E2Q3, 'removeZone()', 0), (NEWID(), @E2Q3, 'disableZoneJs()', 0), (NEWID(), @E2Q3, 'NgZone.disable()', 0),
(NEWID(), @E2Q4, '@if', 1), (NEWID(), @E2Q4, '*ngIf', 0), (NEWID(), @E2Q4, 'ng-template', 0), (NEWID(), @E2Q4, 'ng-container', 0),
(NEWID(), @E2Q5, 'effect()', 1), (NEWID(), @E2Q5, 'ngOnInit()', 0), (NEWID(), @E2Q5, 'ngOnChanges()', 0), (NEWID(), @E2Q5, 'subscribe()', 0);

-- Insert Answer Options for Exam 3
INSERT INTO AnswerOptions (Id, QuestionSetId, OptionText, IsCorrect) VALUES 
(NEWID(), @E3Q1, 'AsNoTracking()', 1), (NEWID(), @E3Q1, 'NoCache()', 0), (NEWID(), @E3Q1, 'IgnoreQueryFilters()', 0), (NEWID(), @E3Q1, 'ToList()', 0),
(NEWID(), @E3Q2, 'Global Query Filters', 1), (NEWID(), @E3Q2, 'Where() clause', 0), (NEWID(), @E3Q2, 'Stored Procedures', 0), (NEWID(), @E3Q2, 'Data Annotations', 0),
(NEWID(), @E3Q3, 'Scaffold-DbContext', 1), (NEWID(), @E3Q3, 'Add-Migration', 0), (NEWID(), @E3Q3, 'Update-Database', 0), (NEWID(), @E3Q3, 'Generate-Models', 0),
(NEWID(), @E3Q4, 'Tracking', 1), (NEWID(), @E3Q4, 'NoTracking', 0), (NEWID(), @E3Q4, 'Lazy Loading', 0), (NEWID(), @E3Q4, 'Disconnected', 0),
(NEWID(), @E3Q5, 'ISaveChangesInterceptor', 1), (NEWID(), @E3Q5, 'IDbCommandInterceptor', 0), (NEWID(), @E3Q5, 'IQueryable', 0), (NEWID(), @E3Q5, 'IDbContextOptions', 0);

-- Insert Answer Options for Exam 4
INSERT INTO AnswerOptions (Id, QuestionSetId, OptionText, IsCorrect) VALUES 
(NEWID(), @E4Q1, 'Clustered Index', 1), (NEWID(), @E4Q1, 'Non-Clustered Index', 0), (NEWID(), @E4Q1, 'Columnstore Index', 0), (NEWID(), @E4Q1, 'Hash Index', 0),
(NEWID(), @E4Q2, 'Reduce Redundancy', 1), (NEWID(), @E4Q2, 'Increase Speed', 0), (NEWID(), @E4Q2, 'Encrypt Data', 0), (NEWID(), @E4Q2, 'Backup Database', 0),
(NEWID(), @E4Q3, 'INNER JOIN', 1), (NEWID(), @E4Q3, 'LEFT JOIN', 0), (NEWID(), @E4Q3, 'RIGHT JOIN', 0), (NEWID(), @E4Q3, 'FULL OUTER JOIN', 0),
(NEWID(), @E4Q4, 'SET SHOWPLAN_ALL ON', 1), (NEWID(), @E4Q4, 'EXPLAIN', 0), (NEWID(), @E4Q4, 'ANALYZE', 0), (NEWID(), @E4Q4, 'DBCC FREEPROCCACHE', 0),
(NEWID(), @E4Q5, 'Foreign Key', 1), (NEWID(), @E4Q5, 'Primary Key', 0), (NEWID(), @E4Q5, 'Unique Key', 0), (NEWID(), @E4Q5, 'Check Constraint', 0);

-- Insert Answer Options for Exam 5
INSERT INTO AnswerOptions (Id, QuestionSetId, OptionText, IsCorrect) VALUES 
(NEWID(), @E5Q1, 'Continuous Integration', 1), (NEWID(), @E5Q1, 'Code Injection', 0), (NEWID(), @E5Q1, 'Continuous Inspection', 0), (NEWID(), @E5Q1, 'Code Integration', 0),
(NEWID(), @E5Q2, 'Dockerfile', 1), (NEWID(), @E5Q2, 'docker-compose.yml', 0), (NEWID(), @E5Q2, '.gitignore', 0), (NEWID(), @E5Q2, 'appsettings.json', 0),
(NEWID(), @E5Q3, 'Two identical production environments', 1), (NEWID(), @E5Q3, 'A UI testing strategy', 0), (NEWID(), @E5Q3, 'A database backup method', 0), (NEWID(), @E5Q3, 'A Git branching model', 0),
(NEWID(), @E5Q4, 'Events (like push or pull_request)', 1), (NEWID(), @E5Q4, 'Manual clicks only', 0), (NEWID(), @E5Q4, 'Database triggers', 0), (NEWID(), @E5Q4, 'Docker builds', 0),
(NEWID(), @E5Q5, 'Azure Key Vault', 1), (NEWID(), @E5Q5, 'appsettings.json', 0), (NEWID(), @E5Q5, 'GitHub Repository', 0), (NEWID(), @E5Q5, 'Hardcoded in C#', 0);

-- Insert ExamAttempt (Student 1 taking Exam 1)
INSERT INTO ExamAttempts (Id, UserId, ExamSetId, StartTime, EndTime, TotalScore)
VALUES (@AttemptId, @Student1, @Exam1, DATEADD(MINUTE, -55, GETUTCDATE()), GETUTCDATE(), 50.00);

-- Insert UserAnswers (Student 1 got 4 right, 1 wrong)
INSERT INTO UserAnswers (Id, AttemptId, QuestionId, SelectedOptionId, IsCorrect) VALUES 
(NEWID(), @AttemptId, @E1Q1, @E1Q1_Correct, 1),
(NEWID(), @AttemptId, @E1Q2, @E1Q2_Correct, 1),
(NEWID(), @AttemptId, @E1Q3, @E1Q3_Correct, 1),
(NEWID(), @AttemptId, @E1Q4, @E1Q4_Correct, 1),
(NEWID(), @AttemptId, @E1Q5, (SELECT TOP 1 Id FROM AnswerOptions WHERE QuestionSetId = @E1Q5 AND IsCorrect = 0), 0);

GO