CREATE DATABASE FASHIONTRACK
GO

USE FASHIONTRACK
GO

-- Tabela que armazena informações dos usuários do sistema.
-- Essencial para controle de acesso e permissões.
CREATE TABLE Users (
    ID_Users INT IDENTITY(1,1),
    FullName VARCHAR(100) NOT NULL,  -- Nome completo do usuário
    Username VARCHAR(50) NOT NULL UNIQUE,  -- Nome de usuário único
    Password VARBINARY(64) NOT NULL,  -- Senha do usuário, armazenada em formato hash
    Adm BIT NOT NULL,  -- Indicador se o usuário é administrador (1) ou não (0)
    CONSTRAINT PK_Usuario PRIMARY KEY (ID_Users)
);

-- Inserindo um usuário administrador inicial
INSERT INTO Users (FullName, Username, Password, Adm) 
VALUES ('Admin User', 'admin', HASHBYTES('SHA2_256', 'adminpassword'), 1);

-- Tabela que armazena informações das cidades.
-- Fundamental para relacionar clientes e fornecedores com suas localizações.
CREATE TABLE City (
    ID_City INT IDENTITY(1,1),
    Description VARCHAR(100) NOT NULL,  -- Nome da cidade
    UF CHAR(2) NOT NULL,  -- Unidade Federativa (estado) da cidade
    CONSTRAINT PK_City PRIMARY KEY (ID_City)
);

-- Inserindo uma cidade exemplo
INSERT INTO City (Description, UF) 
VALUES ('São Paulo', 'SP');

-- Tabela que armazena informações dos clientes.
-- Necessária para registrar vendas e identificar compradores.
CREATE TABLE Customer (
    ID_Customer INT IDENTITY(1,1),
    Name VARCHAR(50) NOT NULL,  -- Nome do cliente
    Surname VARCHAR(50) NOT NULL,  -- Sobrenome do cliente
    CPF CHAR(11) NOT NULL UNIQUE,  -- CPF do cliente, deve ser único
    Cellphone VARCHAR(15) NOT NULL,  -- Telefone celular do cliente
    Address VARCHAR(150) NOT NULL,  -- Endereço do cliente
    ID_City INT NOT NULL,  -- Chave estrangeira para a tabela City
    CONSTRAINT PK_Customer PRIMARY KEY (ID_Customer),
    CONSTRAINT FK_Customer_Cidade FOREIGN KEY (ID_City) REFERENCES City(ID_City) ON DELETE CASCADE
);

-- Inserindo um cliente exemplo
INSERT INTO Customer (Name, Surname, CPF, Cellphone, Address, ID_City) 
VALUES ('João', 'Silva', '12345678901', '11987654321', 'Rua A, 123', 1);

-- Tabela que armazena informações dos fornecedores.
-- Importante para controlar a origem dos produtos.
CREATE TABLE Supplier (
    ID_Supplier INT IDENTITY(1,1),
    CorporateName VARCHAR(100) NOT NULL,  -- Nome da empresa fornecedora
    CNPJ CHAR(14) NOT NULL UNIQUE,  -- CNPJ do fornecedor, deve ser único
    Address VARCHAR(150) NOT NULL,  -- Endereço do fornecedor
    Cellphone VARCHAR(15) NOT NULL,  -- Telefone do fornecedor
    ID_City INT NOT NULL,  -- Chave estrangeira para a tabela City
    CONSTRAINT PK_Supplier PRIMARY KEY (ID_Supplier),
    CONSTRAINT FK_Supplier_City FOREIGN KEY (ID_City) REFERENCES City(ID_City) ON DELETE CASCADE
);

-- Inserindo um fornecedor exemplo
INSERT INTO Supplier (CorporateName, CNPJ, Address, Cellphone, ID_City) 
VALUES ('Fornecedor Exemplo LTDA', '12345678000195', 'Avenida B, 456', '11876543210', 1);

-- Tabela que armazena marcas de produtos.
-- Fundamental para categorizar os produtos oferecidos.
CREATE TABLE Brand (
    BrandId INT PRIMARY KEY IDENTITY(1,1),
    BrandName NVARCHAR(100) NOT NULL  -- Nome da marca
);

-- Inserindo uma marca exemplo
INSERT INTO Brand (BrandName) 
VALUES ('Marca Exemplo');

-- Tabela que armazena cores dos produtos.
-- Necessária para especificar características visuais dos produtos.
CREATE TABLE Color (
    ColorId INT PRIMARY KEY IDENTITY(1,1),
    ColorName NVARCHAR(50) NOT NULL  -- Nome da cor
);

-- Inserindo uma cor exemplo
INSERT INTO Color (ColorName) 
VALUES ('Vermelho');

-- Tabela que armazena tamanhos dos produtos.
-- Essencial para produtos que variam em tamanho, como roupas.
CREATE TABLE Size (
    SizeId INT PRIMARY KEY IDENTITY(1,1),
    SizeDescription NVARCHAR(3) NOT NULL  -- Descrição do tamanho (ex: P, M, G)
);

-- Inserindo um tamanho exemplo
INSERT INTO Size (SizeDescription) 
VALUES ('M');

