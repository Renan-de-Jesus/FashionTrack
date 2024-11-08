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
                // Avoid formatting placeholder text
                if (textBox.Text != "(00)00000-0000")
                {
                    string formattedPhone = ApplyPhoneMask(textBox.Text, allowParenthesisErase: true);
                    textBox.Text = formattedPhone;
                }
            }

            // Adjust the caret position accordingly
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
                    case "secundNameTxtBox":
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
            // Remove all non-digit characters
            string digits = new string(input.Where(char.IsDigit).ToArray());

            // Limit to 11 digits
            if (digits.Length > 11)
                digits = digits.Substring(0, 11);

            // Return empty if no digits are present
            if (string.IsNullOrEmpty(digits)) return string.Empty;

            // Format phone number
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
        private void fillComboBox()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
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
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            fillComboBox();
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

                if (string.IsNullOrEmpty(firstName) || firstName == "Nome do cliente")
                {
                    MessageBox.Show("Por favor, preencha o nome do cliente!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    firstNameTxtBox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(secundName) || secundName == "Sobrenome")
                {
                    MessageBox.Show("Por favor, preencha o sobrenome do cliente!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    secundNameTxtBox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(cpf)|| cpf == "000.000.000-00")
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
                            MessageBox.Show("Cliente já cadastrado com este CPF.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            string saveCustomerQuery = "INSERT INTO Customer (Name, Surname, CPF, Cellphone, Address, ID_City) " +
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

        private void Add_city_btn(object sender, RoutedEventArgs e)
        {
            CityRegister cityRegister = new CityRegister();
            cityRegister.Show();
        }
    }
}
