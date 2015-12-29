using ImageViewer.model;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace ImageViewer
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const int InitialIntervalTime = 60;

        public MainPage()
        {
            this.InitializeComponent();
            this.PageSlider.Maximum = 1;
            this.PageSlider.Minimum = 1;
        }

        DispatcherTimer timer = null;
        ImageFiles files = null;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (timer == null)
            {
                timer = new DispatcherTimer();
            }


            timer.Interval = TimeSpan.FromSeconds(InitialIntervalTime);
            timer.Tick += timer_Tick;
            timer.Start();


            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (timer != null) timer.Stop();
            base.OnNavigatingFrom(e);
        }

        async void timer_Tick(object sender, object e)
        {
            await this.show_next_image();

        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            var list = await picker.PickMultipleFilesAsync();
            files = ImageFiles.GetInstance();
            files.SetStorage(list);
            this.PageSlider.Maximum = files.count;
            this.PageSlider.Minimum = 1;
        }


        private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            timer.Interval = TimeSpan.FromSeconds(1);
        }

        private void MenuFlyoutItem_Click_3(object sender, RoutedEventArgs e)
        {
            timer.Interval = TimeSpan.FromSeconds(3);

        }

        private void MenuFlyoutItem_Click_5(object sender, RoutedEventArgs e)
        {
            timer.Interval = TimeSpan.FromSeconds(5);

        }

        private void image_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            //            await this.show_next_image();
        }

        private async Task show_next_image()
        {
            if (files != null)
            {
                var file = files.GetNext();
                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    var bitmap = new BitmapImage();
                    bitmap.SetSource(stream);
                    this.image.Source = bitmap;
                }
            }

        }

        private async Task show_previous_image()
        {
            if (files != null)
            {
                var file = files.GetPrevious();
                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    var bitmap = new BitmapImage();
                    bitmap.SetSource(stream);
                    this.image.Source = bitmap;
                }
            }

        }

        private async Task show_specified_image(int page)
        {
            if (files != null)
            {
                var file = files.GetSpecified(page);
                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    var bitmap = new BitmapImage();
                    bitmap.SetSource(stream);
                    this.image.Source = bitmap;
                }
            }

        }

        private void image_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            //      
        }

        private void JumpAppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PageNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private async void Grid_ManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {
            Debug.WriteLine("Manipulation Completed : " + e.Position.X.ToString());
            Point currentpoint = e.Position;
            if (currentpoint.X - initialpoint.X >= 10)
            {
                Debug.WriteLine("Swipe Right");
                await this.show_next_image();
            }
            if (initialpoint.X - currentpoint.X >= 10)
            {
                Debug.WriteLine("Swipe Left");
                await this.show_previous_image();
            }
            //            e.Complete();
        }

        private void PageSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
        }

        private void PageSlider_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Debug.WriteLine("pointer released");
        }

        private async void Flyout_Closed(object sender, object e)
        {
            int currentPage = 0;
            int.TryParse(PageNumber.Text, out currentPage);
            Debug.WriteLine("Page jump flyout is closed. " + currentPage.ToString());
            await show_specified_image(currentPage);

        }

        private void JumpPageFlyoutOKButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OK button is pressed");
            this.PageJumpFlyout.Hide();
        }

        private void MenuFlyoutItem_Click_60(object sender, RoutedEventArgs e)
        {
            timer.Interval = TimeSpan.FromSeconds(60);
        }


        private Point initialpoint;

        private void Grid_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            Debug.WriteLine("Manipulation Delta : " + e.Position.X.ToString());
            Point currentpoint = e.Position;
            if (currentpoint.X - initialpoint.X >= 10)
            {
                Debug.WriteLine("Swipe Right");
                //await                this.show_next_image();
            }
            if (initialpoint.X - currentpoint.X >= 10)
            {
                Debug.WriteLine("Swipe Left");
                //                await this.show_previous_image();
            }
            e.Complete();
        }

        private void image_Holding(object sender, Windows.UI.Xaml.Input.HoldingRoutedEventArgs e)
        {
            // await this.show_previous_image();
        }

        private void Grid_ManipulationStarted(object sender, Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
        {
            Debug.WriteLine("Manipulation Started : " + e.Position.X.ToString());
            initialpoint = e.Position;
        }
    }
}
