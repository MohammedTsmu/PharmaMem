using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;

namespace PharmaMem
{
    public partial class ViewDrugDetailsWindow : Window
    {
        private int drugId;
        private Database db;
        private List<BitmapImage> images;
        private int currentImageIndex;
        private DispatcherTimer slideShowTimer;
        private double currentZoom = 1;

        public ViewDrugDetailsWindow(int id)
        {
            InitializeComponent();
            drugId = id;
            db = new Database();
            LoadDrugDetails();
            SetupSlideShow();
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
                PopupImage.Source = images[currentImageIndex];
            }
        }

        private void SetupSlideShow()
        {
            slideShowTimer = new DispatcherTimer();
            slideShowTimer.Interval = TimeSpan.FromSeconds(5);
            slideShowTimer.Tick += (s, e) => NextImage_Click(null, null);
        }

        private void StartSlideShow_Click(object sender, RoutedEventArgs e)
        {
            slideShowTimer.Start();
        }

        private void StopSlideShow_Click(object sender, RoutedEventArgs e)
        {
            slideShowTimer.Stop();
        }

        private void PreviousImage_Click(object sender, RoutedEventArgs e)
        {
            if (images.Count > 0)
            {
                Storyboard slideOutStoryboard = (Storyboard)FindResource("SlideOutAnimation");
                slideOutStoryboard.Completed += (s, ev) =>
                {
                    currentImageIndex = (currentImageIndex - 1 + images.Count) % images.Count;
                    CurrentImage.Source = images[currentImageIndex];
                    PopupImage.Source = images[currentImageIndex];
                    Storyboard slideInStoryboard = (Storyboard)FindResource("SlideInAnimation");
                    slideInStoryboard.Begin();
                };
                slideOutStoryboard.Begin();
            }
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            if (images.Count > 0)
            {
                Storyboard slideOutStoryboard = (Storyboard)FindResource("SlideOutAnimation");
                slideOutStoryboard.Completed += (s, ev) =>
                {
                    currentImageIndex = (currentImageIndex + 1) % images.Count;
                    CurrentImage.Source = images[currentImageIndex];
                    PopupImage.Source = images[currentImageIndex];
                    Storyboard slideInStoryboard = (Storyboard)FindResource("SlideInAnimation");
                    slideInStoryboard.Begin();
                };
                slideOutStoryboard.Begin();
            }
        }



        private void CurrentImage_MouseEnter(object sender, MouseEventArgs e)
        {
            ImagePopup.IsOpen = true;
        }

        private void CurrentImage_MouseLeave(object sender, MouseEventArgs e)
        {
            ImagePopup.IsOpen = false;
        }

        private void CurrentImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (ImagePopup.IsOpen)
            {
                var position = e.GetPosition(CurrentImage);
                double translateX = -(position.X / CurrentImage.ActualWidth) * PopupImage.ActualWidth;
                double translateY = -(position.Y / CurrentImage.ActualHeight) * PopupImage.ActualHeight;

                PopupImageTranslateTransform.X = translateX;
                PopupImageTranslateTransform.Y = translateY;
            }
        }

        private void PopupImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                currentZoom += 0.1;
            }
            else
            {
                currentZoom -= 0.1;
            }
            currentZoom = Math.Max(0.5, Math.Min(2.0, currentZoom));
            PopupImageScaleTransform.ScaleX = currentZoom;
            PopupImageScaleTransform.ScaleY = currentZoom;
        }

        private void CurrentImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                FullImageView fullImageView = new FullImageView(images[currentImageIndex]);
                fullImageView.Show();
            }
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
