#!/bin/bash
set -ev
echo "Testing Build Process"

sudo apt-get install unzip

wget -O docfxzip.zip https://github.com/dotnet/docfx/releases/download/v2.41/docfx.zip
unzip docfxzip.zip -d dfxd

mono dfxd/docfx.exe docfx.json

shopt -s extglob 
rm -rf !(temp)
cd temp
mv * ../
cd ..
rm -rf temp
ls