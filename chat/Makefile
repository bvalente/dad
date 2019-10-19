
commonTypes:
	csc -target:library -out:CommonTypes/CommonTypes.dll -recurse:CommonTypes.cs

server:
	csc -out:Server/Server.exe -recurse:Server.cs -recurse:CommonTypes.cs

client:
	csc -out:Client/Client.exe -recurse:Client.cs -recurse:CommonTypes.cs

client_ui:
	msbuild ClientUI/ClientUI.csproj

all: commonTypes server client client_ui

run_server:
	./Server/Server.exe

run_client:
	./Client/Client.exe

run_client_ui:
	#dotnet run -p ClientUI
	mono ClientUI/bin/Debug/net472/ClientUI.exe

#TODO add for loop to create multiple clients
