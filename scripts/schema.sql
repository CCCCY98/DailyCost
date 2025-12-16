-- DailyCost schema (from PRD)

CREATE TABLE IF NOT EXISTS Users (
    Id CHAR(36) PRIMARY KEY,
    Email VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Nickname VARCHAR(50),
    Avatar VARCHAR(500),
    DefaultCalcMode TINYINT DEFAULT 0,
    Currency VARCHAR(10) DEFAULT 'CNY',
    Timezone VARCHAR(50) DEFAULT 'Asia/Shanghai',
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NOT NULL,
    LastLoginAt DATETIME,
    IsDeleted BOOLEAN DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS Categories (
    Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36),
    Name VARCHAR(50) NOT NULL,
    Icon VARCHAR(100),
    Color VARCHAR(20),
    IsSystem BOOLEAN DEFAULT FALSE,
    SortOrder INT DEFAULT 0,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NOT NULL,
    IsDeleted BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

CREATE TABLE IF NOT EXISTS ExpenseItems (
    Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    CategoryId CHAR(36),
    FamilyId CHAR(36),
    Name VARCHAR(100) NOT NULL,
    Amount DECIMAL(12,2) NOT NULL,
    ExpenseType TINYINT NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE,
    ExpectedDays INT,
    BillingCycle TINYINT,
    AutoRenew BOOLEAN DEFAULT TRUE,
    NextRenewalDate DATE,
    CalcMode TINYINT,
    Status TINYINT DEFAULT 0,
    Note TEXT,
    ImageUrl VARCHAR(500),
    Tags VARCHAR(500),
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NOT NULL,
    DeletedAt DATETIME,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    INDEX idx_user_status (UserId, Status),
    INDEX idx_user_category (UserId, CategoryId)
);

CREATE TABLE IF NOT EXISTS Families (
    Id CHAR(36) PRIMARY KEY,
    Name VARCHAR(50) NOT NULL,
    InviteCode VARCHAR(20) UNIQUE,
    CreatedBy CHAR(36) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NOT NULL,
    IsDeleted BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (CreatedBy) REFERENCES Users(Id)
);

CREATE TABLE IF NOT EXISTS FamilyMembers (
    Id CHAR(36) PRIMARY KEY,
    FamilyId CHAR(36) NOT NULL,
    UserId CHAR(36) NOT NULL,
    Role TINYINT DEFAULT 0,
    JoinedAt DATETIME NOT NULL,
    FOREIGN KEY (FamilyId) REFERENCES Families(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    UNIQUE KEY uk_family_user (FamilyId, UserId)
);

CREATE TABLE IF NOT EXISTS RefreshTokens (
    Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    Token VARCHAR(500) NOT NULL,
    ExpiresAt DATETIME NOT NULL,
    CreatedAt DATETIME NOT NULL,
    RevokedAt DATETIME,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    INDEX idx_token (Token(255))
);

