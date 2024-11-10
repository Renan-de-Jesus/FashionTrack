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
using System.Data;

namespace FashionTrack
{

    public partial class LoginWindow : Window
    {

        public static int LoggedInUserId { get; private set; }
        public LoginWindow()
        {
            InitializeComponent();
            lblUser.Focus();
        }

private void RemoveText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Usuário")
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
                textBox.Text = "Usuário";
                textBox.Opacity = 0.6;
            }
        }

        private void PasswordPlaceholder_GotFocus(object sender, RoutedEventArgs e)
        {
            passwordPlaceholder.Visibility = Visibility.Hidden;
            passwordText.Visibility = Visibility.Visible;
            passwordText.Focus();
        }

        private void PasswordText_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(passwordText.Password))
            {
                passwordText.Visibility = Visibility.Hidden;
                passwordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void PasswordText_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(passwordText.Password))
            {
                passwordPlaceholder.Visibility = Visibility.Hidden;
                passwordText.Visibility = Visibility.Visible;
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = lblUser.Text;
            string password = passwordText.Password;

            string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                try
                {
                    connection.Open();
                    string query = "SELECT ID_Users FROM Users WHERE Username = @username AND Password = HASHBYTES('SHA2_256', @password)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@username", username);

                    byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                    SqlParameter passwordParam = new SqlParameter("@password", SqlDbType.VarBinary);
                    passwordParam.Value = passwordBytes;
                    command.Parameters.Add(passwordParam);

                    var userIdResult = command.ExecuteScalar();

                    if (userIdResult != null)
                    {
                        LoggedInUserId = Convert.ToInt32(userIdResult);

                        int movementId = 0;
                        StockMovement stock = new StockMovement(movementId);
                        stock.Show();
                        //HomePage homePage = new HomePage();
                       // homePage.Show();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Incorrect username or password. Please try again!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        lblUser.Text = "Usuário";
                        passwordPlaceholder.Text = "Senha";
                        passwordText.Password = String.Empty;
                        lblUser.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao conectar ao banco de dados: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void passwordText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        public int GetUserId()
        {
            return LoggedInUserId;
        }

    }

}