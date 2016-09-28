CREATE TABLE [dbo].[TransactionHistory]
(
	[TransactionId] INT IDENTITY(1,1) PRIMARY KEY , 
	[CustomerId] INT NOT NULL, 
    [InventoryId] INT NOT NULL, 
    [Date] DATE NULL
)

GO
