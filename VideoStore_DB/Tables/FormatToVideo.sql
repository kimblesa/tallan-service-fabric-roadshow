CREATE TABLE [dbo].FormatToVideo
(
	[RelationId] INT IDENTITY(1,1) PRIMARY KEY,
	[FormatId] INT NOT NULL , 
    [VideoId] INT NOT NULL, 
    [InventoryId] INT NOT NULL, 
    CONSTRAINT [FK_Format_Video_ToVideo] FOREIGN KEY ([VideoId]) REFERENCES Video([VideoId]), 
    CONSTRAINT [FK_Format_Video_ToMovieFormat] FOREIGN KEY ([FormatId]) REFERENCES VideoFormat([FormatId]), 
    CONSTRAINT [FK_FormatToVideo_ToInventory] FOREIGN KEY (InventoryId) REFERENCES Inventory(InventoryId)
	
)
