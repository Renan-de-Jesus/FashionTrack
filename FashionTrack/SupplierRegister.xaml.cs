using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FashionTrack
{
    public partial class SupplierRegister : Window
    {
        public SupplierRegister()
        {
            InitializeComponent();
            fillComboBox();
        }

        private void fillComboBox()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
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

        private void Window_Activated(object sender, EventArgs e)
        {
            fillComboBox();
        }


        private void RemoveText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Nome do representante" || textBox.Text == "Razão social" || textBox.Text == "Endereço do fornecedor" || textBox.Text == "00.000.000/0000-00" || textBox.Text == "(00)00000-0000")
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
                switch (textBox.Name)
                {
                    case "representativeTxtBox":
                        textBox.Text = "Nome do representante";
                        break;
                    case "corporateReasonTxtBox":
                        textBox.Text = "Razão social";
                        break;
                    case "addressTxtBox":
                        textBox.Text = "Endereço do fornecedor";
                        break;
                    case "cnpjTxtBox":
                        textBox.Text = "00.000.000/0000-00";
                        break;
                    case "phoneTxt":
                        textBox.Text = "(00)00000-0000";
                        break;
                }
                textBox.Opacity = 0.6;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            // Store caret position
            int caretIndex = textBox.CaretIndex;

            // Remove event handler to prevent recursive calls
            textBox.TextChanged -= TextBox_TextChanged;

            if (textBox.Name == "cnpjTxtBox")
            {
                // Existing CNPJ formatting logic
                string formattedCnpj = ApplyCnpjMask(textBox.Text);
                textBox.Text = formattedCnpj;
            }
            else if (textBox.Name == "phoneTxt")
            {
                // Apply phone mask
                string formattedPhone = ApplyPhoneMask(textBox.Text);
                textBox.Text = formattedPhone;

                // Adjust caret position after formatting
                // Ensure the caret position is set correctly
                caretIndex = Math.Min(caretIndex, formattedPhone.Length);
                textBox.CaretIndex = caretIndex;
            }

            // Re-attach event handler
            textBox.TextChanged += TextBox_TextChanged;
        }
        private string ApplyCnpjMask(string input)
        {
            input = new string(input.Where(char.IsDigit).ToArray());
            if (input.Length > 14) input = input.Substring(0, 14);

            if (input.Length > 2) input = input.Insert(2, ".");
            if (input.Length > 6) input = input.Insert(6, ".");
            if (input.Length > 10) input = input.Insert(10, "/");
            if (input.Length > 15) input = input.Insert(15, "-");

            return input;
        }

        private string ApplyPhoneMask(string input)
        {
            // Remove all non-digit characters
            string digits = new string(input.Where(char.IsDigit).ToArray());

            // Limit to a maximum of 11 digits
            if (digits.Length > 11)
                digits = digits.Substring(0, 11);

            // Return empty if no digits
            if (string.IsNullOrEmpty(digits)) return string.Empty;

            // Initialize the formatted number
            string formattedNumber = "";

            // Apply formatting
            if (digits.Length > 0)
            {
                formattedNumber += $"({digits.Substring(0, Math.Min(2, digits.Length))})"; // First two digits

                if (digits.Length > 2)
                {
                    formattedNumber += digits.Substring(2, Math.Min(5, digits.Length - 2)); // Next part (up to 5 digits)

                    if (digits.Length > 7)
                    {
                        formattedNumber += "-" + digits.Substring(7); // Remaining digits
                    }
                }
            }

            return formattedNumber;
        }
        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string corporateReason = corporateReasonTxtBox.Text;
                string cnpj = cnpjTxtBox.Text;
                string representative = representativeTxtBox.Text;
                string phone = phoneTxt.Text;
                string address = addressTxtBox.Text;

                cnpj = cnpj.Replace(".", "").Replace("-", "");
                phone = phone.Replace("(", "").Replace(")", "").Replace("-", "");

                if (string.IsNullOrEmpty(corporateReason) || corporateReasonTxtBox.Text == "Razão social")
                {
                    MessageBox.Show("Por favor, preencha a razão social do fornecedor!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    corporateReasonTxtBox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(cnpj) || cnpj == "00000000000000")
                {
                    MessageBox.Show("Por favor, preencha o cnpj do fornecedor!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    cnpjTxtBox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(representative) || representative == "Nome do representante")
                {
                    MessageBox.Show("Por favor, preencha o nome do representante!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    representativeTxtBox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(phone) || phone == "00000000000")
                {
                    MessageBox.Show("Por favor, preencha o telefone do fornecedor!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    phoneTxt.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(address) || address == "Endereço do fornecedor")
                {
                    MessageBox.Show("Por favor, preencha o endereço do fornecedor!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

                        string checkCustomerQuery = "SELECT COUNT(*) FROM Fornecedor WHERE CNPJ = @CNPJ";
                        SqlCommand checkCustomerCommand = new SqlCommand(checkCustomerQuery, connection);
                        checkCustomerCommand.Parameters.AddWithValue("@CNPJ", cnpj);

                        object result = checkCustomerCommand.ExecuteScalar();
                        int customerCount = result != null ? Convert.ToInt32(result) : 0;

                        if (customerCount > 0)
                        {
                            MessageBox.Show("Fornecedor já cadastrado com este CNPJ.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            string saveSupplierQuery = "INSERT INTO Fornecedor (NomeRazaoSocial, CNPJ, Endereco, Telefone, ID_Cidade, NomeRepresentante) " +
                                                        "VALUES (@corporateReason, @CNPJ, @address, @phone, @selectCityId, @representative)";

                            SqlCommand saveSupplierCommand = new SqlCommand(saveSupplierQuery, connection);

                            saveSupplierCommand.Parameters.AddWithValue("@corporateReason", corporateReason);
                            saveSupplierCommand.Parameters.AddWithValue("@CNPJ", cnpj);
                            saveSupplierCommand.Parameters.AddWithValue("@address", address);
                            saveSupplierCommand.Parameters.AddWithValue("@phone", phone);
                            saveSupplierCommand.Parameters.AddWithValue("@selectCityId", selectCityId);
                            saveSupplierCommand.Parameters.AddWithValue("@representative", representative);

                            int rowsAffected = saveSupplierCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Fornecedor cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Erro ao cadastrar o Fornecedor.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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