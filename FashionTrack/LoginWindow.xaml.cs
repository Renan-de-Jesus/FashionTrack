using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace FashionTrack
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            lblUser.Focus();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
                string username = lblUser.Text;
                string password = passwordText.Password;

                string connectionString = ConfigurationManager.ConnectionStrings["MinhaConexao"].ConnectionString;

                using (SqlConnection conexao = new SqlConnection(connectionString))
                {
                    try
                    {
                        conexao.Open(); // Pode lançar uma exceção
                        string query = "SELECT COUNT(1) FROM Usuarios WHERE Usuarios = @username AND Senha = @password";

                        SqlCommand comando = new SqlCommand(query, conexao);
                        comando.Parameters.AddWithValue("@username", username);
                        comando.Parameters.AddWithValue("@password", password);

                        int count = Convert.ToInt32(comando.ExecuteScalar());

                        if (count == 1)
                        {
                            MainWindow mainWindow = new MainWindow();
                            mainWindow.Show();
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Usuário ou senha incorretos. Tente novamente!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                            lblUser.Text = "";
                            passwordText.Password = "";
                            lblUser.Focus();
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show($"Erro de SQL: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }


        }

    }