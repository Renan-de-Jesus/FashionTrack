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
            idTxt.Text = string.Empty;
            cityTxt.Text = string.Empty;
            ufTxt.Text = string.Empty;
        }



        private void searchCityBtn_Click(object sender, RoutedEventArgs e)
        {
            string ID = idTxt.Text;
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
                        string searchCityQuerry = "SELECT * FROM Cidade WHERE ID_Cidade = @ID";
                        SqlCommand cityCommand = new SqlCommand(searchCityQuerry, connection);
                        cityCommand.Parameters.AddWithValue("ID", ID);

                        SqlDataReader reader = cityCommand.ExecuteReader();

                        if (reader.Read())
                        {
                            cityTxt.Text = reader["Descricao"].ToString();
                            ufTxt.Text = reader["UF"].ToString();
                        }
                        else
                        {
                            MessageBox.Show("O ID inserido não está vinculado a nunhuma cidade", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        reader.Close();
                    }
                    catch (SqlException error)
                    {
                        MessageBox.Show($"Erro de SQL: {error}", "SQL Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }


            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            string ID = idTxt.Text;
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

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            string ID = idTxt.Text;
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
                        string cityDeleteQuerry = "DELETE FROM Cidade WHERE ID_Cidade = @ID";
                        SqlCommand deleteCommand = new SqlCommand(cityDeleteQuerry, connection);
                        deleteCommand.Parameters.AddWithValue("@ID", ID);

                        int rowsAffected = deleteCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Cidade deletada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                            cleanTextBox();
                        }
                        else
                        {
                            MessageBox.Show("Nenhuma cidade encontrada com o ID informado.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
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
