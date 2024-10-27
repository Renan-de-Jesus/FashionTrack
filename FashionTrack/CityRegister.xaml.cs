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
                        {
                            string saveCityQuerry = "INSERT INTO Cidade(Descricao, UF) VALUES(@city, @UF)";
                            SqlCommand cityCommand = new SqlCommand(saveCityQuerry, connection);
                            cityCommand.Parameters.AddWithValue("@city", city);
                            cityCommand.Parameters.AddWithValue("@UF", UF);

                            int rowsAffected = cityCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Cidade cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
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
