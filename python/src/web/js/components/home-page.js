import { appState } from "../state.js"
import { formatDuration } from "../utils/format-duration-util.js"
import { runPlaylist } from "./run-playlist.js"
import { seePlaylist } from "./see-playlist.js"

/**
 * Loads the home page with recently played playlists.
 * Clears the current list and populates it with playlists found in the user's playlists by title.
 * * Each playlist is rendered as a list item with information and action buttons:
 * - Play playlist
 * - See playlist details
 * 
 * @param {string[]} recentlyPlayedPlaylistsTitles - Array of playlist titles recently played.
 */
export const loadHomePage = (recentlyPlayedPlaylistsTitles) => {
  const recentlyPlayedPlaylistsList = document.querySelector('#recently-played-playlists-list')

  console.log('Loading home page.')

  // Clear previous content
  recentlyPlayedPlaylistsList.innerHTML = ''

  // Render each playlist in the list
  recentlyPlayedPlaylistsTitles.forEach(playlistTitle => {
    const playlist = appState.user.playlists.find(p => p.title == playlistTitle)

    if (!playlist) {
      console.warn(`Cannot find playlist with title: ${playlistTitle}.`)
      return
    }

    const playlistLi = document.createElement("li")
    playlistLi.className = "list-row flex justify-between items-center p-4 border-b"

    const infoDiv = document.createElement("div")
    infoDiv.innerHTML = `
      <p class="font-semibold">${playlist.title}</p>
      <p class="text-xs uppercase font-semibold opacity-60">Total tracks: ${playlist.tracks.length}</p>
      <p class="text-xs uppercase font-semibold opacity-60">Total duration: ${formatDuration(playlist.duration)}</p>
    `

    const actionsDiv = document.createElement("div")
    actionsDiv.className = "flex gap-2"

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

    actionsDiv.appendChild(playBtn)
    actionsDiv.appendChild(seeBtn)

    playlistLi.appendChild(infoDiv);
    playlistLi.appendChild(actionsDiv);

    recentlyPlayedPlaylistsList.appendChild(playlistLi)
  })

  console.log('Home page loading completed.')
}