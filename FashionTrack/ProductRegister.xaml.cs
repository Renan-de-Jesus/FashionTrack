using Microsoft.Data.SqlClient;
using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FashionTrack
{
    public partial class ProductRegister : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
        private bool isEditMode = false;
        private int currentProductID = -1;

         private void RemoveText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Código da Marca" || textBox.Text == "Cor" || textBox.Text == "Descrição" || textBox.Text == "Preço")
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
                if (textBox.Name == "BrandCodeTextBox")
                {
                    textBox.Text = "Código da Marca";
                }
                else if (textBox.Name == "CorTextBox")
                {
                    textBox.Text = "Cor";
                }
                else if (textBox.Name == "DescriptionTextBox")
                {
                    textBox.Text = "Descrição";
                }
                else if (textBox.Name == "PriceTextBox")
                {
                    textBox.Text = "Preço";
                }
                textBox.Opacity = 0.6;
            }
        }

        public ProductRegister()
        {
            InitializeComponent();
            LoadComboBox(BrandComboBox, "Brand", "BrandName", "BrandId");
            LoadComboBox(ColorComboBox, "Color", "ColorName", "ColorId");
            LoadComboBox(SizeComboBox, "Size", "SizeDescription", "SizeId");
        }

        public ProductRegister(int productId) : this()
        {
            isEditMode = true;
            currentProductID = productId;
            LoadProductData();
        }

        private void LoadProductData()
        {
            if (currentProductID == -1) return;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Produto WHERE ID_Produto = @ID_Produto";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID_Produto", currentProductID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            DescriptionTextBox.Text = reader["Description"].ToString();
                            PriceTextBox.Text = reader["Price"].ToString();
                            BrandCodeTextBox.Text = reader["BrandCode"].ToString();
                            GenderComboBox.Text = reader["Gender"].ToString();
                            SelectComboBoxItemByValue(BrandComboBox, reader["BrandId"]);
                            SelectComboBoxItemByValue(ColorComboBox, reader["ColorId"]);
                            SelectComboBoxItemByValue(SizeComboBox, reader["SizeId"]);
                        }
                    }
                }
            }
        }

        private void SelectComboBoxItemByValue(ComboBox comboBox, object value)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Tag.Equals(value))
                {
                    comboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void LoadComboBox(ComboBox comboBox, string tableName, string displayMember, string valueMember)
        {
            comboBox.Items.Clear();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = $"SELECT * FROM {tableName}";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ComboBoxItem item = new ComboBoxItem
                        {
                            Content = reader[displayMember].ToString(),
                            Tag = reader[valueMember]
                        };
                        comboBox.Items.Add(item);
                    }
                }
            }
        }

        private void ProductIdTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowedForId(e.Text);
        }

        private void PriceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowedForPrice(e.Text);
        }

        private static bool IsTextAllowedForPrice(string text)
        {
            Regex regex = new Regex("^[0-9,]*$");
            return regex.IsMatch(text);
        }

        private static bool IsTextAllowedForId(string text)
        {
            Regex regex = new Regex("[^0-9]+");
            return !regex.IsMatch(text);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string description = DescriptionTextBox.Text;
            string priceText = PriceTextBox.Text;
            string brandCode = BrandCodeTextBox.Text;
            string gender = GenderComboBox.Text;

            if (!decimal.TryParse(priceText, out decimal price))
            {
                MessageBox.Show("Invalid price format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Round the price to two decimal places
            price = Math.Round(price, 2);

            int brandId = GetSelectedItemId(BrandComboBox);
            int colorId = GetSelectedItemId(ColorComboBox);
            int sizeId = GetSelectedItemId(SizeComboBox);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                try
                {
                    using (SqlCommand cmd = CreateCommand(conn, description, price, brandCode, gender, brandId, colorId, sizeId))
                    {
                        if (isEditMode && currentProductID != -1)
                        {
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Produto salvo com sucesso!", "Boa deu certo!", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            currentProductID = (int)cmd.ExecuteScalar();
                            MessageBox.Show("Product salvo com sucesso!", "Boa deu certo!", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private SqlCommand CreateCommand(SqlConnection conn, string description, decimal price, string brandCode, string gender, int brandId, int colorId, int sizeId)
        {
            SqlCommand cmd;

            if (isEditMode && currentProductID != -1)
            {
                cmd = new SqlCommand("UPDATE Produto SET Description = @Description, Price = @Price, BrandCode = @BrandCode, " +
                                        "Gender = @Gender, BrandId = @BrandId, ColorId = @ColorId, SizeId = @SizeId WHERE ID_Produto = @ID_Produto", conn);
                cmd.Parameters.AddWithValue("@ID_Produto", currentProductID);
            }
            else
            {
                cmd = new SqlCommand("INSERT INTO Produto (Description, Price, BrandCode, Gender, BrandId, ColorId, SizeId) OUTPUT INSERTED.ID_Produto " +
                                        "VALUES (@Description, @Price, @BrandCode, @Gender, @BrandId, @ColorId, @SizeId) ", conn);
            }

            cmd.Parameters.AddWithValue("@Description", description);
            cmd.Parameters.AddWithValue("@Price", price);
            cmd.Parameters.AddWithValue("@BrandCode", brandCode);
            cmd.Parameters.AddWithValue("@Gender", gender);
            cmd.Parameters.AddWithValue("@BrandId", brandId);
            cmd.Parameters.AddWithValue("@ColorId", colorId);
            cmd.Parameters.AddWithValue("@SizeId", sizeId);

            return cmd;
        }

        private bool ValidateInputs(out string errorMessage)
        {
            errorMessage = "";

            if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
                errorMessage = "Campo Descrição não pode estar vazio.";
            else if (string.IsNullOrWhiteSpace(PriceTextBox.Text))
                errorMessage = "Campo Preço não pode estar vazio.";
            else if (string.IsNullOrWhiteSpace(BrandCodeTextBox.Text))
                errorMessage = "Campo Código da Marca não pode estar vazio";
            else if (GetSelectedItemId(BrandComboBox) == -1)
                errorMessage = "Nenhuma Marca Selecionada.";
            else if (GetSelectedItemId(ColorComboBox) == -1)
                errorMessage = "Nenhuma Cor Selecionada.";
            else if (GetSelectedItemId(SizeComboBox) == -1)
                errorMessage = "Nenhum Tamanho Selecionado.";

            return string.IsNullOrEmpty(errorMessage);
        }

        private int GetSelectedItemId(ComboBox comboBox)
        {
            return comboBox.SelectedItem is ComboBoxItem item ? (int)item.Tag : -1;
        }

        private void OpenColorRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            ColorRegister colorRegister = new ColorRegister();
            colorRegister.Show();
            LoadComboBox(ColorComboBox, "Color", "ColorName", "ColorId");
        }

        private void OpenBrandRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            BrandRegister brandRegister = new BrandRegister();
            brandRegister.ShowDialog();
            LoadComboBox(BrandComboBox, "Brand", "BrandName", "BrandId");
        }

        private void OpenSizeRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            SizeRegister sizeRegister = new SizeRegister();
            sizeRegister.ShowDialog();
            LoadComboBox(SizeComboBox, "Size", "SizeDescription", "SizeId");
        }

    }
}
