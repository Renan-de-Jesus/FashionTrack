using Microsoft.Data.SqlClient;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Configuration;

namespace FashionTrack
{
    public partial class BrandRegister : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
        private bool isEditMode = false; // Flag para identificar o modo de edição
        private int currentBrandId = -1; // Armazena o ID da marca atual

        public BrandRegister()
        {
            InitializeComponent();
        }

        private void BrandIdTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Permitir apenas números inteiros
            e.Handled = !IsTextAllowedForId(e.Text);
        }

        private void BrandNameTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Permitir letras e números
            e.Handled = !IsTextAllowedForName(e.Text);
        }

        private static bool IsTextAllowedForId(string text)
        {
            // Verifica se o texto é um número inteiro
            Regex regex = new Regex("[^0-9]+"); // Apenas números
            return !regex.IsMatch(text);
        }

        private static bool IsTextAllowedForName(string text)
        {
            // Verifica se o texto contém apenas letras e números
            Regex regex = new Regex("[^a-zA-Z0-9]+"); // Apenas letras e números
            return !regex.IsMatch(text);
        }

        private void BrandNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PlaceholderTextBlock.Visibility = string.IsNullOrWhiteSpace(BrandNameTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            SaveButton.IsEnabled = true; // Habilita o botão de salvar ao alterar o nome da marca
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string brandName = BrandNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(BrandNameTextBox.Text))
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
                    // Atualizar o registro existente
                    cmd = new SqlCommand("UPDATE Brand SET BrandName = @BrandName WHERE BrandId = @BrandId", conn);
                    cmd.Parameters.AddWithValue("@BrandId", currentBrandId);
                }
                else
                {
                    // Inserir um novo registro
                    cmd = new SqlCommand("INSERT INTO Brand (BrandName) VALUES (@BrandName)", conn);
                }

                cmd.Parameters.AddWithValue("@BrandName", BrandNameTextBox.Text);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show($"Marca '{BrandNameTextBox.Text}' salva com sucesso!");
            ResetForm();
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BrandIdTextBox.Text) && string.IsNullOrWhiteSpace(BrandNameTextBox.Text))
            {
                MessageBox.Show("Por favor preenche um ou mais parâmetros para busca");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Brand WHERE BrandId = @BrandId OR BrandName = @BrandName", conn);
                cmd.Parameters.AddWithValue("@BrandId", BrandIdTextBox.Text);
                cmd.Parameters.AddWithValue("@BrandName", BrandNameTextBox.Text);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    currentBrandId = Convert.ToInt32(reader["BrandId"]);
                    BrandIdTextBox.Text = reader["BrandId"].ToString();
                    BrandNameTextBox.Text = reader["BrandName"].ToString();
                    isEditMode = true; // Ativa o modo de edição
                    SaveButton.IsEnabled = false; // Desabilita o botão de salvar até que o nome da marca seja alterado
                }
                else
                {
                    MessageBox.Show("Marca não encontrada.");
                    ResetForm();
                }
            }
        }

        private void ResetForm()
        {
            BrandIdTextBox.Clear();
            BrandNameTextBox.Clear();
            isEditMode = false;
            currentBrandId = -1;
            SaveButton.IsEnabled = false;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BrandIdTextBox.Text))
            {
                MessageBox.Show("Por favor preencha um ID para deletar");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Brand WHERE BrandId = @BrandId", conn);
                cmd.Parameters.AddWithValue("@BrandId", BrandIdTextBox.Text);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show($"Marca de ID '{BrandIdTextBox.Text}' deletada com sucesso.");
            ResetForm();
        }
    }
}
