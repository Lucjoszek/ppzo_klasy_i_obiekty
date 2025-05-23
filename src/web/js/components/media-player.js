import { getCurrentTrackInfo, getCurrentTrackPosition, isMediaPlaying, pauseCurrentTrack, resumeCurrentTrack, setVolume, skipToNextTrack, skipToPreviousTrack } from "../services/api.js"
import { formatDuration } from "../utils/format-duration-util.js"
import { appState } from "../state.js"

const nextTrack = async () => {
    const playIcon = document.querySelector('#play-pause-btn-play-icon')
    const pauseIcon = document.querySelector('#play-pause-btn-pause-icon')

    const volumeSlider = document.querySelector('#volume-slider')

    console.log('Skipping to next track.')

    const success = skipToNextTrack()

    if (!success) {
        console.error('Failed to skip to next track!')
        return
    }

    playIcon.classList.add('hidden')
    pauseIcon.classList.remove('hidden')

    setVolume(volumeSlider.value / 100)
    await updateTrackInfo()
}

/**
 * Updates the UI with the current playback position of the currently playing track.
 * Fetches data from the backend and updates both time display and progress bar.
 *
 * @async
 * @returns {Promise<void>}
 */
export const updateTrackPosition = async () => {
    const infoTrackCurrentTime = document.querySelector('#track-current-time')
    const infoTrackProgress = document.querySelector('#track-progress')

    const trackPosition = await getCurrentTrackPosition()

    console.log(`Current track position: ${trackPosition}.`)

    infoTrackCurrentTime.textContent = formatDuration(trackPosition)
    infoTrackProgress.value = trackPosition

    if (trackPosition <= 0) {
        console.log('Current track ended. Playing next track.')
        await nextTrack()
    }
}

/**
 * Updates the UI elements with detailed information about the currently playing track.
 * 
 * @async
 * @returns {Promise<void>}
 */
export const updateTrackInfo = async () => {
    const infoTrackTitle = document.querySelector('#track-title')
    const infoTrackArtist = document.querySelector('#track-artist')
    const infoTrackCurrentTime = document.querySelector('#track-current-time')
    const infoTrackCurrentIndex = document.querySelector('#current-track-index')
    const infoTrackEndTime = document.querySelector('#track-end-time')
    const infoTrackProgress = document.querySelector('#track-progress')

    console.log('Updating track info.')

    const trackInfo = await getCurrentTrackInfo()

    if (!trackInfo) {
        console.error('Failed to receive current track info!')
        return
    }

    // Set track details
    infoTrackTitle.textContent = trackInfo.title
    infoTrackArtist.textContent = trackInfo.artist
    infoTrackCurrentIndex.textContent = trackInfo.current_index + 1
    infoTrackCurrentTime.textContent = formatDuration(trackInfo.position)
    infoTrackEndTime.textContent = formatDuration(trackInfo.duration)

    // Set progress bar values
    infoTrackProgress.max = trackInfo.duration
    infoTrackProgress.value = trackInfo.position
}

/**
 * Initializes event listeners for playback control buttons:
 * - Play/Pause
 * - Next Track
 * - Previous Track
 *
 * Handles icon state changes and refreshes track info after navigation.
 */
export const initControlBtns = () => {
    const playPauseBtn = document.querySelector('#play-pause-btn')
    const prevBtn = document.querySelector('#prev-btn')
    const nextBtn = document.querySelector('#next-btn')
    const playIcon = document.querySelector('#play-pause-btn-play-icon')
    const pauseIcon = document.querySelector('#play-pause-btn-pause-icon')

    prevBtn.addEventListener('click', async () => {
        const volumeSlider = document.querySelector('#volume-slider')

        console.log('Skipping to previous track.')

        const success = await skipToPreviousTrack()

        if (!success) {
            console.error('Failed to skip to previous track!')
            return
        }

        playIcon.classList.add('hidden')
        pauseIcon.classList.remove('hidden')

        setVolume(volumeSlider.value / 100)
        await updateTrackInfo()
    })

    nextBtn.addEventListener('click', async () => {
        await nextTrack()
    })

    playPauseBtn.addEventListener('click', async () => {
        const isPlaying = await isMediaPlaying()

        if (isPlaying) {
            console.log('Pausing media player.')

            pauseCurrentTrack()

            pauseIcon.classList.add('hidden')
            playIcon.classList.remove('hidden')

            clearInterval(appState.intervailId)
        } else {
            console.log('Resuming media player.')

            resumeCurrentTrack()

            playIcon.classList.add('hidden')
            pauseIcon.classList.remove('hidden')

            appState.intervailId = setInterval(updateTrackPosition, 1000)
        }
    })
}

/**
 * Initializes the volume slider to control playback volume.
 * Sends updated volume level to the backend on every input change.
 */
export const initVolumeSlider = () => {
    const volumeSlider = document.querySelector('#volume-slider')

    volumeSlider.addEventListener('input', () => {
        setVolume(volumeSlider.value / 100)
    })
}