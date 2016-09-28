CREATE TABLE [dbo].[ProducerToVideo]
(
	[RelationId] INT IDENTITY(1,1) PRIMARY KEY,
	[ProducerId] INT NOT NULL , 
    [VideoId] INT NOT NULL, 
    CONSTRAINT [FK_Producer_Video_ToProducer] FOREIGN KEY ([ProducerId]) REFERENCES Producer([ProducerId]), 
    CONSTRAINT [FK_Producer_Video_ToVideo] FOREIGN KEY ([VideoId]) REFERENCES Video([VideoId])
)
