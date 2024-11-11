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
        private int supplierId;

        public SupplierRegister()
        {
            InitializeComponent();
            fillComboBox();
        }

        public SupplierRegister(int supplierId) : this()
        {
            this.supplierId = supplierId;
            //this.Loaded += SupplierRegister_Loaded;

            if (supplierId > 0)
            {
                LoadSupplierData(supplierId);
            }
        }

        private void LoadSupplierData(int supplierId)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                    SELECT 
                        CorporateName, 
                        CNPJ, 
                        Address, 
                        Telephone, 
                        Representative, 
                        ID_City
                        FROM Supplier 
                        WHERE ID_Supplier = @ID_Supplier";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ID_Supplier", supplierId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                representativeTxtBox.Text = reader["Representative"].ToString();
                                corporateReasonTxtBox.Text = reader["CorporateName"].ToString();
                                addressTxtBox.Text = reader["Address"].ToString();
                                cnpjTxtBox.Text = reader["CNPJ"].ToString();
                                phoneTxt.Text = reader["Telephone"].ToString();
                                cityCbx.ItemsSource = reader["ID_City"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar os dados do fornecedor: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            int originalCaretIndex = textBox.CaretIndex;
            int originalLength = textBox.Text.Length;

            textBox.TextChanged -= TextBox_TextChanged;

            if (textBox.Name == "cnpjTxtBox")
            {
                // Avoid formatting placeholder text
                if (textBox.Text != "00.000.000/0000-00")
                {
                    string formattedCnpj = ApplyCnpjMask(textBox.Text);
                    textBox.Text = formattedCnpj;
                }
            }
            else if (textBox.Name == "phoneTxt")
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

        private string ApplyCnpjMask(string input)
        {
            // Remove all non-digit characters
            input = new string(input.Where(char.IsDigit).ToArray());

            // Limit to 14 digits
            if (input.Length > 14)
                input = input.Substring(0, 14);

            // Format CNPJ
            if (input.Length > 2) input = input.Insert(2, ".");
            if (input.Length > 6) input = input.Insert(6, ".");
            if (input.Length > 10) input = input.Insert(10, "/");
            if (input.Length > 15) input = input.Insert(15, "-");

            return input;
        }

        private void updateSupplier()
        {
            string corporateReason = corporateReasonTxtBox.Text;
            string cnpj = cnpjTxtBox.Text;
            string representative = representativeTxtBox.Text;
            string phone = phoneTxt.Text;
            string address = addressTxtBox.Text;
            int selectCityId = (int)cityCbx.SelectedValue;

            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            phone = phone.Replace("(", "").Replace(")", "").Replace("-", "");

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string updateSupplier = "UPDATE Supplier SET CorporateName = @corporateName, Address = @address, " +
                        "Telephone = @phone, Representative = @representative, ID_City = @cityId " +
                        " WHERE ID_Supplier = @supplierId";
                    SqlCommand update = new SqlCommand(updateSupplier, connection);

                    update.Parameters.AddWithValue("@corporateName", corporateReason);
                    update.Parameters.AddWithValue("@CNPJ", cnpj);
                    update.Parameters.AddWithValue("@address", address);
                    update.Parameters.AddWithValue("@phone", phone);
                    update.Parameters.AddWithValue("@representative", representative);
                    update.Parameters.AddWithValue("@cityId", selectCityId);
                    update.Parameters.AddWithValue("@supplierId", supplierId);

                    update.ExecuteNonQuery();
                }
               MessageBoxResult result = MessageBox.Show("Dados do cliente atualizados com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                if (result == MessageBoxResult.OK)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar os dados do cliente: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }


        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if(supplierId > 0)
            {
                updateSupplier();
                return;
            }
            try
            {
                string corporateReason = corporateReasonTxtBox.Text;
                string cnpj = cnpjTxtBox.Text;
                string representative = representativeTxtBox.Text;
                string phone = phoneTxt.Text;
                string address = addressTxtBox.Text;

                cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
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

                        string checkSupplierQuery = "SELECT COUNT(*) FROM Supplier WHERE CNPJ = @CNPJ";
                        SqlCommand checkSupplierCommand = new SqlCommand(checkSupplierQuery, connection);
                        checkSupplierCommand.Parameters.AddWithValue("@CNPJ", cnpj);

                        object result = checkSupplierCommand.ExecuteScalar();
                        int supplierCount = result != null ? Convert.ToInt32(result) : 0;

                        if (supplierCount > 0)
                        {
                            MessageBox.Show("Fornecedor já cadastrado com este CNPJ.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            string saveSupplierQuery = "INSERT INTO Supplier (CorporateName, CNPJ, Address, Telephone, Representative, ID_City) " +
                                                        "VALUES (@corporateName, @CNPJ, @address, @telephone, @representative, @cityId)";

                            SqlCommand saveSupplierCommand = new SqlCommand(saveSupplierQuery, connection);

                            saveSupplierCommand.Parameters.AddWithValue("@corporateName", corporateReason);
                            saveSupplierCommand.Parameters.AddWithValue("@CNPJ", cnpj);
                            saveSupplierCommand.Parameters.AddWithValue("@address", address);
                            saveSupplierCommand.Parameters.AddWithValue("@telephone", phone);
                            saveSupplierCommand.Parameters.AddWithValue("@representative", representative);
                            saveSupplierCommand.Parameters.AddWithValue("@cityId", selectCityId);

                            int rowsAffected = saveSupplierCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBoxResult resuldt = MessageBox.Show("Fornecedor cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                                if (resuldt == MessageBoxResult.OK)
                                {
                                    this.Close();
                                }
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
