CREATE TABLE [auth].[Users] (
    [UserId]        INT            IDENTITY (1, 1) NOT NULL,
    [Username]      NVARCHAR (50)  NOT NULL,
    [Email]         NVARCHAR (100) NOT NULL,
    [PasswordHash]  NVARCHAR (255) NOT NULL,
    [Role]          INT            NOT NULL,
    [TeamId]        INT            NULL,
    [PlayerRole]    INT            NULL,
    [CreatedDate]   DATETIME2 (7)  DEFAULT (getdate()) NULL,
    [IsActive]      BIT            DEFAULT ((1)) NULL,
    [Name]          NVARCHAR (100) NOT NULL,
    [Surname]       NVARCHAR (100) NOT NULL,
    [ContactNumber] NVARCHAR (20)  NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC),
    UNIQUE NONCLUSTERED ([Email] ASC),
    UNIQUE NONCLUSTERED ([Username] ASC)
);

