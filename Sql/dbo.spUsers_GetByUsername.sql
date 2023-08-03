CREATE procedure [dbo].[spUsers_GetByUsername]
	@Username nvarchar(50)
as
begin
	select *
	from [dbo].[Users]
	where Username = @Username
end