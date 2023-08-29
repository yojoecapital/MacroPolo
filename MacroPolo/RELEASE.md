# Release Notes - Version [1.0.2]

## New Features

- List all macros using `macros (m) [text]` command. The results are ordered by their Levenshtein Distance to `[text]`. 
  - Results are also displayed in a pages that can navigated by the arrow keys.
  - Press Enter to exit the macro display.
  - You can modify the number of macros displayed by setting the `macrosPerPage` parameter in the settings JSON.



## Key Features

- List all the active buffers (i.e. processes being logged) using the `buffers` command
- Add the names of processes you wish for MacroPolo to ignore to a blacklist.
  - To do this you must set `useOneBuffer` to `false` in `settings.json`.
  - Add the name of processes you wish for MacroPolo to ignore in the `blacklist` JSON list.
  - You can also use the command `blacklist-current` to blacklist the current running process such as the terminal running MacroPolo.
- List all the blacklisted processes using the `blacklist` command.
- Reload the settings JSON using `reload (r)` command.
- Open the settings JSON file using `open (o)` command.
- Access the macros JSON file using `open macros` command.
- Add a new macro using `add (a) [key] [value]` command. Key must only contain alphabetical characters.
- Remove an existing macro using `remove (rm) [key]` command.
- Clean the buffers using the `clean` command.
- Start listening for macros with the `start (+)` command.
- Stop listening for macros with the `stop (-)` command.
- Clear the console screen using `clear (cls)` command.
- Exit the program using `quit (q)` command.