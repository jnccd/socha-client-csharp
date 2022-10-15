# C# client for Software-Challenge Germany 2022/2023

[![build and test](https://github.com/jnccd/socha-client-csharp/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/jnccd/socha-client-csharp/actions/workflows/build-and-test.yml)
[![Discord](https://img.shields.io/discord/233577109363097601?color=blue&label=Discord)](https://discord.gg/ARZamDptG5)
[![Documentation](https://img.shields.io/badge/Software--Challenge%20-Documentation-%234299e1)](https://docs.software-challenge.de/)
> Please note that this is a very early version, which may still contain some bugs. However, the client is able to play
> a game from start to end.

This repository contains a simple client for the game "Hey, Danke für den Fisch" written for the .NET Core platform.

Because it is a simple client, it will only do seemingly random moves. If you wish to build your own client on top of this one you can [clone this repo](https://docs.github.com/en/repositories/creating-and-managing-repositories/cloning-a-repository) and add your code to the `Logic.cs` class in the `socha-client-csharp` folder.

## Usage

This project can either be used with Visual Studio 2022 on Windows by opening the .sln file or with another IDE such as Visual Studio Code on any operating system.

To run this project in the latter case change into the `socha-client-csharp` folder and use the `run` command of .NET.

```dotnet run```

If you wish to pass program arguments to the `dotnet ` execution you can use `--` like so:

```dotnet run -- --help```

## Deployment on the socha contest server

Make sure that you only include standard .NET libraries.

Put all files and folders in this folder in a `.zip` archive including the `start.sh`.
Then upload that `.zip` on the contest site.

If you want to make modifications and edit the `start.sh` file, make sure to save it using linux line endings.
You can either edit it on a Linux system or use Notepad++/wsl on Windows.