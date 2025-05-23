using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using PlaylistHub.Pages;
using PlaylistHub.Services;
using Windows.Media.Playback;

namespace PlaylistHub
{
    /// <summary>
    /// Main window of the PlaylistHub application.
    /// Handles UI logic, navigation, and media control bindings.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            InitializeMediaPlayerEventHandlers();

        }

        /// <summary>
        /// Subscribes to audio player events (state, track, and position changes).
        /// </summary>
        private void InitializeMediaPlayerEventHandlers()
        {
            // Update play/pause icon when playback state changes
            AudioPlayerService.Instance.PlaybackStateChanged += (s, e) => DispatcherQueue.TryEnqueue(() => UpdatePlayPauseButtonIcon());

            // Update track info when current track changes
            AudioPlayerService.Instance.TrackChanged += (s, e) => DispatcherQueue.TryEnqueue(() => UpdateTrackInfo());

            // Update slider and time labels when playback position changes
            AudioPlayerService.Instance.GetMediaPlayer().PlaybackSession.PositionChanged += (s, e) => DispatcherQueue.TryEnqueue(() => PlaybackSession_PositionChanged(s, e));
        }

        /// <summary>
        /// Handles the back navigation logic.
        /// </summary>
        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) => TryGoBack();

        private bool TryGoBack()
        {
            if (!ContentFrame.CanGoBack)
                return false;

            // Don't go back if the nav pane is overlayed
            if (NavView.IsPaneOpen &&
                (NavView.DisplayMode == NavigationViewDisplayMode.Compact ||
                 NavView.DisplayMode == NavigationViewDisplayMode.Minimal))
                return false;

            ContentFrame.GoBack();
            return true;
        }

        /// <summary>
        /// Updates the play/pause icon based on the current playback state.
        /// </summary>
        public void UpdatePlayPauseButtonIcon()
        {
            var state = AudioPlayerService.Instance.GetPlaybackState();

            PlayPauseAudioButtonIcon.Glyph =
                (state == MediaPlaybackState.Playing)
                    ? "\uE769" // Pause icon
                    : "\uE768"; // Play icon
        }

        /// <summary>
        /// Updates displayed track information.
        /// </summary>
        public void UpdateTrackInfo()
        {
            var trackIndex = AudioPlayerService.Instance.GetCurrentTrackIndex();
            var totalTracks = AudioPlayerService.Instance.GetTotalTracks();
            var currentTrack = AudioPlayerService.Instance.GetCurrentTrack();

            if (currentTrack != null)
            {
                TrackNumberTextBlock.Text = $"{trackIndex}/{totalTracks}";
                TrackTitleTextBlock.Text = currentTrack.Title;
                TrackArtistTextBlock.Text = currentTrack.Artist;
            }
            else
            {
                TrackNumberTextBlock.Text = "0/0";
                TrackTitleTextBlock.Text = "No track";
                TrackArtistTextBlock.Text = "Unknown";
            }
        }

        /// <summary>
        /// Updates the playback progress bar and timestamps.
        /// </summary>
        private void PlaybackSession_PositionChanged(MediaPlaybackSession sender, object args)
        {
            static string FormatTime(TimeSpan time) => (time.Hours > 0) ? time.ToString(@"h\:mm\:ss") : time.ToString(@"m\:ss");

            TimeSpan position = sender.Position;
            TimeSpan duration = sender.NaturalDuration;

            ProgressSlider.Maximum = duration.TotalSeconds;
            ProgressSlider.Value = position.TotalSeconds;

            CurrentTimeTextBlock.Text = FormatTime(position);
            TotalTimeTextBlock.Text = FormatTime(duration); ;
        }

        /// <summary>
        /// Adjusts audio volume when the slider changes.
        /// </summary>
        private void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            // Convert slider value (0-100) to volume (0.0 - 1.0)
            double volume = e.NewValue / 100.0;
            AudioPlayerService.Instance.SetVolume(volume);
        }

        /// <summary>
        /// Skips to the previous track.
        /// </summary>
        private void PreviousAudio_Click(object sender, RoutedEventArgs e) => AudioPlayerService.Instance.Previous();

        /// <summary>
        /// Toggles play/pause for the current track.
        /// </summary>
        private void PlayPauseAudio_Click(object sender, RoutedEventArgs e)
        {
            AudioPlayerService.Instance.PlayPause();
            UpdatePlayPauseButtonIcon();
        }

        /// <summary>
        /// Skips to the next track.
        /// </summary>
        private void NextAudio_Click(object sender, RoutedEventArgs e) => AudioPlayerService.Instance.Next();

        /// <summary>
        /// Loads the default (Home) page on app startup.
        /// </summary>
        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            NavView.SelectedItem = NavView.MenuItems[0];
            NavView_Navigate(typeof(Home));
        }

        /// <summary>
        /// Navigates to the selected page if it's not already loaded.
        /// </summary>
        private void NavView_Navigate(Type navPageType)
        {
            Type preNavPageType = ContentFrame.CurrentSourcePageType;

            if (navPageType != null && !Type.Equals(preNavPageType, navPageType))
                ContentFrame.Navigate(navPageType);
        }

        /// <summary>
        /// Handles navigation view item selection.
        /// </summary>
        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {
                Type navPageType = Type.GetType(args.SelectedItemContainer.Tag.ToString());
                NavView_Navigate(navPageType);
            }
        }
    }
}
