CREATE TABLE [tournaments].[TournamentRegistrations]
(
	RegistrationId INT IDENTITY(1,1) PRIMARY KEY,
    TournamentId INT NOT NULL,
    TeamId INT NOT NULL,
    RegistrationDate DATETIME2 DEFAULT GETDATE(),
    IsApproved BIT DEFAULT 0,
    CONSTRAINT FK_TournamentReg_Tournament FOREIGN KEY (TournamentId) REFERENCES tournaments.Tournaments(TournamentId),
    CONSTRAINT FK_TournamentReg_Team FOREIGN KEY (TeamId) REFERENCES teams.Teams(TeamId),
    CONSTRAINT UQ_TournamentRegistration UNIQUE (TournamentId, TeamId)
)
