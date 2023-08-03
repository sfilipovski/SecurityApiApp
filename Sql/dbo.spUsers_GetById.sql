create procedure [dbo].[spUsers_GetById]
	@UserId int
as
begin
	select Username, FirstName, LastName
	from [dbo].[Users]
	where Id = @UserId
end