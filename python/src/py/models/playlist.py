"""
Module defining the Playlist data model.
"""

from dataclasses import dataclass
from datetime import timedelta
from typing import List
from models.track import Track


@dataclass
class Playlist:
    title: str
    duration: timedelta
    folder_path: str
    tracks: List[Track]

    @classmethod
    def from_dict(cls, data: dict):
        tracks_data = data.get("tracks", [])
        tracks = [Track.from_dict(t) for t in tracks_data]

        return cls(
            title=data["title"],
            duration=timedelta(seconds=data.get("duration", 0)),
            folder_path=data["folder_path"],
            tracks=tracks,
        )

    def to_dict(self):
        return {
            "title": self.title,
            "duration": self.duration.total_seconds(),
            "folder_path": self.folder_path,
            "tracks": [track.to_dict() for track in self.tracks],
        }
