using ImageViewer.model;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace ImageViewer
{
    /// <summary>
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const int InitialIntervalTime = 5;
        ImageFiles imageFiles = null;
        DispatcherTimer timer = null;

        public MainPage()
        {
            this.InitializeComponent();
            this.PageSlider.Maximum = 1;
            this.PageSlider.Minimum = 1;
            imageFiles = ImageFiles.GetInstance();
            CoreWindow.GetForCurrentThread().KeyUp += Window_KeyUp;
        }

        private async void Window_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            Debug.WriteLine(args.VirtualKey.ToString());
            switch (args.VirtualKey)
            {
                case Windows.System.VirtualKey.Right:
                case Windows.System.VirtualKey.Space:
                    await this.show_next_image();
                    break;
                case Windows.System.VirtualKey.Left:
                    await this.show_previous_image();
                    break;
                case Windows.System.VirtualKey.R:
                    {
                        await imageFiles.RestoreAsync();
                        var dialog = new MessageDialog("Image list is restored.");
                        await dialog.ShowAsync();
                        await this.show_specified_image(0);
                    }
                    break;
                case Windows.System.VirtualKey.S:
                    {
                        await imageFiles.SaveAsync();
                        var dialog = new MessageDialog("Image list is saved.");
                        await dialog.ShowAsync();
                    }
                    break;
                default:
                    break;
            }
            // Restert timer after user operation
            timer.Stop();
            timer.Start();
        }


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
            if (this.imageFiles.count != 0)
            {
                await this.show_next_image();
            }
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            var list = await picker.PickMultipleFilesAsync();
            imageFiles.Clear();
            imageFiles.SetStorage(list);
            this.PageSlider.Maximum = imageFiles.count;
            this.PageSlider.Minimum = 1;
            await this.show_specified_image(0);
//            await imageFiles.SaveAsync();
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

        private async Task show_next_image()
        {
            if (imageFiles != null)
            {
                var file = imageFiles.GetNext();
                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    var bitmap = new BitmapImage();
                    bitmap.SetSource(stream);
                    this.image.Source = bitmap;
                }
                pageDisplay.Text = $"{imageFiles.current}/{imageFiles.count}";
            }

        }

        private async Task show_previous_image()
        {
            if (imageFiles != null)
            {
                var file = imageFiles.GetPrevious();
                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    var bitmap = new BitmapImage();
                    bitmap.SetSource(stream);
                    this.image.Source = bitmap;
                }
                pageDisplay.Text = $"{imageFiles.current}/{imageFiles.count}";
            }

        }

        private async Task show_specified_image(int page)
        {
            if (imageFiles != null)
            {
                var file = imageFiles.GetSpecified(page);
                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    var bitmap = new BitmapImage();
                    bitmap.SetSource(stream);
                    this.image.Source = bitmap;
                }
                pageDisplay.Text = $"{imageFiles.current}/{imageFiles.count}";
            }
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

        private void Grid_ManipulationStarted(object sender, Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
        {
            Debug.WriteLine("Manipulation Started : " + e.Position.X.ToString());
            initialpoint = e.Position;
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
            // Restert timer after user operation
            timer.Stop();
            timer.Start();
        }
    }
}
