ALTER VIEW [dbo].[v_log]
AS
SELECT dbo.[log].*,newid() as new_id
FROM   dbo.[log]