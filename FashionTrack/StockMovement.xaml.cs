using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static FashionTrack.SellScreen;
using System.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FashionTrack
{
    public partial class StockMovement : Window
    {
        private int movementId;
        string movimentType;
        string operation;
        public static DateTime Today { get; }
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public ObservableCollection<SelectedProduct> SelectedProducts { get; set; } = new ObservableCollection<SelectedProduct>();


        private void RemoveText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Id" || textBox.Text == "Produto" || textBox.Text == "Descrição" || textBox.Text == "Documento")
            {
                textBox.Text = "";
                textBox.Opacity = 1;
            }
        }

        private void AddText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox.Name == "SearchTextBox")
                {
                    textBox.Text = "Produto";
                }
                else if (textBox.Name == "descriptionTxt")
                {
                    textBox.Text = "Descrição";
                }
                else if (textBox.Name == "idProductTxt")
                {
                    textBox.Text = "Id";
                } else if (textBox.Name == "documentTxt")
                {
                    textBox.Text = "Documento";
                }
                textBox.Opacity = 0.6;
            }
        }

        public class SelectedProduct : INotifyPropertyChanged
        {
            private int quantity;
            private decimal price;

            public int Id { get; set; }
            public string Description { get; set; }
            public string Color { get; set; }
            public string Brand { get; set; }
            public string Size { get; set; }
            public string Gender { get; set; }
            public int Qty { get; set; }
            public int StockQuantity { get; set; }

            public int Quantity
            {
                get => quantity;
                set
                {
                    if (quantity != value)
                    {
                        quantity = value;
                        OnPropertyChanged(nameof(Quantity));
                    }
                }
            }

            public decimal Price
            {
                get => price;
                set
                {
                    if (price != value)
                    {
                        price = value;
                        OnPropertyChanged(nameof(Price));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public StockMovement()
        {
            InitializeComponent();
            DataContext = this;
            SearchResults.ItemsSource = Products;
            selectedProductsDgv.ItemsSource = SelectedProducts;
        }

        private void idProductTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int idProduct;
                if (!int.TryParse(idProductTxt.Text, out idProduct))
                {
                    MessageBox.Show("Por favor, insira um ID de produto válido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                    try
                    {
                        connection.Open();
                        try
                        {
                            string searchProduct = "SELECT P.ID_Product, P.Description, B.BrandName, C.ColorName, S.SizeDescription, P.Gender, ST.Qty " +
                            "FROM Product AS P " +
                            "INNER JOIN Brand AS B ON P.BrandId = B.BrandId " +
                            "INNER JOIN Color AS C ON P.ColorId = C.ColorId " +
                            "INNER JOIN Size AS S ON P.SizeId = S.SizeId " +
                            "INNER JOIN Stock AS ST ON P.ID_Product = ST.ID_Product " +
                            "WHERE P.ID_Product = @idProduct";

                            SqlCommand searchProductCommand = new SqlCommand(searchProduct, connection);
                            searchProductCommand.Parameters.AddWithValue("@idProduct", idProduct);
                            Products.Clear();
                            try
                            {
                                using (SqlDataReader reader = searchProductCommand.ExecuteReader())
                                    while (reader.Read())
                                    {
                                        var product = new Product
                                        {
                                            Id = reader.GetInt32(reader.GetOrdinal("ID_Product")),
                                            Description = reader["Description"].ToString(),
                                            Color = reader["ColorName"].ToString(),
                                            Brand = reader["BrandName"].ToString(),
                                            Size = reader["SizeDescription"].ToString(),
                                            Gender = reader["Gender"].ToString(),
                                            Qty = reader.GetInt32(reader.GetOrdinal("Qty"))
                                        };
                                        Products.Add(product);
                                        SearchResults.SelectedValuePath = "idProduct";
                                    }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Erro ao repassar os dados para o grid! " + ex.Message, "Erro Grid", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Erro ao buscas informações no banco de dados! " + ex.Message, "Erro Informações", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao conectar ao banco de dados! " + ex.Message, "Erro Banco de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
            }

        }

        private void SearchProducts(string searchText)
        {
            Products.Clear();
            string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT P.ID_Product, P.Description, B.BrandName, C.ColorName, S.SizeDescription, P.Gender, ST.Qty " +
                        "FROM Product AS P " +
                        "INNER JOIN Brand AS B ON P.BrandId = B.BrandId " +
                        "INNER JOIN Color AS C ON P.ColorId = C.ColorId " +
                        "INNER JOIN Size AS S ON P.SizeId = S.SizeId " +
                        "INNER JOIN Stock AS ST ON P.ID_Product = ST.ID_Product " +
                        "WHERE Description LIKE @searchText";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var product = new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ID_Product")),
                            Description = reader["Description"].ToString(),
                            Color = reader["ColorName"].ToString(),
                            Brand = reader["BrandName"].ToString(),
                            Size = reader["SizeDescription"].ToString(),
                            Gender = reader["Gender"].ToString(),
                            Qty = reader.GetInt32(reader.GetOrdinal("Qty"))
                        };
                        Products.Add(product);
                        SearchResults.SelectedValuePath = "idProduct";
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao conectar ao banco de dados: {ex.Message}", "Erro Banco de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SearchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchResults.SelectedItem is Product selectedProduct)
            {
                AddSelectedProduct(selectedProduct.Id, selectedProduct.Description, selectedProduct.Color, selectedProduct.Brand, selectedProduct.Size, selectedProduct.Gender, 0, selectedProduct.Qty);
                Products.Clear();
            }
        }

        private void AddSelectedProduct(int id, string description, string color, string brand, string size, string gender, decimal price, int stockQuantity)
        {
            if (!SelectedProducts.Any(p => p.Id == id && p.Description == description && p.Color == color && p.Brand == brand && p.Size == size && p.Gender == gender))
            {
                SelectedProducts.Add(new SelectedProduct
                {
                    Id = id,
                    Description = description,
                    Color = color,
                    Brand = brand,
                    Size = size,
                    Gender = gender,
                    Quantity = 1,
                    Price = price,
                    StockQuantity = stockQuantity
                });
            }
            else
            {
                MessageBox.Show("Produto já adicionado.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string searchText = ((TextBox)sender).Text;
                SearchProducts(searchText);
            }
        }

        private void ClearBtn(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            idProductTxt.Text = string.Empty;
            Products.Clear();
        }

        public ICommand DeleteCommand => new RelayCommand<SelectedProduct>(ExecuteDeleteCommand);

        void ExecuteDeleteCommand(SelectedProduct product)
        {
            if (product != null && SelectedProducts.Contains(product))
            {
                SelectedProducts.Remove(product);
            }
        }

        private void prohibitedRdBt_Checked(object sender, RoutedEventArgs e)
        {
            movimentType = "Entrada";
            releaseOfComponents();
        }

        private void exitRdBt_Checked(object sender, RoutedEventArgs e)
        {
            movimentType = "Saida";
            releaseOfComponents();
        }

        private void OperationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedOperation = (OperationComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (selectedOperation != null)
            {
                switch (selectedOperation)
                {
                    case "Entrada de Estoque":
                        operation = "E";
                        break;

                    case "Saída de Estoque":
                        operation = "S";
                        break;

                    case "Transferência":
                        operation = "T";
                        break;

                    case "Ajuste de Estoque":
                        operation = "A";
                        break;

                    case "Perda":
                        operation = "P";
                        break;

                    case "Roubo":
                        operation = "R";
                        break;

                    case "Devolução":
                        operation = "D";
                        break;

                    default:
                        MessageBox.Show("Operação desconhecida.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string description = descriptionTxt.Text;
            string document = documentTxt.Text;
            int userId = LoginWindow.LoggedInUserId;
            DateTime date = DateTime.Now;
            bool sucefull;

            if (SelectedProducts.Count == 0)
            {
                MessageBox.Show("Por favor, selecione ao menos um item!", "Erro", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (string.IsNullOrEmpty(movimentType))
            {
                MessageBox.Show("Por favor, selecione o tipo de movimentação!", "Erro", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (string.IsNullOrEmpty(operation))
            {
                MessageBox.Show("Por favor, selecione o tipo de operação!", "Erro", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    try
                    {
                        string searchDocument = "SELECT COUNT(*) " +
                            "FROM StockMovement " +
                            "WHERE Document = @document";
                        SqlCommand search = new SqlCommand(searchDocument, connection);
                        search.Parameters.AddWithValue("@document", document);

                        object result = search.ExecuteScalar();
                        int documentRepited = result != null ? Convert.ToInt32(result) : 0;

                        if (documentRepited > 0)
                        {
                            MessageBox.Show("Já existe uma movimentação com esse número de documento.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                        try
                        {
                            string querry = "INSERT INTO StockMovement(MDescription, Document, MovementType, Operation, MovementDate, ID_Users) " +
                                            "VALUES (@description, @document, @movementType, @operation, @date, @ID_Users) SELECT SCOPE_IDENTITY();";
                            SqlCommand stockMovementCommand = new SqlCommand(querry, connection);

                            stockMovementCommand.Parameters.AddWithValue("@description", description);
                            stockMovementCommand.Parameters.AddWithValue("@document", document);
                            stockMovementCommand.Parameters.AddWithValue("@movementType", movimentType);
                            stockMovementCommand.Parameters.AddWithValue("@operation", operation);
                            stockMovementCommand.Parameters.AddWithValue("@date", date);
                            stockMovementCommand.Parameters.AddWithValue("@ID_Users", userId);

                            int idMovement = Convert.ToInt32(stockMovementCommand.ExecuteScalar());
                            sucefull = true;

                            try
                            {
                                foreach (var selectedProduct in SelectedProducts)
                                {
                                    string querry2 = "INSERT INTO ITEM_MOV(ID_StockMovement, ID_Product, Qty_Mov) " +
                                                     "VALUES (@ID_StockMovement, @ID_Product, @Qty)";
                                    SqlCommand stockMovementCommand2 = new SqlCommand(querry2, connection);

                                    stockMovementCommand2.Parameters.AddWithValue("@ID_StockMovement", idMovement);
                                    stockMovementCommand2.Parameters.AddWithValue("@ID_Product", selectedProduct.Id);
                                    stockMovementCommand2.Parameters.AddWithValue("@Qty", selectedProduct.Quantity);

                                    stockMovementCommand2.ExecuteNonQuery();

                                    if (movimentType == "Entrada")
                                    {
                                        UpdateStock(selectedProduct.Id, selectedProduct.Quantity, true);
                                    }
                                    else
                                    {
                                        UpdateStock(selectedProduct.Id, selectedProduct.Quantity, false);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Erro ao passar as informações para o banco de dados!" + ex.Message, "Erro Itens Movimentação", MessageBoxButton.OK, MessageBoxImage.Error);
                            }

                            if (sucefull)
                            {
                                if (movimentType == "Entrada")
                                {
                                    MessageBox.Show("Entrada realizada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Saida realizada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                ClearScreen();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Erro ao passar as informações para o banco de dados!" + ex.Message, "Erro Movimentação de Estoque", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao procurar documento repetido! " + ex.Message, "Erro de Procura", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao conectar ao banco de dados! " + ex.Message, "Erro Banco de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UpdateStock(int productId, int quantity, bool isEntry)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string updateStockString = isEntry ?
                        "UPDATE Stock SET Qty = Qty + @quantity WHERE ID_Product = @idProduct" :
                        "UPDATE Stock SET Qty = Qty - @quantity WHERE ID_Product = @idProduct";

                    SqlCommand updateCommand = new SqlCommand(updateStockString, connection);
                    updateCommand.Parameters.AddWithValue("@idProduct", productId);
                    updateCommand.Parameters.AddWithValue("@quantity", quantity);

                    updateCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao atualizar o estoque para o produto ID: {productId}. {ex.Message}", "Erro Atualização Estoque", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearScreen()
        {
            SelectedProducts.Clear();
            idProductTxt.Text = string.Empty;
            SearchTextBox.Text = string.Empty;
            descriptionTxt.Text = string.Empty;
            prohibitedRdBt.IsChecked = false;
            exitRdBt.IsChecked = false;
            OperationComboBox.SelectedIndex = 0;
            documentTxt.Text = string.Empty;
        }

        private void selectedProductsDgv_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (movimentType == "Saida")
            {
                if (e.EditAction == DataGridEditAction.Commit)
                {
                    var editedProduct = e.Row.Item as SelectedProduct;
                    if (editedProduct != null)
                    {
                        var textBox = e.EditingElement as TextBox;
                        if (textBox != null)
                        {
                            if (int.TryParse(textBox.Text, out int quantity))
                            {
                                if (quantity > editedProduct.StockQuantity)
                                {
                                    MessageBox.Show($"A quantidade não pode ser maior que o estoque ({editedProduct.StockQuantity} unidades).", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                                    editedProduct.Quantity = editedProduct.StockQuantity;
                                }
                                else
                                {
                                    editedProduct.Quantity = quantity;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Digite uma quantidade válida.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                                editedProduct.Quantity = 1;
                            }
                        }
                    }
                }
            }
        }

        private void releaseOfComponents() {
            idProductTxt.IsEnabled = true;
            SearchResults.IsEnabled = true;
            SearchTextBox.IsEnabled = true;
            descriptionTxt.IsEnabled = true;
            OperationComboBox.IsEnabled = true;
            clearBtn.IsEnabled = true;
            documentTxt.IsEnabled = true;
            selectedProductsDgv.IsEnabled = true;
        }
    }
}