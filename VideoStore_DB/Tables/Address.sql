CREATE TABLE [dbo].[Address]
(
	[AddressId] INT IDENTITY(1,1) PRIMARY KEY, 
    [AddressType] VARCHAR(50) NOT NULL, 
    [Road] VARCHAR(50) NOT NULL, 
    [Town] VARCHAR(50) NOT NULL, 
    [State] VARCHAR(50) NOT NULL, 
    [ZipCode] VARCHAR(10) NOT NULL, 
    [CustomerId] INT NOT NULL, 
    CONSTRAINT [FK_Address_ToCustomer] FOREIGN KEY ([CustomerId]) REFERENCES Customer(CustomerId)
)
