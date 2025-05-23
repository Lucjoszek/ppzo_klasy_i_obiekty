import { appState } from "../state.js"
import { formatDuration } from "../utils/format-duration-util.js"
import { moveTrack } from "../services/api.js"
import { refresh } from "../utils/refresh-util.js"
import { showPage } from "./show-page.js"
import { renameButton, toggleRenameButton } from "../modals/rename-playlist-modal.js"

/**
 * Moves a track within the playlist to a new index.
 * Disables all move buttons during the operation to prevent duplicate actions.
 *
 * @param {Playlist} playlist - Playlist containing the track.
 * @param {number} fromIndex - Index of the track to move.
 * @param {number} toIndex - Target index for the track.
 * @returns {Promise<void>}
 */
const moveTrackBtn = async (playlist, fromIndex, toIndex) => {
  const moveBtns = document.querySelectorAll('.move-btn')

  console.log(`Moving track from index ${fromIndex} to index ${toIndex} in playlist '${playlist.title}'.`)

  // Disable all move buttons to avoid multiple moves
  moveBtns.forEach(btn => {
    btn.disabled = true
  })

  // Validate indices
  if (toIndex < 0 || toIndex >= playlist.tracks.length) {
    console.error('Cannot move this track — invalid target index.')

    // Re-enable buttons
    moveBtns.forEach(btn => {
      btn.disabled = false
    })

    return
  }

  const success = await moveTrack(playlist, fromIndex, toIndex)

  if (!success) {
    console.error('Failed to move track!')
    return
  }

  console.log(`Track from index ${fromIndex} has been moved to index ${toIndex}.`)

  await refresh(['home', 'playlists'])
  seePlaylist(appState.user.playlists.find(p => p.folder_path === playlist.folder_path))
}

/**
 * Displays the playlist details page with its tracks and controls.
 * - Shows track list with move-up/move-down buttons.
 * - Sets up renaming functionality for the playlist.
 * - Disables move buttons at the edges (first/last track).
 *
 * @param {Playlist} playlist - The playlist object to display.
 */
export const seePlaylist = (playlist) => {
  const playlistTracksList = document.querySelector('#playlist-tracks-list')
  const playlistTracksTitle = document.querySelector('#playlist-tracks-title')
  const playlistTitleInput = document.querySelector('#renamePlaylistTitle')
  const renamePlaylistBtn = document.querySelector('#renamePlaylistButton')

  // Remove existing listeners to avoid duplication
  playlistTitleInput.removeEventListener('input', appState.renameInputListener)
  renamePlaylistBtn.removeEventListener('click', appState.renameClickListener)

  // Set up new listeners
  appState.renameInputListener = () => toggleRenameButton(playlist.title)
  appState.renameClickListener = () => renameButton(playlist)
  playlistTitleInput.addEventListener('input', appState.renameInputListener)
  renamePlaylistBtn.addEventListener('click', appState.renameClickListener)

  console.log(`Loading '${playlist.title}' playlist page.`)

  // Clear previous content
  playlistTracksList.innerHTML = ''
  playlistTracksTitle.textContent = playlist.title
  playlistTitleInput.value = playlist.title

  // Generate track list
  playlist.tracks.forEach((track, index) => {
    const trackLi = document.createElement('li')
    trackLi.className = 'list-row flex justify-between items-center p-4 border-b'

    const infoDiv = document.createElement('div')
    infoDiv.innerHTML = `
      <p class="font-semibold">${track.title}</p>
      <p class="text-xs uppercase font-semibold opacity-60">${track.artist}</p>
      <p class="text-xs uppercase font-semibold opacity-60">${formatDuration(track.duration)}</p>
    `

    const actionsDiv = document.createElement('div')
    actionsDiv.className = 'flex gap-2'

    const upBtn = document.createElement('button')
    upBtn.onclick = () => moveTrackBtn(playlist, index, index - 1)
    upBtn.className = "btn"
    upBtn.innerHTML = `
      <svg xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 -960 960 960" width="24px" fill="currentColor">
        <path d="M480-528 296-344l-56-56 240-240 240 240-56 56-184-184Z"/>
      </svg>
    `

    const downBtn = document.createElement('button')
    downBtn.onclick = () => moveTrackBtn(playlist, index, index + 1)
    downBtn.className = "btn"
    downBtn.innerHTML = `
      <svg xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 -960 960 960" width="24px" fill="currentColor">
        <path d="M480-344 240-584l56-56 184 184 184-184 56 56-240 240Z"/>
      </svg>
    `

    if (index === 0) {
      upBtn.disabled = true
      upBtn.classList.add('btn-disabled')
    }
    if (index === playlist.tracks.length - 1) {
      downBtn.disabled = true
      downBtn.classList.add('btn-disabled')
    }

    actionsDiv.appendChild(upBtn)
    actionsDiv.appendChild(downBtn)

    trackLi.appendChild(infoDiv)
    trackLi.appendChild(actionsDiv)

    playlistTracksList.appendChild(trackLi)
  })

  console.log(`'${playlist.title}' playlist page loading completed.`)

  showPage('playlist-tracks')
}