import { addToRecentlyPlayed } from "../services/api.js"
import { refresh } from "../utils/refresh-util.js"

/**
 * Adds the given playlist to the list of recently played playlists on the backend.
 * After successful addition, refreshes relevant parts of the UI (e.g., home page).
 *
 * @param {Playlist} playlist - The playlist object to add to recently played.
 * 
 * @returns {Promise<void>}
 */
export const addToRecent = async (playlist) => {
    console.log(`Adding '${playlist.title}' to list of recently played playlists.`)

    const success = await addToRecentlyPlayed(playlist)

    if (!success) {
        console.error('Failed to add playlist to list of recently played!')
        return
    }

    console.log(`Successfully added '${playlist.title}' to recently played playlists.`)

    await refresh(['home'])
}