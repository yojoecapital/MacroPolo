# Release Notes - Version [1.0.3]

## New Features

- MacroPolo now is built using `.NET 6` for windows.
- MacroPolo now uses the [CliFramework](https://github.com/yojoecapital/CliFramework).
- You can add multiline values for macros by using the code block syntax `${[value]}` where `[value]` can have multiple lines. Use the `codeblock` variable in the settings JSON file to edit the code block syntax.
- You can now add special macros:
  - Ignore case macros
  - Copy first letter's casing of key
  - Pluralize macro if the key ends in "s"
  - Use `special` to get a listing of the special macros 
