# FashionTrack

## 1. Introdução

### Descrição do Sistema
FashionTrack é um sistema de gerenciamento de estoque e vendas para uma loja de roupas. O sistema possui funcionalidades tanto para administradores quanto para membros da equipe, permitindo um controle eficiente do estoque, vendas e relatórios.

### Principais Funcionalidades
- **Controle de Estoque**: Gerenciamento de entradas e saídas de produtos.
- **Vendas**: Registro e acompanhamento de vendas realizadas.
- **Relatórios**: Geração de relatórios detalhados sobre vendas, estoque e cadastros.
- **Gestão de Usuários**: Cadastro e controle de acesso da equipe.
- **Histórico**: Registro de todas as vendas.

## 2. Requisitos

### Requisitos Funcionais
- Gerenciamento de estoque (entradas e saídas de produtos).
- Cadastro de produtos, clientes e fornecedores.
- Registro de vendas e geração de relatórios.
- Suporte a múltiplos métodos de pagamento.

### Requisitos Não Funcionais
- Sistema seguro, com banco de dados acessível apenas por usuários autorizados.
- Performance otimizada para consultas rápidas.
- Senhas criptografadas com uso de HASH.

## 3. Arquitetura do Sistema

### Tecnologias Utilizadas
- **C#**
- **WPF (Windows Presentation Foundation)**
- **SQL Server**

### Diagrama de Arquitetura
[Inclua aqui o diagrama de arquitetura, se disponível]

## 4. Banco de Dados

### Localização do Banco de Dados
O banco de dados deve ser armazenado em um servidor SQL Server seguro, acessível apenas por usuários autorizados.

### Tabelas Principais
A tabela de **produtos** inclui os seguintes campos atômicos:
- Descrição do produto
- Preço
- Marca
- Código de marca
- Tamanho
- Cor

Tabela de **Movimentação de Estoque**:
- Descrição da movimentação
- N° de documento (para controle interno)
- Tipo do movimento (entrada/saída)
- Operação (ajuste, roubo, transferência, ...)
- Data do movimento
- Usuário que realizou a movimentação

Tabela de **Estoque**:
- Produto
- Quantidade

Tabela auxiliar de **Movimentação do Item** (registra a movimentação de cada produto na tabela estoque):
- Identificador da movimentação de estoque
- Produto
- Quantidade movimentada

## 5. Front-End e Interfaces

### Tela de Controle de Estoque
Permite a visualização e atualização do estoque de produtos.

### Tela de Vendas
Interface para registrar e acompanhar vendas. Disponível para todos os usuários.

### Relatórios
Geração de relatórios sobre vendas, estoque e cadastros.

## 6. Funcionalidades Avançadas

### Controle nas Vendas
- **Dados dos Clientes**: Cadastro de clientes permite adicionar vários dados que integram com a venda.
- **Métodos de Pagamento**: Opções de múltiplos métodos de pagamento.
- **Alterações no Valor do Produto**: Opções para aplicar descontos e ajustar preços.

## 7. Guia de Uso

### Como Configurar o Banco de Dados
[Instruções sobre como configurar o SQL Server](https://www.youtube.com/watch?v=LxtLqS-9KYo&ab_channel=B%C3%B3sonTreinamentos)
- O vídeo a cima explica como instalar o SSMS que usamos para configurar o banco.
- Pode usar o SQL que deixamos pronto como base para seu banco, ficando a vontade para adicionar informações, etc.

### Primeiros Passos
1. Com a parte do banco instalada, usando o Visual Studio 2022(ou superior) clone seu repositório.
2. Inicie o código.
3. Com o programa aberto faça login com as credenciais que configurou no SQL do banco de dados.
4. Começe a registrar novos produtos, clientes, fornecedores, movimentações de estoque e testar o programa ao seu modo.

## 8. Contribuindo

Se você quiser contribuir para o desenvolvimento deste projeto, siga as instruções abaixo:
1. Faça um fork deste repositório.
2. Crie uma branch para suas alterações (`git checkout -b feature/novos-recursos`).
3. Commit suas alterações (`git commit -am 'Adicionando novos recursos'`).
4. Envie para o repositório remoto (`git push origin feature/novos-recursos`).
5. Crie uma pull request.

