using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace FashionTrack
{
    public partial class HomePage : Window
    {
        public ObservableCollection<string> Products { get; set; }

        public HomePage()
        {
            InitializeComponent();
            Products = new ObservableCollection<string>();
            SearchResults.ItemsSource = Products;
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
                    string query = "SELECT Descricao FROM Produto WHERE Descricao LIKE @searchText";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Products.Add(reader["Descricao"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao conectar ao banco de dados: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearSearch(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            Products.Clear();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RegisterWindow userRegister = new RegisterWindow();
            userRegister.Show();
        }

        private void customerImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CustomerRegistration registerNewCustomer = new CustomerRegistration();
            registerNewCustomer.Show();
        }
    }
}