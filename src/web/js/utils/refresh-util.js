import { getUserData } from "../services/api.js"
import { appState } from "../state.js"
import { loadHomePage } from "../components/home-page.js"
import { loadPlaylistPage } from "../components/playlists-page.js"

/**
 * Refreshes the user data from the backend and reloads specified pages.
 *
 * @param {string[]} pages - Array of page names to refresh. Supported values: 'home', 'playlists'.
 * @returns {Promise<void>}
 */
export const refresh = async (pages) => {
    console.log('Refreshing user data.')

    appState.user = await getUserData()

    console.log(`Refreshing pages: ${[...pages].join(', ')}.`)

    pages.forEach(page => {
        switch (page) {
            case 'home':
                loadHomePage(appState.user.recently_played_playlists)
                break

            case 'playlists':
                loadPlaylistPage(appState.user.playlists)
                break

            default:
                console.log(`Page '${page}' doesn't exists`)
        }
    })
}