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


        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}