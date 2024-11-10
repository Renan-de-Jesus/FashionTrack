using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;

namespace FashionTrack
{
    public partial class SellList : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

        public SellList()
        {
            InitializeComponent();
            LoadSell();
        }

        private void LoadSell()
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            s.ID_Sell, 
                            s.ID_Customer, 
                            s.Sell_Document, 
                            s.SellDate, 
                            s.TotalPrice, 
                            s.PaymentMethod,
                            c.Name AS CustomerName
                        FROM Sell s
                        LEFT JOIN Customer c ON s.ID_Customer = c.ID_Customer";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.Fill(dataTable);
                    }
                }

                if (dataTable.Rows.Count > 0)
                {
                    SellDataGrid.ItemsSource = dataTable.DefaultView;
                }
                else
                {
                    MessageBox.Show("Nenhuma venda encontrada.", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar as vendas: " + ex.Message);
            }
        }



        private void DeleteSellButton_Click(object sender, RoutedEventArgs e)
        {
            if (SellDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int sellId = Convert.ToInt32(selectedRow["ID_Sell"]);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Sell WHERE ID_Sell = @ID_Sell";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID_Sell", sellId);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Venda deletada com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadSell();
            }
            else
            {
                MessageBox.Show("Por favor selecione uma venda para deletar.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NewSellButton_Click(object sender, RoutedEventArgs e)
        {
            SellScreen sellScreen = new SellScreen();
            sellScreen.Closed += (s, args) => LoadSell();
            sellScreen.ShowDialog();
        }

        private void SellDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SellDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int sellId = Convert.ToInt32(selectedRow["ID_Sell"]);

                SellScreen sellRegister = new SellScreen(sellId);
                sellRegister.Closed += (s, args) => LoadSell();
                sellRegister.ShowDialog();
            }
        }
    }
}