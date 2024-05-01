# BattleshipGame

BattleshipGame is a console-based Battleship game implemented in C#.

## Overview

This project provides a simple implementation of the classic Battleship game that you can play directly in the console. It's a fun and interactive way to enjoy the game without needing any graphical user interface.

## Building the Game

To build the game for your desired platform, follow these steps:

1. **Build the game**: Run the following command to build the game in Release configuration:

`dotnet build --configuration Release`

2. **Publish the game**: After building the game, use the `dotnet publish` command to publish it:

`dotnet publish -c Release -r [runtime_identifier]`


Replace `[runtime_identifier]` with the appropriate runtime identifier for your target operating system. For example:
- `win-x64` for 64-bit Windows
- `linux-x64` for 64-bit Linux
- `osx-x64` for macOS

## How to Play

Once you've built or published the game, simply run the executable to start playing Battleship! Follow the on-screen instructions to place your ships and guess your opponent's ship locations.
