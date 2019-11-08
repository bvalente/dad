# Design and Implementation of Distributed Applications

## Introduction

This project consists on 5 programs

- PCS (Process Creation Service) - process that creates servers and clients
- lib - dll library with data structure
- Puppet Master - master process that manages all the servers and clients
- Server - program that manages and distributes the data to other servers and clients
- Client - user interface that can create data structures and send them to the servers

## IP Ports

| Program | Ports |
| ------- | ----: |
| PCS | 10000 |
| PuppetMaster | 10001 |
| Servers | user defined |
| Client | user defined |

## Install Avalonia

Our project uses the Avalonia UI Framework because it is suported in Windows, MacOS and Linux.

To Install Avalonia either download the Visual Studio extension [here](https://marketplace.visualstudio.com/items?itemName=AvaloniaTeam.AvaloniaforVisualStudio) or follow the instructions [here](https://github.com/AvaloniaUI/avalonia-dotnet-templates#installing-the-templates) to install it through the _dotnet_ tool.

## Compile and Run

*Remeber to compile dependencies if needed*

- MacOs and Linux - [Makefile](msdad/Makefile)
	- Use the Makefile on the *msdad* directory
	- make [project] - compile project
	- make all - compile all projects
	- make "run [project]" - run project
- Windows
	- change to desired directory and use the dotnet tool
	- dotnet build - compiles project
	- dotnet run - compiles and executes project


## Reminders

- https://docs.microsoft.com/en-us/dotnet/core/porting/net-framework-tech-unavailable
- https://github.com/AvaloniaUI/Avalonia/issues/2504