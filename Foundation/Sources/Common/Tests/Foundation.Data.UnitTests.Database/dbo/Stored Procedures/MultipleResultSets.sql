-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE dbo.MultipleResultSets
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	create table #temp
	(
		number int
	)

	insert into #temp (number) 	values(1)
	insert into #temp (number) 	values(2)
	insert into #temp (number) 	values(3)
	insert into #temp (number) 	values(4)
	insert into #temp (number) 	values(5)
	insert into #temp (number) 	values(6)
	insert into #temp (number) 	values(7)
	insert into #temp (number) 	values(8)
	insert into #temp (number) 	values(9)
	insert into #temp (number) 	values(10)
	insert into #temp (number) 	values(11)
	insert into #temp (number) 	values(12)
	insert into #temp (number) 	values(13)

	select * from #temp where number < 5 and number > 0
 	select * from #temp where number < 9 and number >= 5
	select * from #temp where number < 13 and number >= 9
	select * from #temp where number < 21 and number >= 13

	drop table #temp
END