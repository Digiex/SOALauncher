using QDFeedParser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;

namespace SOALauncher
{
    /// <summary>
    /// Interaction logic for NewsTab.xaml
    /// </summary>
    public partial class NewsTab : UserControl
    {
        public NewsTab()
        {
            this.InitializeComponent();
            NewsList.Items.Add(new Rss20FeedItem()
            {
                Title = "Loading news...",
                Content = "Please wait while the news load..."
            });
            try
            {
                HttpFeedFactory fact = new HttpFeedFactory();
                fact.BeginCreateFeed(new Uri("http://rss.indiedb.com/games/seed-of-andromeda/downloads/feed/rss.xml"), (cb) =>
                {
                    feed = fact.EndCreateFeed(cb);
                    Dispatcher.InvokeAsync(() =>
                    {
                        NewsList.Items.Clear();
                        NewsList.ItemsSource = feed.Items;
                    });
                });
            }
            catch (Exception ex)
            {
                NewsList.Items.Clear();
                NewsList.Items.Add(new Rss20FeedItem()
                {
                    Title = "Failed to load news!",
                    Content = "Failed to load news! Error: " + ex.Message
                });
            }
        }
        IFeed feed;
    }
}