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
    public partial class ProductRegister : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
        private bool isEditMode = false;
        private int currentProductId = -1;

        public ProductRegister()
        {
            InitializeComponent();
            LoradMarcaComboBox();
            LoradCorComboBox();
            LoradTamanhoComboBox();
        }

        private void LoradMarcaComboBox()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Marca";
                    SqlCommand comm = new SqlCommand(query, conn);
                    SqlDataReader reader = comm.ExecuteReader();

                    while (reader.Read())
                    {
                        ComboBoxItem item = new ComboBoxItem
                        {
                            Content = reader["MarcaNome"].ToString(),
                            Tag = reader["MarcaId"]
                        };
                        MarcaComboBox.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar marcas: " + ex.Message);
                }
            }
        }

        private void LoradCorComboBox()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Cor";
                    SqlCommand comm = new SqlCommand(query, conn);
                    SqlDataReader reader = comm.ExecuteReader();

                    while (reader.Read())
                    {
                        ComboBoxItem item = new ComboBoxItem
                        {
                            Content = reader["CorNome"].ToString(),
                            Tag = reader["CorId"]
                        };
                        CorComboBox.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar marcas: " + ex.Message);
                }
            }
        }

        private void LoradTamanhoComboBox()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Tamanho";
                    SqlCommand comm = new SqlCommand(query, conn);
                    SqlDataReader reader = comm.ExecuteReader();

                    while (reader.Read())
                    {
                        ComboBoxItem item = new ComboBoxItem
                        {
                            Content = reader["TamanhoDescricao"].ToString(),
                            Tag = reader["TamanhoId"]
                        };
                        TamanhoComboBox.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar marcas: " + ex.Message);
                }
            }
        }

        private void ProductIdTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !isTextAllowedForId(e.Text);
        }

        private void PriceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !isTextAllowedForPrice(e.Text);
        }

        private static bool isTextAllowedForPrice(string text)
        {
            Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            return regex.IsMatch(text);
        }

        private static bool isTextAllowedForId(string text)
        {
            Regex regex = new Regex("[^0-9]+");
            return !regex.IsMatch(text);
        }

        private static bool isTextAllowedForName(string text)
        {
            Regex regex = new Regex("[^a-zA-Z]+");
            return !regex.IsMatch(text);
        }

        private void SalvarButton_Click(object sender, RoutedEventArgs e)
        {
            string Descricao = DescricaoTextBox.Text;
            string Preco = PrecoTextBox.Text;
            string CodigoMarca = CodigoMarcaTextBox.Text;
            string Genero = GeneroComboBox.Text;
            
            ComboBoxItem selectedMarcaItem = (ComboBoxItem)MarcaComboBox.SelectedItem;
            int MarcaId = int.Parse(selectedMarcaItem.Tag.ToString());
            ComboBoxItem selectedCorItem = (ComboBoxItem)CorComboBox.SelectedItem;
            int CorId = int.Parse(selectedCorItem.Tag.ToString());
            ComboBoxItem selectedTamanhoItem = (ComboBoxItem)TamanhoComboBox.SelectedItem;
            int TamanhoId = int.Parse(selectedTamanhoItem.Tag.ToString());

            if (string.IsNullOrWhiteSpace(Descricao))
            {
                MessageBox.Show("Campo descrição não pode estar vazio");
                return;
            }

            if (string.IsNullOrWhiteSpace(Preco))
            {
                MessageBox.Show("Campo preço não pode estar vazio");
                return;
            }

            if (string.IsNullOrWhiteSpace(CodigoMarca))
            {
                MessageBox.Show("Campo código da marca não pode estar vazio");
                return;
            }

            if (selectedMarcaItem == null)
            {
                MessageBox.Show("Nenhuma marca selecionada.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                MarcaComboBox.Focus();
                return;
            }

            if (selectedCorItem == null)
            {
                MessageBox.Show("Nenhuma cor selecionada.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                CorComboBox.Focus();
                return;
            }

            if (selectedTamanhoItem == null)
            {
                MessageBox.Show("Nenhum tamanho selecionado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                TamanhoComboBox.Focus();
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd;

                try 
                {
                    if (isEditMode && currentProductId != -1)
                    {
                        cmd = new SqlCommand("UPDATE Produto SET Descricao = @Descricao, Preco = @Preco, CodigoMarca = @CodigoMarca, Genero = @Genero, MarcaId = @MarcaId, ColorId = @ColorId, TamanhoId = @TamanhoId WHERE ID_Produto = @ID_Produto", conn);
                        cmd.Parameters.AddWithValue("@ProdutoId", currentProductId);
                    }
                    else
                    {
                        cmd = new SqlCommand("INSERT INTO Produto (Descricao, Preco, CodigoMarca, Genero, MarcaId, CorId, TamanhoId) VALUES (@Descricao, @Preco, @CodigoMarca, @Genero, @MarcaId, @ColorId, @TamanhoId)", conn);
                    }

                    cmd.Parameters.AddWithValue("@Descricao", Descricao);
                    cmd.Parameters.AddWithValue("@Preco", Preco);
                    cmd.Parameters.AddWithValue("@CodigoMarca", CodigoMarca);
                    cmd.Parameters.AddWithValue("@Genero", Genero);
                    cmd.Parameters.AddWithValue("@MarcaId", MarcaId);
                    cmd.Parameters.AddWithValue("@ColorId", CorId);
                    cmd.Parameters.AddWithValue("@TamanhoId", TamanhoId);
                    cmd.ExecuteNonQuery();

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Produto salvo com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information); ;
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
       

        private void TamanhoComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //text
        }

        private void GeneroComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //text
        }

        private void CodigoMarcaTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //aa
        }

        private void OpenColorRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            ColorRegister colorRegister = new ColorRegister();
            colorRegister.Show();
        }

        private void MarcaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //aa
        }

        private void CorComboBox_TextChanged(object sender, SelectionChangedEventArgs e)
        {
            //aa
        }

        private void OpenMarcaRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            MarcaRegister marcaRegister = new MarcaRegister();
            marcaRegister.ShowDialog();
        }
        private void OpenTamanhoRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            TamanhoRegister tamanhoRegister = new TamanhoRegister();
            tamanhoRegister.ShowDialog();
        }

        private void DescricaoTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //aa
        }

        private void PrecoTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //aa
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchId = SearchIdTextBox.Text;
            string searchDescription = SearchDescriptionTextBox.Text;

            if (string.IsNullOrWhiteSpace(searchId) && string.IsNullOrWhiteSpace(searchDescription))
            {
                MessageBox.Show("Please enter an ID or Description to search.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Produto WHERE 1=1";
                    SqlCommand cmd = new SqlCommand();

                    if (!string.IsNullOrWhiteSpace(searchId))
                    {
                        query += " AND ID_Produto = @ID_Produto";
                        cmd.Parameters.AddWithValue("@ID_Produto", searchId);
                    }

                    if (!string.IsNullOrWhiteSpace(searchDescription))
                    {
                        query += " AND Descricao LIKE @Descricao";
                        cmd.Parameters.AddWithValue("@Descricao", "%" + searchDescription + "%");
                    }

                    cmd.CommandText = query;
                    cmd.Connection = conn;

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            // Preencher os campos com os dados do produto
                            DescricaoTextBox.Text = reader["Descricao"].ToString();
                            PrecoTextBox.Text = reader["Preco"].ToString();
                            CodigoMarcaTextBox.Text = reader["CodigoMarca"].ToString();
                            GeneroComboBox.Text = reader["Genero"].ToString();

                            // Selecionar os itens corretos nos ComboBoxes
                            SelectComboBoxItemByTag(MarcaComboBox, reader["MarcaId"]);
                            SelectComboBoxItemByTag(CorComboBox, reader["CorId"]);
                            SelectComboBoxItemByTag(TamanhoComboBox, reader["TamanhoId"]);

                            // Atualizar o modo de edição e o ID do produto atual
                            isEditMode = true;
                            currentProductId = int.Parse(reader["ID_Produto"].ToString());
                        }
                    }
                    else
                    {
                        MessageBox.Show("No product found with the given ID or Description.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SelectComboBoxItemByTag(ComboBox comboBox, object tag)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Tag.ToString() == tag.ToString())
                {
                    comboBox.SelectedItem = item;
                    break;
                }
            }
        }
    }
}