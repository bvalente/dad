
server:
	csc -out:Server/Server.exe -recurse:Server.cs -recurse:CommonTypes.cs

client:
	csc -out:Client/Client.exe -recurse:Client.cs -recurse:CommonTypes.cs

default: server client

run_server:
	./Server/Server.exe

run_client:
	./Client/Client.exe

run_client_ui:
	dotnet run -p ClientUI

#TODO add for loop to create multiple clients