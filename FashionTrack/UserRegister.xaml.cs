using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
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
    
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtBoxName.Text;
            string username = txtBoxUser.Text;
            string password = pwdPassword.Password;
            string confirmPassword = pwdConfirmPassword.Password;
            bool isAdmin = toggleAdmin.IsChecked == true;

            if (string.IsNullOrWhiteSpace(fullName))
            {
                MessageBox.Show("O campo Nome está vazio.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                txtBoxName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("O campo Usuário está vazio.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                txtBoxUser.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("O campo Senha está vazio.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                pwdPassword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("O campo Confirme a senha está vazio.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                pwdConfirmPassword.Focus();
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("As senhas não coincidem.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
                try
                {
                    connection.Open();

                    string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                    SqlCommand checkUserCommand = new SqlCommand(checkUserQuery, connection);
                    checkUserCommand.Parameters.AddWithValue("@username", username);

                    object result = checkUserCommand.ExecuteScalar();
                    int userCount = result != null ? Convert.ToInt32(result) : 0;

                    if (userCount > 0)
                    {
                        MessageBox.Show("Usuário já existe.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        string insertQuery = "INSERT INTO Users (FullName, Username, Password, Adm) VALUES (@fullName, @username, HASHBYTES('SHA2_256', @password), @isAdmin)";
                        SqlCommand insertCommand = new SqlCommand(insertQuery, connection);

                        insertCommand.Parameters.AddWithValue("@fullName", fullName);
                        insertCommand.Parameters.AddWithValue("@username", username);

                        byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                        SqlParameter passwordParam = new SqlParameter("@password", SqlDbType.VarBinary);
                        passwordParam.Value = passwordBytes;
                        insertCommand.Parameters.Add(passwordParam);

                        insertCommand.Parameters.AddWithValue("@isAdmin", isAdmin);

                        int resultInsert = insertCommand.ExecuteNonQuery();

                        if (resultInsert > 0)
                        {
                            MessageBox.Show("Usuário cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                           
                        }
                        else
                        {
                            MessageBox.Show("Falha ao cadastrar o usuário.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
        }
    }
}
