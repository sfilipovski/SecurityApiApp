CREATE procedure [dbo].[spUsers_Create]
	@FirstName nvarchar(50),
	@LastName nvarchar(50),
	@Username nvarchar(50),
	@Role nvarchar(50),
	@PasswordHash nvarchar(100)
as
begin
	insert into [dbo].[Users] (FirstName, LastName, Username, Role, PasswordHash)
	values (@FirstName, @LastName, @Username, @Role, @PasswordHash)

	select Username, FirstName, LastName
	from [dbo].[Users]
	where Id = SCOPE_IDENTITY();
end