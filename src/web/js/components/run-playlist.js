import { addToRecent } from "./add-to-recent.js"
import { mediaPlay, setVolume } from "../services/api.js"
import { appState } from "../state.js"
import { updateTrackPosition, updateTrackInfo } from "./media-player.js"

/**
 * Starts playing the given playlist.
 * - Adds it to recently played list.
 * - Loads the first track for playback.
 * - Updates UI with track info and progress.
 * - Manages volume and play/pause state.
 *
 * @param {Playlist} playlist - The playlist object to play.
 * @returns {Promise<void>}
 */
export const runPlaylist = async (playlist) => {
    const infoTrackLastIndex = document.querySelector('#last-track-index')
    const playPauseBtnPlayIcon = document.querySelector('#play-pause-btn-play-icon')
    const playPauseBtnPauseIcon = document.querySelector('#play-pause-btn-pause-icon')
    const volumeSlider = document.querySelector('#volume-slider')

    // Add playlist to recently played list
    addToRecent(playlist)

    console.log(`Starting playlist: '${playlist.title}'.`)

    const success = await mediaPlay(playlist)

    if (!success) {
        console.error(`Failed to load playlist or playlist has no tracks!`)
        return
    }

    console.log(`Successfully started playlist: '${playlist.title}'`)

    // Clear any existing interval and start a new one for updating track position
    clearInterval(appState.intervailId)
    appState.intervailId = setInterval(updateTrackPosition, 1000)

    // Update UI elements
    infoTrackLastIndex.textContent = playlist.tracks.length
    setVolume(volumeSlider.value / 100)

    // Switch play/pause button icon
    playPauseBtnPlayIcon.classList.add('hidden')
    playPauseBtnPauseIcon.classList.remove('hidden')

    await updateTrackInfo()
}