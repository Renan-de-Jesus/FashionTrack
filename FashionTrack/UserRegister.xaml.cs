using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace FashionTrack
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
            toggleAdmin.IsChecked = false;
        }

        private void toggleAdmin_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void toggleAdmin_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void txtBoxUser_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string name = txtBoxName.Text;
            string username = txtBoxUser.Text;
            string password = pwdPassword.Password;
            string confirmPassword = pwdConfirmPassword.Password;
            bool isAdmin = toggleAdmin.IsChecked == true;

            string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
                try
                {
                    connection.Open();
                    string querry = "INSERT INTO Usuarios(NomeCompleto, Usuario, Senha, Adm) VALUES (@name, @username, @password, @IsAdmin)";
                    SqlCommand command = new SqlCommand(querry, connection);

                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@IsAdmin", isAdmin);

                    if (password == confirmPassword)
                    {
                        int result = command.ExecuteNonQuery();

                        if ((result > 0))
                        {

                            MessageBox.Show("Usuário cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Falha ao cadastrar o usuário.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                    }
                    else
                    {
                        MessageBox.Show("As senhas não coincidem.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Erro de SQL: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
        }
    }
}