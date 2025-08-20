CREATE TABLE [scrims].[Scrims]
(
	ScrimId INT IDENTITY(1,1) PRIMARY KEY,
    Team1Id INT NOT NULL,
    Team2Id INT NULL,
    ScheduledDate DATETIME2 NOT NULL,
    Status INT DEFAULT 1,
    WinnerTeamId INT NULL,
    RequestedBy INT NOT NULL,
    CreatedDate DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Scrims_Team1 FOREIGN KEY (Team1Id) REFERENCES teams.Teams(TeamId),
    CONSTRAINT FK_Scrims_Team2 FOREIGN KEY (Team2Id) REFERENCES teams.Teams(TeamId),
    CONSTRAINT FK_Scrims_Winner FOREIGN KEY (WinnerTeamId) REFERENCES teams.Teams(TeamId),
    CONSTRAINT FK_Scrims_RequestedBy FOREIGN KEY (RequestedBy) REFERENCES auth.Users(UserId)
)
