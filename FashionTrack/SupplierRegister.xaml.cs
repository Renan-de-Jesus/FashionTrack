using Microsoft.Data.SqlClient;
using System;
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

        private void removeTextCorporateReason(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "RAZÃO SOCIAL")
            {
                textBox.Text = "";
                textBox.Opacity = 1;
            }
        }

        private void addTextFirstName(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "RAZÃO SOCIAL";
                textBox.Opacity = 0.6;
            }
        }
        private void removeTextCNPJ(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "00.000.000/0000-00")
            {
                textBox.Text = "";
                textBox.Opacity = 1;
            }
        }

        private void addTextCNPJ(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "00.000.000/0000-00";
                textBox.Opacity = 0.6;
            }
        }

        private void removeTextAddress(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "ENDEREÇO DO FORNECEDOR")
            {
                textBox.Text = "";
                textBox.Opacity = 1;
            }
        }

        private void addTextAddress(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "ENDEREÇO DO FORNECEDOR";
                textBox.Opacity = 0.6;
            }
        }

        private void removeTextPhone(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "(99)99999-9999")
            {
                textBox.Text = "";
                textBox.Opacity = 1;
            }
        }

        private void addTextPhone(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "(99)99999-9999";
                textBox.Opacity = 0.6;
            }
        }

        private void removeTextRepresentative(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "NOME DO REPRESENTANTE")
            {
                textBox.Text = "";
                textBox.Opacity = 1;
            }
        }

        private void addTextRepresentative(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "NOME DO REPRESENTANTE";
                textBox.Opacity = 0.6;
            }
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

                if (string.IsNullOrEmpty(corporateReason))
                {
                    MessageBox.Show("Por favor, preencha a razão social do fornecedor!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    corporateReasonTxtBox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(cnpj))
                {
                    MessageBox.Show("Por favor, preencha o cnpj do fornecedor!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    cnpjTxtBox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(representative))
                {
                    MessageBox.Show("Por favor, preencha o nome do representante!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    representativeTxtBox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(phone))
                {
                    MessageBox.Show("Por favor, preencha o telefone do fornecedor!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    phoneTxt.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(address))
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

        private void addCityBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CityRegister cityRegister = new CityRegister();
            cityRegister.Show();
        }
    }
}