CREATE TABLE [notifications].[Notifications]
(
	NotificationId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    NotificationType INT NOT NULL,
    IsRead BIT DEFAULT 0,
    CreatedDate DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Notifications_User FOREIGN KEY (UserId) REFERENCES auth.Users(UserId)
)
