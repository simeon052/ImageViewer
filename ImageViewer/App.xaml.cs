﻿using ImageViewer.model;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// 空のアプリケーション テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234227 を参照してください

namespace ImageViewer
{
    /// <summary>
    /// 既定の Application クラスに対してアプリケーション独自の動作を実装します。
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// 単一アプリケーション オブジェクトを初期化します。これは、実行される作成したコードの
        /// 最初の行であり、main() または WinMain() と論理的に等価です。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        bool EventRegistered;
        /// <summary>
        /// アプリケーションがエンド ユーザーによって正常に起動されたときに呼び出されます。他のエントリ ポイントは、
        /// アプリケーションが特定のファイルを開くために起動されたときなどに使用されます。
        /// </summary>
        /// <param name="e">起動の要求とプロセスの詳細を表示します。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // ウィンドウに既にコンテンツが表示されている場合は、アプリケーションの初期化を繰り返さずに、
            // ウィンドウがアクティブであることだけを確認してください
            if (rootFrame == null)
            {
                // ナビゲーション コンテキストとして動作するフレームを作成し、最初のページに移動します
                rootFrame = new Frame();
                // 既定の言語を設定します
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 以前中断したアプリケーションから状態を読み込みます

                }

                // フレームを現在のウィンドウに配置します
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // ナビゲーションの履歴スタックが復元されていない場合、最初のページに移動します。
                // このとき、必要な情報をナビゲーション パラメーターとして渡して、新しいページを
                // 作成します
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // 現在のウィンドウがアクティブであることを確認します
            Window.Current.Activate();

            if (!this.EventRegistered)
            {
                SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;
                this.EventRegistered = true;
            }
        }

        // http://gihyo.jp/dev/serial/01/win8gen-devel/0003
        void OnCommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs eventArgs)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var str = loader.GetString("PrivacyPolicyStr");
            var handler = new UICommandInvokedHandler(OnSettingsCommand);

            var policyCommand = new SettingsCommand("policy", str, handler);
            eventArgs.Request.ApplicationCommands.Add(policyCommand);
        }

        void OnSettingsCommand(IUICommand command)
        {
            var settingsCommand = (SettingsCommand)command;
            switch (settingsCommand.Id.ToString())
            {
                case "policy":
                    ShowPrivacyPolicy();
                    break;
            }
        }

        // プライバシーポリシーの表示
        async void ShowPrivacyPolicy()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/simeon052/ImageViewer/wiki/Privacy-policy"));
        }
        /// <summary>
        /// 特定のページへの移動が失敗したときに呼び出されます
        /// </summary>
        /// <param name="sender">移動に失敗したフレーム</param>
        /// <param name="e">ナビゲーション エラーの詳細</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// アプリケーションの実行が中断されたときに呼び出されます。
        /// アプリケーションが終了されるか、メモリの内容がそのままで再開されるかに
        /// かかわらず、アプリケーションの状態が保存されます。
        /// </summary>
        /// <param name="sender">中断要求の送信元。</param>
        /// <param name="e">中断要求の詳細。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            //var deferral = e.SuspendingOperation.GetDeferral();
            ////TODO: アプリケーションの状態を保存してバックグラウンドの動作があれば停止します
            //ImageFiles files = ImageFiles.GetInstance();
            //DataContractSerializer serializer = new DataContractSerializer(typeof(ImageFiles));
            //XmlWriterSettings settings = new XmlWriterSettings();
            //settings.Encoding = new System.Text.UTF8Encoding(false);
            //Windows.Storage.StorageFolder installedFolder = ApplicationData.Current.LocalFolder;

            //using (Stream st = await installedFolder.OpenStreamForWriteAsync("ImageListBackup.xml", Windows.Storage.CreationCollisionOption.ReplaceExisting))
            //using (XmlWriter xw = XmlWriter.Create(st, settings))
            //{
            //    //シリアル化し、XMLファイルに保存する
            //    serializer.WriteObject(xw, files);
            //}
            //deferral.Complete();
        }
    }
}
