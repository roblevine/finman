#!/bin/sh
echo "starting installation script"

echo "update and install apt packages"
sudo apt update
sudo apt upgrade -y
sudo apt install -y vim iputils-ping dos2unix

echo "install Claude Code"
npm install -g @anthropic-ai/claude-code

echo "configure Docker permissions for non-root user"
# Add vscode user to docker group if it exists
if getent group docker > /dev/null 2>&1; then
    sudo usermod -aG docker vscode
    echo "Added vscode user to docker group"
fi

echo "installation script complete"
