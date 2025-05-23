using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using PlaylistHub.Models;
using System.Diagnostics;

namespace PlaylistHub.Helpers
{
    /// <summary>
    /// Provides methods for loading and saving user data and playlists.
    /// </summary>
    internal class UserHelper
    {
        /// <summary>
        /// Path to "C:\ProgramData\PlaylistHub".
        /// </summary>
        private static readonly string ProgramData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PlaylistHub");
        private static readonly JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };

        /// <summary>
        /// Loads a user from disk and refreshes their playlists from the filesystem.
        /// </summary>
        public static async Task<User?> LoadUserAsync(string username)
        {
            string userJSONFilePath = Path.Combine(ProgramData, username, "user.json");

            try
            {
                string json = File.ReadAllText(userJSONFilePath);

                User? user = JsonSerializer.Deserialize<User>(json);

                if (user == null)
                    return null;

                user.Playlists = await LoadPlaylistsAsync(user);
                return user;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading user: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Loads and refreshes playlists for a given user by scanning their folders for audio files.
        /// </summary>
        private static async Task<List<Playlist>> LoadPlaylistsAsync(User user)
        {
            List<Playlist> refreshedPlaylists = [];
            string[] audioExtensions = [".mp3", ".wav", ".flac", ".aac", ".m4a", ".ogg"];

            foreach (Playlist playlist in user.Playlists)
            {
                if (!Directory.Exists(playlist.FolderPath))
                    continue;

                try
                {
                    var playlistFolder = await StorageFolder.GetFolderFromPathAsync(playlist.FolderPath);
                    var files = await playlistFolder.GetFilesAsync();

                    var audioFiles = files
                        .Where(f => audioExtensions.Contains(Path.GetExtension(f.Name).ToLower()))
                        .ToList();

                    // Zachowaj istniejące utwory i kolejność
                    List<Track> updatedTracks = [.. playlist.Tracks];
                    TimeSpan totalDuration = playlist.Tracks.Aggregate(TimeSpan.Zero, (sum, t) => sum + t.Duration);

                    // Dodaj nowe utwory na koniec
                    foreach (var audioFile in audioFiles)
                    {
                        // Sprawdzanie, czy plik już istnieje w playliście
                        bool fileExists = updatedTracks.Any(t => t.FilePath == audioFile.Path);

                        if (fileExists)
                            continue;

                        try
                        {
                            var props = await audioFile.Properties.GetMusicPropertiesAsync();
                            var newTrack = new Track(
                                string.IsNullOrWhiteSpace(props.Title) ? audioFile.DisplayName : props.Title,
                                string.IsNullOrWhiteSpace(props.Artist) ? "Unknown" : props.Artist,
                                props.Duration,
                                audioFile.Path
                            );

                            updatedTracks.Add(newTrack);
                            totalDuration += props.Duration;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Skipped file: '{audioFile.Name}': {ex.Message}");
                        }
                    }

                    playlist.Tracks = updatedTracks;
                    playlist.Duration = totalDuration;

                    refreshedPlaylists.Add(playlist);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error loading playlist: '{playlist.Title}': {ex.Message}");
                }
            }

            return refreshedPlaylists;
        }

        /// <summary>
        /// Saves the user's data to disk as JSON.
        /// </summary>
        public static bool SaveUser(User user)
        {
            try
            {
                string userFolderPath = Path.Combine(ProgramData, user.Username);

                if (!Directory.Exists(userFolderPath))
                    Directory.CreateDirectory(userFolderPath);

                string json = JsonSerializer.Serialize(user, jsonSerializerOptions);
                File.WriteAllText(Path.Combine(userFolderPath, "user.json"), json);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving user: {ex.Message}");
            }

            return false;
        }
    }
}
