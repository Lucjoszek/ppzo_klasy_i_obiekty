using System;
using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using PlaylistHub.Models;
using PlaylistHub.Helpers;

namespace PlaylistHub
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        // Reference to the main window of the app
        public static MainWindow MainWindow { get; private set; } = new();

        // Currently logged-in use
        public static User CurrentUser { get; private set; } = new(Environment.UserName);

        /// <summary>
        /// Initializes the singleton application object. This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            try
            {
                // Load user from disk using the current system username
                var loadedUser = await UserHelper.LoadUserAsync(Environment.UserName);
                if (loadedUser != null)
                    CurrentUser = loadedUser;
            }
            catch (Exception ex)
            {
                string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PlaylistHub", "startup_error.log");
                Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
                File.WriteAllText(logPath, ex.ToString());
            }

            MainWindow.AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;
            MainWindow.AppWindow.SetIcon("Assets/AppLogo.ico");
            MainWindow.Activate();
        }
    }
}
