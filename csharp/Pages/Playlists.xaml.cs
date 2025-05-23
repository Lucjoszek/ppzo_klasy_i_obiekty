using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;
using Windows.Storage;
using PlaylistHub.Services;
using PlaylistHub.Models;
using PlaylistHub.Helpers;
using System.Diagnostics;

namespace PlaylistHub.Pages
{
    /// <summary>
    /// A page that displays and manages available playlists
    /// </summary>
    public sealed partial class Playlists : Page
    {
        /// <summary>
        /// Collection of user playlists bound to the UI.
        /// </summary>
        private readonly ObservableCollection<Playlist> playlists = [.. App.CurrentUser.Playlists];

        public Playlists()
        {
            this.InitializeComponent();
            LoadPlaylists();

        }

        /// <summary>
        /// Loads playlists and updates UI visibility accordingly.
        /// </summary>
        private void LoadPlaylists()
        {
            if (playlists.Any())
            {
                NothingToDisplayText.Visibility = Visibility.Collapsed;
                PlaylistsListView.Visibility = Visibility.Visible;
            }

            PlaylistsListView.ItemsSource = playlists;
        }

        /// <summary>
        /// Handles the creation of a new playlist through a dialog.
        /// </summary>
        private async void CreatePlaylist_Click(object sender, RoutedEventArgs e)
        {
            var (title, folder) = await ShowCreatePlaylistDialogAsync();
            if (string.IsNullOrWhiteSpace(title) || folder == null)
                return;

            var tracks = await LoadTracksAsync(folder);
            TimeSpan duration = new(tracks.Sum(t => t.Duration.Ticks));
            Playlist playlist = new(title, duration, folder.Path, tracks);
            
            App.CurrentUser.AddPlaylist(playlist);

            if (UserHelper.SaveUser(App.CurrentUser))
            {
                playlists.Add(playlist);

                NothingToDisplayText.Visibility = Visibility.Collapsed;
                PlaylistsListView.Visibility = Visibility.Visible;
            }
            else
            {
                App.CurrentUser.RemovePlaylist(playlist);
            }
        }

        /// <summary>
        /// Displays a dialog to input playlist title and select a folder.
        /// </summary>
        private async Task<(string? title, StorageFolder? folder)> ShowCreatePlaylistDialogAsync()
        {
            TextBox titleBox = new() { Header = "Playlist title" };
            TextBox folderBox = new() { Header = "Playlist folder", PlaceholderText = "None", IsReadOnly = true };
            Button browseButton = new() { Content = "Browse folder" };

            StorageFolder? selectedFolder = null;

            browseButton.Click += async (_, _) =>
            {
                selectedFolder = await PickFolderAsync();
                if (selectedFolder != null)
                    folderBox.Text = selectedFolder.Path;
            };

            StackPanel panel = new() { Spacing = 12 };
            panel.Children.Add(titleBox);
            panel.Children.Add(folderBox);
            panel.Children.Add(browseButton);

            ContentDialog dialog = new()
            {
                XamlRoot = this.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "New playlist",
                PrimaryButtonText = "Create",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                Content = panel
            };

            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary
                ? (titleBox.Text.Trim(), selectedFolder)
                : (null, null);
        }

        /// <summary>
        /// Opens a folder picker and returns the selected folder.
        /// </summary>
        private static async Task<StorageFolder> PickFolderAsync()
        {
            var picker = new FolderPicker { SuggestedStartLocation = PickerLocationId.MusicLibrary };

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            return await picker.PickSingleFolderAsync();
        }

        /// <summary>
        /// Loads supported audio tracks from the specified folder.
        /// </summary>
        private static async Task<List<Track>> LoadTracksAsync(StorageFolder folder)
        {
            string[] audioExtensions = [".mp3", ".wav", ".flac", ".aac", ".m4a", ".ogg"];
            var files = await folder.GetFilesAsync();

            var audioFiles = files
                .Where(f => audioExtensions.Contains(Path.GetExtension(f.Name).ToLower()))
                .ToList();

            List<Track> tracks = [];

            foreach (var audioFile in audioFiles)
            {
                try
                {
                    var props = await audioFile.Properties.GetMusicPropertiesAsync();
                    Track track = new(
                        string.IsNullOrWhiteSpace(props.Title) ? audioFile.DisplayName : props.Title,
                        string.IsNullOrWhiteSpace(props.Artist) ? "Unknown" : props.Artist,
                        props.Duration,
                        audioFile.Path
                    );
                    tracks.Add(track);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error reading audio file: '{audioFile.Name}': {ex.Message}");
                }
            }

            return tracks;
        }

        /// <summary>
        /// Navigates to a page showing playlist details.
        /// </summary>
        private void SeePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Playlist playlist)
                Frame.Navigate(typeof(SelectedPlaylist), playlist);
        }

        /// <summary>
        /// Loads the playlist into the audio player and updates the UI and user data.
        /// </summary>
        private void RunPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Playlist playlist)
            {
                AudioPlayerService.Instance.LoadPlaylist(playlist.Tracks);
                App.MainWindow.UpdatePlayPauseButtonIcon();

                App.CurrentUser.AddToRecentlyPlayed(playlist);
                UserHelper.SaveUser(App.CurrentUser);
            }
        }

        /// <summary>
        /// Deletes a playlist and updates UI and user data accordingly.
        /// </summary>
        private void DeletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuItem && menuItem.DataContext is Playlist playlist)
            {
                App.CurrentUser.RemovePlaylist(playlist);

                if (UserHelper.SaveUser(App.CurrentUser))
                {
                    playlists.Remove(playlist);

                    if (!playlists.Any())
                    {
                        NothingToDisplayText.Visibility = Visibility.Visible;
                        PlaylistsListView.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    App.CurrentUser.AddPlaylist(playlist);
                }
            }
        }
    }
}