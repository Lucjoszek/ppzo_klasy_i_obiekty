import { removePlaylist } from "../services/api.js"
import { refresh } from "../utils/refresh-util.js"

/**
 * Removes the specified playlist by calling the backend,
 * then refreshes the 'home' and 'playlists' pages.
 *
 * @param {Playlist} playlist - The playlist object to remove.
 * @returns {Promise<void>}
 */
export const removePlaylistBtn = async (playlist) => {
    console.log(`Removing '${playlist.title}' playlist.`)

    const success = await removePlaylist(playlist)

    if (!success) {
        console.error(`Failed to remove playlist: '${playlist.title}'`)
        return
    }

    console.log(`Playlist '${playlist.title}' has been removed.`)

    await refresh(['home', 'playlists'])
}