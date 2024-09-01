# FashionTrack

## Descrição
FashionTrack é um sistema de gerenciamento de estoque e vendas para uma loja de roupas. O sistema possui funcionalidades tanto para administradores quanto para membros da equipe, permitindo um controle eficiente do estoque, vendas e relatórios.

## Funcionalidades

### Banco de Dados
- **Localização do Banco de Dados:** O banco de dados deve ser armazenado em um servidor SQL Server seguro, acessível apenas por usuários autorizados.
- **Tipo de Tabela das Roupas/Produtos:** A tabela de produtos deve incluir campos atômicos (simples) para garantir a normalização e eficiência nas consultas.
- **Campos Atômicos:** Nome do produto, descrição, preço, quantidade em estoque, categoria, fornecedor, data de entrada, etc.

### Front End

#### Fluxo Admin
- **Tela de Controle de Estoque:** Permite a visualização e atualização do estoque.
- **Membros em Organização:** Gerenciamento de membros da equipe.
- **Tela de Vendas:** Interface para registrar e acompanhar vendas.
- **Logs de Vendas:** Histórico detalhado de todas as vendas realizadas.
- **Tela de Relatórios:** Geração de relatórios de vendas, estoque e desempenho.

#### Fluxo Membro
- **Tela de Estoque:** Visualização do estoque disponível.
- **Tela de Vendas:** Interface simplificada para registrar vendas.
- **Configurações:** Opções de configuração para o usuário.

### Controle nas Vendas
- **Dados dos Clientes:** Campos para adicionar informações dos clientes.
- **Métodos de Pagamento:** Suporte para múltiplos métodos de pagamento.
- **Vendas Online e Físicas:** Separação e gerenciamento de vendas online e físicas.
- **Descontos e Alterações no Valor do Produto:** Opções para aplicar descontos e ajustar preços.

### Tela de Logs
- **Histórico de Preços:** Registro de alterações nos preços dos produtos.
- **Cobranças Adicionais:** Possibilidade de adicionar cobranças extras.

### Observações
- **Pendências dos Clientes:** Visualização de pendências e débitos dos clientes.
- **Formas de Pagamento:** Suporte para várias formas de pagamento.
- **Opções de Desconto e Taxas:** Aplicação de descontos e taxas fixas.
- **Custos Genéricos:** Registro de custos adicionais como embalagem, gasolina, etc.

## Tecnologias Utilizadas
- C#
- WPF (Windows Presentation Foundation)
- SQL Server

## Como Executar
1. Clone o repositório:
   ```sh
   git clone https://github.com/seu-usuario/FashionTrack.git