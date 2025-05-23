"""
Module containing logic for renaming playlists in the application.
"""

import logging
import state
import user_helper
from models.playlist import Playlist


def rename_playlist(playlist: Playlist, new_title: str) -> bool:
    """
    Renames the given playlist to the specified new title.
    
    Ensures:
    - The playlist exists in the current user's list.
    - Changes are saved to persistent storage.
    - If saving fails, the title is reverted automatically (rollback).
    """

    logging.info(f"Renaming playlist '{playlist.title}' to '{new_title}'.")

    try:
        playlist_index = state.user.playlists.index(playlist)
    except ValueError:
        logging.error("Playlist not found!")
        return False

    targeted_playlist = state.user.playlists[playlist_index]
    title_backup = targeted_playlist.title
    targeted_playlist.title = new_title

    if not user_helper.save_user(state.user):
        logging.error("Failed to save user data! Rolling back changes.")
        
        targeted_playlist.title = title_backup
        return False

    logging.info(f"Playlist has been renamed to '{new_title}'.")
    return True
