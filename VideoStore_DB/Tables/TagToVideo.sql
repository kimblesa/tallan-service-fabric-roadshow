CREATE TABLE [dbo].[TagToVideo]
(
	[RelationId] INT IDENTITY(1,1) PRIMARY KEY,
	[TagId] INT NOT NULL , 
    [VideoId] INT NOT NULL, 
    CONSTRAINT [FK_Tag_Video_ToTag] FOREIGN KEY ([TagId]) REFERENCES Tag([TagId]), 
    CONSTRAINT [FK_Tag_Video_ToVideo] FOREIGN KEY ([VideoId]) REFERENCES Video([VideoId])
)
