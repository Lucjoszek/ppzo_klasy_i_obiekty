using System;
using System.Collections.Generic;
using System.Linq;

namespace PlaylistHub.Models
{
    /// <summary>
    /// Represents a playlist composed of audio tracks and associated metadata.
    /// </summary>
    /// <param name="title">The title of the playlist.</param>
    /// <param name="duration">The total duration of all tracks.</param>
    /// <param name="folderPath">The folder path where the playlist is located.</param>
    /// <param name="tracks">The list of audio tracks in the playlist.</param>
    public class Playlist(string title, TimeSpan duration, string folderPath, List<Track> tracks)
    {
        public string Title { get; set; } = title;
        public TimeSpan Duration { get; set; } = duration;
        public string FolderPath { get; set; } = folderPath;
        public List<Track> Tracks { get; set; } = tracks;

        /// <summary>
        /// Gets the duration formatted as a string (mm:ss or hh:mm:ss).
        /// </summary>
        public string FormattedDuration =>
        Duration.TotalHours >= 1
            ? Duration.ToString(@"hh\:mm\:ss")
            : Duration.ToString(@"mm\:ss");
    }
}
