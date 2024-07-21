using System.Windows;
using System.Windows.Media.Imaging;

namespace PharmaMem
{
    public partial class FullImageView : Window
    {
        public FullImageView(BitmapImage image)
        {
            InitializeComponent();
            FullImage.Source = image;
        }
    }
}
