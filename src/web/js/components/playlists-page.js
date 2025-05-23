import { formatDuration } from "../utils/format-duration-util.js"
import { runPlaylist } from "./run-playlist.js"
import { seePlaylist } from "./see-playlist.js"
import { removePlaylistBtn } from "./remove-playlist.js"

/**
 * Loads the playlists page.
 * Clears the current playlists list in the DOM and populates it with the given playlists.
 * Each playlist is rendered as a list item with information and action buttons:
 * - Play playlist
 * - See playlist details
 * - More options dropdown
 *  - Delete
 * 
 * @param {Playlist[]} playlists - Array of playlist objects to display.
 */
export const loadPlaylistPage = (playlists) => {
  const playlistsList = document.querySelector('#playlists-list')

  console.log('Loading playlists page.')

  // Clear previous content
  playlistsList.innerHTML = ""

  // Render each playlist in the list
  playlists.forEach(playlist => {
    const playlistLi = document.createElement("li")
    playlistLi.className = "list-row flex justify-between items-center p-4 border-b"

    const infoDiv = document.createElement("div")
    infoDiv.innerHTML = `
      <p class="font-semibold">${playlist.title}</p>
      <p class="text-xs uppercase font-semibold opacity-60">Total tracks: ${playlist.tracks.length}</p>
      <p class="text-xs uppercase font-semibold opacity-60">Total duration: ${formatDuration(playlist.duration)}</p>
    `

    const actionsDiv = document.createElement("div")
    actionsDiv.className = "flex gap-2 justify-center items-center"

    const playBtn = document.createElement("button")
    playBtn.onclick = () => runPlaylist(playlist)
    playBtn.className = "btn"
    playBtn.innerHTML = `
      <svg xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 -960 960 960" width="24px" fill="currentColor">
        <path d="M320-200v-560l440 280-440 280Zm80-280Zm0 134 210-134-210-134v268Z" />
      </svg>
    `

    const seeBtn = document.createElement("button")
    seeBtn.onclick = () => seePlaylist(playlist)
    seeBtn.className = "btn"
    seeBtn.textContent = "See playlist"

    const moreDiv = document.createElement('div')
    moreDiv.className = 'dropdown dropdown-top dropdown-center'

    const dropdownBtn = document.createElement('div')
    dropdownBtn.className = "btn m-1"
    dropdownBtn.role = "button"
    dropdownBtn.tabIndex = 0
    dropdownBtn.innerHTML = `
      <svg xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 -960 960 960" width="24px" fill="currentColor">
        <path d="M480-160q-33 0-56.5-23.5T400-240q0-33 23.5-56.5T480-320q33 0 56.5 23.5T560-240q0 33-23.5 56.5T480-160Zm0-240q-33 0-56.5-23.5T400-480q0-33 23.5-56.5T480-560q33 0 56.5 23.5T560-480q0 33-23.5 56.5T480-400Zm0-240q-33 0-56.5-23.5T400-720q0-33 23.5-56.5T480-800q33 0 56.5 23.5T560-720q0 33-23.5 56.5T480-640Z"/>
      </svg>
    `

    const dropdownUl = document.createElement('ul')
    dropdownUl.className = 'dropdown-content menu bg-base-100 rounded-box z-1 w-24 p-2 shadow-sm'
    dropdownUl.tabIndex = 0

    const removeBtnLi = document.createElement('li')
    const removeBtn = document.createElement('button')
    removeBtn.textContent = 'Delete'
    removeBtn.onclick = () => removePlaylistBtn(playlist)

    removeBtnLi.appendChild(removeBtn)
    dropdownUl.appendChild(removeBtnLi)
    moreDiv.appendChild(dropdownBtn)
    moreDiv.appendChild(dropdownUl)

    actionsDiv.appendChild(playBtn)
    actionsDiv.appendChild(seeBtn)
    actionsDiv.appendChild(moreDiv)

    playlistLi.appendChild(infoDiv)
    playlistLi.appendChild(actionsDiv)

    playlistsList.appendChild(playlistLi)
  })

  console.log('Playlists page loading completed.')
}