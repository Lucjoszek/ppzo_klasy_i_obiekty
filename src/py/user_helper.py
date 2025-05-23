"""
Module for saving and loading user data including playlists and tracks.
Implements serialization to JSON and handles dynamic playlist updates.
"""

import json
from pathlib import Path
from models.user import User
from models.playlist import Playlist
from models.track import Track
from config import PROGRAM_DATA, AUDIO_EXTENSIONS
from typing import List
import logging
import tinytag
from datetime import timedelta


def save_user(user: User) -> bool:
    """
    Saves the given user's data to a JSON file in their dedicated folder.
    """

    logging.info(f"Saving user '{user.username}'.")

    try:
        user_folder_path = PROGRAM_DATA / user.username

        # Create user directory if it doesn't exist
        # if not user_folder_path.exists():
        user_folder_path.mkdir(parents=True, exist_ok=True)

        # Define path to user.json
        user_json_path = user_folder_path / "user.json"

        # Convert User object to dictionary for serialization
        user_dict = user.to_dict()

        # Write data to JSON file
        with open(user_json_path, "w", encoding="utf-8") as f:
            json.dump(user_dict, f, indent=4, ensure_ascii=False)

        logging.info(f"User '{user.username}' has been saved.")

        return True

    except Exception as e:
        logging.error(f"Error saving user: {e}")

        return False


def load_user(username: str) -> User:
    """
    Loads a user from their JSON file or creates a new one if not found.
    """

    user_file = PROGRAM_DATA / username / "user.json"

    if not user_file.is_file():
        logging.warn(f"user.json does not exist for '{username}'!")

        return User(username)

    with user_file.open("r", encoding="utf-8") as f:
        user_data = json.load(f)

    # Deserialize user data
    user = User.from_dict(user_data)

    # Load and refresh associated playlist
    user.playlists = _load_playlists(user)

    return user


def _load_playlists(user: User) -> List[Playlist]:
    """
    Reloads each playlist's folder content to detect new audio files.
    Updates track lists and recalculates durations accordingly.
    """

    logging.info(f"Loading '{user.username}' playlists.")
    logging.info(f"Detected playlists: {len(user.playlists)}'")

    refreshed_playlists: List[Playlist] = []

    for playlist in user.playlists:
        logging.info(f"Refreshing '{playlist.title}' playlist.")

        playlist_folder = Path(playlist.folder_path)

        if not playlist_folder.is_dir():
            logging.warning(
                f"Skipping playlist '{playlist.title}': Folder does not exist: {playlist_folder}"
            )
            continue

        try:
            # Get all valid audio files from the playlist folder
            audio_files = [
                f
                for f in playlist_folder.iterdir()
                if f.is_file() and f.suffix.lower() in AUDIO_EXTENSIONS
            ]

            logging.info(f"Found {len(audio_files)} audio files in '{playlist.title}'.")

            # Prepare existing tracks and total duration
            updated_tracks = list(playlist.tracks)
            total_duration = sum((t.duration for t in playlist.tracks), timedelta())

            # Set of existing file paths to avoid duplicates
            existing_paths = {t.file_path for t in updated_tracks}

            # Process each file and add new one
            for audio_file in audio_files:
                file_path_str = str(audio_file)

                if file_path_str in existing_paths:
                    continue

                try:
                    tag = tinytag.TinyTag.get(file_path_str)

                    title = tag.title or audio_file.stem
                    artist = tag.artist or "Unknown"
                    duration = timedelta(seconds=tag.duration or 0)

                    new_track = Track(title, artist, duration, file_path_str)
                    updated_tracks.append(new_track)
                    total_duration += duration

                    logging.info(f"Added new track: '{new_track.title}'.")

                except Exception as ex:
                    logging.warning(f"Skipped file '{audio_file.name}': {ex}")

            logging.info(f"Finished refreshing playlist '{playlist.title}'.")

            # Update playlist with new data
            playlist.tracks = updated_tracks
            playlist.duration = total_duration

            refreshed_playlists.append(playlist)

        except Exception as ex:
            logging.error(f"Failed to load playlist '{playlist.title}': {ex}")

    return refreshed_playlists
