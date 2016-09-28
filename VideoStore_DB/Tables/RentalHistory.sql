CREATE TABLE [dbo].[RentalHistory]
(
	[TransactionId] INT PRIMARY KEY,
	[CustomerId] INT NOT NULL,
    [InventoryId] INT NOT NULL, 
    [VideoId] VARCHAR(10) NOT NULL, 
    [StartDate] DATE NULL, 
    [EndDate] DATE NULL, 
    CONSTRAINT [FK_RentalHistory_ToTransactionHistory] FOREIGN KEY (TransactionId) REFERENCES TransactionHistory(TransactionId)
)
