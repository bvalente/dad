#unix Makefile

SERVER_PORT=8080
CLIENT_PORT=8090
CLIENT_NAME=teste

#compile

lib:
	msbuild lib/lib.csproj

server:
	msbuild server/server.csproj

client:
	msbuild client/client.csproj

pcs:
	msbuild pcs/pcs.csproj

puppetMaster:
	msbuild puppetMaster/puppetMaster.csproj

all: lib server client pcs puppetMaster

#run

run\ server: server
	mono server/bin/Debug/net472/server.exe ${SERVER_PORT}

run\ client: client
	mono client/bin/Debug/net472/client.exe ${CLIENT_NAME} ${CLIENT_PORT}

run\ pcs: pcs
	mono pcs/bin/Debug/net472/pcs.exe

run\ puppetMaster: puppetMaster
	mono puppetMaster/bin/Debug/net472/puppetMaster.exe
