# MacroPolo

MacroPolo is a simple console application built with .NET that allows you to create and manage keyboard macros, perform text substitutions, and automate repetitive typing tasks. It provides an easy-to-use command-line interface for defining macros and executing them system-wide.

## Features

- Define custom macros: Create keyboard macros to replace specific keys with predefined values. Note that the keys can only contain alphabetical characters. These macros are saved to a local JSON file.
- Text substitutions: Automate text substitutions for frequently used symbols.
- System-wide monitoring: Capture keyboard input across all applications, ensuring macros work seamlessly in any context.
- Macro management: Add, update, and remove macros as needed to adapt to changing requirements.

## Getting Started

- Check the [releases](https://github.com/yojoecapital/MacroPolo/releases) tab to download
- Unzip the file and run `macro-polo.exe`

## Usage

The MacroPolo application provides the following commands:

- `add`: Add a new macro. Syntax: `add <key> <value>`. The `<key>` is the trigger phrase, and `<value>` is the replacement text. The `<key>` must have only  alphabetical characters.
- `remove`: Remove a macro. Syntax: `remove <key>`.
- `macros`: List all existing macros.
- `open`: Copy the path to the macros JSON file to the clipboard. 
- `stop`: Stop the listener. (MacroPolo listens on startup)
- `start`: Start the lister.
- `clear`: Clear the screen.
- `clean`: Clean all buffers (there are 10 in total)
- `quit`: Exit the application.

## Building

1. Clone the repository: `git clone https://github.com/your-username/MacroPolo.git`
2. Build the application using the .NET CLI: `dotnet build`
3. Run the application: `dotnet run`