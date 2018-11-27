using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Warframe_Toolbox
{
    
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            NotificationQueue = new List<Notification>();
            Alerts = new ObservableCollection<Alert>();
            Invasions = new ObservableCollection<Invasion>();
            MarketItems = new List<MarketItem>();
            SearchResults = new List<MarketItem>();

            OpenNotificationWindowCommand = new RelayCommand(OpenNotificationWindow);
            OpenNotificationPreviewWindowCommand = new RelayCommand(OpenNotificationPreviewWindow);
            CheckPriceCommand = new RelayCommand(CheckPrice);

            CheckForNewNotifications(null,EventArgs.Empty);
            DispatcherTimer newNotificationTimer = new DispatcherTimer();
            newNotificationTimer.Interval = TimeSpan.FromSeconds(30);
            newNotificationTimer.Tick += CheckForNewNotifications;
            newNotificationTimer.Start();

            DispatcherTimer displayNotificationTimer = new DispatcherTimer();
            displayNotificationTimer.Interval = TimeSpan.FromSeconds(10);
            displayNotificationTimer.Tick += DisplayNotification;
            displayNotificationTimer.Start();

            MarketItems = WarframeInfoProvider.GetMarketItems();
            var v = WarframeInfoProvider.Factions;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void DisplayNotification(object sender, EventArgs e)
        {
            bool isWindowOpen = false;

            foreach (Window w in Application.Current.Windows)
            {
                if (w is NotificationWindow || w is NotificationPreviewWindow)
                {
                    isWindowOpen = true;
                }
            }
            if (!isWindowOpen)
            {
                if (NotificationQueue.Count != 0)
                {
                    Notification n = NotificationQueue.First();
                    NotificationQueue.Remove(n);
                    NotificationWindow newWindow = new NotificationWindow();
                    (newWindow.DataContext as NotificationWindowViewModel).Notification = n.NotificationString;
                    newWindow.Show();
                }
                
            }
        }

        private void CheckForNewNotifications(object sender, EventArgs e)
        {
            var world = WarframeInfoProvider.GetWorldState();
            var newAlerts = WarframeInfoProvider.GetAlerts(world);

            var tmpAlerts = new ObservableCollection<Alert>();
            foreach (Alert alert in newAlerts)
            {
                
                if (!(Alerts.Any(x => x._id.oid == alert._id.oid)) && alert!=null)
                {
                    NotificationQueue.Add(alert);
                }
                alert.ToNotificationString();
                tmpAlerts.Add(alert);
            }
            Alerts = tmpAlerts;
            
            WarframeInfoProvider.GetInvasions(world);

            var newInvasions = WarframeInfoProvider.GetInvasions(world);

            var tmpInvasions = new ObservableCollection<Invasion>();
            foreach (Invasion invasion in newInvasions)
            {

                if (!(Invasions.Any(x => x._id.oid == invasion._id.oid)) && invasion != null)
                {
                    NotificationQueue.Add(invasion);
                }
                invasion.ToNotificationString();
                tmpInvasions.Add(invasion);
            }
            Invasions = tmpInvasions;
        }

        private List<Notification> _notificationQueue;
        public List<Notification> NotificationQueue
        {
            get { return _notificationQueue; }
            set { _notificationQueue = value; }
        }

        private ObservableCollection<Alert> _alerts;
        public ObservableCollection<Alert> Alerts
        {
            get { return _alerts; }
            set {
                _alerts = value;
                NotifyPropertyChanged("Alerts");
            }
        }

        private ObservableCollection<Invasion> _invasions;
        public ObservableCollection<Invasion> Invasions
        {
            get { return _invasions; }
            set
            {
                _invasions = value;
                NotifyPropertyChanged("Alerts");
            }
        }

        List<MarketItem> _marketItems;
        public List<MarketItem> MarketItems
        {
            get { return _marketItems; }
            set { _marketItems = value; }
        }

        List<MarketItem> _searchResults;
        public List<MarketItem> SearchResults
        {
            get { return _searchResults; }
            set { _searchResults = value; }
        }

        private ICommand _openNotificationWindowCommand;
        public ICommand OpenNotificationWindowCommand
        {
            get { return _openNotificationWindowCommand; }
            set { _openNotificationWindowCommand = value; }
        }

        private void OpenNotificationWindow(object obj)
        {
            bool isWindowOpen = false;

            foreach (Window w in Application.Current.Windows)
            {
                if (w is NotificationWindow)
                {
                    isWindowOpen = true;
                    w.Activate();
                }
            }
            if (!isWindowOpen)
            {
                NotificationWindow newwindow = new NotificationWindow();
                newwindow.Show();
            }
        }

        private ICommand _openNotificationPreviewWindowCommand;
        public ICommand OpenNotificationPreviewWindowCommand
        {
            get { return _openNotificationPreviewWindowCommand; }
            set { _openNotificationPreviewWindowCommand = value; }
        }

        private void OpenNotificationPreviewWindow(object obj)
        {
            bool isWindowOpen = false;

            foreach (Window w in Application.Current.Windows)
            {
                if (w is NotificationPreviewWindow)
                {
                    isWindowOpen = true;
                    w.Activate();
                }
            }
            if (!isWindowOpen)
            {
                NotificationPreviewWindow newwindow = new NotificationPreviewWindow();
                newwindow.Show();
            }
        }

        private ICommand _checkPriceCommand;
        public ICommand CheckPriceCommand
        {
            get { return _checkPriceCommand; }
            set { _checkPriceCommand = value; }
        }

        public async void CheckPrice(object obj)
        {
            
            await Task.Run(() => CheckPriceAsync());
        }

        private async Task CheckPriceAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            double screenLeft = SystemParameters.VirtualScreenLeft;
            double screenTop = SystemParameters.VirtualScreenTop;
            double screenWidth = SystemParameters.VirtualScreenWidth;
            double screenHeight = SystemParameters.VirtualScreenHeight;

            Bitmap bmp = new Bitmap((int)screenWidth,(int)screenHeight);
            //bmp.SetResolution(500, 500);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen((int)screenLeft, (int)screenTop, 0, 0, bmp.Size);
            }

            //Bitmap newBitmap = new Bitmap((int)(screenWidth * 2), (int)(screenHeight * 2));
            //newBitmap.SetResolution(300, 300);
            //using (Graphics g1 = Graphics.FromImage((Image)bmp))
            //{
            //    g1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //    g1.DrawImage(newBitmap, 0, 0, (int)(screenWidth * 2), (int)(screenHeight * 2));
            //}

            
            byte[] fileBytes;
            // Read the file bytes
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                fileBytes=ms.ToArray();
            }
            var fileName = "file.jpg";

            // Set up the multipart request
            var requestContent = new MultipartFormDataContent();

            // Add the demo API key ("helloworld")
            requestContent.Add(new StringContent("7ac917b09388957"), "apikey");

            // Add the file content
            var imageContent = new ByteArrayContent(fileBytes);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            requestContent.Add(imageContent, "file", fileName);

            // POST to the API
            var client = new HttpClient();
            var response = await client.PostAsync("https://api.ocr.space/parse/image", requestContent);

            var s = await response.Content.ReadAsStringAsync();

            //var s = await TestOcrAsync(@"./Images/test6.jpg");

            //var client = new RestClient("https://api.warframe.market/v1");
            //var request = new RestRequest("items", Method.GET);

            //IRestResponse response = client.Execute(request);
            ////Text = response.Content;

            //JObject content = JObject.Parse(response.Content);

            //// get JSON result objects into a list
            //IList<JToken> results = content["payload"]["items"]["en"].Children().ToList();


            //foreach (JToken result in results)
            //{
            //    // JToken.ToObject is a helper method that uses JsonSerializer internally
            //    MarketItem marketItem = result.ToObject<MarketItem>();
            //    MarketItems.Add(marketItem);
            //}

            //for(int i=0;i<5;i++)
            //{
            //    request = new RestRequest("items/" + SearchResults[i].Url_name + "/statistics", Method.GET);

            //    response = client.Execute(request);
            //    SearchResults[i].Price = JObject.Parse(response.Content)["payload"]["statistics"]["90days"][0]["avg_price"].ToString();
            //}

            //string Text = "";
            //try
            //{
            //    using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.TesseractOnly, "config"))
            //    {
            //        //engine.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");

            //        using (var img = bmp)  //var img = Pix.LoadFromFile(@"./Images/test6.tif")
            //        {

            //            using (var page = engine.Process(img))
            //            {
            //                Text = Regex.Replace(page.GetText(), @"[^a-zA-Z0-9\- ]", "", RegexOptions.Singleline);
            //                //Text = Regex.Replace(Text, @"[\\r\\n]", " "); 
            //                //Text = page.GetText();
            //            }
            //        }

            //    }
            //}
            //catch (Exception e)
            //{
            //    Trace.TraceError(e.ToString());
            //    Console.WriteLine("Unexpected Error: " + e.Message);
            //    Console.WriteLine("Details: ");
            //    Console.WriteLine(e.ToString());
            //}


            foreach (MarketItem mi in MarketItems)
            {

                if (Regex.IsMatch(s, mi.Item_name, RegexOptions.IgnoreCase))
                {
                    Console.WriteLine(mi.Item_name);
                    SearchResults.Add(mi);
                    mi.UpdatePrice();
                }
            }

            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
        }

        public async Task<string> TestOcrAsync(string filePath)
        {
            // Read the file bytes
            var fileBytes = File.ReadAllBytes(filePath);
            var fileName = Path.GetFileName(filePath);

            // Set up the multipart request
            var requestContent = new MultipartFormDataContent();

            // Add the demo API key ("helloworld")
            requestContent.Add(new StringContent("7ac917b09388957"), "apikey");

            // Add the file content
            var imageContent = new ByteArrayContent(fileBytes);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            requestContent.Add(imageContent, "file", fileName);

            // POST to the API
            var client = new HttpClient();
            var response = await client.PostAsync("https://api.ocr.space/parse/image", requestContent);

            return await response.Content.ReadAsStringAsync();
        }
    }
    
}