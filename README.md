# MacroPolo

- This is a program for Windows Systems that allows you to create and manage keyboard macros and perform text substitutions through and an easy-to-use command-line interface.
- To access it easier, place the path of the `MacroPolo` directory in user's `Path` Environment Variable.

## Usage

The MacroPolo application provides the following commands:

- `macros [text]` or `m [text]`: List all macros sorted by their similarity to `[text]`
- `open` `o`: Open the settings JSON file
- `open macros`: Open the macros JSON file
- `reload` or `r`: Reload the settings JSON file
- `buffers`: List all the active buffers
- `blacklist-current`: Blacklist the current process's name
- `blacklist`: List all blacklisted processes
- `add [key] [value]` or `a [key] [value]`: Add a new macro (keys can only contain alphabetical characters)
- `remove [key]` or `rm [key]`: Remove an existing macro
- `clean`: Clean the buffer
- `start` or `+`: Start listening for macros
- `stop` or `-`: Stop listening for macros
- `clear` or `cls`: Clear the console screen
- `quit` or `q`: Exit the program

## Building

1. Clone the repository: `git clone https://github.com/yojoecapital/MacroPolo.git`
2. Clone the [CliFramework](https://github.com/yojoecapital/CliFramework) submodule: `cd CliFramework && git git submodule init && git submodule update`
3. Restore the NuGet Packages using the NuGet CLI: `nuget restore`
4. Build the application using the .NET CLI: `dotnet msbuild`
5. Run the executable located in `MacroPolo/bin`

### Releasing

```
dotnet build --property:Configuration=Release && cd MacroPoloCore/bin/Release/net6.0-windows && del ./MacroPolo.zip && 7z a MacroPolo.zip * && gh release create v1.0.3 ./MacroPolo.zip -t "v1.0.3" --target main -F ./RELEASE.md && cd ../../../..
```

## Contact

For any inquiries or feedback, contact me at [yousefsuleiman10@gmail.com](mailto:yousefsuleiman10@gmail.com).