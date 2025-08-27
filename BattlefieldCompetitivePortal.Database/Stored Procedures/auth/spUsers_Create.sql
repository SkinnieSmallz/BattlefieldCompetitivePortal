CREATE PROCEDURE [auth].[spUsers_Create]
    -- Add the parameters for the stored procedure here
    @Username NVARCHAR(50),
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(255),
    @Name NVARCHAR(100),
    @Surname NVARCHAR(100),
    @ContactNumber NVARCHAR(20) = NULL,
    @Role INT = 3,
    @TeamId INT = NULL, -- Optional parameter, defaults to NULL
    @PlayerRole INT = NULL -- Optional parameter, defaults to NULL
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Basic check to prevent duplicate usernames, you can expand on this
    IF EXISTS (SELECT 1 FROM auth.Users WHERE Username = @Username)
    BEGIN
        -- Raiserror is a good way to send a specific error back to the application
        RAISERROR ('Username already exists.', 16, 1);
        RETURN;
    END

    -- Basic check to prevent duplicate emails
    IF EXISTS (SELECT 1 FROM auth.Users WHERE Email = @Email)
    BEGIN
        RAISERROR ('Email address is already in use.', 16, 1);
        RETURN;
    END

    -- Insert statement for the new user
    INSERT INTO auth.Users (
        Username,
        Email,
        PasswordHash,
        Name,
        Surname,
        ContactNumber,
        [Role], -- Role is a reserved keyword in some SQL versions, good to use brackets
        TeamId,
        PlayerRole,
        CreatedDate,
        IsActive
    )
    VALUES (
        @Username,
        @Email,
        @PasswordHash,
        @Name,
        @Surname,
        @ContactNumber,
        @Role,
        @TeamId,
        @PlayerRole,
        SYSDATETIME(), -- More precise than GETDATE()
        1 -- Hardcoded to active upon creation
    );

    -- Select the ID of the newly created user row
    -- SCOPE_IDENTITY() returns the last identity value inserted into an identity column
    -- in the same scope.
    SELECT SCOPE_IDENTITY() AS NewUserId;

END
GO
