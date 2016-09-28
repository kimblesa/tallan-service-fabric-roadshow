CREATE TABLE [dbo].[Item]
(
	[InventoryId] INT NOT NULL PRIMARY KEY, 
    [Description] VARCHAR(500) NULL, 
    [Name] VARCHAR(50) NOT NULL, 
    CONSTRAINT [FK_Item_ToInventory] FOREIGN KEY ([InventoryId]) REFERENCES Inventory([InventoryId])
)
