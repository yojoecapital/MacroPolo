# Release Notes - Version [1.0.1]

## New Features

- Add the names of processes you wish for MacroPolo to ignore to a blacklist.
  - To do this you must set `useOneBuffer` to `false` in `settings.json`.
  - Add the name of processes you wish for MacroPolo to ignore in the `blacklist` JSON list.
  - You can also use the command `blacklist-current (bc)` to blacklist the current running process such as the terminal running MacroPolo.

- Reload the settings JSON using `reload (r)` command.


## Key Features

- List all macros using `macros (m)` command.
- Open the settings JSON file using `open (o)` command.
- Access the macros JSON file using `open macros` command.
- Add a new macro using `add (a) [key] [value]` command. Key must only contain alphabetical characters.
- Remove an existing macro using `remove (rm) [key]` command.
- Clean the buffer using the `clean (c)` command.
- Start listening for macros with the `start (+)` command.
- Stop listening for macros with the `stop (-)` command.
- Clear the console screen using `clear (cls)` command.
- Exit the program using `quit (q)` command.