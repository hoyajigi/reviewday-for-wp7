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

//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

//using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace reviewday
{
    public partial class Page1 : PhoneApplicationPage
    {
        public Page1()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string id = NavigationContext.QueryString["id"];
            string domain = NavigationContext.QueryString["domain"];
            WebClient WC = new WebClient();
            WC.DownloadStringAsync(new Uri("http://me2day.net/api/get_posts_by_content.xml?domain="+domain+"&identifier=" + id+"&from_me2live=true&page=1&count=10&akey=3345257cb3f6681909994ea2c0566e80&asig=MTMzOTE2NDY1MiQkYnlidWFtLnEkJDYzZTVlM2EwOWUyYmI5M2Q0OGU4ZjlmNzA4ZjUzYjMz&locale=ko-KR"));

            WC.DownloadStringCompleted += new System.Net.DownloadStringCompletedEventHandler(Completed);

        }
        void Completed(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            string rssContent;
            rssContent = "hello";
            try
            {
                rssContent = e.Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No Internet Connection");
                return;
            }
            XDocument rssParser = XDocument.Parse(rssContent);
            //LINQ
            var rssList = from rssTree in rssParser.Descendants("post")
                          select new Post
                          {
                              name = rssTree.Element("callbackUrl").Value,

                              body = rssTree.Element("body").Value
//                              Img = new Uri(rssTree.Element("imgpath").Value, UriKind.Absolute),
//                              Url = rssTree.Element("bookid").Value

                          };


            foreach(Post rss in rssList){
                         textBlock1.Text += rss.body;

            }
        }
        public class Post
        {
            public string name { get; set; }
            public string body { get; set; }
            public Uri Img { get; set; }
            public string Url { get; set; }
        }
    }
}