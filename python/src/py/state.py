"""
Module for holding globally accessible application state.

This module defines variables that are used across multiple modules,
such as the currently logged-in user.
"""

from models.user import User

# Represents the currently logged-in user.
user: User = None