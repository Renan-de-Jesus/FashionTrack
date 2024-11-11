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
                        SELECT SM.ID_StockMovement, SM.ID_Product, P.Description, SM.MDescription, SM.Document, 
                        SM.MovementType, SM.Operation, SM.Qty, SM.MovementDate, SM.ID_Users, U.Username
                        FROM StockMovement AS SM
                        INNER JOIN Product AS P ON SM.ID_Product = P.ID_Product
                        INNER JOIN Users AS U ON SM.ID_Users = U.ID_Users";

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
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        string deleteItemMovQuery = "DELETE FROM StockMovement WHERE ID_StockMovement = @ID_StockMovement";
                        using (SqlCommand deleteItemMovCmd = new SqlCommand(deleteItemMovQuery, conn, transaction))
                        {
                            deleteItemMovCmd.Parameters.AddWithValue("@ID_StockMovement", movementId);
                            deleteItemMovCmd.ExecuteNonQuery();
                        }
                        string deleteStockMovQuery = "DELETE FROM StockMovement WHERE ID_StockMovement = @ID_StockMovement";
                        using (SqlCommand deleteStockMovCmd = new SqlCommand(deleteStockMovQuery, conn, transaction))
                        {
                            deleteStockMovCmd.Parameters.AddWithValue("@ID_StockMovement", movementId);
                            deleteStockMovCmd.ExecuteNonQuery();
                        }
                        transaction.Commit();

                        MessageBox.Show("Movimentação deletada com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadMovement();
                    }
                    catch (SqlException sqlEx)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Erro ao deletar a movimentação do banco de dados: {sqlEx.Message}", "Erro SQL", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ocorreu um erro inesperado: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecione uma movimentação para deletar.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NewMovButton_Click(object sender, RoutedEventArgs e)
        {
            StockMovement stockMovement = new StockMovement();
            stockMovement.Closed += (s, args) => LoadMovement();
            stockMovement.ShowDialog();
        }

        private void MovementDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MovementDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int supplierId = Convert.ToInt32(selectedRow["ID_StockMovement"]);

                StockMovement movementRegister = new StockMovement();
                movementRegister.Closed += (s, args) => LoadMovement();
                movementRegister.ShowDialog();
            }
        }
    }
}