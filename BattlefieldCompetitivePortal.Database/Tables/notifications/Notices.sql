CREATE TABLE [notifications].[Notices]
(
	NoticeId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    CreatedBy INT NOT NULL,
    CreatedDate DATETIME2 DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    CONSTRAINT FK_Notices_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES auth.Users(UserId)
)
