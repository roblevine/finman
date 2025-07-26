#!/bin/sh
echo "starting installation script"

echo "update and install apt packages"
sudo apt update
sudo apt upgrade -y
sudo apt install -y vim iputils-ping dos2unix

echo "installation script complete"
