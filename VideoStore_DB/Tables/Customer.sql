CREATE TABLE [dbo].[Customer]
(
	[CustomerId] INT IDENTITY(1,1) PRIMARY KEY, 
    [Email] VARCHAR(50) NULL, 
    [UserId] NVARCHAR(500) NULL 
)
