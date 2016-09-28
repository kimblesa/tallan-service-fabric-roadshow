CREATE TABLE [dbo].[Inventory]
(
	[InventoryId] INT IDENTITY(1,1) PRIMARY KEY, 
    [Quantity] INT NOT NULL, 
    [Price] DECIMAL(10, 2) NOT NULL, 
    [Description] VARCHAR(50) NULL, 
    [Image] VARCHAR(200) NULL, 
    [Category] VARCHAR(50) NULL
)
