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
    public partial class ProductRegister : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
        private bool isEditMode = false;
        private int currentProductID = -1;

        public ProductRegister()
        {
            InitializeComponent();
            LoadBrandComboBox();
            LoadColorComboBox();
            LoadSizeComboBox();
        }

        private void LoadBrandComboBox()
        {
            BrandComboBox.Items.Clear(); // Clear items before reloading
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Brand";
                    SqlCommand comm = new SqlCommand(query, conn);
                    SqlDataReader reader = comm.ExecuteReader();

                    while (reader.Read())
                    {
                        ComboBoxItem item = new ComboBoxItem
                        {
                            Content = reader["BrandName"].ToString(),
                            Tag = reader["BrandId"]
                        };
                        BrandComboBox.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading brands: " + ex.Message);
                }
            }
        }

        private void LoadColorComboBox()
        {
            ColorComboBox.Items.Clear();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Color";
                    SqlCommand comm = new SqlCommand(query, conn);
                    SqlDataReader reader = comm.ExecuteReader();

                    while (reader.Read())
                    {
                        ComboBoxItem item = new ComboBoxItem
                        {
                            Content = reader["ColorName"].ToString(),
                            Tag = reader["ColorId"]
                        };
                        ColorComboBox.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading colors: " + ex.Message);
                }
            }
        }

        private void LoadSizeComboBox()
        {
            SizeComboBox.Items.Clear();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Size";
                    SqlCommand comm = new SqlCommand(query, conn);
                    SqlDataReader reader = comm.ExecuteReader();

                    while (reader.Read())
                    {
                        ComboBoxItem item = new ComboBoxItem
                        {
                            Content = reader["SizeDescription"].ToString(),
                            Tag = reader["SizeId"]
                        };
                        SizeComboBox.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading sizes: " + ex.Message);
                }
            }
        }

        private void ProductIdTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !isTextAllowedForId(e.Text);
        }

        private void PriceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !isTextAllowedForPrice(e.Text);
        }

        private static bool isTextAllowedForPrice(string text)
        {
            Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            return regex.IsMatch(text);
        }

        private static bool isTextAllowedForId(string text)
        {
            Regex regex = new Regex("[^0-9]+");
            return !regex.IsMatch(text);
        }

        private static bool isTextAllowedForName(string text)
        {
            Regex regex = new Regex("[^a-zA-Z]+");
            return !regex.IsMatch(text);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string description = DescriptionTextBox.Text;
            string price = PriceTextBox.Text;
            string brandCode = BrandCodeTextBox.Text;
            string gender = GenderComboBox.Text;

            ComboBoxItem selectedBrandItem = (ComboBoxItem)BrandComboBox.SelectedItem;
            int brandId = int.Parse(selectedBrandItem.Tag.ToString());
            ComboBoxItem selectedColorItem = (ComboBoxItem)ColorComboBox.SelectedItem;
            int colorId = int.Parse(selectedColorItem.Tag.ToString());
            ComboBoxItem selectedSizeItem = (ComboBoxItem)SizeComboBox.SelectedItem;
            int sizeId = int.Parse(selectedSizeItem.Tag.ToString());

            if (string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Description field cannot be empty.");
                return;
            }

            if (string.IsNullOrWhiteSpace(price))
            {
                MessageBox.Show("Price field cannot be empty.");
                return;
            }

            if (string.IsNullOrWhiteSpace(brandCode))
            {
                MessageBox.Show("Brand code field cannot be empty.");
                return;
            }

            if (selectedBrandItem == null)
            {
                MessageBox.Show("No brand selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                BrandComboBox.Focus();
                return;
            }

            if (selectedColorItem == null)
            {
                MessageBox.Show("No color selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ColorComboBox.Focus();
                return;
            }

            if (selectedSizeItem == null)
            {
                MessageBox.Show("No size selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                SizeComboBox.Focus();
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd;

                try
                {
                    if (isEditMode && currentProductID != -1)
                    {
                        cmd = new SqlCommand("UPDATE Product SET Description = @Description, Price = @Price, BrandCode = @BrandCode, Gender = @Gender, BrandId = @BrandId, ColorId = @ColorId, SizeId = @SizeId WHERE ID_Product = @ID_Product", conn);
                        cmd.Parameters.AddWithValue("@ID_Product", currentProductID);
                    }
                    else
                    {
                        cmd = new SqlCommand("INSERT INTO Product (Description, Price, BrandCode, Gender, BrandId, ColorId, SizeId) OUTPUT INSERTED.ID_Product VALUES (@Description, @Price, @BrandCode, @Gender, @BrandId, @ColorId, @SizeId)", conn);
                    }

                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@BrandCode", brandCode);
                    cmd.Parameters.AddWithValue("@Gender", gender);
                    cmd.Parameters.AddWithValue("@BrandId", brandId);
                    cmd.Parameters.AddWithValue("@ColorId", colorId);
                    cmd.Parameters.AddWithValue("@SizeId", sizeId);

                    if (isEditMode && currentProductID != -1)
                    {
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        currentProductID = (int)cmd.ExecuteScalar();
                        SearchIDTextBox.Text = currentProductID.ToString();
                    }

                    MessageBox.Show("Product saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //text
        }

        private void GenderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //text
        }

        private void BrandCodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //aa
        }

        private void OpenColorRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            ColorRegister colorRegister = new ColorRegister();
            colorRegister.Show();
            LoadColorComboBox();
        }

        private void BrandComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //aa
        }

        private void ColorComboBox_TextChanged(object sender, SelectionChangedEventArgs e)
        {
            //aa
        }

        private void OpenBrandRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            BrandRegister brandRegister = new BrandRegister();
            brandRegister.ShowDialog();
            LoadBrandComboBox();
        }

        private void OpenSizeRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            SizeRegister sizeRegister = new SizeRegister();
            sizeRegister.ShowDialog();
            LoadSizeComboBox();
        }

        private void DescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //aa
        }

        private void PriceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //aa
        }


        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchId = SearchIDTextBox.Text;
            string searchDescription = SearchTextBox.Text;

            if (string.IsNullOrWhiteSpace(searchId) && string.IsNullOrWhiteSpace(searchDescription))
            {
                MessageBox.Show("Please enter an ID or Description to search.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    string query = "SELECT * FROM Product WHERE 1=1"; // Initialize the query
                    if (int.TryParse(SearchIDTextBox.Text, out int productId))
                    {
                        cmd = new SqlCommand("SELECT * FROM Product WHERE ID_Product = @ID_Product", conn);
                        cmd.Parameters.AddWithValue("@ID_Product", productId);
                    }
                    else
                    {
                        cmd = new SqlCommand("SELECT * FROM Product WHERE Description LIKE @Description", conn);
                        cmd.Parameters.AddWithValue("@Description", "%" + searchDescription + "%");
                    }

                    if (!string.IsNullOrWhiteSpace(searchId))
                    {
                        query += " AND ID_Product = @ID_Product";
                        cmd.Parameters.AddWithValue("@ID_Product", searchId);
                    }

                    if (!string.IsNullOrWhiteSpace(searchDescription))
                    {
                        query += " AND Description LIKE @Description";
                        cmd.Parameters.AddWithValue("@Description", "%" + searchDescription + "%");
                    }

                    cmd.CommandText = query;
                    cmd.Connection = conn;

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            // Fill the fields with product data
                            DescriptionTextBox.Text = reader["Description"].ToString();
                            PriceTextBox.Text = reader["Price"].ToString();
                            BrandCodeTextBox.Text = reader["BrandCode"].ToString();
                            GenderComboBox.Text = reader["Gender"].ToString();

                            // Select the correct items in the ComboBoxes
                            SelectComboBoxItemByTag(BrandComboBox, reader["BrandId"]);
                            SelectComboBoxItemByTag(ColorComboBox, reader["ColorId"]);
                            SelectComboBoxItemByTag(SizeComboBox, reader["SizeId"]);

                            // Update edit mode and current product ID
                            isEditMode = true;
                            currentProductID = int.Parse(reader["ID_Product"].ToString());
                        }
                        reader.Close();
                    }
                    else
                    {
                        MessageBox.Show("No product found with the given ID or Description.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SelectComboBoxItemByTag(ComboBox comboBox, object tag)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Tag.ToString() == tag.ToString())
                {
                    comboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void SearchIDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //aa
        }

        private void SearchIDTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            return text.All(char.IsDigit);
        }

        private void SearchIDButton_Click(object sender, RoutedEventArgs e)
        {
            //aa
        }

    }
}
