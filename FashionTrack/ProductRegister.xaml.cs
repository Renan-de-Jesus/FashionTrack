﻿using Microsoft.Data.SqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FashionTrack
{
    public partial class ProductRegister : Window
    {
        private int productId;

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
            //LoadComboBox(SupplierComboBox, "Supplier", "CorporateName", "ID_Supplier");
        }

        public ProductRegister(int productId) : this()
        {
            this.productId = productId;
            isEditMode = true;
            currentProductID = productId;
            LoadProductData(productId);
        }

        private void LoadProductData(int productId)
        {
            if (currentProductID == -1) return;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Product WHERE ID_Product = @ID_Product";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID_Product", currentProductID);
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
                string query = $"SELECT * FROM {tableName} ORDER BY {displayMember} ASC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    foreach (DataRow row in dt.Rows)
                    {
                        ComboBoxItem item = new ComboBoxItem
                        {
                            Content = row[displayMember].ToString(),
                            Tag = row[valueMember]
                        };
                        comboBox.Items.Add(item);
                    }
                    if (dt.Rows.Count > 0)
                    {
                        comboBox.SelectedIndex = 0;
                    }
                }
            }
        }

        private void UpdateProduct()
        {
            string brandCode = BrandCodeTextBox.Text;
            decimal price = decimal.Parse(PriceTextBox.Text);
            string description = DescriptionTextBox.Text;
            int brandId = (int)(BrandComboBox.SelectedItem as ComboBoxItem)?.Tag;
            int sizeId = (int)(SizeComboBox.SelectedItem as ComboBoxItem)?.Tag;
            int colorId = (int)(ColorComboBox.SelectedItem as ComboBoxItem)?.Tag;
            string gender = GenderComboBox.Text;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("UpdateProduct", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ProductId", productId);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@BrandCode", brandCode);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Parameters.AddWithValue("@BrandId", brandId);
                        cmd.Parameters.AddWithValue("@ColorId", colorId);
                        cmd.Parameters.AddWithValue("@SizeId", sizeId);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Produto atualizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao atualizar produto: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (productId > 0)
            {
                UpdateProduct();
                return;
            }

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
                MessageBox.Show("Preço com formato inválido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            price = Math.Round(price, 2);

            int brandId = GetSelectedItemId(BrandComboBox);
            int colorId = GetSelectedItemId(ColorComboBox);
            int sizeId = GetSelectedItemId(SizeComboBox);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("InsertProduct", conn, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Passar os parâmetros
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@BrandCode", brandCode);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Parameters.AddWithValue("@BrandId", brandId);
                        cmd.Parameters.AddWithValue("@ColorId", colorId);
                        cmd.Parameters.AddWithValue("@SizeId", sizeId);

                        // Executar a procedure
                        cmd.ExecuteNonQuery();

                        // Commit da transação
                        transaction.Commit();
                    }

                    MessageBox.Show("Produto salvo com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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
