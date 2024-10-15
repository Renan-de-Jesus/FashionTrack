using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
    public partial class CustomerRegistration : Window
    {
        public CustomerRegistration()
        {
            InitializeComponent();
            fillComboBox();
        }

        private void fillComboBox()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
                try
                {
                    connection.Open();
                    string cityQuery = "SELECT ID_Cidade, Descricao FROM Cidade";
                    SqlCommand cityCommand = new SqlCommand(cityQuery, connection);
                    DataTable dt = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(cityCommand);
                    adapter.Fill(dt);
                    cityCbx.ItemsSource = dt.DefaultView;
                    cityCbx.DisplayMemberPath = "Descricao";
                    cityCbx.SelectedValuePath = "ID_Cidade";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void RemoveTextFirstName(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "NOME DO CLIENTE")
            {
                textBox.Text = "";
                textBox.Opacity = 1;
            }
        }

        private void AddTextFirstName(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "NOME DO CLIENTE";
                textBox.Opacity = 0.6;
            }
        }
        private void RemoveTextSecundName(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "SOBRENOME DO CLIENTE")
            {
                textBox.Text = "";
                textBox.Opacity = 1;
            }
        }

        private void AddTextSecundName(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "SOBRENOME DO CLIENTE";
                textBox.Opacity = 0.6;
            }
        }

        private void RemoveTextCPF(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "000.000.000-00")
            {
                textBox.Text = "";
                textBox.Opacity = 1;
            }
        }

        private void AddTextCPF(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "000.000.000-00";
                textBox.Opacity = 0.6;
            }
        }

        private void RemoveTextPhone(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "(99)99999-9999")
            {
                textBox.Text = "";
                textBox.Opacity = 1;
            }
        }

        private void AddTextPhone(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "(99)99999-9999";
                textBox.Opacity = 0.6;
            }
        }

        private void RemoveTextAddress(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "ENDEREÇO DO CLIENTE")
            {
                textBox.Text = "";
                textBox.Opacity = 1;
            }
        }

        private void AddTextAddress(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "ENDEREÇO DO CLIENTE";
                textBox.Opacity = 0.6;
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string firstName = firstNameTxtBox.Text;
                string secundName = secundNameTxtBox.Text;
                string cpf = cpfTxtBox.Text;
                string phone = phoneTxt.Text;
                string address = addressTxtBox.Text;

                cpf = cpf.Replace(".", "").Replace("-", "");
                phone = phone.Replace("(", "").Replace(")", "").Replace("-", "");

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
                    phoneTxt.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(address))
                {
                    MessageBox.Show("Por favor, preencha o endereço do cliente!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    addressTxtBox.Focus();
                    return;
                }

                if (cityCbx.SelectedValue == null)
                {
                    MessageBox.Show("Por favor, selecione uma cidade!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    cityCbx.Focus();
                    return;
                }

                int selectCityId = (int)cityCbx.SelectedValue;

                string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        string checkCustomerQuery = "SELECT COUNT(*) FROM Cliente WHERE CPF = @CPF";
                        SqlCommand checkCustomerCommand = new SqlCommand(checkCustomerQuery, connection);
                        checkCustomerCommand.Parameters.AddWithValue("@CPF", cpf);

                        object result = checkCustomerCommand.ExecuteScalar();
                        int customerCount = result != null ? Convert.ToInt32(result) : 0;

                        if (customerCount > 0)
                        {
                            MessageBox.Show("Cliente já cadastrado com este CPF.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            string saveCustomerQuery = "INSERT INTO Cliente (Nome, Sobrenome, CPF, Telefone, Endereco, ID_Cidade) " +
                                                        "VALUES (@firstName, @secundName, @CPF, @phone, @address, @selectCityId)";

                            SqlCommand saveCustomerCommand = new SqlCommand(saveCustomerQuery, connection);

                            saveCustomerCommand.Parameters.AddWithValue("@firstName", firstName);
                            saveCustomerCommand.Parameters.AddWithValue("@secundName", secundName);
                            saveCustomerCommand.Parameters.AddWithValue("@CPF", cpf);
                            saveCustomerCommand.Parameters.AddWithValue("@phone", phone);
                            saveCustomerCommand.Parameters.AddWithValue("@address", address);
                            saveCustomerCommand.Parameters.AddWithValue("@selectCityId", selectCityId);

                            int rowsAffected = saveCustomerCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Cliente cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Erro ao cadastrar o cliente.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        MessageBox.Show($"Erro de SQL: {sqlEx.Message}", "SQL Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro geral: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void addCityBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CityRegister cityRegister = new CityRegister();
            cityRegister.Show();
        }
    }
}
