using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PlaylistHub.Models;
using PlaylistHub.Services;
using PlaylistHub.Helpers;

namespace PlaylistHub.Pages
{
    /// <summary>
    /// A page that displays and allows modification of a selected playlist and its tracks.
    /// </summary>
    public sealed partial class SelectedPlaylist : Page
    {
        private Playlist currentPlaylist;
        private ObservableCollection<Track> viewTracks = [];

        public SelectedPlaylist()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Loads and displays the selected playlist passed via navigation parameters.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            if (args.Parameter is Playlist playlist)
            {
                currentPlaylist = playlist;

                PlaylistTitle.Text = playlist.Title;
                PlaylistPath.Text = playlist.FolderPath;

                viewTracks = [.. playlist.Tracks];

                if (viewTracks.Any())
                {
                    NothingToDisplayText.Visibility = Visibility.Collapsed;
                    TracksListView.Visibility = Visibility.Visible;
                }

                TracksListView.ItemsSource = viewTracks;
            }
        }

        /// <summary>
        /// Opens a dialog allowing the user to rename the current playlist.
        /// </summary>
        private async void RenamePlaylistTitle_Click(object sender, RoutedEventArgs e)
        {
            var titleBox = new TextBox
            {
                Header = "New title",
                PlaceholderText = PlaylistTitle.Text
            };

            var panel = new StackPanel { Spacing = 12 };
            panel.Children.Add(titleBox);

            var dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "Rename playlist",
                PrimaryButtonText = "Rename",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                Content = panel
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary && !string.IsNullOrWhiteSpace(titleBox.Text))
            {
                currentPlaylist.Title = titleBox.Text;
                PlaylistTitle.Text = titleBox.Text;

                UserHelper.SaveUser(App.CurrentUser);
            }
        }

        /// <summary>
        /// Moves a track one position up in the playlist.
        /// </summary>
        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Track track)
            {
                var index = viewTracks.IndexOf(track);

                if (index > 0)
                {
                    viewTracks.Move(index, index - 1);
                    currentPlaylist.Tracks = [.. viewTracks];

                    if (!UserHelper.SaveUser(App.CurrentUser))
                    {
                        viewTracks.Move(index - 1, index);
                        currentPlaylist.Tracks = [.. viewTracks];
                    }
                }
            }
        }

        /// <summary>
        /// Moves a track one position down in the playlist.
        /// </summary>
        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Track track)
            {
                var index = viewTracks.IndexOf(track);

                if (index < viewTracks.Count - 1)
                {
                    viewTracks.Move(index, index + 1);
                    currentPlaylist.Tracks = [.. viewTracks];

                    if (!UserHelper.SaveUser(App.CurrentUser))
                    {
                        viewTracks.Move(index + 1, index);
                        currentPlaylist.Tracks = [.. viewTracks];
                    }
                }
            }
        }

        /// <summary>
        /// Loads the current playlist into the audio player and updates recent history.
        /// </summary>
        private void RunPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                AudioPlayerService.Instance.LoadPlaylist(currentPlaylist.Tracks);
                App.MainWindow.UpdatePlayPauseButtonIcon();

                App.CurrentUser.AddToRecentlyPlayed(currentPlaylist);
                UserHelper.SaveUser(App.CurrentUser);
            }
        }
    }
}
