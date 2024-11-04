﻿using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.ComponentModel;
using static FashionTrack.HomePage;
using System.Windows.Markup;
using Microsoft.IdentityModel.Tokens;

namespace FashionTrack
{
    public partial class HomePage : Window
    {
        string paymentMethod;
        public static DateTime Today { get; }
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public ObservableCollection<SelectedProduct> SelectedProducts { get; set; } = new ObservableCollection<SelectedProduct>();
        public ObservableCollection<Customer> Customers { get; set; } = new ObservableCollection<Customer>();

        public class Customer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string CPF { get; set; }
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

            public int Quantity
            {
                get => quantity;
                set
                {
                    if (quantity != value)
                    {
                        quantity = value;
                        OnPropertyChanged(nameof(Quantity));
                        OnPropertyChanged(nameof(TotalPrice));
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
                        OnPropertyChanged(nameof(TotalPrice));
                    }
                }
            }

            public decimal TotalPrice => Quantity * Price;

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void UpdateTotalPrice()
        {
            decimal total = SelectedProducts.Sum(p => p.TotalPrice);
            fullPriceLbl.Content = total.ToString("F2");
        }

        public decimal Total
        {
            get
            {
                return SelectedProducts.Sum(product => product.Quantity * product.Price);
            }
        }

        public HomePage()
        {
            InitializeComponent();
            DataContext = this;
            SearchResults.ItemsSource = Products;
            selectedProductsDgv.ItemsSource = SelectedProducts;
            CustomerResults.ItemsSource = Customers;

            SelectedProducts.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (SelectedProduct item in e.NewItems)
                    {
                        item.PropertyChanged += (sender, args) => UpdateTotalPrice();
                    }
                }
                UpdateTotalPrice();
            };
        }

        private void SearchCustomers(string searchText)
        {
            Customers.Clear();
            string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ID_Customer, Name, Surname, CPF " +
                        "FROM Customer " +
                        "WHERE Name LIKE @searchText OR Surname LIKE @searchText";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var customer = new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ID_Customer")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Surname = reader.GetString(reader.GetOrdinal("Surname")),
                            CPF = reader.GetString(reader.GetOrdinal("CPF"))
                        };
                        Customers.Add(customer);
                        CustomerResults.SelectedValuePath = "ID_Customer";
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao conectar ao banco de dados: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void SearchCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string searchText = ((TextBox)sender).Text;
                SearchCustomers(searchText);
            }
        }

        private void CustomerResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomerResults.SelectedItem is Customer selectedCustomer)
            {
                SearchCustomer.Text = $"{selectedCustomer.Name} {selectedCustomer.Surname}";
                SearchCustomer.IsEnabled = false;
                idCustomerTxt.Text = selectedCustomer.Id.ToString();
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

        private void SearchProducts(string searchText)
        {
            Products.Clear();
            string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT P.ID_Product, P.Description, B.BrandName, C.ColorName, S.SizeDescription, P.Gender " +
                        "FROM Product AS P " +
                        "INNER JOIN Brand AS B ON P.BrandId = B.BrandId " +
                        "INNER JOIN Color AS C ON P.ColorId = C.ColorId " +
                        "INNER JOIN Size AS S ON P.SizeId = S.SizeId " +
                        "WHERE Description LIKE @searchText ";
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
                        };
                        Products.Add(product);
                        SearchResults.SelectedValuePath = "idProduct";
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao conectar ao banco de dados: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SearchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchResults.SelectedItem is Product selectedProduct)
            {
                AddSelectedProduct(selectedProduct.Id, selectedProduct.Description, selectedProduct.Color, selectedProduct.Brand, selectedProduct.Size, selectedProduct.Gender, 0);
            }
        }

        private void AddSelectedProduct(int id, string description, string color, string brand, string size, string gender, decimal price)
        {
            if (!SelectedProducts.Any(p =>p.Id == id && p.Description == description && p.Color == color && p.Brand == brand && p.Size == size && p.Gender == gender))
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
                    Price = price
                });
            }
            else
            {
                MessageBox.Show("Produto já adicionado.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ClearSearch(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            Products.Clear();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            DateTime date = DateTime.Today;
            decimal totalPrice = Convert.ToDecimal(fullPriceLbl.Content.ToString());
            if (string.IsNullOrEmpty(idCustomerTxt.Text) || !int.TryParse(idCustomerTxt.Text, out int ID_Customer))
            {
                MessageBox.Show("Por favor, selecione um cliente válido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (idCustomerTxt.Text == "0")
            {
                MessageBox.Show("Por favor, selecione um cliente.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                SearchCustomer.Focus();
                return;
            }

            if(!SelectedProducts.Any())
            {
                MessageBox.Show("Por favor, selecione ao menos um produto.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                SearchTextBox.Focus();
                return;
            }

            if (paymentMethod.IsNullOrEmpty())
            {
                MessageBox.Show("Por favor, selecione uma forma de pagamento.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))

                try
                {
                    connection.Open();
                    try
                    {
                        string sellRegisterString = "INSERT INTO Sell(ID_Customer, Sell_Document, SellDate, TotalPrice, PaymentMethod) " +
                            "VALUES(@idCustomer, 9, @date, @totalPrice, @PaymentMethod); SELECT SCOPE_IDENTITY();";
                        SqlCommand sellRegisterCommand = new SqlCommand(sellRegisterString, connection);
                        sellRegisterCommand.Parameters.AddWithValue("@idCustomer", ID_Customer);
                        sellRegisterCommand.Parameters.AddWithValue("@date", date);
                        sellRegisterCommand.Parameters.AddWithValue("@totalPrice", totalPrice);
                        sellRegisterCommand.Parameters.AddWithValue("@PaymentMethod", paymentMethod);

                        int sellId = Convert.ToInt32(sellRegisterCommand.ExecuteScalar());
                        try {

                            foreach (var selectedProduct in SelectedProducts)
                            {
                                string itemSellString = "INSERT INTO ItemSell(ID_Sell, ID_Product, Qty, PartialPrice) " +
                                                        "VALUES(@ID_Sell, @ID_Product, @Qty, @PartialPrice)";
                                SqlCommand itemSellCommand = new SqlCommand(itemSellString, connection);
                                itemSellCommand.Parameters.AddWithValue("@ID_Sell", sellId);
                                itemSellCommand.Parameters.AddWithValue("@ID_Product", selectedProduct.Id);
                                itemSellCommand.Parameters.AddWithValue("@Qty", selectedProduct.Quantity);
                                itemSellCommand.Parameters.AddWithValue("@PartialPrice", selectedProduct.Price);

                                itemSellCommand.ExecuteNonQuery();
                            }
                        }
                        catch(Exception ex){
                            MessageBox.Show("Erro: " + ex.Message, "Error ItemVenda", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao finalizar a venda" + ex.Message, "Error Venda", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao conectar ao banco de dados" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchCustomer.IsEnabled = true;
            SearchCustomer.Text = string.Empty;
            Customers.Clear();
        }

        private void debitoRdBtn_Checked(object sender, RoutedEventArgs e)
        {
            paymentMethod = "Debito";
        }

        private void creditoRdBtn_Checked(object sender, RoutedEventArgs e)
        {
            paymentMethod = "Credito";
        }

        private void pixRdBtn_Checked(object sender, RoutedEventArgs e)
        {
            paymentMethod = "Pix";
        }

        private void moneyRdBtn_Checked(object sender, RoutedEventArgs e)
        {
            paymentMethod = "Dinheiro";
        }
    }
}