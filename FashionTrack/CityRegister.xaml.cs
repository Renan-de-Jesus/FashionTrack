using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FashionTrack
{

    public partial class CityRegister : Window
    {
        public CityRegister()
        {
            InitializeComponent();
        }

        private void cleanTextBox()
        {
            cityTxt.Text = string.Empty;
            ufTxt.Text = string.Empty;
        }

        private void RemoveText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "UF")
            {
                textBox.Text = "";
                textBox.Opacity = 1;
            }
            else if (textBox.Text == "Cidade")
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
                textBox.Text = "Cidade";
                textBox.Opacity = 0.6;
            }
            else if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "UF";
                textBox.Opacity = 0.6;
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            string city = cityTxt.Text;
            string UF = ufTxt.Text;

            string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    try
                    {
                        string checkCityQuery = "SELECT COUNT(*) FROM Cidade WHERE Descricao = @city AND UF = @UF";
                        SqlCommand checkCityCommand = new SqlCommand(checkCityQuery, connection);
                        checkCityCommand.Parameters.AddWithValue("@city", city);
                        checkCityCommand.Parameters.AddWithValue("@UF", UF);

                        int cityExists = (int)checkCityCommand.ExecuteScalar();

                        if (cityExists > 0)
                        {
                            MessageBox.Show("Cidade já cadastrada!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        if (string.IsNullOrWhiteSpace(cityTxt.Text) || cityTxt.Text == "Cidade")
                        {
                            MessageBox.Show("O campo cidade não pode estar vazio");
                            return;
                        }
                        else
                        if (string.IsNullOrWhiteSpace(ufTxt.Text) || ufTxt.Text == "UF")
                        {
                            MessageBox.Show("O campo UF não pode estar vazio");
                            return;
                        }
                        else
                        {
                            string saveCityQuerry = "INSERT INTO Cidade(Descricao, UF) VALUES(@city, @UF)";
                            SqlCommand cityCommand = new SqlCommand(saveCityQuerry, connection);
                            cityCommand.Parameters.AddWithValue("@city", city);
                            cityCommand.Parameters.AddWithValue("@UF", UF);

                            int rowsAffected = cityCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBoxResult result = MessageBox.Show("Cidade cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                                if (result == MessageBoxResult.OK)
                                {
                                    ufTxt.Clear();
                                    cityTxt.Clear();
                                    this.Close();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Erro ao cadastrar a cidade.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                            }

                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show($"Erro de SQL: {ex}", "SQL Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
