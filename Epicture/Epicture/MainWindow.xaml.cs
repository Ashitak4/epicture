using FlickrNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Epicture
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // init la connexion
            Flickr flickr = new Flickr("374da8b4ace3673f7e3b29cdbee4ec98");

            // la variables options va nous permettre de filtrer
            var options = new PhotoSearchOptions { Tags = "cactus", PerPage = 20, Page = 1 };
            PhotoCollection photos = flickr.PhotosSearch(options);

            foreach (Photo photo in photos)
            {
                Debug.WriteLine("Photo {0} has url {1}", photo.PhotoId, photo.LargeUrl);
            }
        }

        private void Epicture_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
