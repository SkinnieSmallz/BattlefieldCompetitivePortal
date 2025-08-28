CREATE PROCEDURE auth.[spUsers_ValidateLogin]
	@Username NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

SELECT
	UserId,
	Username,
	Email,
	Name,
	Surname,
	ContactNumber,
	PasswordHash,
	Role,
	TeamId,
	PlayerRole,
	CreatedDate,
	IsActive
FROM auth.Users
WHERE Username = @Username
	AND IsActive = 1;
END

