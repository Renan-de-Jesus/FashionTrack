CREATE DATABASE FASHIONTRACK
GO

USE FASHIONTRACK
GO

CREATE TABLE Users (
    ID_Users INT IDENTITY(1,1),
    FullName VARCHAR(100) NOT NULL,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Password VARBINARY(64) NOT NULL,
    Adm BIT NOT NULL,
    CONSTRAINT PK_Usuario PRIMARY KEY (ID_Users)
);

INSERT INTO Users (FullName, Username, Password, Adm) 
VALUES ('Admin User', 'admin', HASHBYTES('SHA2_256', 'adminpassword'), 1);

CREATE TABLE City (
    ID_City INT IDENTITY(1,1),
    Description VARCHAR(100) NOT NULL,
    UF CHAR(2) NOT NULL,
    CONSTRAINT PK_City PRIMARY KEY (ID_City)
);

INSERT INTO City (Description, UF) 
VALUES ('São Paulo', 'SP');

CREATE TABLE Customer (
    ID_Customer INT IDENTITY(1,1),
    Name VARCHAR(50) NOT NULL,
    Surname VARCHAR(50) NOT NULL,
    CPF CHAR(11) NOT NULL UNIQUE,
    Cellphone VARCHAR(15) NOT NULL,
    Address VARCHAR(150) NOT NULL,
    ID_City INT NOT NULL,
    CONSTRAINT PK_Customer PRIMARY KEY (ID_Customer),
    CONSTRAINT FK_Customer_Cidade FOREIGN KEY (ID_City) REFERENCES City(ID_City)
);

INSERT INTO Customer (Name, Surname, CPF, Cellphone, Address, ID_City) 
VALUES ('João', 'Silva', '12345678901', '11987654321', 'Rua A, 123', 1);

CREATE TABLE Supplier (
    ID_Supplier INT IDENTITY(1,1),
    CorporateName VARCHAR(100) NOT NULL,
    CNPJ CHAR(14) NOT NULL UNIQUE,
    Address VARCHAR(150) NOT NULL,
    Telephone VARCHAR(15) NOT NULL,
    Representative NVARCHAR(150) NULL,
    ID_City INT NOT NULL,
    CONSTRAINT PK_Supplier PRIMARY KEY (ID_Supplier),
    CONSTRAINT FK_Supplier_City FOREIGN KEY (ID_City) REFERENCES City(ID_City)
);

INSERT INTO Supplier (CorporateName, CNPJ, Address, Telephone, Representative, ID_City) 
VALUES ('Fornecedor Exemplo LTDA', '12345678000195', 'Avenida B, 456', '11876543210', 'Hugo', 1);

CREATE TABLE Brand (
    BrandId INT PRIMARY KEY IDENTITY(1,1),
    BrandName NVARCHAR(100) NOT NULL
);

INSERT INTO Brand (BrandName) 
VALUES ('Marca Exemplo');

CREATE TABLE Color (
    ColorId INT PRIMARY KEY IDENTITY(1,1),
    ColorName NVARCHAR(50) NOT NULL
);

INSERT INTO Color (ColorName) 
VALUES ('Vermelho');

CREATE TABLE Size (
    SizeId INT PRIMARY KEY IDENTITY(1,1),
    SizeDescription NVARCHAR(3) NOT NULL
);

INSERT INTO Size (SizeDescription) 
VALUES ('M');

CREATE TABLE Product (
    ID_Product INT IDENTITY(1,1),
    BrandCode VARCHAR(50),
    BrandId INT,
    ColorId INT,
    Description VARCHAR(255) NOT NULL,
    SizeId INT,
    Gender VARCHAR(10) NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    CONSTRAINT PK_Product PRIMARY KEY (ID_Product),
    CONSTRAINT FK_Product_Brand FOREIGN KEY (BrandId) REFERENCES Brand(BrandId),
    CONSTRAINT FK_Product_Color FOREIGN KEY (ColorId) REFERENCES Color(ColorId),
    CONSTRAINT FK_Product_Size FOREIGN KEY (SizeId) REFERENCES Size(SizeId)
);

