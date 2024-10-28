using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
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

            if (string.IsNullOrWhiteSpace(name))
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

    string checkUserQuery = "SELECT COUNT(*) FROM Usuarios WHERE Usuario = @username";
    SqlCommand checkUserCommand = new SqlCommand(checkUserQuery, connection);
    checkUserCommand.Parameters.AddWithValue("@username", username);

    int userCount = (int)checkUserCommand.ExecuteScalar();

    if (userCount > 0)
    {
        MessageBox.Show("Usuário já existe.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
    }
    else
    {
        string insertQuery = "INSERT INTO Usuarios(NomeCompleto, Usuario, Senha, Adm) VALUES (@name, @username, HASHBYTES('SHA2_256', @password), @IsAdmin)";
        SqlCommand insertCommand = new SqlCommand(insertQuery, connection);

        insertCommand.Parameters.AddWithValue("@name", name);
        insertCommand.Parameters.AddWithValue("@username", username);
        insertCommand.Parameters.AddWithValue("@Password", password);
        insertCommand.Parameters.AddWithValue("@IsAdmin", isAdmin);

        int result = insertCommand.ExecuteNonQuery();

        if (result > 0)
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