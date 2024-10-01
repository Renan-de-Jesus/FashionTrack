using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FashionTrack
{
    /// <summary>
    /// Interaction logic for CustomerRegistration.xaml
    /// </summary>
    public partial class CustomerRegistration : Window
    {
        public CustomerRegistration()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void secundNameTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void cpfTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void phoneTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void addressTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            string firstName = firstNameTxtBox.Text;
            string secundName = secundNameTxtBox.Text;
            string cpf = cpfTxtBox.Text;
            string phone = phoneTxtBox.Text;
            string address = addressTxtBox.Text;

            if (string.IsNullOrEmpty(firstName))
            {
                MessageBox.Show("Por favor, preencha o nome do cliente!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                firstNameTxtBox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(secundName))
            {
                MessageBox.Show("Por favor, preencha o sobrenome do cliente!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                secundNameTxtBox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(cpf))
            {
                MessageBox.Show("Por favor, preencha o CPF do cliente!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                cpfTxtBox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Por favor, preencha o telefone do cliente!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                phoneTxtBox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(address))
            {
                MessageBox.Show("Por favor, preencha o endereço do cliente!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                addressTxtBox.Focus();
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
                try
                {
                    connection.Open();

                    string checkCustomerQuerry = "SELECT * FROM Produto WHERE CPF = @cpf";
                    SqlCommand checkCustomerCommand = new SqlCommand(checkCustomerQuerry,connection);
                    checkCustomerCommand.Parameters.AddWithValue("@cpf", cpf);
                }
        }
    }
}
