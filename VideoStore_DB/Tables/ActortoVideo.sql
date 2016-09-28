CREATE TABLE [dbo].[ActorToVideo]
(
	[RelationId] INT IDENTITY(1,1) PRIMARY KEY,
	[ActorId] INT NOT NULL , 
    [VideoId] INT NOT NULL,     
    CONSTRAINT [FK_Actor_Video_ToVideo] FOREIGN KEY ([VideoId]) REFERENCES Video([VideoId]), 
    CONSTRAINT [FK_Actor_Video_ToActor] FOREIGN KEY ([ActorId]) REFERENCES Actor([ActorId])
)
