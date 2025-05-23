using System.Collections.Generic;

namespace PlaylistHub.Models
{
    /// <summary>
    /// Represents a user with a collection of playlists and a history of recently played playlists.
    /// </summary>
    /// <param name="username">The user's username.</param>
    public class User(string username)
    {
        public string Username { get; } = username;
        public List<Playlist> Playlists { get; set; } = [];
        public List<string> RecentlyPlayedPlaylists { get; set; } = [];

        /// <summary>
        /// Adds a playlist to the user's playlist collection.
        /// </summary>
        /// <param name="playlist">The playlist to add.</param>
        public void AddPlaylist(Playlist playlist) => Playlists.Add(playlist);

        /// <summary>
        /// Removes a playlist from the user's playlist collection.
        /// </summary>
        /// <param name="playlist">The playlist to remove.</param>
        public void RemovePlaylist(Playlist playlist) => Playlists.Remove(playlist);

        /// <summary>
        /// Adds the title of a playlist to the recently played list.
        /// Maintains a maximum of 3 recent entries.
        /// </summary>
        /// <param name="playlist">The playlist to mark as recently played.</param>
        public void AddToRecentlyPlayed(Playlist playlist)
        {
            string title = playlist.Title;

            // Remove existing occurrence to move it to the end (most recent)
            RecentlyPlayedPlaylists.Remove(title);

            RecentlyPlayedPlaylists.Add(title);

            // Keep only the last 3
            while (RecentlyPlayedPlaylists.Count > 5)
            {
                RecentlyPlayedPlaylists.RemoveAt(0);
            }
        }
    }
}
