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
            HttpFeedFactory fact = new HttpFeedFactory();
            fact.BeginCreateFeed(new Uri("http://rss.indiedb.com/games/seed-of-andromeda/downloads/feed/rss.xml"), (cb) =>
            {
                feed = fact.EndCreateFeed(cb);
                Dispatcher.BeginInvoke(
                new Action<NewsTab>((sender) =>
                {
                    NewsList.ItemsSource = feed.Items;
                }),
                new object[] { this }
                );
            });
        }
        IFeed feed;
    }
}