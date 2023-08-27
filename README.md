# MacroPolo

- This is a program for Windows Systems that allows you to create and manage keyboard macros and perform text substitutions through and an easy-to-use command-line interface
- To access it easier, place the path of the `MacroPolo` directory in user's `Path` Environment Variable.

## Usage

The MacroPolo application provides the following commands:

- `macros` or `m`: List all macros
- `open` `o`: Open the settings JSON file
- `open macros`: Open the macros JSON file
- `add [key] [value]` or `a [key] [value]`: Add a new macro (keys can only contain alphabetical characters)
- `remove [key]` or `rm [key]`: Remove an existing macro
- `clean` or `c`: Clean the buffer
- `start` or `+`: Start listening for macros
- `stop` or `-`: Stop listening for macros
- `clear` or `cls`: Clear the console screen
- `quit` or `q`: Exit the program

## Building

1. Clone the repository: `git clone https://github.com/yojoecapital/MacroPolo.git`
2. Restore the NuGet Packages using the NuGet CLI: `nuget restore`
3. Build the application using the .NET CLI: `dotnet msbuild`
4. Run the executable located in `MacroPolo/bin`

### Releasing

```
dotnet msbuild --property:Configuration=Release && cd MacroPolo/bin/Release && 7z a MacroPolo.zip * && gh release create v1.0.0 ./MacroPolo.zip -t --target main "v1.0.0" -F ./RELEASE.md && cd ../../..
```

## Contact

For any inquiries or feedback, contact me at [yousefsuleiman10@gmail.com](mailto:yousefsuleiman10@gmail.com).