﻿using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;

namespace FashionTrack
{
    public partial class ItemListWindow : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

        public ItemListWindow()
        {
            InitializeComponent();
            LoadItems();
        }

        private void LoadItems()
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            p.ID_Produto, 
                            p.Description, 
                            p.Price, 
                            p.BrandCode, 
                            p.Gender, 
                            c.ColorName, 
                            s.SizeDescription, 
                            b.BrandName
                        FROM Produto p
                        LEFT JOIN Color c ON p.ColorId = c.ColorId
                        LEFT JOIN Size s ON p.SizeId = s.SizeId
                        LEFT JOIN Brand b ON p.BrandId = b.BrandId"; // Usando LEFT JOINs

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.Fill(dataTable);
                    }
                }

                if (dataTable.Rows.Count > 0)
                {
                    ItemsDataGrid.ItemsSource = dataTable.DefaultView;
                }
                else
                {
                    MessageBox.Show("No items found.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar os itens: " + ex.Message);
            }
        }



        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int productId = Convert.ToInt32(selectedRow["ID_Produto"]); // Ajuste o nome da coluna conforme necessário

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Produto WHERE ID_Produto = @ID_Produto"; // Ajuste conforme necessário
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID_Produto", productId);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Item deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadItems(); // Reload items after deletion
            }
            else
            {
                MessageBox.Show("Please select an item to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ItemsDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e){ /*something*/ }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            ProductRegister productRegister = new ProductRegister();
            productRegister.Closed += (s, args) => LoadItems();
            productRegister.ShowDialog();
        }

        private void ItemsDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ItemsDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int productId = Convert.ToInt32(selectedRow["ID_Produto"]);

                // Abre a janela de cadastro e passe os dados do item selecionado
                ProductRegister productRegister = new ProductRegister(productId);
                productRegister.Closed += (s, args) => LoadItems();
                productRegister.ShowDialog();
            }
        }
    }
}