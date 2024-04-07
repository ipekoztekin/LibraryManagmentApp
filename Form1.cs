using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LibraryManagmentSystem
{
    public partial class Form1 : Form
    {

        private string connectionString = "Server=DESKTOP-NFUGI3G\\SQLEXPRESS;Database=LibraryManagement;User Id=sa;Password=996633;";
        public SqlCommand SqlCommand { get; private set; }
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Books";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                DataTable table = new DataTable();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    table.Load(reader);
                }
                dataGridView1.DataSource = table;
                connection.Close();
            }
        }

        private void delete_Click(object sender, EventArgs e)
        {
            // Kullanıcı tarafından seçilen satırı alın
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Seçilen satırın indeksini alın
                int rowIndex = dataGridView1.SelectedRows[0].Index;

                // Veritabanından silinecek satırın kimliğini alın
                int id = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["BookId"].Value); // IDColumn, veritabanındaki benzersiz kimliği içeren sütun adıdır

                // Veritabanından seçilen satırı kaldırın
                VeritabanındanSil(id);

                // DataGridView'i güncelleyin
                button1_Click_1(sender, e);
            }
            else
            {
                MessageBox.Show("Lütfen Silmek İstediğiniz Kitabı Seçip Tekrar Deneyiniz.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Veritabanından satırı silmek için bir yöntem
        private void VeritabanındanSil(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Books WHERE BookId = @BookId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@BookId", id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private void add_Click(object sender, EventArgs e)
        {
            // textbox1'deki metni al
            string bookName = textBox1.Text;

            // Ekleme sorgusunu oluştur
            string query = "";

            // Girişlerin boş olup olmadığını kontrol edin
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Lütfen Eksik Bilgi Girmeyiniz Verilen Bilgi Kutucuklarını Kontrol Ediniz.", "HATA !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Eğer herhangi bir alan boş ise işlemi durdur
            }
            else
            {
                // ISBN'nin veritabanında mevcut olup olmadığını kontrol et
                if (ISBNExists(textBox3.Text))
                {
                    MessageBox.Show("Girmiş Olunan Seri Numarasında Bir Kitap Zaten Mevcuttur. Bilgileri Tekrar Kontrol Ediniz.", "Mevcut", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                query = "INSERT INTO Books (Title,Author,ISBN,Available) VALUES (@BookTitle,@BookAuthor,@BookISBN,@BookAvailable)";
            }

            // SqlConnection ve SqlCommand nesnelerini oluştur
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // Parametre ekleyerek güvenli bir sorgu yapın
                command.Parameters.AddWithValue("@BookTitle", textBox1.Text);
                command.Parameters.AddWithValue("@BookAuthor", textBox2.Text);
                command.Parameters.AddWithValue("@BookISBN", textBox3.Text);
                command.Parameters.AddWithValue("@BookAvailable",comboBox1.Text);

                // Bağlantıyı açın ve sorguyu çalıştırın
                connection.Open();
                command.ExecuteNonQuery();
            }

            // Ekleme işlemi tamamlandıktan sonra DataGridView'i güncelle
            button1_Click_1(sender, e);
        }

       

        // ISBN'nin veritabanında mevcut olup olmadığını kontrol et
        private bool ISBNExists(string isbn)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Books WHERE ISBN = @ISBN";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ISBN", isbn);

                connection.Open();
                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }
    }
}
