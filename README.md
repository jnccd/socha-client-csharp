# C# client for Software-Challenge Germany 2022/2023

[![build and test](https://github.com/jnccd/socha-client-csharp/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/jnccd/socha-client-csharp/actions/workflows/build-and-test.yml)
[![NuGet version (Socha)](https://img.shields.io/nuget/v/socha)](https://www.nuget.org/packages/socha/)
[![Discord](https://img.shields.io/discord/233577109363097601?color=blue&label=Discord)](https://discord.gg/ARZamDptG5)
[![Documentation](https://img.shields.io/badge/Software--Challenge%20-Documentation-%234299e1)](https://docs.software-challenge.de/)

This repository contains a random client for the game "Hey, Danke für den Fisch" written for the .NET platform.

Because it is a random client, it will only do seemingly random moves. If you wish to build your own client on top of this one you have two options. 

## Usage

You can either clone this repository and add your code or use the nuget package.

Both approaches require you to have the [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download) installed!

### Using the nuget package

If you use Visual Studio 2022 (without the "Code") you can create a new console project and add the Socha package through the GUI.

If you use another IDE, create a new folder you want to work in, then execute the following commands in the CLI:

```
dotnet new console
dotnet add package Socha
```

Then paste the following code into the Program.cs:

```
using SochaClient.Backend;

Starter.Main(args, new Logic());

public class Logic : SochaClient.Backend.Logic
{
    public Logic()
    {
        // TODO: Add init logic
    }

    public override Move GetMove()
    {
        // TODO: Add your game logic
        
        return GameState.GetAllPossibleMoves().First();
    }
}
```

Finally you can use the following command in the console to compile and execute the program:

```
dotnet run
```

### Using the repository directly

With Git installed you can [clone this repo](https://docs.github.com/en/repositories/creating-and-managing-repositories/cloning-a-repository) and add your code to the `Logic.cs` class in the `socha-logic` folder.

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