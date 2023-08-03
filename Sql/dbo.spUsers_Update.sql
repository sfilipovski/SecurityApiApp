CREATE procedure [dbo].[spUsers_Update]
	@FirstName nvarchar(50),
	@LastName nvarchar(50),
	@Username nvarchar(50),
	@UserId int
as
begin
	update [dbo].[Users] 
	set FirstName = @FirstName, LastName = @LastName, Username = @Username
	where Id = @UserId

	select Username, FirstName, LastName
	from [dbo].[Users]
	where Id = @UserId
end