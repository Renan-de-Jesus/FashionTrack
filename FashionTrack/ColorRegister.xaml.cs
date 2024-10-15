using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
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
using System.Configuration;
using System.Text.RegularExpressions;

namespace FashionTrack
{
    public partial class ColorRegister : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
        private bool isEditMode = false; // Flag para identificar o modo de edição
        private int currentColorId = -1; // Armazena o ID da cor atual

        public ColorRegister()
        {
            InitializeComponent();
        }

        private void ColorIdTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Permitir apenas números inteiros
            e.Handled = !IsTextAllowedForId(e.Text);
        }

        private void ColorNameTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Permitir apenas letras
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
            // Verifica se o texto contém apenas letras
            Regex regex = new Regex("[^a-zA-Z]+"); // Apenas letras
            return !regex.IsMatch(text);
        }

        private void ColorNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PlaceholderTextBlock.Visibility = string.IsNullOrWhiteSpace(ColorNameTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            SaveButton.IsEnabled = true; // Habilita o botão de salvar ao alterar o nome da cor
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string colorName = ColorNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(ColorNameTextBox.Text))
            {
                MessageBox.Show("Campo cor não pode estar vazio");
                return;
            }

            if (IsColorNameDuplicate(colorName))
            {
                MessageBox.Show("O nome da cor já está cadastrado. Por favor, escolha outro nome.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd;

                if (isEditMode && currentColorId != -1)
                {
                    // Atualizar o registro existente
                    cmd = new SqlCommand("UPDATE Cor SET CorNome = @CorNome WHERE CorId = @CorId", conn);
                    cmd.Parameters.AddWithValue("@CorId", currentColorId);
                }
                else
                {
                    // Inserir um novo registro
                    cmd = new SqlCommand("INSERT INTO Cor (CorNome) VALUES (@CorNome)", conn);
                }

                cmd.Parameters.AddWithValue("@CorNome", ColorNameTextBox.Text);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show($"Cor '{ColorNameTextBox.Text}' salva com sucesso!");
            ResetForm();
        }

        private bool IsColorNameDuplicate(string colorName)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Cor WHERE CorNome = @CorNome", conn);
                cmd.Parameters.AddWithValue("@CorNome", colorName);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ColorIdTextBox.Text) && string.IsNullOrWhiteSpace(ColorNameTextBox.Text))
            {
                MessageBox.Show("Por favor preenche um ou mais parâmetros para busca");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Cor WHERE CorId = @CorId OR CorNome = @CorNome", conn);
                cmd.Parameters.AddWithValue("@CorId", ColorIdTextBox.Text);
                cmd.Parameters.AddWithValue("@CorNome", ColorNameTextBox.Text);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    currentColorId = Convert.ToInt32(reader["CorId"]);
                    ColorIdTextBox.Text = reader["CorId"].ToString();
                    ColorNameTextBox.Text = reader["CorNome"].ToString();
                    isEditMode = true; // Ativa o modo de edição
                    SaveButton.IsEnabled = false; // Desabilita o botão de salvar até que o nome da cor seja alterado
                }
                else
                {
                    MessageBox.Show("Cor não encontrada.");
                    ResetForm();
                }
            }
        }

        private void ResetForm()
        {
            ColorIdTextBox.Clear();
            ColorNameTextBox.Clear();
            isEditMode = false;
            currentColorId = -1;
            SaveButton.IsEnabled = false;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ColorIdTextBox.Text))
            {
                MessageBox.Show("Por favor preencha um ID para deletar");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Cor WHERE CorId = @CorId", conn);
                cmd.Parameters.AddWithValue("@CorId", ColorIdTextBox.Text);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show($"Cor de ID '{ColorIdTextBox.Text}' deletada com sucesso.");
            ResetForm();
        }
    }
}
