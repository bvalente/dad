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
| PCS | 8070 |
| PuppetMaster | 8075 |
| Servers | 8090-8099 |
| Client | 8080-8089 |

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