using FlickrNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Net.Http;
using System.Net;
using System.Net.Sockets;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Forms;
using System.IO;

namespace Epicture.Models
{
    public class EpictureViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public OAuthAccessToken AccessToken;
        public OAuthRequestToken r;
        public Flickr s;
        private static readonly HttpClient client = new HttpClient();
        public string ApiKey;
        public string ApiSecretKey;
        public string UserToken = "";
        public string UserSecretToken = "";
        private string _AToken;
        private string _tag;
        public ObservableCollection<ImageViewModel> ImageList { get; set; }

        void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
                RaisePropertyChanged("Tag");
            }
        }


        public ICommand Connexion { get; private set; }
        public ICommand GetImages { get; private set; }
        public ICommand Search { get; private set; }
        public ICommand Upload { get; private set; }

        public EpictureViewModel()
        {
            ImageList = new ObservableCollection<ImageViewModel>();
            ApiKey = "374da8b4ace3673f7e3b29cdbee4ec98";
            ApiSecretKey = "7bed2e8547a7a7e0";
            Connexion = new GetToken(GetAccessToken);
            Search = new CheckToken(OnSearch);
            Upload = new UploadImage(OnUpLoad);
            GetImages = new MyImages(OnGetImage);
        }

        public string AToken
        {
            get
            {
                return _AToken;
            }
            set
            {
                _AToken = value;
                RaisePropertyChanged("AToken");
            }
        }

        public void GetAccessToken(object param)
        {
            try
            {
                var redirectUri = $"http://{IPAddress.Loopback}:{GetRandomUnusedPort()}/";
                s = new FlickrNet.Flickr("ca7a4ec8f19c1648a353009971c37ed0", "e0f981822b1ed09e");
                r = s.OAuthGetRequestToken(redirectUri);

                var url = s.OAuthCalculateAuthorizationUrl(r.Token, FlickrNet.AuthLevel.Delete, true);
                System.Diagnostics.Process.Start(url);

                var http = new HttpListener();
                http.Prefixes.Add(redirectUri);
                http.Start();
                Console.WriteLine("HTTP server start.");

                var context = http.GetContext();


                var response = context.Response;
                string responseString = string.Format("<html><head><meta http-equiv='refresh' content='10;url=https://www.flickr.com'></head><body>Please return to the app.</body></html>");
                var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                var responseOutput = response.OutputStream;
                responseOutput.Write(buffer, 0, buffer.Length);
                responseOutput.Close();
                http.Stop();
                Console.WriteLine("HTTP server stopped.");

                var verifier = context.Request.QueryString.Get("oauth_verifier");

                var access = s.OAuthGetAccessToken(r, verifier);
                s.GalleriesCreate("MaGallerie", "Mes favories");
                AccessToken = access;
                //Flickr flickr = new Flickr("374da8b4ace3673f7e3b29cdbee4ec98");

                // la variables options va nous permettre de filtrer

                //var flickr = new FlickrNet.Flickr("ca7a4ec8f19c1648a353009971c37ed0", "e0f981822b1ed09e");
                //flickr.OAuthAccessToken = access.Token;
                //flickr.OAuthAccessTokenSecret = access.TokenSecret;
                //var galleries = flickr.GalleriesGetList();
                UserToken = access.Token;
                UserSecretToken = access.TokenSecret;
            }
            catch (Exception e)
            {

            }
        }

        private static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        public void OnSearch(object param)
        {
            if (UserToken == "")
            {
                System.Windows.MessageBox.Show("You need to login");
                return;
            }
            var options = new PhotoSearchOptions { Tags = Tag, PerPage = 100, Page = 1 };
            PhotoCollection photos = new PhotoCollection();
            photos = s.PhotosSearch(options);

            foreach (Photo photo in photos)
            {
                ImageList.Add(new ImageViewModel() { Title = photo.Title, ImageUrl = photo.SmallUrl, WebUrl = photo.WebUrl, Date = photo.DateTaken, Id = photo.PhotoId });
            }
        }

        public void OnGetImage(object param)
        {
            if (UserToken == "")
            {
                System.Windows.MessageBox.Show("You need to login");
                return;
            }
        }

        public void OnUpLoad(object param)
        {
            if (UserToken == "")
            {
                System.Windows.MessageBox.Show("You need to login");
                return;
            }
            var FD = new System.Windows.Forms.OpenFileDialog();
            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string Image = FD.FileName;
                var response = s.UploadPicture(Image);
            }
        }
    }

    class UploadImage : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly Action<object> execute;

        public UploadImage(Action<object> execute)
        {
            this.execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }

    class GetToken : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly Action<object> execute;

        public GetToken(Action<object> execute)
        {
            this.execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }

    class CheckToken : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly Action<object> execute;

        public CheckToken(Action<object> execute)
        {
            this.execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }

    class SearchImage : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly Action<object> execute;

        public SearchImage(Action<object> execute)
        {
            this.execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }

    class MyImages : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly Action<object> execute;

        public MyImages(Action<object> execute)
        {
            this.execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
