#!/bin/bash

# Define the paths
build_path="MacroPoloCore/bin/Release/net6.0-windows"
release_path="MacroPolo.zip"
release_tag="v1.1.0"
release_title="v1.1.0"
release_target="main"
release_notes="./RELEASE.md"

# Check if the release and tag exist and delete them
if gh release view "$release_tag" > /dev/null 2>&1; then
  echo "Deleting existing release: $release_tag"
  yes | gh release delete "$release_tag"
fi

if git rev-parse --verify --quiet "$release_tag" >/dev/null; then
    git tag -d "$release_tag"       # Delete the tag locally
    git push origin --delete "$release_tag"  # Delete the tag on the remote repository
    echo "Tag $release_tag deleted successfully."
fi

# Build the project with Release configuration
dotnet build --property:Configuration=Release

# Delete existing .zip files in the build directory
rm "$build_path"/*.zip

# Create a new .zip archive
7z a "$release_path" "$build_path"/*

# Create a new GitHub release
gh release create "$release_tag" "$release_path" -t "$release_title" --target "$release_target" -F "$build_path/$release_notes"
