using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;
using static FashionTrack.SellScreen;

namespace FashionTrack
{
    public partial class UserList : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

        public UserList()
        {
            InitializeComponent();
            LoadUser();
        }

        private void LoadUser()
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            u.ID_Users, 
                            u.FullName, 
                            u.Username,
                            u.Adm
                        FROM Users u";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.Fill(dataTable);
                    }
                }

                if (dataTable.Rows.Count > 0)
                {
                    UserDataGrid.ItemsSource = dataTable.DefaultView;
                }
                else
                {
                    MessageBox.Show("Nenhum usuário encontrado.", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar os usuários: " + ex.Message);
            }
        }



        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int userId = Convert.ToInt32(selectedRow["ID_Users"]);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Users WHERE ID_Users = @ID_Users";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID_Users", userId);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Usuário deletado com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadUser();
            }
            else
            {
                MessageBox.Show("Por favor selecione um usuário para deleter.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}


