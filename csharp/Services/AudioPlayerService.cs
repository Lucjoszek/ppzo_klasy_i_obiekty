using System;
using System.Collections.Generic;
using Windows.Media.Core;
using Windows.Media.Playback;
using PlaylistHub.Models;

namespace PlaylistHub.Services
{
    /// <summary>
    /// An audio player service that can play a list of tracks one by one.
    /// Uses the Singleton pattern to keep one shared instance across the app.
    /// </summary>
    class AudioPlayerService
    {
        // Singleton instance
        private static AudioPlayerService? _instance;

        /// <summary>
        /// Gets the shared instance of the AudioPlayerService.
        /// If it doesn't exist yet, it will be created.
        /// </summary>
        public static AudioPlayerService Instance => _instance ??= new AudioPlayerService();

        private readonly MediaPlayer player = new();
        private List<Track> playerTracks = [];
        private int index = 0;

        public int GetCurrentTrackIndex() => index + 1;
        public int GetTotalTracks() => playerTracks.Count;
        public Track? GetCurrentTrack() => (playerTracks.Count != 0) ? playerTracks[index] : null;
        public MediaPlayer GetMediaPlayer() => player;
        public MediaPlaybackState GetPlaybackState() => player.PlaybackSession.PlaybackState;

        public event EventHandler? TrackChanged;
        public event EventHandler? PlaybackStateChanged;

        private AudioPlayerService()
        {
            // When playback state changes, notify
            player.PlaybackSession.PlaybackStateChanged += (s, e) =>
                PlaybackStateChanged?.Invoke(this, EventArgs.Empty);

            // When track finishes, move to the next one automatically
            player.MediaEnded += (s, e) => Next(); // Auto-play next track
        }

        /// <summary>
        /// Loads a list of tracks into the player and starts playing the first one.
        /// </summary>
        public void LoadPlaylist(List<Track> tracks)
        {
            if (tracks == null || tracks.Count == 0)
                return;

            playerTracks = tracks;
            index = 0;
            PlayCurrentTrack();
        }

        /// <summary>
        /// Plays the track at the current index.
        /// </summary>
        private void PlayCurrentTrack()
        {
            if (index >= 0 && index < playerTracks.Count)
            {
                var track = playerTracks[index];
                player.Source = MediaSource.CreateFromUri(new Uri(track.FilePath));
                player.Play();

                TrackChanged?.Invoke(this, EventArgs.Empty); // Notify about track change
            }
        }

        /// <summary>
        /// Sets the volume of the player (0.0 to 1.0).
        /// </summary>
        /// <param name="volume">Volume level (between 0.0 and 1.0)</param>
        public void SetVolume(double volume) => player.Volume = volume;

        /// <summary>
        /// Toggles between playing and pausing the current track.
        /// </summary>
        public void PlayPause()
        {
            if (player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
                player.Pause();
            else
                player.Play();
        }

        /// <summary>
        /// Skips to the next track in the playlist, if available.
        /// </summary>
        public void Next()
        {
            if (index < playerTracks.Count - 1)
            {
                ++index;
                PlayCurrentTrack();
            }
        }

        /// <summary>
        /// Skips to the previous track in the playlist, if available.
        /// </summary>
        public void Previous()
        {
            if (index > 0)
            {
                --index;
                PlayCurrentTrack();
            }
        }
    }
}
