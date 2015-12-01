﻿using ImageViewer.model;
using System;
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
        const int InitialIntervalTime = 5;

        public MainPage()
        {
            this.InitializeComponent();
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

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            var list = await picker.PickMultipleFilesAsync();
            files = ImageFiles.GetInstance();
            files.SetStorage(list);
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
    }
}
