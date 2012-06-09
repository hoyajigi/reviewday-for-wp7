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
        List<int> book=new List<int>();
        List<int> movie=new List<int>();
        List<int> music=new List<int>();


        public class Book
        {
            public string Title { get; set; }
            public string Contents { get; set; }
            public Uri Img { get; set; }
            public string Url { get; set; }
        }

        List<Book> books = new List<Book>();

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
                        music.Add(Convert.ToInt32(obj["identifier"].ToString()));
                    }
                    if (obj["domain"].ToString() == "movie")
                    {
                        movie.Add(Convert.ToInt32(obj["identifier"].ToString()));
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
     //       while (books.Count() < 10) ;

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
                        FirstListBox.ItemsSource = books;
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
                Book abook = new Book();
                abook.Title = dict["detail"]["title"].ToString();
                abook.Contents = dict["detail"]["author"].ToString();
                abook.Img = new Uri(dict["detail"]["image_url"].ToString());
                abook.Url = dict["identifier"].ToString();
                books.Add(abook);
                if (books.Count() == 10)
                {
                    FirstListBox.ItemsSource = books;
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
            if (FirstListBox.SelectedIndex!=-1&&FirstListBox.ItemsSource==books)
            NavigationService.Navigate(new Uri("/Detail.xaml?id=" + books[FirstListBox.SelectedIndex].Url+"&domain=book", UriKind.Relative));
        }
    }
}