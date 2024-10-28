using Microsoft.Data.SqlClient;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Configuration;

namespace FashionTrack
{
    public partial class SizeRegister : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
        private bool isEditMode = false; 
        private int currentSizeId = -1; 

        public SizeRegister()
        {
            InitializeComponent();
        }

        private void RemoveText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Digite o tamanho (P, M, G, etc.)")
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
                textBox.Text = "Digite o tamanho (P, M, G, etc.)";
                textBox.Opacity = 0.6;
            }
        }

        private void SizeDescriptionTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Permitir letras e números
            e.Handled = !IsTextAllowedForDescription(e.Text);
        }
        private void SizeIdTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Permitir apenas números inteiros
            e.Handled = !IsTextAllowedForId(e.Text);
        }

        private static bool IsTextAllowedForId(string text)
        {
            // Verifica se o texto é um número inteiro
            Regex regex = new Regex("[^0-9]+"); // Apenas números
            return !regex.IsMatch(text);
        }

        private static bool IsTextAllowedForDescription(string text)
        {
            // Verifica se o texto contém apenas letras e números
            Regex regex = new Regex("[^a-zA-Z0-9]+"); // Apenas letras e números
            return !regex.IsMatch(text);
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string sizeDescription = SizeDescriptionTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(sizeDescription) || SizeDescriptionTextBox.Text == "Digite o tamanho (P, M, G, etc.)")
            {
                MessageBox.Show("O campo Descrição do Tamanho não pode estar vazio");
                return;
            }

            if (IsSizeDescricaoDuplicate(sizeDescription))
            {
                MessageBox.Show("A descrição do tamanho já está cadastrada. Por favor, escolha outra descrição.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd;

                if (isEditMode && currentSizeId != -1)
                {
                    // Atualizar o registro existente
                    cmd = new SqlCommand("UPDATE Size SET SizeDescription = @SizeDescription WHERE SizeId = @SizeId", conn);
                    cmd.Parameters.AddWithValue("@TamanhoId", currentSizeId);
                }
                else
                {
                    cmd = new SqlCommand("INSERT INTO Size (SizeDescription) VALUES (@SizeDescription)", conn);
                }

                cmd.Parameters.AddWithValue("@SizeDescription", sizeDescription);
                cmd.ExecuteNonQuery();
            }

            MessageBoxResult result = MessageBox.Show($"Tamanho '{sizeDescription}' salvo com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            if (result == MessageBoxResult.OK)
            {
                SizeDescriptionTextBox.Clear();
                this.Close();
            }
        }

        private bool IsSizeDescricaoDuplicate(string sizeDescription)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Size WHERE SizeDescription = @SizeDescription", conn);
                cmd.Parameters.AddWithValue("@SizeDescription", sizeDescription);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private void ResetForm()
        {
            SizeDescriptionTextBox.Clear();
            isEditMode = false;
            currentSizeId = -1;
            SaveButton.IsEnabled = false;
        }

        private void ResetaForm()
        {
            SizeDescriptionTextBox.Clear();
            isEditMode = false;
            currentSizeId = -1;
            SaveButton.IsEnabled = false;
        }
    }
}
