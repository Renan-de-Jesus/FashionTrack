using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Windows;

namespace FashionTrack
{
    public partial class RegisterWindow : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
        private string originalPasswordHash;
        private int userId;

        public RegisterWindow(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            if (userId != 0)
            {
                LoadUserData(userId);
            }
        }

        private void LoadUserData(int userId)
        {
            var user = GetUserById(userId);
            if (user != null)
            {
                txtBoxName.Text = user.FullName;
                txtBoxUser.Text = user.Username;
              /*toggleAdmin.IsChecked = user.Adm;*/
                pwdPassword.Password = user.PasswordHash;
                originalPasswordHash = user.PasswordHash;
            }
        }

        private User GetUserById(int userId)
        {
            User user = null;
            string query = "SELECT ID_Users, FullName, Username, Adm, CONVERT(VARCHAR(MAX), Password, 1) AS PasswordHash FROM Users WHERE ID_Users = @ID_Users";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID_Users", userId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                ID_Users = reader.GetInt32(0),
                                FullName = reader.GetString(1),
                                Username = reader.GetString(2),
                                /* Adm = reader.GetBoolean(3),*/
                                PasswordHash = reader.GetString(4)
                            };
                        }
                    }
                }
            }

            return user;
        }

        public class User
        {
            public int ID_Users { get; set; }
            public string FullName { get; set; }
            public string Username { get; set; }

           /* public bool Adm { get; set; }*/
            public string PasswordHash { get; set; }
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

            if (password != originalPasswordHash)
            {
                if (string.IsNullOrWhiteSpace(password) || password != confirmPassword)
                {
                    MessageBox.Show("As senhas não coincidem.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    if (userId != 0)
                    {
                        string updateQuery = "UPDATE Users SET FullName = @fullName, Username = @username, Adm = @isAdmin";
                        if (password != originalPasswordHash)
                        {
                            updateQuery += ", Password = HASHBYTES('SHA2_256', @password)";
                        }
                        updateQuery += " WHERE ID_Users = @userId";

                        SqlCommand updateCommand = new SqlCommand(updateQuery, connection);

                        updateCommand.Parameters.AddWithValue("@fullName", fullName);
                        updateCommand.Parameters.AddWithValue("@username", username);
                        if (password != originalPasswordHash)
                        {
                            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                            SqlParameter passwordParam = new SqlParameter("@password", SqlDbType.VarBinary);
                            passwordParam.Value = passwordBytes;
                            updateCommand.Parameters.Add(passwordParam);
                        }
                        updateCommand.Parameters.AddWithValue("@isAdmin", isAdmin);
                        updateCommand.Parameters.AddWithValue("@userId", userId);

                        int resultUpdate = updateCommand.ExecuteNonQuery();

                        if (resultUpdate > 0)
                        {
                            MessageBox.Show("Usuário atualizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Falha ao atualizar o usuário.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
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
}
