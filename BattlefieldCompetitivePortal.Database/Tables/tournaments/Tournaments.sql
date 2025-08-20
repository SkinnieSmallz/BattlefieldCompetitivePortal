CREATE TABLE [tournaments].[Tournaments]
(
	TournamentId INT IDENTITY(1,1) PRIMARY KEY,
    TournamentName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NOT NULL,
    RegistrationDeadline DATETIME2 NOT NULL,
    MaxTeams INT NOT NULL,
    Status INT DEFAULT 1,
    CreatedBy INT NOT NULL,
    CreatedDate DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Tournaments_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES auth.Users(UserId)
)
