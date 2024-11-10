using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;

namespace FashionTrack
{
    public partial class MovementList : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

        public int movementId { get; private set; }

        public MovementList()
        {
            InitializeComponent();
            LoadMovement();
        }

        private void LoadMovement()
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            m.ID_StockMovement, 
                            m.ID_Product, 
                            m.MDescription, 
                            m.Document, 
                            m.MovementType, 
                            m.Operation,
                            m.Qty,
                            m.MovementDate,
                            m.ID_Users,
                            p.Description AS Product,
                            u.UserName AS Users
                        FROM StockMovement m
                        LEFT JOIN Product p ON m.ID_Product = p.ID_Product 
                        LEFT JOIN Users u ON m.ID_Users = u.ID_Users";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.Fill(dataTable);
                    }
                }

                if (dataTable.Rows.Count > 0)
                {
                    MovementDataGrid.ItemsSource = dataTable.DefaultView;
                }
                else
                {
                    MessageBox.Show("Nenhuma movimentação encontrada.", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar as movimentações de estoque: " + ex.Message);
            }
        }



        private void DeleteMovButton_Click(object sender, RoutedEventArgs e)
        {
            if (MovementDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int movementId = Convert.ToInt32(selectedRow["ID_StockMovement"]);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM StockMovement WHERE ID_StockMovement = @ID_StockMovement";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID_StockMovement", movementId);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Movimentação deletada com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadMovement();
            }
            else
            {
                MessageBox.Show("Por favor selecione uma movimentação para deletar.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NewMovButton_Click(object sender, RoutedEventArgs e)
        {
            StockMovement stockMovement = new StockMovement(movementId);
            stockMovement.Closed += (s, args) => LoadMovement();
            stockMovement.ShowDialog();
        }

        private void MovementDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MovementDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int supplierId = Convert.ToInt32(selectedRow["ID_StockMovement"]);

                StockMovement movementRegister = new StockMovement(movementId);
                movementRegister.Closed += (s, args) => LoadMovement();
                movementRegister.ShowDialog();
            }
        }
    }
}