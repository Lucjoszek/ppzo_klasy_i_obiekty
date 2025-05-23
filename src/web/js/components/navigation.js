import { showPage } from "./show-page.js"

/**
 * Initializes navigation buttons for switching between different pages/views.
 * Currently supports:
 * - Home page ('home')
 * - Playlists page ('playlists')
 */
export const initNavBtns = () => {
    const homeNavBtn = document.querySelector('#btn-home')
    const playlistsNavBtn = document.querySelector('#btn-playlists')

    homeNavBtn.addEventListener('click', () => showPage('home'))
    playlistsNavBtn.addEventListener('click', () => showPage('playlists'))
}