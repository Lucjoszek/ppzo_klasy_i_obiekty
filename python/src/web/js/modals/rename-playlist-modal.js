import { renamePlaylist } from "../services/api.js"
import { refresh } from "../utils/refresh-util.js"
import { seePlaylist } from "../components/see-playlist.js"
import { appState } from "../state.js"

/**
 * Enables or disables the rename button based on whether the input value is non-empty
 * and different from the current playlist title.
 *
 * @param {string} playlistTitle - The current title of the playlist to compare against.
 */
export const toggleRenameButton = (playlistTitle) => {
    const playlistTitleInput = document.querySelector('#renamePlaylistTitle')
    const renamePlaylistBtn = document.querySelector('#renamePlaylistButton')

    playlistTitleInput.value != "" && playlistTitleInput.value != playlistTitle
        ? renamePlaylistBtn.disabled = false
        : renamePlaylistBtn.disabled = true
}

/**
 * Renames the given playlist to the new title provided in the input field.
 * Calls backend to perform the rename operation and refreshes relevant pages on success.
 * Closes the rename modal regardless of success.
 * 
 * @param {Playlist} playlist - The playlist object to rename.
 * @returns {Promise<void>}
 */
export const renameButton = async (playlist) => {
    const playlistTitleInput = document.querySelector('#renamePlaylistTitle')
    const renamePlaylistModal = document.querySelector('#renamePlaylist_modal')

    const newTitle = playlistTitleInput.value.trim()

    console.log(`Renaming playlist '${playlist.title}' to '${newTitle}'.`)

    renamePlaylistModal.close()

    const success = await renamePlaylist(playlist, newTitle)

    if (!success) {
        console.error('Failed to rename playlist!')
        return
    }

    console.log('Playlist has been renamed.')

    await refresh(['home', 'playlists'])
    seePlaylist(appState.user.playlists.find(p => p.folder_path === playlist.folder_path))
}
