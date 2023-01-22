# Electron project for ECUSimGUI
This directory contains sources to build ECUSimGUI app as desktop app, by using electron.

# Before running this.
Before running this, please build backend asp.net binary (root of this project), and copy binarise to `server-bin`.

```
cd ECUSimGUI
dotnet publish
cd electron
mkdir server-bin
cd server-bin
cp -r ../bin/Debug/net6.0/publish/* ./
```
