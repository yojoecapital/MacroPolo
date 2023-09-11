#!/bin/bash

# Define the paths
BUILD_PATH="MacroPoloCore/bin/Release/net6.0-windows"
RELEASE_PATH="MacroPolo.zip"
RELEASE_TAG="v1.1.0"
RELEASE_TITLE="$RELEASE_TAG"
RELEASE_TARGET="main"
RELEASE_NOTES="./RELEASE.md"

# Check if the release and tag exist and delete them
if gh release view "$RELEASE_TAG" > /dev/null 2>&1; then
  echo "Deleting existing release: $RELEASE_TAG"
  yes | gh release delete "$RELEASE_TAG"
fi

if git rev-parse --verify --quiet "$RELEASE_TAG" >/dev/null; then
    git tag -d "$RELEASE_TAG"       # Delete the tag locally
    git push origin --delete "$RELEASE_TAG"  # Delete the tag on the remote repository
    echo "Tag $RELEASE_TAG deleted successfully."
fi

# Build the project with Release configuration
dotnet build --property:Configuration=Release

# Enter directory
cd "$BUILD_PATH"

# Create a new .zip archive
7z a "$RELEASE_PATH" *

# Delete existing .zip file
rm "$RELEASE_PATH"/*.zip

# Create a new GitHub release
gh release create "$RELEASE_TAG" "$RELEASE_PATH" -t "$RELEASE_TITLE" --target "$RELEASE_TARGET" -F "$BUILD_PATH/$RELEASE_NOTES"
