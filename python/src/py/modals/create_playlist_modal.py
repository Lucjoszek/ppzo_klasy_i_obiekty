"""
Module for handling folder selection and playlist creation.
"""

import subprocess
import logging
from typing import List
from pathlib import Path
from tinytag import TinyTag
from datetime import timedelta
import state
import user_helper
from config import AUDIO_EXTENSIONS
from models.track import Track
from models.playlist import Playlist

# Path to the folder picker script used for selecting music folders
FOLDER_PICKER_PATH = Path(__file__).parent.parent / "utils" / "folder_picker.py"


def pick_folder() -> str:
    """
    Launches an external script to open a folder selection dialog.
    Returns the selected folder path as a string.
    """

    logging.info("Starting folder_picker subprocess.")

    result = subprocess.run(
        ["python", str(FOLDER_PICKER_PATH)], capture_output=True, text=True
    )

    logging.info(f"Result of folder_picker: \n${result}")

    return result.stdout.strip()


def create_playlist(title, folder_path) -> bool:
    """
    Creates a new playlist from audio files in the given folder.

    Ensures:
    - Playlist title is unique.
    - Folder path is not already used by another playlist.
    """

    logging.info(f"Creating playlist: {title}, {folder_path}.")

    for playlist in state.user.playlists:
        if playlist.title == title:
            logging.error(f"A playlist with title '{title}' already exists.")
            return False
        if playlist.folder_path == folder_path:
            logging.error("A playlist with this folder path already exists.")
            return False

    tracks = _load_tracks(folder_path)
    total_duration = sum((track.duration for track in tracks), timedelta())
    new_playlist = Playlist(title, total_duration, folder_path, tracks)

    state.user.add_playlist(new_playlist)

    if not user_helper.save_user(state.user):
        logging.error("Failed to save user data. Rolling back changes.")

        if not state.user.remove_playlist(new_playlist):
            logging.critical("Failed to revert changes!")

        return False

    logging.info(f"Playlist '{title}' has been added.")
    return True


def _load_tracks(folder_path) -> List[Track]:
    """
    Loads all valid audio files from the given folder and extracts their metadata.
    Returns a list of Track objects.
    """

    folder = Path(folder_path)

    if not folder.is_dir():
        return False

    # Filter files by supported audio extensions
    audio_files = [
        f
        for f in folder.iterdir()
        if f.is_file() and f.suffix.lower() in AUDIO_EXTENSIONS
    ]

    logging.info(f"Detected files: {len(audio_files)}.")

    tracks: List[Track] = []

    # Extract metadata using TinyTag
    for audio_file in audio_files:
        file_path = str(audio_file)

        logging.info(f"Reading file: {file_path}.")

        try:
            tag = TinyTag.get(file_path)

            title = tag.title or audio_file.stem
            artist = tag.artist or "Unknown"
            duration = timedelta(seconds=tag.duration or 0.0)

            track = Track(title, artist, duration, file_path)

            tracks.append(track)
        except Exception as e:
            logging.warning(f"Error reading audio file: '{file_path}': {e}.")

    return tracks
