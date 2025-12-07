using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VP_Lab_9_b
{
    public partial class DashboardWindow : Window
    {
        private string connectionString = @"Data Source=FURQANARSHAD\SQLEXPRESS;Initial Catalog=LibraryDB;Integrated Security=True; Trust Server Certificate=True";

        public DashboardWindow()
        {
            InitializeComponent();
            LoadBooks();
        }

        private void LoadBooks(string searchTerm = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM Books";
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        query += " WHERE Title LIKE @search OR Author LIKE @search OR ISBN LIKE @search";
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    if (!string.IsNullOrEmpty(searchTerm))
                        adapter.SelectCommand.Parameters.AddWithValue("@search", "%" + searchTerm + "%");

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgBooks.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading books: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            AddBookWindow addBookWindow = new AddBookWindow();
            addBookWindow.ShowDialog(); 

            LoadBooks();
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadBooks(txtSearch.Text.Trim());
        }
        private void ViewBooks_Click(object sender, RoutedEventArgs e)
        {
            LoadBooks();
        }
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            LoadBooks();
        }
        private void UpdateBook_Click(object sender, RoutedEventArgs e)
        {
            if (dgBooks.SelectedItem == null)
            {
                MessageBox.Show("Please select a book to update.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView row = dgBooks.SelectedItem as DataRowView;

            int bookId = Convert.ToInt32(row["BookID"]);
            string title = row["Title"].ToString();
            string author = row["Author"].ToString();
            string publisher = row["Publisher"].ToString();
            string category = row["Category"].ToString();
            string isbn = row["ISBN"].ToString();
            int quantity = Convert.ToInt32(row["Quantity"]);
            decimal price = Convert.ToDecimal(row["Price"]);
            string rackNo = row["RackNo"].ToString();

            UpdateBookWindow updateWindow = new UpdateBookWindow(bookId, title, author, publisher, category, isbn, quantity, price, rackNo);
            updateWindow.ShowDialog();

            LoadBooks();
        }

        private void DeleteBook_Click(object sender, RoutedEventArgs e)
        {
            if (dgBooks.SelectedItem == null)
            {
                MessageBox.Show("Please select a book to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView row = dgBooks.SelectedItem as DataRowView;
            int bookId = Convert.ToInt32(row["BookID"]);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Books WHERE BookID=@BookID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@BookID", bookId);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Book deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting book: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
