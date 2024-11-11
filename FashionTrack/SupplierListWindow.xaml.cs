using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;

namespace FashionTrack
{
    public partial class SupplierListWindow : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

        public SupplierListWindow()
        {
            InitializeComponent();
            LoadSupplier();
        }

        private void LoadSupplier()
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            s.ID_Supplier, 
                            s.CorporateName, 
                            s.CNPJ, 
                            s.Telephone, 
                            s.Representative, 
                            s.ID_City,
                            c.Description AS City,
                            c.UF
                        FROM Supplier s
                        LEFT JOIN City c ON s.ID_City = c.ID_City";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.Fill(dataTable);
                    }
                }

                if (dataTable.Rows.Count > 0)
                {
                    SupplierDataGrid.ItemsSource = dataTable.DefaultView;
                }
                else
                {
                    MessageBox.Show("Fornecedores não encontrados.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar os fornecedores: " + ex.Message);
            }
        }



        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int supplierId = Convert.ToInt32(selectedRow["ID_Supplier"]);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Supplier WHERE ID_Supplier = @ID_Supplier";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID_Supplier", supplierId);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Fornecessor excluido com sucesso.", "Successo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadSupplier();
            }
            else
            {
                MessageBox.Show("Por favor, selecione um fornecedor para poder deletar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SupplierDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) { /*something*/ }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            SupplierRegister supplierRegister = new SupplierRegister();
            supplierRegister.Closed += (s, args) => LoadSupplier();
            supplierRegister.ShowDialog();
        }

        private void SupplierDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SupplierDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int supplierId = Convert.ToInt32(selectedRow["ID_Supplier"]);

                SupplierRegister supplierRegister = new SupplierRegister(supplierId);
                supplierRegister.Closed += (s, args) => LoadSupplier();
                supplierRegister.ShowDialog();
            }
        }
    }
}