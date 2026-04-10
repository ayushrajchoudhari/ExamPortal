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

-- 1. Declare variables to maintain referential integrity across all inserts
DECLARE @TenantId UNIQUEIDENTIFIER = NEWID();
DECLARE @AdminUserId UNIQUEIDENTIFIER = NEWID();
DECLARE @StudentUserId UNIQUEIDENTIFIER = NEWID();
DECLARE @ExamSetId UNIQUEIDENTIFIER = NEWID();
DECLARE @InstructionSetId UNIQUEIDENTIFIER = NEWID();

DECLARE @Question1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Question2Id UNIQUEIDENTIFIER = NEWID();

DECLARE @Q1_OptionA UNIQUEIDENTIFIER = NEWID();
DECLARE @Q1_OptionB UNIQUEIDENTIFIER = NEWID();
DECLARE @Q1_OptionC UNIQUEIDENTIFIER = NEWID();
DECLARE @Q1_OptionD UNIQUEIDENTIFIER = NEWID();

DECLARE @Q2_OptionA UNIQUEIDENTIFIER = NEWID();
DECLARE @Q2_OptionB UNIQUEIDENTIFIER = NEWID();
DECLARE @Q2_OptionC UNIQUEIDENTIFIER = NEWID();
DECLARE @Q2_OptionD UNIQUEIDENTIFIER = NEWID();

DECLARE @AttemptId UNIQUEIDENTIFIER = NEWID();

-- 2. Insert Tenant
INSERT INTO Tenants (Id, Name, Domain, IsActive)
VALUES (@TenantId, 'Global Tech University', 'globaltech.edu', 1);

-- 3. Insert Users (1 Admin, 1 Student)
-- Note: Passwords should be hashed in a real application.
INSERT INTO Users (Id, TenantId, Role, Email, PasswordHash)
VALUES 
(@AdminUserId, @TenantId, 'Admin', 'admin@globaltech.edu', 'hashed_pw_admin_123'),
(@StudentUserId, @TenantId, 'User', 'student@globaltech.edu', 'hashed_pw_student_456');

-- 4. Insert ExamSet
INSERT INTO ExamSets (Id, TenantId, Title, DurationMinutes, PassingScore, IsActive)
VALUES (@ExamSetId, @TenantId, 'C# and.NET 8 Advanced Architecture', 60, 50.00, 1);

-- 5. Insert InstructionSet
INSERT INTO InstructionSets (Id, ExamSetId, Content, AgreementRequired)
VALUES (@InstructionSetId, @ExamSetId, 'Please read all questions carefully. You cannot pause the timer once started. Navigating away from the window will result in termination.', 1);

-- 6. Insert QuestionSets (2 Questions)
INSERT INTO QuestionSets (Id, ExamSetId, QuestionText, Points, DisplayOrder)
VALUES 
(@Question1Id, @ExamSetId, 'Which Entity Framework Core feature automatically applies filtering to all queries for a specific entity, making it ideal for Multi-Tenancy?', 10.00, 1),
(@Question2Id, @ExamSetId, 'What is the recommended Dependency Injection lifetime for an EF Core DbContext in an ASP.NET Core web application?', 10.00, 2);

-- 7. Insert AnswerOptions for Question 1
INSERT INTO AnswerOptions (Id, QuestionSetId, OptionText, IsCorrect)
VALUES 
(@Q1_OptionA, @Question1Id, 'Global Query Filters (HasQueryFilter)', 1),
(@Q1_OptionB, @Question1Id, 'Interceptor Pipeline', 0),
(@Q1_OptionC, @Question1Id, 'DbContext.SetTenant()', 0),
(@Q1_OptionD, @Question1Id, 'Data Annotations ()', 0);

-- 8. Insert AnswerOptions for Question 2
INSERT INTO AnswerOptions (Id, QuestionSetId, OptionText, IsCorrect)
VALUES 
(@Q2_OptionA, @Question2Id, 'Singleton', 0),
(@Q2_OptionB, @Question2Id, 'Transient', 0),
(@Q2_OptionC, @Question2Id, 'Scoped', 1),
(@Q2_OptionD, @Question2Id, 'Static', 0);

-- 9. Insert ExamAttempt (Simulating a completed exam by the student)
INSERT INTO ExamAttempts (Id, UserId, ExamSetId, StartTime, EndTime, TotalScore)
VALUES 
(@AttemptId, @StudentUserId, @ExamSetId, DATEADD(MINUTE, -45, GETUTCDATE()), GETUTCDATE(), 20.00);

-- 10. Insert UserAnswers (Student got both questions right)
INSERT INTO UserAnswers (Id, AttemptId, QuestionId, SelectedOptionId, IsCorrect)
VALUES 
(NEWID(), @AttemptId, @Question1Id, @Q1_OptionA, 1),
(NEWID(), @AttemptId, @Question2Id, @Q2_OptionC, 1);

GO