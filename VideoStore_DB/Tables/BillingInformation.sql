CREATE TABLE [dbo].[BillingInformation]
(
	[BillingId] INT IDENTITY(1,1) PRIMARY KEY, 
    [CreditCard] VARCHAR(16) NOT NULL, 
    [AddressId] INT NOT NULL, 
    [Name] VARCHAR(50) NOT NULL, 
    CONSTRAINT [FK_BillingInformation_ToAddress] FOREIGN KEY (AddressId) REFERENCES Address(AddressId)
)
