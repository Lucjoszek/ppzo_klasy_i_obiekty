/**
 * Displays the specified page and updates the navigation UI accordingly.
 *
 * @param {'home' | 'playlists' | 'playlist-tracks'} page - The name of the page to show.
 */
export const showPage = (page) => {
    const homeNavBtn = document.querySelector('#btn-home')
    const playlistsNavBtn = document.querySelector('#btn-playlists')
    const navBtns = document.querySelectorAll('.nav-btn')
    const pages = document.querySelectorAll('.page')
    const homePage = document.querySelector('#home-page')
    const playlistsPage = document.querySelector('#playlists-page')
    const playlistTracksPage = document.querySelector('#playlist-tracks-page')

    console.log(`Changing page to: ${page}`)

    // Hide all pages
    pages.forEach(page => {
        page.classList.add('hidden')
    })

    // Deactive all navigation buttons
    navBtns.forEach(btn => {
        btn.classList.remove('btn-active')
    })

    // Show a given page and update navigation UI
    switch (page) {
        case 'home':
            homePage.classList.remove('hidden')
            homeNavBtn.classList.add('btn-active')
            break

        case 'playlists':
            playlistsPage.classList.remove('hidden')
            playlistsNavBtn.classList.add('btn-active')
            break

        case 'playlist-tracks':
            playlistTracksPage.classList.remove('hidden')
            break

        default:
            console.warn(`Unknown page requested: ${page}`)
            break
    }
}

