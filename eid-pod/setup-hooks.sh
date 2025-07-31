#!/bin/bash
set -e

HOOKS_DIR=".githooks"
GIT_HOOKS_DIR=".git/hooks"

# Check if we're in the root directory
if [ ! -d ".git" ]; then
  echo "Error: This script must be run from the root of the git repository."
  exit 1
fi

# Check if hooks directory exists
if [ ! -d "$HOOKS_DIR" ]; then
  echo "Error: '$HOOKS_DIR' directory not found. Cannot install hooks."
  exit 1
fi

echo "Installing git hooks from '$HOOKS_DIR'..."

for hook in pre-commit pre-push; do
  SRC="$HOOKS_DIR/$hook"
  DEST="$GIT_HOOKS_DIR/$hook"

  if [ ! -f "$SRC" ]; then
    echo "Warning: Hook '$SRC' not found. Skipping."
    continue
  fi

  # Define start and end tags for our managed section
  START_TAG="# >>> Managed by setup-hooks.sh"
  END_TAG="# <<< End managed section"

  # If the hook already exists, check if we already injected this section
  if [ -f "$DEST" ]; then
    if grep -q "$START_TAG" "$DEST"; then
      echo "Hook '$hook' already contains managed section. Skipping append."
    else
      echo "Appending managed section to existing '$hook'..."
      {
        echo ""
        echo "$START_TAG"
        cat "$SRC"
        echo "$END_TAG"
      } >> "$DEST"
      chmod +x "$DEST"
      echo "Appended to existing $hook"
    fi
  else
    echo "Installing new hook '$hook'..."
    {
      echo "#!/bin/bash"
      echo "$START_TAG"
      cat "$SRC"
      echo "$END_TAG"
    } > "$DEST"
    chmod +x "$DEST"
    echo "Installed $hook"
  fi
done

echo "Git hooks installed successfully."
