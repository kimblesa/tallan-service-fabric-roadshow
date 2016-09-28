CREATE TABLE [dbo].Video
(
	[VideoId] INT IDENTITY(1,1) PRIMARY KEY, 
    [Title] VARCHAR(50) NOT NULL, 
    [Length] INT NULL, 
    [Description] VARCHAR(500) NULL 
)
