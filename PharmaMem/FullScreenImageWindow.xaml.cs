using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PharmaMem
{
    public partial class FullScreenImageWindow : Window
    {
        private List<BitmapImage> images;
        private int currentImageIndex;

        public FullScreenImageWindow(List<BitmapImage> images, int startIndex)
        {
            InitializeComponent();
            this.images = images;
            this.currentImageIndex = startIndex;
            FullScreenImage.Source = images[currentImageIndex];
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            else if (e.Key == Key.Right)
            {
                currentImageIndex = (currentImageIndex + 1) % images.Count;
                FullScreenImage.Source = images[currentImageIndex];
            }
            else if (e.Key == Key.Left)
            {
                currentImageIndex = (currentImageIndex - 1 + images.Count) % images.Count;
                FullScreenImage.Source = images[currentImageIndex];
            }
        }
    }
}
