/**
 * @typedef {Object} User
 * @property {string} username - Username of the user.
 * @property {Playlist[]} playlists - List of user's playlists.
 * @property {string[]} recently_played_playlists - Titles of recently played playlists.
 */

/**
 * @typedef {Object} Playlist
 * @property {string} title - Title of the playlist.
 * @property {number} duration - Total duration in seconds.
 * @property {string} folder_path - Path to the playlist folder.
 * @property {Track[]} tracks - List of tracks in the playlist.
 */

/**
 * @typedef {Object} Track
 * @property {string} title - Title of the track.
 * @property {string} artist - Artist name.
 * @property {number} duration - Duration in seconds.
 * @property {string} file_path - Path to the audio file.
 */

// /** @typedef {import('./types.js').User} User */
// /** @typedef {import('./types.js').Playlist} Playlist */
// /** @typedef {import('./types.js').Track} Track */