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

        private void btnCadastrar_Click(object sender, RoutedEventArgs e)
        {
            if((txtBoxUser.Text != null) && (txtBoxUser.Text != null) && (txtPassword.Text != null) && (txtConfirmPassword.Text != null))
            {
                if(txtPassword.Text == txtConfirmPassword.Text)
                {
                    MessageBox.Show("Usuário cadastrado com sucesso");
                }
            }
        }
    }
}