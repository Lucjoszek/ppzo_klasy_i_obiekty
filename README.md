# Zadanie - Tworzenie klas i obiektów
Zadanie wykonane w ramach przedmiotu "Podstawy programowania zorientowane obiektowo".
Wybrałem zadanie nr. 12: System playlist muzycznych - klasy `Track`, `Playlist`, `User`; tworzenie, modyfikacja i odtwarzanie list utworów.

## Uwaga!
Aby uruchomić aplikację w Pythonie należy uruchomić plik `app.py`, który znajduje się w `.\src\py\app.py`.

## Struktura aplikacji
Aplikacja zawiera **trzy główne strony**:
- **Home**
  - Domyślna strona startowa.
  - Wyświetla ostatnio odtwarzane playlisty.
- **Playlists**
  - Wyświetla listę dostępnych playlist.
  - W prawym górnym rogu znajduje się przycisk do tworzenia playlisty – należy podać nazwę oraz wybrać ścieżkę do folderu z plikami audio.
- **Selected Playlist**
  - Wyświetla utwory danej playlisty.
  - Umożliwia zmianę kolejności utworów.
  - W prawym górnym rogu znajduje się przycisk do zmiany nazwy playlisty.

Na stronach **Home** i **Playlists** dostępne są:
- Przyciski do odtwarzania playlist.
- Przyciski do przejścia na stronę wybranej playlisty.

## 💻 Technologie

### 🧩 C#
- .NET 9 (9.0.300)
Aplikację wykonałem z użyciem natywnych komponentów UI z [WinUI 3](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/).

### Python
- Python 3.13.3
Aplikację wykonałem z użyciem [Eel](https://github.com/python-eel/Eel), który pozwala na komunikację między Pythonem a JavaScriptem.
Na backendzie znajduje się Python, a na frontendzie HTML, CSS i JavaScript.
Do odtwarzania muzyki użyłem biblioteki [just_playback](https://github.com/cheofusi/just_playback).
