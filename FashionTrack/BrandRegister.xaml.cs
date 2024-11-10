using Microsoft.Data.SqlClient;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Configuration;
using System.ComponentModel;

namespace FashionTrack
{
    public partial class BrandRegister : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
        private bool isEditMode = false; 
        private int currentBrandId = -1; 

        private void RemoveText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Digite o nome da marca")
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
                textBox.Text = "Digite o nome da marca";
                textBox.Opacity = 0.6;
            }
        }
        public BrandRegister()
        {
            InitializeComponent();
        }

        private void BrandIdTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowedForId(e.Text);
        }

        private void BrandNameTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowedForName(e.Text);
        }

        private static bool IsTextAllowedForId(string text)
        {
            Regex regex = new Regex("[^0-9]+");
            return !regex.IsMatch(text);
        }

        private static bool IsTextAllowedForName(string text)
        {
            Regex regex = new Regex("[^a-zA-Z0-9]+");
            return !regex.IsMatch(text);
        }

        private void BrandNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
          
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string brandName = BrandNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(BrandNameTextBox.Text) || BrandNameTextBox.Text == "Digite o nome da marca" )
            {
                MessageBox.Show("Campo marca não pode estar vazio");
                return;
            }

            if (IsBrandNameDuplicate(brandName))
            {
                MessageBox.Show("O nome da marca já está cadastrado. Por favor, escolha outro nome.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd;

                if (isEditMode && currentBrandId != -1)
                {
                    cmd = new SqlCommand("UPDATE Brand SET BrandName = @BrandName WHERE BrandId = @BrandId", conn);
                    cmd.Parameters.AddWithValue("@BrandId", currentBrandId);
                }
                else
                {
                    cmd = new SqlCommand("INSERT INTO Brand (BrandName) VALUES (@BrandName)", conn);
                }

                cmd.Parameters.AddWithValue("@BrandName", BrandNameTextBox.Text);
                cmd.ExecuteNonQuery();
            }
            MessageBoxResult result = MessageBox.Show($"Marca '{BrandNameTextBox.Text}' salvo com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            if (result == MessageBoxResult.OK)
            {           
                this.Close();
            }
        }

        private bool IsBrandNameDuplicate(string brandName)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Brand WHERE BrandName = @BrandName", conn);
                cmd.Parameters.AddWithValue("@BrandName", brandName);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private void ResetForm()
        {

            isEditMode = false;
            currentBrandId = -1;
            SaveButton.IsEnabled = false;
        }
       
    }
}
