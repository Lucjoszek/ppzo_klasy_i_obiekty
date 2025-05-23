"""
Main application module that initializes the Eel and handles communication between Python backend and JavaScript frontend.
"""

import eel
import logging
import copy
from browsers import browsers
from pathlib import Path
import user_helper
import state
from modals import create_playlist_modal, rename_playlist_modal
from config import USERNAME
from models.user import User
from models.playlist import Playlist
from media_player import MediaPlayer

# The path to the first available browser on the system
DEFAULT_BROWSER_PATH = next(browsers())["path"]

# The path to the web directory containing the frontend assets
WEB_DIR = Path(__file__).resolve().parent.parent / "web"

# Load the current user's data at startup
state.user = user_helper.load_user(USERNAME)

# Initialize the media player instance
media_player = MediaPlayer()

eel.init(WEB_DIR)


@eel.expose
def pick_folder() -> str:
    return create_playlist_modal.pick_folder()


@eel.expose
def create_playlist(title: str, folder_path: str) -> bool:
    return create_playlist_modal.create_playlist(title, folder_path)


@eel.expose
def rename_playlist(playlist_dict: dict, new_title: str) -> bool:
    playlist = Playlist.from_dict(playlist_dict)

    return rename_playlist_modal.rename_playlist(playlist, new_title)


@eel.expose
def get_current_track_position() -> float:
    return media_player.get_current_track_position()


@eel.expose
def is_playing() -> bool:
    return media_player.is_playing()


@eel.expose
def play_playlist(playlist_dict: dict) -> bool:
    playlist = Playlist.from_dict(playlist_dict)

    success = media_player.load_playlist(playlist)

    if success:
        media_player.play()

    return success


@eel.expose
def set_volume(volume: float) -> None:
    media_player.set_volume(volume)


@eel.expose
def resume_current_track() -> None:
    media_player.resume()


@eel.expose
def pause_current_track() -> None:
    media_player.pause()


@eel.expose
def next_track() -> bool:
    if media_player.next_track():
        media_player.play()
        return True
    return False


@eel.expose
def prev_track() -> bool:
    if media_player.prev_track():
        media_player.play()
        return True
    return False


@eel.expose
def get_current_track_info():
    return media_player.get_current_track_info()


@eel.expose
def get_user_data() -> User:
    logging.info("Retrieving user data.")

    data = state.user.to_dict()

    return data


@eel.expose
def add_to_recently_played(playlist_dict: dict) -> bool:
    playlist = Playlist.from_dict(playlist_dict)

    logging.info(f"Adding '{playlist.title}' playlist to recently played.")

    recently_played_playlists_backup = copy.deepcopy(
        state.user.recently_played_playlists
    )

    state.user.add_to_recently_played(playlist)

    if not user_helper.save_user(state.user):
        logging.error("Failed to save user data! Rolling back changes.")
        state.user.recently_played_playlists = (
            recently_played_playlists_backup
        )
        return False

    logging.info(f"Playlist '{playlist.title}' has been added to recently played.")
    return True


@eel.expose
def remove_playlist(playlist_dict: dict) -> bool:
    playlist = Playlist.from_dict(playlist_dict)

    logging.info(f"Removing '{playlist.title}' playlist.")

    try:
        playlist_index = state.user.playlists.index(playlist)
    except ValueError:
        logging.error("Playlist not found!")
        return False

    playlist_backup = copy.deepcopy(state.user.playlists[playlist_index])

    state.user.remove_playlist(playlist)

    if not user_helper.save_user(state.user):
        logging.error("Failed to save user data! Rolling back changes.")
        state.user.playlists.insert(playlist_index, playlist_backup)
        return False

    logging.info(f"Playlist '{playlist.title}' has been removed.")
    return True


@eel.expose
def move_track(playlist_dict: dict, from_index: int, to_index: int) -> bool:
    playlist = Playlist.from_dict(playlist_dict)

    logging.info(
        f"Moving track from {from_index} to {to_index} in playlist '{playlist.title}'."
    )

    try:
        playlist_index = state.user.playlists.index(playlist)
    except ValueError:
        logging.error("Playlist not found!")
        return False

    targeted_playlist = state.user.playlists[playlist_index]
    playlist_backup = copy.deepcopy(state.user.playlists[playlist_index])

    # Check indices
    if (
        from_index < 0
        or to_index < 0
        or from_index >= len(targeted_playlist.tracks)
        or to_index >= len(targeted_playlist.tracks)
    ):
        logging.error("Invalid indices for track move!")
        return False

    # Move track
    track = targeted_playlist.tracks.pop(from_index)
    targeted_playlist.tracks.insert(to_index, track)

    if not user_helper.save_user(state.user):
        logging.error("Failed to save user data! Rolling back changes.")
        targeted_playlist = playlist_backup
        return False

    logging.info(f"Track has been moved in playlist '{targeted_playlist.title}'.")
    return True


eel.start(
    "index.html",
    mode="custom",
    cmdline_args=[DEFAULT_BROWSER_PATH, "--app=http://localhost:8000/index.html"],
)
