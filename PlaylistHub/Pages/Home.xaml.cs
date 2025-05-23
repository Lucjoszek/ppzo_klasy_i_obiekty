using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PlaylistHub.Models;
using PlaylistHub.Services;

namespace PlaylistHub.Pages
{
    /// <summary>
    /// Represents the home page displaying recently played playlists.
    /// </summary>
    public sealed partial class Home : Page
    {
        /// <summary>
        /// Collection of recently played playlists bound to the UI.
        /// </summary>
        private readonly ObservableCollection<Playlist> recentlyPlayedPlaylists = [];

        public Home()
        {
            this.InitializeComponent();
            LoadRecentlyPlayedPlaylists();
        }

        /// <summary>
        /// Loads the recently played playlists and updates the UI accordingly.
        /// </summary>
        private void LoadRecentlyPlayedPlaylists()
        {
            // Retrieve the titles of recently played playlists from the current user
            var recentlyPlayedPlaylistsTitles = App.CurrentUser.RecentlyPlayedPlaylists;

            var recentlyPlayed = App.CurrentUser.Playlists
                .Where(playlist => recentlyPlayedPlaylistsTitles.Contains(playlist.Title))
                .ToList();

            // Add the filtered playlists to the observable collection
            foreach (var playlist in recentlyPlayed)
                recentlyPlayedPlaylists.Add(playlist);

            // Update UI elements based on whether there are playlists to display
            if (recentlyPlayedPlaylists.Any())
            {
                NothingToDisplayText.Visibility = Visibility.Collapsed;
                RecentlyPlayedPlaylistsListView.Visibility = Visibility.Visible;
            }

            // Bind the observable collection to the ListView
            RecentlyPlayedPlaylistsListView.ItemsSource = recentlyPlayedPlaylists;
        }

        /// <summary>
        /// Navigates to the SelectedPlaylist page to show playlist details.
        /// </summary>
        private void SeePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Playlist playlist)
                Frame.Navigate(typeof(SelectedPlaylist), playlist);
        }

        /// <summary>
        /// Loads the selected playlist into the audio player and updates UI.
        /// </summary>
        private void RunPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Playlist playlist)
            {
                AudioPlayerService.Instance.LoadPlaylist(playlist.Tracks);
                App.MainWindow.UpdatePlayPauseButtonIcon();
            }
        }
    }
}
