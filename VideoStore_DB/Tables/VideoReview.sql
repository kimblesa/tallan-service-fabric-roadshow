CREATE TABLE [dbo].[VideoReview]
(
	[ReviewId] INT IDENTITY(1,1) PRIMARY KEY,
	[InventoryId] INT NOT NULL , 
    [Review] VARCHAR(500) NULL, 
    [Rating] SMALLINT NOT NULL, 
    [CustomerId] INT NOT NULL, 
    CONSTRAINT [FK_VideoReview_ToCustomer] FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId), 
    CONSTRAINT [FK_VideoReview_ToInventory] FOREIGN KEY (InventoryId) REFERENCES Inventory(InventoryId)
)

GO
