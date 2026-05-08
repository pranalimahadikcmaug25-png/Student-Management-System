-- ============================================================
--  Student Management System — SQL Server Setup Script
--  Run this ONLY if you prefer manual DB setup over EF Migrations
-- ============================================================

-- 1. Create database (skip if it already exists)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'StudentManagementDB')
BEGIN
    CREATE DATABASE StudentManagementDB;
    PRINT 'Database StudentManagementDB created.';
END
GO

USE StudentManagementDB;
GO

-- ============================================================
-- 2. Users table  (for JWT authentication)
-- ============================================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES
               WHERE TABLE_NAME = 'Users')
BEGIN
    CREATE TABLE Users (
        Id           INT           IDENTITY(1,1) PRIMARY KEY,
        Username     NVARCHAR(100) NOT NULL,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        Role         NVARCHAR(50)  NOT NULL DEFAULT 'User',
        CreatedDate  DATETIME2     NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT UQ_Users_Username UNIQUE (Username)
    );
    PRINT 'Table [Users] created.';
END
GO

-- ============================================================
-- 3. Students table
-- ============================================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES
               WHERE TABLE_NAME = 'Students')
BEGIN
    CREATE TABLE Students (
        Id          INT           IDENTITY(1,1) PRIMARY KEY,
        Name        NVARCHAR(100) NOT NULL,
        Email       NVARCHAR(150) NOT NULL,
        Age         INT           NOT NULL,
        Course      NVARCHAR(100) NOT NULL,
        CreatedDate DATETIME2     NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT UQ_Students_Email UNIQUE (Email),
        CONSTRAINT CK_Students_Age   CHECK (Age BETWEEN 1 AND 120)
    );

    -- Index for fast look-ups by email
    CREATE NONCLUSTERED INDEX IX_Students_Email
        ON Students (Email);

    PRINT 'Table [Students] created.';
END
GO

-- ============================================================
-- 4. EF Core __EFMigrationsHistory stub
--    (Prevents EF from complaining if you run migrations later)
-- ============================================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES
               WHERE TABLE_NAME = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE __EFMigrationsHistory (
        MigrationId    NVARCHAR(150) NOT NULL PRIMARY KEY,
        ProductVersion NVARCHAR(32)  NOT NULL
    );
    PRINT 'Table [__EFMigrationsHistory] created.';
END
GO

-- ============================================================
-- 5. Sample seed data — Students
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Students)
BEGIN
    INSERT INTO Students (Name, Email, Age, Course, CreatedDate) VALUES
    ('Aarav Sharma',   'aarav.sharma@example.com',   20, 'Computer Science',       GETUTCDATE()),
    ('Priya Patel',    'priya.patel@example.com',    22, 'Information Technology', GETUTCDATE()),
    ('Rohan Mehta',    'rohan.mehta@example.com',    21, 'Electronics',            GETUTCDATE()),
    ('Sneha Gupta',    'sneha.gupta@example.com',    23, 'Mechanical Engineering', GETUTCDATE()),
    ('Vikram Singh',   'vikram.singh@example.com',   19, 'Civil Engineering',      GETUTCDATE());
    PRINT '5 sample students inserted.';
END
GO

-- ============================================================
-- 6. Useful diagnostic queries
-- ============================================================
-- Check all students
-- SELECT * FROM Students ORDER BY CreatedDate DESC;

-- Check all users
-- SELECT Id, Username, Role, CreatedDate FROM Users;

PRINT 'Setup script completed successfully.';
GO
