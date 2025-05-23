import { pickFolder, createPlaylist } from "../services/api.js"
import { refresh } from "../utils/refresh-util.js"

/**
 * Initializes the "Create Playlist" modal UI.
 * Sets up event listeners for:
 * - Input validation (title and folder path)
 * - Folder selection via backend
 * - Playlist creation and page refresh
 */
export const initCreatePlaylistModal = () => {
    const newPlaylistTitle = document.querySelector('#newPlaylistTitle')
    const newPlaylistFolder = document.querySelector('#newPlaylistFolder')
    const browseFolderButton = document.querySelector('#browseFolderButton')
    const createNewPlaylistButton = document.querySelector('#createNewPlaylistButton')

    /**
     * Enables or disables the "Create" button based on input values.
     * Requires both title and folder path to be filled.
     */
    const toggleCreateButton = () => {
        (newPlaylistTitle.value == "" || newPlaylistFolder.value == "")
            ? createNewPlaylistButton.setAttribute('disabled', 'disabled')
            : createNewPlaylistButton.removeAttribute('disabled')
    }

    // Listen for changes in title input to update button state
    newPlaylistTitle.addEventListener('input', toggleCreateButton)

    // Handle folder selection button click
    browseFolderButton.addEventListener('click', async () => {
        console.log('Opening folder picker dialog.')

        const folderPath = await pickFolder()

        console.log('Browsing folder completed.')

        if (!folderPath) {
            console.warn('No folder selected.')
            return
        }

        console.log(`Selected folder: ${folderPath}`)

        newPlaylistFolder.value = folderPath
        toggleCreateButton()
    })

    // Handle playlist creation request
    createNewPlaylistButton.addEventListener('click', async () => {
        const newPlaylistModal = document.querySelector('#newPlaylist_modal')

        const title = newPlaylistTitle.value
        const folderPath = newPlaylistFolder.value

        console.log(`Creating new playlist: ${title} - ${folderPath}`)

        if (!title || !folderPath) {
            console.error('Missing title or folder path!')
            return
        }

        newPlaylistModal.close()

        const success = await createPlaylist(title, folderPath)

        if (!success) {
            console.error('Failed to create playlist!')
            return
        }

        console.log(`Playlist '${title}' has been created.`)

        await refresh(['playlists'])
    })
}