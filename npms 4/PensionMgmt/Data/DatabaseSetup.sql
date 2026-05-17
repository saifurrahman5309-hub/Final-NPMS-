-- ============================================================
--  PensionDB  -  Database Setup Script
--  Run once in SQL Server Management Studio (SSMS)
--  against your SQLEXPRESS instance.
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'newdbnpms6')
    CREATE DATABASE newdbnpms6;
GO

USE newdbnpms6;
GO

-- ── User/Auth tables ─────────────────────────────────────────

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='SystemAdmin')
CREATE TABLE SystemAdmin (
    Id       INT IDENTITY PRIMARY KEY,
    UserName NVARCHAR(20)  NOT NULL UNIQUE,
    Password NVARCHAR(10)  NOT NULL
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='PensionAdmin')
CREATE TABLE PensionAdmin (
    Id       INT IDENTITY PRIMARY KEY,
    UserName NVARCHAR(20)  NOT NULL UNIQUE,
    Password NVARCHAR(10)  NOT NULL
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='Manager')
CREATE TABLE Manager (
    Id       INT IDENTITY PRIMARY KEY,
    UserName NVARCHAR(20)  NOT NULL UNIQUE,
    Password NVARCHAR(10)  NOT NULL
);

-- PensionHolder replaces Viewer. Each holder is linked to an employee record.
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='PensionHolder')
CREATE TABLE PensionHolder (
    Id           INT IDENTITY PRIMARY KEY,
    UserName     NVARCHAR(20)  NOT NULL UNIQUE,
    Password     NVARCHAR(10)  NOT NULL,
    EmployeeId   NVARCHAR(20)  NULL   -- links to Employees.EmployeeId
);

-- ── System-managed user list (approved users) ────────────────

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='Users')
CREATE TABLE Users (
    Id         INT IDENTITY PRIMARY KEY,
    FullName   NVARCHAR(100) NOT NULL,
    EmployeeId NVARCHAR(20)  NOT NULL UNIQUE,
    Role       NVARCHAR(30)  NOT NULL,
    Password   NVARCHAR(10)  NOT NULL
);

-- ── Pending Registrations (awaiting admin approval) ──────────
--   System Admin requests can only be approved by an existing System Admin.
--   All other roles can be approved by System Admin or Pension Admin.

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='PendingRegistrations')
CREATE TABLE PendingRegistrations (
    Id            INT IDENTITY PRIMARY KEY,
    FullName      NVARCHAR(100) NOT NULL,
    EmployeeId    NVARCHAR(20)  NOT NULL,
    RequestedRole NVARCHAR(30)  NOT NULL DEFAULT 'Pension Holder',
    Password      NVARCHAR(10)  NOT NULL,
    RequestedOn   DATETIME      NOT NULL DEFAULT GETDATE(),
    Status        NVARCHAR(20)  NOT NULL DEFAULT 'Pending'
    -- Status values: 'Pending', 'Approved', 'Rejected'
);

-- ── Employees ────────────────────────────────────────────────

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='Employees')
CREATE TABLE Employees (
    Id           INT IDENTITY PRIMARY KEY,
    FullName     NVARCHAR(100) NOT NULL,
    EmployeeId   NVARCHAR(20)  NOT NULL UNIQUE,
    Department   NVARCHAR(100) NOT NULL,
    Rank         NVARCHAR(50)  NOT NULL DEFAULT 'Assistant',
    BasicSalary  DECIMAL(12,2) NOT NULL,
    DateOfBirth  DATE          NOT NULL,
    JoiningDate  DATE          NOT NULL,
    Status       NVARCHAR(20)  NOT NULL DEFAULT 'Active'
);

-- ── Payouts ──────────────────────────────────────────────────

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='Payouts')
CREATE TABLE Payouts (
    Id             INT IDENTITY PRIMARY KEY,
    EmployeeId     NVARCHAR(20)  NOT NULL,
    FullName       NVARCHAR(100) NOT NULL,
    MonthlyPension DECIMAL(12,2) NOT NULL,
    LumpSum        DECIMAL(12,2) NOT NULL,
    ProcessedDate  DATE          NOT NULL DEFAULT GETDATE(),
    Notes          NVARCHAR(300)
);

-- ── Default System Admin account ─────────────────────────────
-- Login: 1000000001  Password: 123456

IF NOT EXISTS (SELECT * FROM SystemAdmin WHERE UserName='1000000001')
    INSERT INTO SystemAdmin (UserName, Password) VALUES ('1000000001', '123456');

IF NOT EXISTS (SELECT * FROM Users WHERE EmployeeId='1000000001')
    INSERT INTO Users (FullName, EmployeeId, Role, Password)
    VALUES ('Default System Admin', '1000000001', 'System Admin', '123456');

PRINT 'newdbnpms6 setup complete.';
PRINT 'Default System Admin  =>  Employee ID: 1000000001  |  Password: 123456';
GO
