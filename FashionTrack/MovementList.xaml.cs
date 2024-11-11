using Microsoft.Data.SqlClient;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.DirectoryServices;
using System.Windows;
using Microsoft.Win32;

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
                        SELECT SM.ID_StockMovement, SM.MDescription, SM.Document, SM.MovementType,
                        SM.Operation, SM.MovementDate, U.Username, IM.ID_Product, IM.Qty_Mov, P.Description
                        FROM StockMovement AS SM
                        INNER JOIN ITEM_MOV AS IM ON SM.ID_StockMovement = IM.ID_StockMovement
                        INNER JOIN Product AS P ON IM.ID_Product = P.ID_Product
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
                        // Deletar registros na tabela ITEM_MOV
                        string deleteItemMovQuery = "DELETE FROM ITEM_MOV WHERE ID_StockMovement = @ID_StockMovement";
                        using (SqlCommand deleteItemMovCmd = new SqlCommand(deleteItemMovQuery, conn, transaction))
                        {
                            deleteItemMovCmd.Parameters.AddWithValue("@ID_StockMovement", movementId);
                            deleteItemMovCmd.ExecuteNonQuery();
                        }

                        // Deletar registros na tabela StockMovement
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

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT SM.ID_StockMovement, SM.MDescription, SM.Document, SM.MovementType, 
                        SM.Operation, SM.MovementDate, U.Username, 
                        IM.ID_Product, IM.Qty_Mov, P.Description
                        FROM StockMovement AS SM
                        INNER JOIN ITEM_MOV AS IM ON SM.ID_StockMovement = IM.ID_StockMovement
                        INNER JOIN Product AS P ON IM.ID_Product = P.ID_Product
                        INNER JOIN Users AS U ON SM.ID_Users = U.ID_Users";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.Fill(dataTable);
                    }
                }

                if (dataTable.Rows.Count > 0)
                {
                    MigraDoc.DocumentObjectModel.Document document = new MigraDoc.DocumentObjectModel.Document();
                    MigraDoc.DocumentObjectModel.Section section = document.AddSection();

                    // Set page size to A4
                    section.PageSetup.PageFormat = PageFormat.A4;
                    section.PageSetup.LeftMargin = "1.7cm";
                    section.PageSetup.RightMargin = "1cm";
                    section.PageSetup.TopMargin = "2cm";
                    section.PageSetup.BottomMargin = "2cm";

                    // Add title
                    MigraDoc.DocumentObjectModel.Paragraph title = section.AddParagraph("Relatório de Movimentações de Estoque");
                    title.Format.Font.Size = 14;
                    title.Format.Font.Bold = true;
                    title.Format.SpaceAfter = 10;
                    title.Format.Alignment = ParagraphAlignment.Center;

                    // Add table
                    MigraDoc.DocumentObjectModel.Tables.Table table = section.AddTable();
                    table.Borders.Width = 0.75;

                    // Define columns
                    MigraDoc.DocumentObjectModel.Tables.Column dateColumn = table.AddColumn(MigraDoc.DocumentObjectModel.Unit.FromCentimeter(3));
                    MigraDoc.DocumentObjectModel.Tables.Column productColumn = table.AddColumn(MigraDoc.DocumentObjectModel.Unit.FromCentimeter(5));
                    MigraDoc.DocumentObjectModel.Tables.Column documentColumn = table.AddColumn(MigraDoc.DocumentObjectModel.Unit.FromCentimeter(4));
                    MigraDoc.DocumentObjectModel.Tables.Column qtyColumn = table.AddColumn(MigraDoc.DocumentObjectModel.Unit.FromCentimeter(3));
                    MigraDoc.DocumentObjectModel.Tables.Column movementTypeColumn = table.AddColumn(MigraDoc.DocumentObjectModel.Unit.FromCentimeter(3));

                    // Add header row
                    MigraDoc.DocumentObjectModel.Tables.Row headerRow = table.AddRow();
                    headerRow.Cells[0].AddParagraph("Data").Format.Alignment = ParagraphAlignment.Center;
                    headerRow.Cells[1].AddParagraph("Produto").Format.Alignment = ParagraphAlignment.Center;
                    headerRow.Cells[2].AddParagraph("Documento").Format.Alignment = ParagraphAlignment.Center;
                    headerRow.Cells[3].AddParagraph("Quantidade").Format.Alignment = ParagraphAlignment.Center;
                    headerRow.Cells[4].AddParagraph("Tipo de Movimento").Format.Alignment = ParagraphAlignment.Center;

                    // Add data rows
                    foreach (DataRow row in dataTable.Rows)
                    {
                        MigraDoc.DocumentObjectModel.Tables.Row dataRow = table.AddRow();

                        MigraDoc.DocumentObjectModel.Paragraph dateParagraph = dataRow.Cells[0].AddParagraph(Convert.ToDateTime(row["MovementDate"]).ToString("dd/MM/yyyy"));
                        dateParagraph.Format.Alignment = ParagraphAlignment.Center;

                        MigraDoc.DocumentObjectModel.Paragraph productParagraph = dataRow.Cells[1].AddParagraph(row["Description"].ToString());
                        productParagraph.Format.Alignment = ParagraphAlignment.Center;

                        MigraDoc.DocumentObjectModel.Paragraph documentParagraph = dataRow.Cells[2].AddParagraph(row["Document"].ToString());
                        documentParagraph.Format.Alignment = ParagraphAlignment.Center;

                        MigraDoc.DocumentObjectModel.Paragraph qtyParagraph = dataRow.Cells[3].AddParagraph(row["Qty_Mov"].ToString());
                        qtyParagraph.Format.Alignment = ParagraphAlignment.Center;

                        MigraDoc.DocumentObjectModel.Paragraph movementTypeParagraph = dataRow.Cells[4].AddParagraph(row["MovementType"].ToString());
                        movementTypeParagraph.Format.Alignment = ParagraphAlignment.Center;
                    }

                    // Render PDF
                    PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);
                    pdfRenderer.Document = document;
                    pdfRenderer.RenderDocument();

                    // Save PDF
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Filter = "PDF Files|*.pdf",
                        Title = "Save Movement Report"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        string fileName = saveFileDialog.FileName;
                        pdfRenderer.PdfDocument.Save(fileName);
                        MessageBox.Show("Report generated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Open the PDF file
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(fileName) { UseShellExecute = true });
                    }
                }
                else
                {
                    MessageBox.Show("No movement data found.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}