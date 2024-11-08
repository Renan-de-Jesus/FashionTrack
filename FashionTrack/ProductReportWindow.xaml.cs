using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using Microsoft.Data.SqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Win32;
using MigraDoc.DocumentObjectModel.Tables;
using System.Diagnostics;

namespace FashionTrack
{
    public partial class ProductReportWindow : Window
    {
        public ProductReportWindow()
        {
            InitializeComponent();
            LoadProductData();
        }

        private void LoadProductData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            p.Description, 
                            p.Price, 
                            p.BrandCode, 
                            p.Gender, 
                            b.BrandName AS Brand, 
                            c.ColorName AS Color, 
                            s.SizeDescription AS Size
                        FROM 
                            Product p
                        LEFT JOIN 
                            Brand b ON p.BrandId = b.BrandId
                        LEFT JOIN 
                            Color c ON p.ColorId = c.ColorId
                        LEFT JOIN 
                            Size s ON p.SizeId = s.SizeId";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.Fill(dataTable);
                    ProductDataGrid.ItemsSource = dataTable.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading product data: " + ex.Message);
                }
            }
        }

        private void ExportToPDFButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dataTable = ((DataView)ProductDataGrid.ItemsSource).ToTable();
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF file (*.pdf)|*.pdf",
                    FileName = ".pdf"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string fileName = saveFileDialog.FileName;
                    Document document = CreateDocument(dataTable);
                    PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer();
                    pdfRenderer.Document = document;
                    pdfRenderer.RenderDocument();
                    pdfRenderer.PdfDocument.Save(saveFileDialog.FileName);

                    MessageBox.Show("PDF exported successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Open the PDF file
                    Process.Start(new ProcessStartInfo(fileName) { UseShellExecute = true });

                    // Close the window
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting PDF: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Document CreateDocument(DataTable dataTable)
        {
            Document document = new Document();
            Section section = document.AddSection();

            // Set page size to A4
            section.PageSetup.PageFormat = PageFormat.A4;
            section.PageSetup.LeftMargin = "1,7cm";
            section.PageSetup.RightMargin = "1cm";
            section.PageSetup.TopMargin = "2cm";
            section.PageSetup.BottomMargin = "2cm";

            // Add title
            Paragraph title = section.AddParagraph("Relatório de Produtos Cadastrados");
            title.Format.Font.Size = 16;
            title.Format.Font.Bold = true;
            title.Format.SpaceAfter = "1cm";
            title.Format.Alignment = ParagraphAlignment.Center;

            // Add table
            Table table = section.AddTable();
            table.Borders.Width = 0.75;


            // Define columns
            table.AddColumn("4cm");
            table.AddColumn("2cm");
            table.AddColumn("2cm");
            table.AddColumn("2cm");
            table.AddColumn("3cm");
            table.AddColumn("3cm");
            table.AddColumn("2cm");

            // Add header row
            Row headerRow = table.AddRow();
            headerRow.Shading.Color = Colors.LightGray;
            headerRow.Cells[0].AddParagraph("Descrição");
            headerRow.Cells[1].AddParagraph("Código Marca");
            headerRow.Cells[2].AddParagraph("Genero");
            headerRow.Cells[3].AddParagraph("Marca");
            headerRow.Cells[4].AddParagraph("Cor");
            headerRow.Cells[5].AddParagraph("Tamanho");
            headerRow.Cells[6].AddParagraph("Preço");

            // Add data rows and calculate total price
            decimal totalPrice = 0;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                Row row = table.AddRow();
                row.Cells[0].AddParagraph(dataRow["Description"].ToString());
                row.Cells[1].AddParagraph(dataRow["BrandCode"].ToString());
                row.Cells[2].AddParagraph(dataRow["Size"].ToString());
                row.Cells[3].AddParagraph(dataRow["Gender"].ToString());
                row.Cells[4].AddParagraph(dataRow["Brand"].ToString());
                row.Cells[5].AddParagraph(dataRow["Color"].ToString());
                row.Cells[6].AddParagraph(dataRow["Price"].ToString());

                // Sum the prices
                if (decimal.TryParse(dataRow["Price"].ToString(), out decimal price))
                {
                    totalPrice += price;
                }
            }

            // Add total price row
            Row totalRow = table.AddRow();
            totalRow.Cells[0].MergeRight = 6;
            totalRow.Cells[0].AddParagraph("Preço Total: " + totalPrice.ToString("C"));
            totalRow.Cells[0].Format.Font.Bold = true;
            totalRow.Cells[0].Format.Alignment = ParagraphAlignment.Right;

            return document;
        }
    }
}
