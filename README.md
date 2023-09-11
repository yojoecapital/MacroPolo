# MacroPolo

- This is a program for Windows Systems that allows you to create and manage keyboard macros and perform text substitutions through and an easy-to-use command-line interface.
- To access it easier, place the path of the `MacroPolo` directory in user's `Path` Environment Variable.

## Usage

The MacroPolo application provides the following commands:

- `macros [text]` or `m [text]`: List all macros sorted by their similarity to `[text]`
- `special [text]` or `s [text]`: List all special macros sorted by their similarity to `[text]`
- `open` or `o`: Open the settings JSON file
- `open macros`: Open the macros JSON file
- `open special`: Open the special macros JSON file
- `reload` or `r`: Reload the settings JSON file
- `buffers`: List all the active buffers
- `blacklist-current`: Blacklist the current process's name
- `blacklist`: List all blacklisted processes
- `add [key] [value]` or `a [key] [value]`: Add a new macro (keys can only contain alphabetical characters)
  - Use `add-ignore` or `a!` to add a macro that ignores its key's casing
  - Use `add-first` or `a@` to add a macro that will copy the casing of first character in the key typed by the user to the associated value (i.e `btw → between` and `Btw → Between`)
  - Use `add-plural` or `a$` to add a macro that is associated with 2 values (singular and plural)
    - The REPL will prompt you to enter in the plural version after the singular version is entered (press enter to just use the singular version with an `"s"` appended to it)
    - If the user enters in `"[key]"`, the singular version is used (i.e. `ft → foot`)
    - If the user enters in `"[key]s"` the plural version is used (i.e. `fts → feet`)

- `remove [key]` or `rm [key]`: Remove an existing macro
- `clean`: Clean the buffer
- `start` or `+`: Start listening for macros
- `stop` or `-`: Stop listening for macros
- `clear` or `cls`: Clear the console screen
- `quit` or `q`: Exit the program

## Building

1. Clone the repository: `git clone https://github.com/yojoecapital/MacroPolo.git`
2. Clone the [CliFramework](https://github.com/yojoecapital/CliFramework) submodule: `git submodule update`
3. Restore the NuGet Packages using the NuGet CLI: `nuget restore`
4. Build the application using the .NET CLI: `dotnet msbuild`
5. Run the executable located in `MacroPolo/bin`

## Contact

For any inquiries or feedback, contact me at [yousefsuleiman10@gmail.com](mailto:yousefsuleiman10@gmail.com).