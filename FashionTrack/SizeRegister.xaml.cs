using Microsoft.Data.SqlClient;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Configuration;

namespace FashionTrack
{
    public partial class TamanhoRegister : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
        private bool isEditMode = false; // Flag para identificar o modo de edição
        private int currentTamanhoId = -1; // Armazena o ID do tamanho atual

        public TamanhoRegister()
        {
            InitializeComponent();
        }

        private void TamanhoDescricaoTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Permitir letras e números
            e.Handled = !IsTextAllowedForDescription(e.Text);
        }
        private void TamanhoIdTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

        private void TamanhoDescricaoTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PlaceholderTextBlock.Visibility = string.IsNullOrWhiteSpace(TamanhoDescricaoTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            SaveButton.IsEnabled = true; // Habilita o botão de salvar ao alterar o nome da cor
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string tamanhoDescricao = TamanhoDescricaoTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(tamanhoDescricao))
            {
                MessageBox.Show("O campo Descrição do Tamanho não pode estar vazio");
                return;
            }

            if (IsTamanhoDescricaoDuplicate(tamanhoDescricao))
            {
                MessageBox.Show("A descrição do tamanho já está cadastrada. Por favor, escolha outra descrição.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd;

                if (isEditMode && currentTamanhoId != -1)
                {
                    // Atualizar o registro existente
                    cmd = new SqlCommand("UPDATE Tamanho SET TamanhoDescricao = @TamanhoDescricao WHERE TamanhoId = @TamanhoId", conn);
                    cmd.Parameters.AddWithValue("@TamanhoId", currentTamanhoId);
                }
                else
                {
                    // Inserir um novo registro
                    cmd = new SqlCommand("INSERT INTO Tamanho (TamanhoDescricao) VALUES (@TamanhoDescricao)", conn);
                }

                cmd.Parameters.AddWithValue("@TamanhoDescricao", tamanhoDescricao);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show($"Tamanho '{tamanhoDescricao}' salvo com sucesso!");
            TamanhoIdTextBox.Clear();
            TamanhoDescricaoTextBox.Clear();
        }

        private bool IsTamanhoDescricaoDuplicate(string tamanhoDescricao)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Tamanho WHERE TamanhoDescricao = @TamanhoDescricao", conn);
                cmd.Parameters.AddWithValue("@TamanhoDescricao", tamanhoDescricao);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private void ResetForm()
        {
            TamanhoDescricaoTextBox.Clear();
            isEditMode = false;
            currentTamanhoId = -1;
            SaveButton.IsEnabled = false;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentTamanhoId == -1)
            {
                MessageBox.Show("Por favor, selecione um tamanho para deletar");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Tamanho WHERE TamanhoId = @TamanhoId", conn);
                cmd.Parameters.AddWithValue("@TamanhoId", currentTamanhoId);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show($"Tamanho de ID '{currentTamanhoId}' deletado com sucesso.");
            ResetForm();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TamanhoIdTextBox.Text) && string.IsNullOrWhiteSpace(TamanhoDescricaoTextBox.Text))
            {
                MessageBox.Show("Por favor preenche um ou mais parâmetros para busca");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Tamanho WHERE TamanhoId = @TamanhoId OR TamanhoDescricao = @TamanhoDescricao", conn);
                cmd.Parameters.AddWithValue("@TamanhoId", TamanhoIdTextBox.Text);
                cmd.Parameters.AddWithValue("@TamanhoDescricao", TamanhoDescricaoTextBox.Text);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    currentTamanhoId = Convert.ToInt32(reader["TamanhoId"]);
                    TamanhoIdTextBox.Text = reader["TamanhoId"].ToString();
                    TamanhoDescricaoTextBox.Text = reader["TamanhoDescricao"].ToString();
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
            TamanhoIdTextBox.Clear();
            TamanhoDescricaoTextBox.Clear();
            isEditMode = false;
            currentTamanhoId = -1;
            SaveButton.IsEnabled = false;
        }

        private void TamanhoIdTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //aa
        }
    }
}
