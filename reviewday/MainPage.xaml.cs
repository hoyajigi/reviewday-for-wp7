using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

//using System.Runtime.Serialization.Json;
//using System.IO;
//using System.Collections.ObjectModel;
//using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace reviewday
{
    

    public partial class MainPage : PhoneApplicationPage
    {
        List<Book> books = new List<Book>();
        List<Movie> movies = new List<Movie>();
        public List<Music> musics = new List<Music>();

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);


            
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

            WebClient WC = new WebClient();
            WC.DownloadStringAsync(new Uri("http://me2day.net/api/get_best_contents.json?&akey=3345257cb3f6681909994ea2c0566e80&asig=MTMzOTE2NDY1MiQkYnlidWFtLnEkJDYzZTVlM2EwOWUyYmI5M2Q0OGU4ZjlmNzA4ZjUzYjMz&locale=ko-KR"));
            WC.DownloadStringCompleted += new System.Net.DownloadStringCompletedEventHandler(Completed);
        }

        void Completed(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            try
            {
                var dict = (JObject)JsonConvert.DeserializeObject(e.Result);
                foreach (var obj in dict["contents"])
                {
                    if(obj["domain"].ToString()=="music_album"){
                        //music.Add(Convert.ToInt32(obj["identifier"].ToString()));
                        WebClient WC = new WebClient();
                        WC.DownloadStringAsync(new Uri("http://me2day.net/api/get_content.json?domain=music_album&identifier=" + Convert.ToInt32(obj["identifier"].ToString()) + "&akey=3345257cb3f6681909994ea2c0566e80&asig=MTMzOTE2NDY1MiQkYnlidWFtLnEkJDYzZTVlM2EwOWUyYmI5M2Q0OGU4ZjlmNzA4ZjUzYjMz&locale=ko-KR"));
                        WC.DownloadStringCompleted += new System.Net.DownloadStringCompletedEventHandler(musicCompleted);
                    }
                    if (obj["domain"].ToString() == "movie")
                    {
                        //movie.Add(Convert.ToInt32(obj["identifier"].ToString()));
                        WebClient WC = new WebClient();
                        WC.DownloadStringAsync(new Uri("http://me2day.net/api/get_content.json?domain=movie&identifier=" + Convert.ToInt32(obj["identifier"].ToString()) + "&akey=3345257cb3f6681909994ea2c0566e80&asig=MTMzOTE2NDY1MiQkYnlidWFtLnEkJDYzZTVlM2EwOWUyYmI5M2Q0OGU4ZjlmNzA4ZjUzYjMz&locale=ko-KR"));
                        WC.DownloadStringCompleted += new System.Net.DownloadStringCompletedEventHandler(movieCompleted);
                    }
                    if (obj["domain"].ToString() == "book")
                    {
                        WebClient WC = new WebClient();
                        WC.DownloadStringAsync(new Uri("http://me2day.net/api/get_content.json?domain=book&identifier="+Convert.ToInt32(obj["identifier"].ToString())+"&akey=3345257cb3f6681909994ea2c0566e80&asig=MTMzOTE2NDY1MiQkYnlidWFtLnEkJDYzZTVlM2EwOWUyYmI5M2Q0OGU4ZjlmNzA4ZjUzYjMz&locale=ko-KR"));
                        WC.DownloadStringCompleted += new System.Net.DownloadStringCompletedEventHandler(bookCompleted);                       
                    }
                }
                

            }
            catch (Exception ex)
            {
                MessageBox.Show("No Internet Connection");
                return;
            }
        }

        void bookCompleted(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            try
            {
                var dict = (JObject)JsonConvert.DeserializeObject(e.Result);
                Book abook = new Book();
                    abook.Title = dict["detail"]["title"].ToString();
                    abook.Contents = dict["detail"]["author"].ToString();
                    abook.Img = new Uri(dict["detail"]["image_url"].ToString());
                    abook.Url = dict["identifier"].ToString();
                    books.Add(abook);
                    if (books.Count() == 10)
                    {
                        SecondListBox.ItemsSource = books;
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No Internet Connection");
                return;
            }
        }
        void movieCompleted(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            try
            {
                var dict = (JObject)JsonConvert.DeserializeObject(e.Result);
                Movie abook = new Movie();
                abook.Title = dict["detail"]["title"].ToString();
                abook.Contents = dict["detail"]["cast"].ToString();
                abook.Img = new Uri(dict["detail"]["image_url"].ToString());
                abook.Url = dict["identifier"].ToString();
                movies.Add(abook);
                if (movies.Count() == 10)
                {
                    FirstListBox.ItemsSource = movies;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No Internet Connection");
                return;
            }
        }
        void musicCompleted(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            try
            {
                var dict = (JObject)JsonConvert.DeserializeObject(e.Result);
                Music abook = new Music();
                abook.Title = dict["detail"]["title"].ToString();
                abook.Contents = dict["detail"]["artist"].ToString();
                abook.Img = new Uri(dict["detail"]["image_url"].ToString());
                abook.Url = dict["identifier"].ToString();
                musics.Add(abook);
                if (books.Count() == 10)
                {
                    ThirdListBox.ItemsSource = musics;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No Internet Connection");
                return;
            }
        }

        private void FirstListBox_Tap(object sender, GestureEventArgs e)
        {
            
        }

        private void FirstListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FirstListBox.SelectedIndex!=-1&&FirstListBox.ItemsSource==movies)
                NavigationService.Navigate(new Uri("/Detail.xaml?id=" + movies[FirstListBox.SelectedIndex].Url + "&domain=movie&title=" + movies[FirstListBox.SelectedIndex].Title + "&imgurl=" + movies[FirstListBox.SelectedIndex].Img.ToString(), UriKind.Relative));
        }

        private void SecondListBox_Tap(object sender, GestureEventArgs e)
        {

        }

        private void SecondListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Detail.xaml?id=" + books[SecondListBox.SelectedIndex].Url + "&domain=book&title=" + books[SecondListBox.SelectedIndex].Title + "&imgurl=" + books[SecondListBox.SelectedIndex].Img.ToString(), UriKind.Relative));
        }

        private void ThirdListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Detail.xaml?id=" + musics[ThirdListBox.SelectedIndex].Url + "&domain=music_album&title=" + musics[ThirdListBox.SelectedIndex].Title+"&imgurl="+musics[ThirdListBox.SelectedIndex].Img.ToString(), UriKind.Relative));
        }
    }
    public class Book
    {
        public string Title { get; set; }
        public string Contents { get; set; }
        public Uri Img { get; set; }
        public string Url { get; set; }
    }


    public class Movie
    {
        public string Title { get; set; }
        public string Contents { get; set; }
        public Uri Img { get; set; }
        public string Url { get; set; }
    }


    public class Music
    {
        public string Title { get; set; }
        public string Contents { get; set; }
        public Uri Img { get; set; }
        public string Url { get; set; }
    }

}