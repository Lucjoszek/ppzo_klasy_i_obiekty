/**
 * Main application entry point.
 * Initializes UI components and services after DOM is fully loaded.
 * Loads initial data and displays the home page.
 */

import { initControlBtns, initVolumeSlider } from "./components/media-player.js"
import { initNavBtns } from "./components/navigation.js"
import { showPage } from "./components/show-page.js"
import { initCreatePlaylistModal } from "./modals/create-playlist-modal.js"
import { refresh } from "./utils/refresh-util.js"

document.addEventListener('DOMContentLoaded', async () => {
    // Initialize UI controls
    initNavBtns()
    initVolumeSlider()
    initControlBtns()
    initCreatePlaylistModal()

    // Load user data and refresh pages
    await refresh(['home', 'playlists'])

    // Show the default page
    showPage('home')
})