INSERT INTO Product (BrandCode, BrandId, ColorId, Description, SizeId, Gender, Price) 
VALUES ('MEX123', 1, 1, 'Camisa Vermelha', 1, 'Masculino', 59.90);

CREATE TABLE ProductSupplier (
    ID_Supplier INT,
    ID_Product INT,
    CONSTRAINT PK_ProductSupplier PRIMARY KEY (ID_Supplier, ID_Product),
    CONSTRAINT FK_ProductSuppler_Supplier FOREIGN KEY (ID_Supplier) REFERENCES Supplier(ID_Supplier),
    CONSTRAINT FK_SupplierProduct_Product FOREIGN KEY (ID_Product) REFERENCES Product(ID_Product),
    CONSTRAINT UQ_ProductSupplier UNIQUE (ID_Supplier, ID_Product)
);

INSERT INTO ProductSupplier (ID_Supplier, ID_Product) 
VALUES (1, 1);

CREATE TABLE Sell (
    ID_Sell INT IDENTITY(1,1),
    ID_Customer INT NOT NULL,
    Sell_Document INT NOT NULL,
    SellDate DATETIME NOT NULL CONSTRAINT DF_SellDate DEFAULT GETDATE(),
    TotalPrice DECIMAL(10, 2) NOT NULL,
    PaymentMethod VARCHAR(50) NOT NULL,
    CONSTRAINT PK_Sell PRIMARY KEY (ID_Sell),
    CONSTRAINT FK_Sell_Customer FOREIGN KEY (ID_Customer) REFERENCES Customer(ID_Customer)
);

INSERT INTO Sell (ID_Customer, Sell_Document, SellDate, PaymentMethod, TotalPrice) 
VALUES (1, 1, '31/10/2024', 'Debito',59.90);

CREATE TABLE ItemSell (
    ID_ItemSell INT IDENTITY(1,1),
    ID_Sell INT NOT NULL,
    ID_Product INT NOT NULL,
    Qty INT NOT NULL, 
    PartialPrice DECIMAL(10, 2) NOT NULL,
    CONSTRAINT PK_ITEMVENDAS PRIMARY KEY (ID_ItemSell),
    CONSTRAINT FK_ITEMVENDAS_PRODUTOS FOREIGN KEY (ID_Product) REFERENCES Product(ID_Product),
    CONSTRAINT FK_ITEMVENDAS_VENDAS FOREIGN KEY (ID_Sell) REFERENCES Sell(ID_Sell)
);

INSERT INTO ItemSell (ID_Sell, ID_Product, Qty, PartialPrice) 
VALUES (1, 1, 1, 59.90);

CREATE TABLE StockMovement (
    ID_StockMovement INT IDENTITY(1,1) PRIMARY KEY,
    MDescription NVARCHAR(100) NOT NULL, 
    Document INT NULL,
    MovementType NVARCHAR(10) NOT NULL, CHECK (MovementType IN ('Entrada', 'Saida')),
    Operation NVARCHAR(2) NOT NULL, CHECK (Operation IN ('E', 'S', 'T', 'A', 'P', 'R', 'D')),
    MovementDate DATETIME NOT NULL DEFAULT GETDATE(),
    ID_Users INT NULL,
    CONSTRAINT FK_Movement_User FOREIGN KEY (ID_Users) REFERENCES Users(ID_Users)
);

INSERT INTO StockMovement (MDescription, Document, MovementType, Operation, ID_Users) 
VALUES ('Movimento 1', 1, 'Entrada', 'E', 1);

CREATE TABLE Stock (
    ID_Stock INT IDENTITY(1,1) PRIMARY KEY,
    ID_Product INT NOT NULL,
    Qty INT NOT NULL,
    CONSTRAINT FK_Stock_Product FOREIGN KEY (ID_Product) REFERENCES Product(ID_Product)
);

INSERT INTO Stock (ID_Product, Qty) 
VALUES (1, 100);

CREATE TABLE ITEM_MOV (
    ID_Item_Mov INT IDENTITY (1,1), 
    ID_StockMovement INT NOT NULL,
    ID_Product INT NOT NULL,
    Qty_Mov INT NOT NULL,
    CONSTRAINT PK_Item_Mov PRIMARY KEY (ID_Item_Mov),
    CONSTRAINT FK_Stock_Movement FOREIGN KEY (ID_StockMovement) REFERENCES StockMovement(ID_StockMovement),
    CONSTRAINT FK_Mov_Product FOREIGN KEY (ID_Product) REFERENCES Product(ID_Product)
);

