CREATE DATABASE FashionTrack
GO

USE FashionTrack
GO

CREATE TABLE Usuarios (
    ID_Usuario INT IDENTITY(1,1),
    NomeCompleto VARCHAR(100) NOT NULL,
    Usuario VARCHAR(50) NOT NULL UNIQUE,
    Senha VARCHAR(255) NOT NULL,
    Adm BIT NOT NULL,  -- 0 para nao, 1 para sim
    CONSTRAINT PK_Usuario PRIMARY KEY (ID_Usuario)
);

CREATE TABLE Cidade (
    ID_Cidade INT IDENTITY(1,1),
    Descricao VARCHAR(100) NOT NULL,
    UF CHAR(2) NOT NULL,
    CONSTRAINT PK_Cidade PRIMARY KEY (ID_Cidade)
);

CREATE TABLE Cliente (
    ID_Cliente INT IDENTITY(1,1),
    Nome VARCHAR(50) NOT NULL,
    Sobrenome VARCHAR(50) NOT NULL,
    CPF CHAR(11) NOT NULL UNIQUE,
    Telefone VARCHAR(15) NOT NULL,
    Endereco VARCHAR(150) NOT NULL,
    ID_Cidade INT NOT NULL,
    CONSTRAINT PK_Cliente PRIMARY KEY (ID_Cliente),
    CONSTRAINT FK_Cliente_Cidade FOREIGN KEY (ID_Cidade) REFERENCES Cidade(ID_Cidade)
);

CREATE TABLE Fornecedor (
    ID_Fornecedor INT IDENTITY(1,1),
    NomeRazaoSocial VARCHAR(100) NOT NULL,
    CNPJ CHAR(14) NOT NULL UNIQUE,
    Endereco VARCHAR(150) NOT NULL,
    Telefone VARCHAR(15) NOT NULL,
    ID_Cidade INT NOT NULL,
    CONSTRAINT PK_Fornecedor PRIMARY KEY (ID_Fornecedor),
    CONSTRAINT FK_Fornecedor_Cidade FOREIGN KEY (ID_Cidade) REFERENCES Cidade(ID_Cidade)
);

-- Cadastro de Marca
CREATE TABLE Brand (
    BrandId INT PRIMARY KEY IDENTITY(1,1),
    BrandName NVARCHAR(100) NOT NULL
);

-- Cadastro de Cor
CREATE TABLE Color (
    ColorId INT PRIMARY KEY IDENTITY(1,1),
    ColorName NVARCHAR(50) NOT NULL
);

-- Cadastro de Tamanho
CREATE TABLE Size (
    SizeId INT PRIMARY KEY IDENTITY(1,1),
    SizeDescription NVARCHAR(3) NOT NULL
);

-- Cadastro de Produto com as Rela��es
CREATE TABLE Produto (
    ID_Produto INT IDENTITY(1,1),
    BrandCode VARCHAR(50),
    BrandId INT,
    ColorId INT,
    Description VARCHAR(255) NOT NULL,
    SizeId INT,
    Gender VARCHAR(10) NOT NULL,
    Price FLOAT(30) NOT NULL,
    CONSTRAINT PK_Produto PRIMARY KEY (ID_Produto),
    CONSTRAINT FK_Produto_Marca FOREIGN KEY (BrandId) REFERENCES Brand(BrandId),
    CONSTRAINT FK_Produto_Cor FOREIGN KEY (ColorId) REFERENCES Cor(ColorId),
    CONSTRAINT FK_Produto_Tamanho FOREIGN KEY (SizeId) REFERENCES Size(SizeId)
);

CREATE TABLE FornecedorProduto (
    ID_Fornecedor INT,
    ID_Produto INT,
    CONSTRAINT PK_FornecedorProduto PRIMARY KEY (ID_Fornecedor, ID_Produto),
    CONSTRAINT FK_FornecedorProduto_Fornecedor FOREIGN KEY (ID_Fornecedor) REFERENCES Fornecedor(ID_Fornecedor),
    CONSTRAINT FK_FornecedorProduto_Produto FOREIGN KEY (ID_Produto) REFERENCES Produto(ID_Produto)
);


INSERT INTO Usuarios(NomeCompleto, Usuario, Senha, Adm)
VALUES
		('admin', 'admin', 'admin', 1),
		('Guilherme Cella', 'GuiCella', '1234', 1),
		('gui', 'gui', 'gui', 1)

INSERT INTO Cidade (Descricao, UF)
VALUES 
		('Erechim', 'RS'),
		('Porto Alegre', 'RS'),
		('Sao Paulo', 'SP'),
		('Rio de Janeiro', 'RJ'),
		('Curitiba', 'PR'),
		('Florianopolis', 'SC'),
		('Belo Horizonte', 'MG'),
		('Brasilia', 'DF'),
		('Salvador', 'BA'),
		('Fortaleza', 'CE');

INSERT INTO Fornecedor (NomeRazaoSocial, CNPJ, Endereco, Telefone, ID_Cidade)
VALUES
		('Fornecedor ABC', '12345678000199', 'Rua das Flores, 100', '51999990001', 1),  -- Erechim
		('Fornecedor XYZ', '98765432000188', 'Av. Central, 200', '51999990002', 2);     -- Porto Alegre

INSERT INTO Cliente (Nome, Sobrenome, CPF, Telefone, Endereco, ID_Cidade)
VALUES
		('Joao', 'Silva', '12345678901', '51999991111', 'Rua Verde, 50', 3),    -- Sao Paulo
		('Maria', 'Oliveira', '98765432100', '51999992222', 'Av. Paulista, 101', 4);  -- Rio de Janeiro

INSERT INTO Color (ColorName)
VALUES
		('Branco'),
		('Preto');

INSERT INTO Size(SizeDescription)
VALUES
		('M'),
		('G');

INSERT INTO Produto(BrandCode, ColorId, Description, SizeId, Gender, Price)
VALUES
		(NULL, 1, 'Camiseta Basica', 1, 'Masculino',85.00),  -- Produto sem Codigo de Marca
		('123AA', 2, 'Camiseta Preta', 2, 'Feminino', 85.00);
