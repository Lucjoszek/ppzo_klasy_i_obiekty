// User
export const getUserData = async () => { return await eel.get_user_data()() }

// Playlist
export const createPlaylist = async (title, folderPath) => { return await eel.create_playlist(title, folderPath)() }
export const addToRecentlyPlayed = async (playlist) => { return await eel.add_to_recently_played(playlist)() }
export const renamePlaylist = async (playlist, newTitle) => { return await eel.rename_playlist(playlist, newTitle)() }
export const removePlaylist = async (playlist) => { return await eel.remove_playlist(playlist)() }

// Util
export const pickFolder = async () => { return await eel.pick_folder()() }

// MediaPlayer
export const getCurrentTrackPosition = async () => { return await eel.get_current_track_position()() }
export const getCurrentTrackInfo = async () => { return await eel.get_current_track_info()() }
export const isMediaPlaying = async () => { return await eel.is_playing()() }
export const mediaPlay = async (playlist) => { return await eel.play_playlist(playlist)() }
export const skipToPreviousTrack = async () => { return await eel.prev_track()() }
export const skipToNextTrack = async () => { return await eel.next_track()() }
export const moveTrack = async (playlist, fromIndex, toIndex) => { return await eel.move_track(playlist, fromIndex, toIndex)() }
export const pauseCurrentTrack = () => { eel.pause_current_track() }
export const resumeCurrentTrack = () => { eel.resume_current_track() }
export const setVolume = (volume) => { eel.set_volume(volume) }