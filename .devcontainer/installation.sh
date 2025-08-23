#!/bin/sh
echo "starting installation script"

echo "update and install apt packages"
sudo apt update
sudo apt upgrade -y
sudo apt install -y vim iputils-ping dos2unix curl podman-compose

echo "creating Docker network: finman-network"
# Create the Docker network if it doesn't exist
docker network ls | grep -q finman-network || docker network create finman-network
# Connect the dev container to the network
docker network connect finman-network finman_devcontainer 2>/dev/null || true

echo "install Claude Code"
npm install -g @anthropic-ai/claude-code

#echo "install Cursor CLI"
#curl https://cursor.com/install -fsS | bash

echo "configure Docker permissions for non-root user"
# Add vscode user to docker group if it exists
if getent group docker > /dev/null 2>&1; then
    sudo usermod -aG docker vscode
    echo "Added vscode user to docker group"
fi

echo "to check that docker DooD is configured correctly, run

. .devcontainer/docker-test.sh
"

echo "installation script complete"
