# Release Notes - Version [1.1.0]

## New Features

- MacroPolo now is built using `.NET 6` for windows.
- MacroPolo now uses the [CliFramework](https://github.com/yojoecapital/CliFramework).
- You can add multiline values for macros by using the code block syntax `${[text]}` where `[text]` can have multiple lines. Use the `codeblock` variable in the settings JSON file to edit the code block syntax.
- You can now add special macros:
  - Use `special` or `s` to get a listing of the special macros 
  - Use `add-ignore` or `a!` to add a macro that ignores its key's casing
  - Use `add-first` or `a@` to add a macro that will copy the casing of first character in the key typed by the user to the associated value (i.e `btw → between` and `Btw → Between`)
  - Use `add-plural` or `a$` to add a macro that is associated with 2 values (singular and plural)
    - The REPL will prompt you to enter in the plural version after the singular version is entered (press enter to just use the singular version with an `"s"` appended to it)
    - If the user enters in `"[key]"`, the singular version is used (i.e. `ft → foot`)
    - If the user enters in `"[key]s"` the plural version is used (i.e. `fts → feet`)
