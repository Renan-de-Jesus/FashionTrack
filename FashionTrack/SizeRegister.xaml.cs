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
        private bool isEditMode = false; // Flag para identificar o modo de edição
        private int currentSizeId = -1; // Armazena o ID do tamanho atual

        public SizeRegister()
        {
            InitializeComponent();
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

        private void SizeDescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PlaceholderTextBlock.Visibility = string.IsNullOrWhiteSpace(SizeDescriptionTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            SaveButton.IsEnabled = true; // Habilita o botão de salvar ao alterar o nome da cor
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string sizeDescription = SizeDescriptionTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(sizeDescription))
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
                    // Inserir um novo registro
                    cmd = new SqlCommand("INSERT INTO Size (SizeDescription) VALUES (@SizeDescription)", conn);
                }

                cmd.Parameters.AddWithValue("@SizeDescription", sizeDescription);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show($"Tamanho '{sizeDescription}' salvo com sucesso!");
            SizeIdTextBox.Clear();
            SizeDescriptionTextBox.Clear();
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

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentSizeId == -1)
            {
                MessageBox.Show("Por favor, selecione um tamanho para deletar");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Size WHERE SizeId = @SizeId", conn);
                cmd.Parameters.AddWithValue("@SizeId", currentSizeId);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show($"Tamanho de ID '{currentSizeId}' deletado com sucesso.");
            ResetForm();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SizeIdTextBox.Text) && string.IsNullOrWhiteSpace(SizeDescriptionTextBox.Text))
            {
                MessageBox.Show("Por favor preenche um ou mais parâmetros para busca");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Size WHERE SizeId = @SizeId OR SizeDescription = @SizeDescription", conn);
                cmd.Parameters.AddWithValue("@SizeId", SizeIdTextBox.Text);
                cmd.Parameters.AddWithValue("@SizeDescription", SizeDescriptionTextBox.Text);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    currentSizeId = Convert.ToInt32(reader["SizeId"]);
                    SizeIdTextBox.Text = reader["SizeId"].ToString();
                    SizeDescriptionTextBox.Text = reader["SizeDescription"].ToString();
                    isEditMode = true;
                    SaveButton.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show("Tamanho não encontrado.");
                }
            }
        }
        private void ResetaForm()
        {
            SizeIdTextBox.Clear();
            SizeDescriptionTextBox.Clear();
            isEditMode = false;
            currentSizeId = -1;
            SaveButton.IsEnabled = false;
        }

        private void SizeIdTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //aa
        }
    }
}
