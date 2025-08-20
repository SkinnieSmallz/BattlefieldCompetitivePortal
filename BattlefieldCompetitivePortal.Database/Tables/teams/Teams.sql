﻿CREATE TABLE [teams].[Teams]
(
	TeamId INT IDENTITY(1,1) PRIMARY KEY,
    TeamName NVARCHAR(100) UNIQUE NOT NULL,
    CaptainId INT NOT NULL,
    CreatedDate DATETIME2 DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    CONSTRAINT FK_Teams_Captain FOREIGN KEY (CaptainId) REFERENCES auth.Users(UserId)
)
