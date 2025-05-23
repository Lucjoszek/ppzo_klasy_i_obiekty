"""
Global configuration module for the application.

Contains constants and system-specific settings used across different modules.
"""

import os
from pathlib import Path

# Supported audio file extensions
AUDIO_EXTENSIONS = {".mp3", ".wav", ".flac", ".aac", ".m4a", ".ogg"}

# Application data directory: OS-agnostic path using PROGRAMDATA environment variable
PROGRAM_DATA = Path("C:/ProgramData/PlaylistHub")

# Name of the currently logged-in user (used for identifying user-specific data)
USERNAME = os.getlogin()

