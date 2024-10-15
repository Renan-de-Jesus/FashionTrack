using Microsoft.Data.SqlClient;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Configuration;

namespace FashionTrack
{
    public partial class MarcaRegister : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
        private bool isEditMode = false; // Flag para identificar o modo de edição
        private int currentMarcaId = -1; // Armazena o ID da marca atual

        public MarcaRegister()
        {
            InitializeComponent();
        }

        private void MarcaIdTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Permitir apenas números inteiros
            e.Handled = !IsTextAllowedForId(e.Text);
        }

        private void MarcaNameTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

        private void MarcaNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PlaceholderTextBlock.Visibility = string.IsNullOrWhiteSpace(MarcaNameTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            SaveButton.IsEnabled = true; // Habilita o botão de salvar ao alterar o nome da marca
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string marcaName = MarcaNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(MarcaNameTextBox.Text))
            {
                MessageBox.Show("Campo marca não pode estar vazio");
                return;
            }

            if (IsMarcaNameDuplicate(marcaName))
            {
                MessageBox.Show("O nome da marca já está cadastrado. Por favor, escolha outro nome.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd;

                if (isEditMode && currentMarcaId != -1)
                {
                    // Atualizar o registro existente
                    cmd = new SqlCommand("UPDATE Marca SET MarcaNome = @MarcaNome WHERE MarcaId = @MarcaId", conn);
                    cmd.Parameters.AddWithValue("@MarcaId", currentMarcaId);
                }
                else
                {
                    // Inserir um novo registro
                    cmd = new SqlCommand("INSERT INTO Marca (MarcaNome) VALUES (@MarcaNome)", conn);
                }

                cmd.Parameters.AddWithValue("@MarcaNome", MarcaNameTextBox.Text);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show($"Marca '{MarcaNameTextBox.Text}' salva com sucesso!");
            ResetForm();
        }

        private bool IsMarcaNameDuplicate(string marcaName)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Marca WHERE MarcaNome = @MarcaNome", conn);
                cmd.Parameters.AddWithValue("@MarcaNome", marcaName);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MarcaIdTextBox.Text) && string.IsNullOrWhiteSpace(MarcaNameTextBox.Text))
            {
                MessageBox.Show("Por favor preenche um ou mais parâmetros para busca");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Marca WHERE MarcaId = @MarcaId OR MarcaNome = @MarcaNome", conn);
                cmd.Parameters.AddWithValue("@MarcaId", MarcaIdTextBox.Text);
                cmd.Parameters.AddWithValue("@MarcaNome", MarcaNameTextBox.Text);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    currentMarcaId = Convert.ToInt32(reader["MarcaId"]);
                    MarcaIdTextBox.Text = reader["MarcaId"].ToString();
                    MarcaNameTextBox.Text = reader["MarcaNome"].ToString();
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
            MarcaIdTextBox.Clear();
            MarcaNameTextBox.Clear();
            isEditMode = false;
            currentMarcaId = -1;
            SaveButton.IsEnabled = false;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MarcaIdTextBox.Text))
            {
                MessageBox.Show("Por favor preencha um ID para deletar");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Marca WHERE MarcaId = @MarcaId", conn);
                cmd.Parameters.AddWithValue("@MarcaId", MarcaIdTextBox.Text);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show($"Marca de ID '{MarcaIdTextBox.Text}' deletada com sucesso.");
            ResetForm();
        }
    }
}
