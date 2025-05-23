"""
Module defining the Track data model.
"""

from dataclasses import dataclass
from datetime import timedelta


@dataclass
class Track:
    title: str
    artist: str
    duration: timedelta
    file_path: str

    @classmethod
    def from_dict(cls, data: dict):
        return cls(
            title=data["title"],
            artist=data["artist"],
            duration=timedelta(seconds=data.get("duration", 0)),
            file_path=data["file_path"],
        )

    def to_dict(self):
        return {
            "title": self.title,
            "artist": self.artist,
            "duration": self.duration.total_seconds(),
            "file_path": self.file_path,
        }
