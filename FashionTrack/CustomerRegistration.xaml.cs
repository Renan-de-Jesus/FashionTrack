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

        public CustomerRegistration(int customerId)
        {
            InitializeComponent();
            fillComboBox();
            LoadCities();
            this.customerId = customerId;

            if (customerId > 0)
            {
                LoadCustomerData(customerId);
            }
        }

        private void LoadCities()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ID_City, Description FROM City";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable cities = new DataTable();
                            cities.Load(reader);
                            cityCbx.ItemsSource = cities.DefaultView;
                            cityCbx.DisplayMemberPath = "Description";
                            cityCbx.SelectedValuePath = "ID_City";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar as cidades: " + ex.Message);
            }
        }

        private void LoadCustomerData(int customerId)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                        C.Name, 
                        C.Surname, 
                        C.CPF, 
                        C.Cellphone, 
                        C.Address, 
                        C.ID_City 
                        FROM Customer AS C
                        WHERE ID_Customer = @ID_Customer";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ID_Customer", customerId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                firstNameTxtBox.Text = reader["Name"].ToString();
                                secondNameTxtBox.Text = reader["Surname"].ToString();
                                cpfTxtBox.Text = reader["CPF"].ToString();
                                phoneTxt.Text = reader["Cellphone"].ToString();
                                addressTxtBox.Text = reader["Address"].ToString();
                                cityCbx.ItemsSource = reader["ID_City"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar os dados do cliente: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void RemoveText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Nome do cliente" || textBox.Text == "Sobrenome" || textBox.Text == "Endereço do cliente" || textBox.Text == "000.000.000-00" || textBox.Text == "(00)00000-0000")
            {
                textBox.Text = "";
                textBox.Opacity = 1;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int originalCaretIndex = textBox.CaretIndex;
            int originalLength = textBox.Text.Length;

            textBox.TextChanged -= TextBox_TextChanged;

            if (textBox.Name == "phoneTxt")
            {
                if (textBox.Text != "(00)00000-0000")
                {
                    string formattedPhone = ApplyPhoneMask(textBox.Text, allowParenthesisErase: true);
                    textBox.Text = formattedPhone;
                }
            }

            int newLength = textBox.Text.Length;
            int adjustedCaretIndex = originalCaretIndex + (newLength - originalLength);
            adjustedCaretIndex = Math.Max(0, Math.Min(adjustedCaretIndex, textBox.Text.Length));
            textBox.CaretIndex = adjustedCaretIndex;

            textBox.TextChanged += TextBox_TextChanged;
        }
        private void AddText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                switch (textBox.Name)
                {
                    case "firstNameTxtBox":
                        textBox.Text = "Nome do cliente";
                        break;
                    case "secondNameTxtBox":
                        textBox.Text = "Sobrenome";
                        break;
                    case "addressTxtBox":
                        textBox.Text = "Endereço do cliente";
                        break;
                    case "cpfTxtBox":
                        textBox.Text = "000.000.000-00";
                        break;
                    case "phoneTxt":
                        textBox.Text = "(00)00000-0000";
                        break;
                }
                textBox.Opacity = 0.6;
            }
        }

        private string ApplyPhoneMask(string input, bool allowParenthesisErase)
        {
            string digits = new string(input.Where(char.IsDigit).ToArray());

            if (digits.Length > 11)
                digits = digits.Substring(0, 11);

            if (string.IsNullOrEmpty(digits)) return string.Empty;

            string formattedNumber = "";

            if (digits.Length > 0)
            {
                if (allowParenthesisErase)
                {
                    formattedNumber += $"({digits.Substring(0, Math.Min(2, digits.Length))})";
                }
                else
                {
                    formattedNumber += $"({digits.Substring(0, Math.Min(2, digits.Length))})";
                }

                if (digits.Length > 2)
                {
                    formattedNumber += " " + digits.Substring(2, Math.Min(5, digits.Length - 2));

                    if (digits.Length > 7)
                    {
                        formattedNumber += "-" + digits.Substring(7);
                    }
                }
            }

            return formattedNumber;
        }

        private string ApplyCpfMask(string input)
        {
            string digits = new string(input.Where(char.IsDigit).ToArray());

            if (digits.Length > 11)
                digits = digits.Substring(0, 11);

                if (string.IsNullOrEmpty(digits)) return string.Empty;

                    string formattedCpf = "";

                    if (digits.Length > 0)
                    {
                        formattedCpf += digits.Substring(0, Math.Min(3, digits.Length));

                        if (digits.Length > 3)
                        {
                            formattedCpf += "." + digits.Substring(3, Math.Min(3, digits.Length - 3));

                            if (digits.Length > 6)
                            {
                                formattedCpf += "." + digits.Substring(6, Math.Min(3, digits.Length - 6));

                                if (digits.Length > 9)
                                {
                                    formattedCpf += "-" + digits.Substring(9);
                                }
                            }
                        }
                    }

            return formattedCpf;
        }

        private void fillComboBox()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string cityQuery = "SELECT ID_City, Description FROM City ORDER BY Description ASC";
                    SqlCommand cityCommand = new SqlCommand(cityQuery, connection);
                    DataTable dt = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(cityCommand);
                    adapter.Fill(dt);
                    cityCbx.ItemsSource = dt.DefaultView;
                    cityCbx.DisplayMemberPath = "Description";
                    cityCbx.SelectedValuePath = "ID_City";
                    if (dt.Rows.Count > 0)
                    {
                        cityCbx.SelectedIndex = 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            fillComboBox();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (customerId > 0)
            {
                UpdateCustomerData();
                return;
            }
            try
            {
                string firstName = firstNameTxtBox.Text;
                string secondName = secondNameTxtBox.Text;
                string cpf = cpfTxtBox.Text;
                string phone = phoneTxt.Text;
                string address = addressTxtBox.Text;

                cpf = cpf.Replace(".", "").Replace("-", "");
                phone = phone.Replace("(", "").Replace(")", "").Replace("-", "");

                if (string.IsNullOrEmpty(firstName) || firstName == "Nome do cliente")
                {
                    MessageBox.Show("Por favor, preencha o nome do cliente!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    firstNameTxtBox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(secondName) || secondName == "Sobrenome")
                {
                    MessageBox.Show("Por favor, preencha o sobrenome do cliente!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    secondNameTxtBox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(cpf) || cpf == "000.000.000-00")
                {
                    MessageBox.Show("Por favor, preencha o CPF do cliente!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    cpfTxtBox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(phone) || phone == "(00)00000-0000")
                {
                    MessageBox.Show("Por favor, preencha o telefone do cliente!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    phoneTxt.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(address) || address == "Endereço do cliente")
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
                            MessageBox.Show("Cliente já cadastrado com este CPF.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else
                        {
                            string saveCustomerQuery = "INSERT INTO Customer (Name, Surname, CPF, Cellphone, Address, ID_City) " +
                                                        "VALUES (@Name, @Surname, @CPF, @Telephone, @Address, @selectCityId)";

                            SqlCommand saveCustomerCommand = new SqlCommand(saveCustomerQuery, connection);

                            saveCustomerCommand.Parameters.AddWithValue("@Name", firstName);
                            saveCustomerCommand.Parameters.AddWithValue("@Surname", secondName);
                            saveCustomerCommand.Parameters.AddWithValue("@CPF", cpf);
                            saveCustomerCommand.Parameters.AddWithValue("@Telephone", phone);
                            saveCustomerCommand.Parameters.AddWithValue("@Address", address);
                            saveCustomerCommand.Parameters.AddWithValue("@selectCityId", selectCityId);

                            int rowsAffected = saveCustomerCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                
                                MessageBoxResult resuldt = MessageBox.Show("Cliente cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                                if (resuldt == MessageBoxResult.OK)
                                {
                                    this.Close();
                                }
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

        private void Add_city_btn(object sender, RoutedEventArgs e)
        {
            CityRegister cityRegister = new CityRegister();
            cityRegister.Show();
        }

        private void cpfTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int originalCaretIndex = textBox.CaretIndex;
            int originalLength = textBox.Text.Length;

            textBox.TextChanged -= cpfTxtBox_TextChanged;

            if (textBox.Name == "cpfTxtBox")
            {
                if (textBox.Text != "000.000.000-00")
                {
                    string formattedCpf = ApplyCpfMask(textBox.Text);
                    textBox.Text = formattedCpf;
                    int newLength = textBox.Text.Length;
                    int lengthDifference = newLength - originalLength;
                    textBox.CaretIndex = originalCaretIndex + lengthDifference;
                }
            }

            textBox.TextChanged += cpfTxtBox_TextChanged;
        }

        private void UpdateCustomerData()
        {

            string firstName = firstNameTxtBox.Text;
            string secondName = secondNameTxtBox.Text;
            string cpf = cpfTxtBox.Text;
            string phone = phoneTxt.Text;
            string address = addressTxtBox.Text;

            cpf = cpf.Replace(".", "").Replace("-", "");
            phone = phone.Replace("(", "").Replace(")", "").Replace("-", "");
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                    UPDATE Customer
                    SET 
                    Name = @Name,
                    Surname = @Surname,
                    CPF = @CPF,
                    Cellphone = @Cellphone,
                    Address = @Address,
                    ID_City = @ID_City
                    WHERE ID_Customer = @ID_Customer";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", firstName);
                        cmd.Parameters.AddWithValue("@Surname", secondName);
                        cmd.Parameters.AddWithValue("@CPF", cpf);
                        cmd.Parameters.AddWithValue("@Cellphone", phone);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@ID_City", cityCbx.SelectedValue);
                        cmd.Parameters.AddWithValue("@ID_Customer", customerId);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Dados do cliente atualizados com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar os dados do cliente: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
