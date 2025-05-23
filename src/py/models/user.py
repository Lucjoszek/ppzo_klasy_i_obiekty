"""
Module defining the User data model.
"""

import logging
from dataclasses import dataclass, field
from typing import List
from models.playlist import Playlist

@dataclass
class User:
    username: str
    playlists: List[Playlist] = field(default_factory=list)
    recently_played_playlists: List[str] = field(default_factory=list)

    def add_playlist(self, playlist: Playlist) -> None:
        logging.info(f"Adding '{playlist.title}' playlist.")
        self.playlists.append(playlist)

    def remove_playlist(self, playlist: Playlist) -> bool:
        try:
            self.playlists.remove(playlist)
            return True
        except ValueError:
            logging.error(f"Failed to remove '{playlist.title}' playlist")
            return False


    def add_to_recently_played(self, playlist: Playlist) -> None:
        title = playlist.title

        if title not in self.recently_played_playlists:
            self.recently_played_playlists.append(title)

        while len(self.recently_played_playlists) > 5:
            self.recently_played_playlists.pop(0)

    @classmethod
    def from_dict(cls, data: dict):
        playlists_data = data.get("playlists", [])
        playlists = [Playlist.from_dict(pl) for pl in playlists_data]

        recently_played = data.get("recently_played_playlists", [])

        return cls(
            username=data["username"],
            playlists=playlists,
            recently_played_playlists=recently_played,
        )

    def to_dict(self):
        return {
            "username": self.username,
            "playlists": [playlist.to_dict() for playlist in self.playlists],
            "recently_played_playlists": self.recently_played_playlists,
        }
