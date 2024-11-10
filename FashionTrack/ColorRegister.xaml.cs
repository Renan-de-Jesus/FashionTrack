using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
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
using System.Configuration;
using System.Text.RegularExpressions;

namespace FashionTrack
{
    public partial class ColorRegister : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
        private bool isEditMode = false;
        private int currentColorId = -1;

        private void RemoveText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Digite o nome da cor")
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
                textBox.Text = "Digite o nome da cor";
                textBox.Opacity = 0.6;
            }
        }
        public ColorRegister()
        {
            InitializeComponent();
        }

        private void ColorIdTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowedForId(e.Text);
        }

        private void ColorNameTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowedForName(e.Text);
        }

        private static bool IsTextAllowedForId(string text)
        {
            Regex regex = new Regex("[^0-9]+");
            return !regex.IsMatch(text);
        }

        private static bool IsTextAllowedForName(string text)
        {
            Regex regex = new Regex("[^a-zA-Z]+");
            return !regex.IsMatch(text);
        }

        private void ColorNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string colorName = ColorNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(ColorNameTextBox.Text) || ColorNameTextBox.Text == "Digite o nome da cor")
            {
                MessageBox.Show("Campo cor não pode estar vazio");
                return;
            }

            if (IsColorNameDuplicate(colorName))
            {
                MessageBox.Show("O nome da cor já está cadastrado. Por favor, escolha outro nome.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd;

                if (isEditMode && currentColorId != -1)
                {
                    cmd = new SqlCommand("UPDATE Color SET ColorName = @ColorName WHERE ColorId = @ColorId", conn);
                    cmd.Parameters.AddWithValue("@CorId", currentColorId);
                }
                else
                {
                    cmd = new SqlCommand("INSERT INTO Color (ColorName) VALUES (@ColorName)", conn);
                }

                cmd.Parameters.AddWithValue("@ColorName", ColorNameTextBox.Text);
                cmd.ExecuteNonQuery();
            }

            MessageBoxResult result = MessageBox.Show($"Cor '{ColorNameTextBox.Text}' salva com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            if (result == MessageBoxResult.OK)
            {
                this.Close();
            }
            
        }

        private bool IsColorNameDuplicate(string colorName)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Color WHERE ColorName = @ColorName", conn);
                cmd.Parameters.AddWithValue("@ColorName", colorName);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
        private void ResetForm()
        {
            ColorNameTextBox.Clear();
            isEditMode = false;
            currentColorId = -1;
            SaveButton.IsEnabled = false;
        }   
    }
}
