#unix Makefile

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
	mono server/bin/Debug/net472/server.exe

run\ client: client
	mono client/bin/Debug/net472/client.exe

run\ pcs: pcs
	mono pcs/bin/Debug/net472/pcs.exe

run\ puppetMaster: puppetMaster
	mono puppetMaster/bin/Debug/net472/puppetMaster.exe