-- Tabela que armazena informações dos produtos.
-- Central para a gestão do estoque e vendas.
CREATE TABLE Product (
    ID_Product INT IDENTITY(1,1),
    BrandCode VARCHAR(50),  -- Código do produto da marca
    BrandId INT,  -- Chave estrangeira para a tabela Brand
    ColorId INT,  -- Chave estrangeira para a tabela Color
    Description VARCHAR(255) NOT NULL,  -- Descrição do produto
    SizeId INT,  -- Chave estrangeira para a tabela Size
    Gender VARCHAR(10) NOT NULL,  -- Gênero para o qual o produto é destinado
    Price DECIMAL(10, 2) NOT NULL,  -- Preço do produto
    CONSTRAINT PK_Product PRIMARY KEY (ID_Product),
    CONSTRAINT FK_Product_Brand FOREIGN KEY (BrandId) REFERENCES Brand(BrandId),
    CONSTRAINT FK_Product_Color FOREIGN KEY (ColorId) REFERENCES Color(ColorId),
    CONSTRAINT FK_Product_Size FOREIGN KEY (SizeId) REFERENCES Size(SizeId)
);

-- Inserindo um produto exemplo
INSERT INTO Product (BrandCode, BrandId, ColorId, Description, SizeId, Gender, Price) 
VALUES ('MEX123', 1, 1, 'Camisa Vermelha', 1, 'Masculino', 59.90);

-- Tabela que relaciona fornecedores a produtos.
-- Necessária para gerenciar a origem dos produtos no estoque.
CREATE TABLE ProductSupplier (
    ID_Supplier INT,
    ID_Product INT,
    CONSTRAINT PK_ProductSupplier PRIMARY KEY (ID_Supplier, ID_Product),
    CONSTRAINT FK_ProductSuppler_Supplier FOREIGN KEY (ID_Supplier) REFERENCES Supplier(ID_Supplier),
    CONSTRAINT FK_SupplierProduct_Product FOREIGN KEY (ID_Product) REFERENCES Product(ID_Product),
    CONSTRAINT UQ_ProductSupplier UNIQUE (ID_Supplier, ID_Product)  -- Garante que um produto não pode ter o mesmo fornecedor mais de uma vez
);

-- Inserindo um fornecedor de produto exemplo
INSERT INTO ProductSupplier (ID_Supplier, ID_Product) 
VALUES (1, 1);

-- Tabela que armazena informações das vendas.
-- Crucial para o registro de transações e histórico de vendas.
CREATE TABLE Sell (
    ID_Sell INT IDENTITY(1,1),
    ID_Customer INT NOT NULL,  -- Chave estrangeira para a tabela Customer
    SellDate DATETIME NOT NULL CONSTRAINT DF_SellDate DEFAULT GETDATE(),  -- Data da venda
    TotalPrice DECIMAL(10, 2) NOT NULL,  -- Preço total da venda
    CONSTRAINT PK_Sell PRIMARY KEY (ID_Sell),
    CONSTRAINT FK_Sell_Customer FOREIGN KEY (ID_Customer) REFERENCES Customer(ID_Customer) ON DELETE CASCADE
);

-- Inserindo uma venda exemplo
INSERT INTO Sell (ID_Customer, TotalPrice) 
VALUES (1, 59.90);

-- Tabela que armazena itens de cada venda.
-- Necessária para detalhar quais produtos foram vendidos em cada transação.
CREATE TABLE ItemSell (
    ID_ItemSell INT IDENTITY(1,1),
    ID_Sell INT NOT NULL,  -- Chave estrangeira para a tabela Sell
    ID_Product INT NOT NULL,  -- Chave estrangeira para a tabela Product
    Qty INT NOT NULL,  -- Quantidade do produto vendido
    PartialPrice DECIMAL(10, 2) NOT NULL,  -- Preço parcial da linha de venda
    CONSTRAINT FK_ITEMVENDAS PRIMARY KEY (ID_ItemSell),
    CONSTRAINT FK_ITEMVENDAS_PRODUTOS FOREIGN KEY (ID_Product) REFERENCES Product(ID_Product) ON DELETE CASCADE,
    CONSTRAINT FK_ITEMVENDAS_VENDAS FOREIGN KEY (ID_Sell) REFERENCES Sell(ID_Sell) ON DELETE CASCADE
);

-- Inserindo um item de venda exemplo
INSERT INTO ItemSell (ID_Sell, ID_Product, Qty, PartialPrice) 
VALUES (1, 1, 1, 59.90);

-- Tabela que armazena movimentações de estoque.
-- Crucial para o controle de entradas e saídas de produtos no estoque.
CREATE TABLE StockMovement (
    ID_StockMovement INT IDENTITY(1,1) PRIMARY KEY,
    ID_Product INT NOT NULL,  -- Chave estrangeira para a tabela Product
    MovementType NVARCHAR(10) NOT NULL CHECK (MovementType IN ('Entrada', 'Saida')),  -- Tipo de movimentação
    Qty INT NOT NULL,  -- Quantidade movimentada
    MovementDate DATETIME NOT NULL DEFAULT GETDATE(),  -- Data da movimentação
    UserID INT NULL,  -- Chave estrangeira para a tabela Users, opcional
