CREATE procedure [dbo].[spUsers_Delete]
	
	@UserId int
as
begin
	delete 
	from [dbo].[Users]
	where Id = @UserId
end