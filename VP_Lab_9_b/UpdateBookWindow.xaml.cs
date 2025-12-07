using System;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace VP_Lab_9_b
{
    public partial class UpdateBookWindow : Window
    {
        private string connectionString = @"Data Source=FURQANARSHAD\SQLEXPRESS;Initial Catalog=LibraryDB;Integrated Security=True;Trust Server Certificate=True";

        public UpdateBookWindow(int bookId, string title, string author, string publisher, string category, string isbn, int quantity, decimal price, string rackNo)
        {
            InitializeComponent();

            txtBookID.Text = bookId.ToString();
            txtTitle.Text = title;
            txtAuthor.Text = author;
            txtPublisher.Text = publisher;
            cmbCategory.Text = category;
            txtISBN.Text = isbn;
            txtQuantity.Text = quantity.ToString();
            txtPrice.Text = price.ToString();
            txtRackNo.Text = rackNo;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
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

            string rackNo = txtRackNo.Text.Trim();
            string category = (cmbCategory.SelectedItem as ComboBoxItem)?.Content.ToString();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"UPDATE Books 
                                     SET Quantity=@Quantity, Price=@Price, RackNo=@RackNo, Category=@Category
                                     WHERE BookID=@BookID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@RackNo", rackNo);
                    cmd.Parameters.AddWithValue("@Category", category);
                    cmd.Parameters.AddWithValue("@BookID", Convert.ToInt32(txtBookID.Text));

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Book updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating book: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
