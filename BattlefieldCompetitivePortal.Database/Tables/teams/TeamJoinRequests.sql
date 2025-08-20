CREATE TABLE [teams].[TeamJoinRequests]
(
	RequestId INT IDENTITY(1,1) PRIMARY KEY,
    PlayerId INT NOT NULL,
    TeamId INT NOT NULL,
    RequestDate DATETIME2 DEFAULT GETDATE(),
    Status INT DEFAULT 1,
    CONSTRAINT FK_TeamJoinReq_Player FOREIGN KEY (PlayerId) REFERENCES auth.Users(UserId),
    CONSTRAINT FK_TeamJoinReq_Team FOREIGN KEY (TeamId) REFERENCES teams.Teams(TeamId)
)
