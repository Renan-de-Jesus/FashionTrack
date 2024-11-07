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
        private int customerId;

        public CustomerRegistration()
        {
            InitializeComponent();
            fillComboBox();
        }

        public CustomerRegistration(int customerId) : this()
        {
            this.customerId = customerId;
            this.Loaded += CustomerRegister_Loaded;
        }

        private void CustomerRegister_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCustomerData(customerId);
        }

        private void fillComboBox()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
                try
                {
                    connection.Open();
                    string cityQuery = "SELECT ID_City, Description FROM City";
                    SqlCommand cityCommand = new SqlCommand(cityQuery, connection);
                    DataTable dt = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(cityCommand);
                    adapter.Fill(dt);
                    cityCbx.ItemsSource = dt.DefaultView;
                    cityCbx.DisplayMemberPath = "Description";
                    cityCbx.SelectedValuePath = "ID_City";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
        }

        private void LoadCustomerData(int customerId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Name, Surname, CPF, Cellphone, Address, ID_City FROM Customer WHERE ID_Customer = @ID_Customer";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ID_Customer", customerId);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        firstNameTxtBox.Text = reader["Name"].ToString();
                        surnameNameTxtBox.Text = reader["Surname"].ToString();
                        cpfTxtBox.Text = reader["CPF"].ToString();
                        phoneTxt.Text = reader["Cellphone"].ToString();
                        addressTxtBox.Text = reader["Address"].ToString();
                        cityCbx.SelectedValue = reader["ID_City"];
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar os dados do fornecedor: " + ex.Message);
                }
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            fillComboBox();
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
        private void RemoveTextSurnameName(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "SOBRENOME DO CLIENTE")
            {
                textBox.Text = "";
                textBox.Opacity = 1;
            }
        }

        private void AddTextSurnameName(object sender, RoutedEventArgs e)
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

                cpfTxtBox.SelectionStart = 0;
                cpfTxtBox.SelectionLength = 0;
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
                string surnameName = surnameNameTxtBox.Text;
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

                if (string.IsNullOrEmpty(surnameName))
                {
                    MessageBox.Show("Por favor, preencha o sobrenome do cliente!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    surnameNameTxtBox.Focus();
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

                        string checkCustomerQuery = "SELECT COUNT(*) FROM Customer WHERE CPF = @CPF";
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
                            string saveCustomerQuery = "INSERT INTO Customer (Name, Surname, CPF, Telephone, Address, ID_City) " +
                                                        "VALUES (@Name, @Surname, @CPF, @Telephone, @Address, @selectCityId)";

                            SqlCommand saveCustomerCommand = new SqlCommand(saveCustomerQuery, connection);

                            saveCustomerCommand.Parameters.AddWithValue("@Name", firstName);
                            saveCustomerCommand.Parameters.AddWithValue("@Surname", surnameName);
                            saveCustomerCommand.Parameters.AddWithValue("@CPF", cpf);
                            saveCustomerCommand.Parameters.AddWithValue("@Telephone", phone);
                            saveCustomerCommand.Parameters.AddWithValue("@Address", address);
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
