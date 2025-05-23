"""
Simple script to open a folder selection dialog and print the selected folder path.
Used for selecting music directories via GUI in other parts of the application.
"""

import tkinter as tk
from tkinter import filedialog

root = tk.Tk()
root.withdraw()
root.attributes("-topmost", True)
folder = filedialog.askdirectory()
print(folder)