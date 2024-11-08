using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;

namespace FashionTrack
{
    public partial class CustomerListWindow : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

        public CustomerListWindow()
        {
            InitializeComponent();
            LoadCustomer();
        }

        private void LoadCustomer()
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            c.ID_Customer, 
                            c.Name, 
                            c.Surname,
                            c.CPF,
                            c.Cellphone,  
                            t.ID_City, 
                            t.Description AS CCity,
                            t.UF
                        FROM Customer c
                        LEFT JOIN City t ON c.ID_City = t.ID_City";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.Fill(dataTable);
                    }
                }

                if (dataTable.Rows.Count > 0)
                {
                    CustomerDataGrid.ItemsSource = dataTable.DefaultView;
                }
                else
                {
                    MessageBox.Show("Nenhum cliente encontrado.", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar os clientes: " + ex.Message);
            }
        }



        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int customerId = Convert.ToInt32(selectedRow["ID_Customer"]);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Customer WHERE ID_Customer = @ID_Customer";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID_Customer", customerId);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Cliente deletado com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadCustomer();
            }
            else
            {
                MessageBox.Show("Por favor selecione um cliente para deleter.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CustomerDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) { /*something*/ }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerRegistration customerRegister = new CustomerRegistration();
            customerRegister.Closed += (s, args) => LoadCustomer();
            customerRegister.ShowDialog();
        }

        private void CustomerDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (CustomerDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int customerId = Convert.ToInt32(selectedRow["ID_Customer"]);

                CustomerRegistration customerRegister = new CustomerRegistration(customerId);
                customerRegister.Closed += (s, args) => LoadCustomer();
                customerRegister.ShowDialog();
            }
        }
    }
}