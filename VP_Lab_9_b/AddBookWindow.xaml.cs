using System;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace VP_Lab_9_b
{
    public partial class AddBookWindow : Window
    {
        private string connectionString = @"Data Source=FURQANARSHAD\SQLEXPRESS;Initial Catalog=LibraryDB;Integrated Security=True; Trust Server Certificate=True";
        public AddBookWindow()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string title = txtTitle.Text.Trim();
            string author = txtAuthor.Text.Trim();
            string publisher = txtPublisher.Text.Trim();
            string category = (cmbCategory.SelectedItem as ComboBoxItem)?.Content.ToString();
            string isbn = txtISBN.Text.Trim();
            string rackNo = txtRackNo.Text.Trim();
            string dateAdded = dpDateAdded.SelectedDate.HasValue ? dpDateAdded.SelectedDate.Value.ToString("yyyy-MM-dd") : null;

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(author) || string.IsNullOrEmpty(publisher) ||
                string.IsNullOrEmpty(category) || string.IsNullOrEmpty(isbn) || string.IsNullOrEmpty(rackNo) || string.IsNullOrEmpty(dateAdded))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtQuantity.Text.Trim(), out int quantity) || quantity < 0)
            {
                MessageBox.Show("Quantity must be a valid non-negative number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text.Trim(), out decimal price) || price < 0)
            {
                MessageBox.Show("Price must be a valid non-negative number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"INSERT INTO Books (Title, Author, Publisher, Category, ISBN, Quantity, Price, RackNo, DateAdded)
                                     VALUES (@Title, @Author, @Publisher, @Category, @ISBN, @Quantity, @Price, @RackNo, @DateAdded)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Author", author);
                    cmd.Parameters.AddWithValue("@Publisher", publisher);
                    cmd.Parameters.AddWithValue("@Category", category);
                    cmd.Parameters.AddWithValue("@ISBN", isbn);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@RackNo", rackNo);
                    cmd.Parameters.AddWithValue("@DateAdded", dateAdded);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Book added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding book: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
