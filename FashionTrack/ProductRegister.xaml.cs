using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace FashionTrack
{
    public partial class ProductRegister : Window
    {
        public ProductRegister()
        {
            InitializeComponent();
        }
        private void SalvarButton_Click(object sender, RoutedEventArgs e)
        {
            string codigoMarca = CodigoMarcaTextBox.Text;
            string cor = CorTextBox.Text;
            string descricao = DescricaoTextBox.Text;
            string tamanho = (TamanhoComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string genero = (GeneroComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string precoString = PrecoTextBox.Text;
            decimal preco;

            if (string.IsNullOrWhiteSpace(codigoMarca))
            {
                MessageBox.Show("O campo Código da Marca está vazio.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                CodigoMarcaTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(cor))
            {
                MessageBox.Show("O campo Cor está vazio.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                CorTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(descricao))
            {
                MessageBox.Show("O campo Descrição está vazio.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                DescricaoTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(tamanho))
            {
                MessageBox.Show("O campo Tamanho está vazio.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                TamanhoComboBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(genero))
            {
                MessageBox.Show("O campo Gênero está vazio.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                GeneroComboBox.Focus();
                return;
            }

            // Tentar converter a string precoString para decimal
            bool isPrecoValido = decimal.TryParse(precoString, out preco);

            if (!isPrecoValido)
            {
                MessageBox.Show("Preço inválido!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                PrecoTextBox.Focus();
                return;
            }

            // Conexão com o banco de dados
            string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString)) 
            {
                try
                {
                    connection.Open();

                    // Inserir novo produto
                    string insertQuery = "INSERT INTO Produto (CodigoMarca, Cor, Descricao, Tamanho, Genero, Preco) " + "VALUES (@CodigoMarca, @Cor, @Descricao, @Tamanho, @Genero, @Preco)";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);

                        insertCommand.Parameters.AddWithValue("@CodigoMarca", codigoMarca);
                        insertCommand.Parameters.AddWithValue("@Cor", cor);
                        insertCommand.Parameters.AddWithValue("@Descricao", descricao);
                        insertCommand.Parameters.AddWithValue("@Tamanho", tamanho);
                        insertCommand.Parameters.AddWithValue("@Genero", genero);
                        insertCommand.Parameters.AddWithValue("@Preco", preco);

                        int result = insertCommand.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Produto salvo com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Falha ao salvar o produto.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        private void CodigoMarcaTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //text
        }
        private void CorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //tetx
        }
        private void TamanhoComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //text
        }

        private void GeneroComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //text
        }

        private void PrecoTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //text
        }

        private void DescricaoTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //text
        }
    }
}