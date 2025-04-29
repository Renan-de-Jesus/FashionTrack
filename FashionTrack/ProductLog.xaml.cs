using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FashionTrack
{
    public partial class ProductLog : Window
    {
        public ObservableCollection<ProductAuditEntry> AuditEntries { get; set; }

        public ProductLog()
        {
            InitializeComponent();
            AuditEntries = new ObservableCollection<ProductAuditEntry>();
            DataContext = this;
            dgLogs.ItemsSource = AuditEntries;
            LoadAuditData();
        }

        private void LoadAuditData(string filter = null)
        {
            AuditEntries.Clear(); // Limpa os dados antigos

            string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

            string query = "SELECT AuditId, ProductId, CurrentDescription, OldDesc, NewDesc, OldPrice, NewPrice, ChangeDate FROM vw_ProductAudit";

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query += " WHERE " +
                         "LOWER(CurrentDescription) LIKE @filter OR " +
                         "LOWER(OldDesc) LIKE @filter OR " +
                         "LOWER(NewDesc) LIKE @filter OR " +
                         "CAST(ProductId AS NVARCHAR) LIKE @filter OR " +
                         "CAST(AuditId AS NVARCHAR) LIKE @filter";
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        command.Parameters.AddWithValue("@filter", "%" + filter.ToLower() + "%");
                    }

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        AuditEntries.Add(new ProductAuditEntry
                        {
                            AuditId = reader.GetInt32(0),
                            ProductId = reader.GetInt32(1),
                            CurrentDescription = reader.IsDBNull(2) ? null : reader.GetString(2),
                            OldDesc = reader.IsDBNull(3) ? null : reader.GetString(3),
                            NewDesc = reader.IsDBNull(4) ? null : reader.GetString(4),
                            OldPrice = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5),
                            NewPrice = reader.IsDBNull(6) ? 0 : reader.GetDecimal(6),
                            ChangeDate = reader.GetDateTime(7)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar os logs: " + ex.Message);
            }
        }

        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchLogTextBox.Text.Trim();
            LoadAuditData(searchText); // <-- Carrega do banco de dados usando o texto de busca
        }

        private void SearchLogTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //aa
        }
    }

    public class ProductAuditEntry
    {
        public int AuditId { get; set; }
        public int ProductId { get; set; }
        public string CurrentDescription { get; set; }
        public string OldDesc { get; set; }
        public string NewDesc { get; set; }
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public DateTime ChangeDate { get; set; }
    }
}
