# NanoTips

An automated FAQ builder using Postmark inbound email parsing 

# Getting started
The project requires .NET Core 9.x and MongDB 8.x.
To get started:
- make sure that dotnet is available on the path
- start the required docker containers: enter the `Docker` directory and run `docker compose up`
- open the Mongo Express interface: `http://localhost:8081`
- create a new collection called `nanotips`