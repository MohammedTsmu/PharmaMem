using Microsoft.Win32;
using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PharmaMem
{
    public partial class ManageImagesWindow : Window
    {
        private int drugId;
        private Database db;

        public ManageImagesWindow(int id)
        {
            InitializeComponent();
            drugId = id;
            db = new Database();
            LoadImages();
        }

        private void LoadImages()
        {
            ImagesList.Items.Clear();
            SQLiteCommand command = new SQLiteCommand("SELECT ImagePath FROM DrugImages WHERE DrugId = @DrugId", db.Connection);
            command.Parameters.AddWithValue("@DrugId", drugId);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string imagePath = reader["ImagePath"].ToString();
                ImagesList.Items.Add(imagePath);
            }
        }

        private void AddImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = openFileDialog.FileName;
                string targetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", Path.GetFileName(selectedImagePath));

                // Create the Images directory if it doesn't exist
                if (!Directory.Exists(Path.GetDirectoryName(targetPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                }

                File.Copy(selectedImagePath, targetPath, true);

                SQLiteCommand command = new SQLiteCommand("INSERT INTO DrugImages (DrugId, ImagePath) VALUES (@DrugId, @ImagePath)", db.Connection);
                command.Parameters.AddWithValue("@DrugId", drugId);
                command.Parameters.AddWithValue("@ImagePath", targetPath);
                command.ExecuteNonQuery();

                LoadImages();
            }
        }

        private void DeleteImage_Click(object sender, RoutedEventArgs e)
        {
            if (ImagesList.SelectedItem is string selectedImagePath)
            {
                SQLiteCommand command = new SQLiteCommand("DELETE FROM DrugImages WHERE DrugId = @DrugId AND ImagePath = @ImagePath", db.Connection);
                command.Parameters.AddWithValue("@DrugId", drugId);
                command.Parameters.AddWithValue("@ImagePath", selectedImagePath);
                command.ExecuteNonQuery();

                if (File.Exists(selectedImagePath))
                {
                    File.Delete(selectedImagePath);
                }

                LoadImages();
            }
        }
    }
}