INSERT INTO ITEM_MOV (ID_StockMovement, ID_Product, Qty_Mov) VALUES (1, 1, 50);

--Procedure para INSERT dos produtos
CREATE PROCEDURE InsertProduct
	@Description NVARCHAR(255),
	@Price DECIMAL(18, 2),
	@BrandCode NVARCHAR(50),
	@Gender NVARCHAR(10),
	@BrandId INT,
	@ColorId INT,
	@SizeId INT
AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION;

		INSERT INTO Product (Description, Price, BrandCode, Gender, BrandId, ColorId, SizeId)
		VALUES (@Description, @Price, @BrandCode, @Gender, @BrandId, @ColorId, @SizeId);

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;
		THROW;
	END CATCH
END;

--Procedure para UPDATE dos produtos cadastrados
CREATE PROCEDURE UpdateProduct
    @ProductId INT,
    @Description NVARCHAR(255),
    @Price DECIMAL(10, 2),
    @BrandCode VARCHAR(50),
    @Gender VARCHAR(10),
    @BrandId INT,
    @ColorId INT,
    @SizeId INT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION

        UPDATE Product
        SET 
            Description = @Description,
            Price = @Price,
            BrandCode = @BrandCode,
            Gender = @Gender,
            BrandId = @BrandId,
            ColorId = @ColorId,
            SizeId = @SizeId
        WHERE ID_Product = @ProductId;

        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

--Trigger para controlar o After Insert do PRODUTO
CREATE TRIGGER trg_AfterInsertProduct
ON Product
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Para cada produto recém-inserido, cria o estoque com quantidade 0
    INSERT INTO Stock (ID_Product, Qty)
    SELECT ID_Product, 0
    FROM INSERTED;
END;

--Tabela para auditoria de mudança de nomes na tela de Produtos
CREATE TABLE ProductAudit (
	AuditId INT PRIMARY KEY IDENTITY,
	ProductId INT,
	OldDesc NVARCHAR (255),
	NewDesc NVARCHAR (255),
	OldPrice DECIMAL(10,2),
	NewPrice DECIMAL(10,2),
	ChangeDate DATETIME DEFAULT GETDATE()
);

--Trigger para registrar logs na tabela de ProductAudit
CREATE TRIGGER trg_AfterUpdateProduct
ON Product
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO ProductAudit (ProductId, OldDesc, NewDesc, OldPrice, NewPrice)
    SELECT 
        i.ID_Product,
        d.Description,
        i.Description,
        d.Price,
        i.Price
    FROM INSERTED i
    INNER JOIN DELETED d ON i.ID_Product = d.ID_Product
    WHERE 
        i.Description <> d.Description
        OR i.Price <> d.Price;
END;

--View que faz a busca dos logs para que possamos ver eles na tela do sistema
CREATE VIEW vw_ProductAudit AS
SELECT 
    A.AuditId,
    A.ProductId,
    P.Description AS CurrentDescription,
    A.OldDesc,
    A.NewDesc,
    A.OldPrice,
    A.NewPrice,
    A.ChangeDate
FROM ProductAudit A
LEFT JOIN Product P ON A.ProductId = P.ID_Product;

--Função para que a consulta do fornecedor traga Cidade-UF juntos
CREATE FUNCTION dbo.fn_NomeCidadeComUF (@ID INT)
RETURNS NVARCHAR(100)
AS
BEGIN
    DECLARE @Retorno NVARCHAR(100)

    SELECT @Retorno = Description + ' - ' + UF
    FROM City
    WHERE ID_City = @ID

    RETURN @Retorno
END

--View para facilitar o uso da função
CREATE VIEW vw_SupplierList AS
SELECT 
    s.ID_Supplier, 
    s.CorporateName, 
    s.CNPJ, 
    s.Telephone, 
    s.Representative, 
    dbo.fn_NomeCidadeComUF(s.ID_City) AS Cidade
FROM Supplier s;

