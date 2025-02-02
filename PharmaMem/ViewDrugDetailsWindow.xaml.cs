﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace PharmaMem
{
    public partial class ViewDrugDetailsWindow : Window
    {
        private int drugId;
        private Database db;
        private List<BitmapImage> images;
        private int currentImageIndex;

        public ViewDrugDetailsWindow(int id)
        {
            InitializeComponent();
            drugId = id;
            db = new Database();
            LoadDrugDetails();
        }

        private void LoadDrugDetails()
        {
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Drugs WHERE Id = @Id", db.Connection);
            command.Parameters.AddWithValue("@Id", drugId);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                GenericNameText.Text = reader["GenericName"].ToString();
                BrandNameText.Text = reader["BrandName"].ToString();
                TypeText.Text = reader["Type"].ToString();
                DosageText.Text = reader["Dosage"].ToString();
                UsesText.Text = reader["Uses"].ToString();
                SideEffectsText.Text = reader["SideEffects"].ToString();
                GroupText.Text = reader["Group"].ToString();
                CategoryText.Text = reader["Category"].ToString();
                FormText.Text = reader["Form"].ToString();
                FamilyText.Text = reader["Family"].ToString();
                MechanismText.Text = reader["Mechanism"].ToString();
                MainJobText.Text = reader["MainJob"].ToString();
                LoadDrugImages();
            }
        }

        private void LoadDrugImages()
        {
            SQLiteCommand command = new SQLiteCommand("SELECT ImagePath FROM DrugImages WHERE DrugId = @DrugId", db.Connection);
            command.Parameters.AddWithValue("@DrugId", drugId);
            SQLiteDataReader reader = command.ExecuteReader();
            images = new List<BitmapImage>();

            while (reader.Read())
            {
                string imagePath = reader["ImagePath"].ToString();
                if (System.IO.File.Exists(imagePath))
                {
                    BitmapImage image = new BitmapImage(new Uri(imagePath));
                    images.Add(image);
                }
            }

            if (images.Count > 0)
            {
                currentImageIndex = 0;
                CurrentImage.Source = images[currentImageIndex];
            }
        }

        private void PreviousImage_Click(object sender, RoutedEventArgs e)
        {
            if (images.Count > 0)
            {
                currentImageIndex = (currentImageIndex - 1 + images.Count) % images.Count;
                AnimateImageTransition();
                CurrentImage.Source = images[currentImageIndex];
            }
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            if (images.Count > 0)
            {
                currentImageIndex = (currentImageIndex + 1) % images.Count;
                AnimateImageTransition();
                CurrentImage.Source = images[currentImageIndex];
            }
        }

        private void AnimateImageTransition()
        {
            Storyboard storyboard = (Storyboard)FindResource("SlideInAnimation");
            storyboard.Begin();
        }

        private void EditDrug_Click(object sender, RoutedEventArgs e)
        {
            EditDrugWindow editWindow = new EditDrugWindow(drugId);
            editWindow.ShowDialog();
            LoadDrugDetails(); // Refresh the details after editing
        }

        private void DeleteDrug_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this drug?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                SQLiteCommand command = new SQLiteCommand("DELETE FROM Drugs WHERE Id = @Id", db.Connection);
                command.Parameters.AddWithValue("@Id", drugId);
                command.ExecuteNonQuery();
                MessageBox.Show("Drug deleted successfully.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
        }
    }
}
