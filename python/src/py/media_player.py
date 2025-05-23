"""
Module defining a media player capable of handling playlist playback with basic controls.
"""

import logging
from just_playback import Playback
from models.playlist import Playlist
from models.track import Track


class MediaPlayer:
    def __init__(self):
        self.playback = Playback()
        self.playlist: Playlist | None = None
        self.current_index: int = 0

    def load_playlist(self, playlist: Playlist) -> bool:
        """
        Loads a playlist into the media player and starts from the first track.
        """

        logging.info(f"Loading '{playlist.title}' playlist into media player.")

        self.playlist = playlist
        self.current_index = 0

        if not self.playlist.tracks:
            logging.warn("Cannot load playlist. It contains no tracks.")

            return False

        return self._load_current_track()

    def _load_current_track(self) -> bool:
        if not self.playlist or self.current_index >= len(self.playlist.tracks):
            logging.warn("No valid track to load")

        track: Track = self.playlist.tracks[self.current_index]
        try:
            logging.info(f"Loading track: {track.title} - {track.file_path}")
            self.playback.stop()
            self.playback = Playback()  # Reset playback
            self.playback.load_file(track.file_path)

            return True

        except Exception as e:
            logging.error(f"Failed to load track '{track.title}': {e}")

            return False

    def resume(self) -> None:
        self.playback.resume()

    def set_volume(self, volume) -> None:
        self.playback.set_volume(volume)

    def play(self) -> None:
        if self.playback and not self.playback.playing:
            self.playback.play()

    def pause(self) -> None:
        if self.playback and self.playback.playing:
            self.playback.pause()

    def next_track(self) -> bool:
        if self.playlist and self.current_index < len(self.playlist.tracks) - 1:
            self.current_index += 1

            return self._load_current_track()

        return False

    def prev_track(self) -> bool:
        if self.playlist and self.current_index > 0:
            self.current_index -= 1

            return self._load_current_track()

        return False

    def get_current_track_info(self) -> dict:
        if self.playlist and self.current_index < len(self.playlist.tracks):
            track: Track = self.playlist.tracks[self.current_index]

            return {
                "title": track.title,
                "artist": track.artist,
                "file_path": track.file_path,
                "duration": track.duration.total_seconds(),
                "position": self.playback.curr_pos,
                "is_playing": self.playback.playing,
                "current_index": self.current_index,
            }

        return {}

    def get_current_track_position(self) -> float:
        return self.playback.curr_pos

    def is_playing(self) -> bool:
        return self.playback.playing
