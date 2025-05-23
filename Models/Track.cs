using System;

namespace PlaylistHub.Models
{
    /// <summary>
    /// Represents a single audio track with metadata.
    /// </summary>
    /// <param name="title">The title of the track.</param>
    /// <param name="artist">The artist of the track.</param>
    /// <param name="duration">The duration of the track.</param>
    /// <param name="filePath">The full file path to the track.</param>
    public class Track(string title, string artist, TimeSpan duration, string filePath)
    {
        public string Title { get; set; } = title;
        public string Artist { get; set; } = artist;
        public TimeSpan Duration { get; set; } = duration;
        public string FilePath { get; set; } = filePath;

        /// <summary>
        /// Gets the duration formatted as a string (mm:ss or hh:mm:ss).
        /// </summary>
        public string FormattedDuration =>
        Duration.TotalHours >= 1
            ? Duration.ToString(@"hh\:mm\:ss")
            : Duration.ToString(@"mm\:ss");
    }
}
