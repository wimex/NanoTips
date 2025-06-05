# NanoTips

An automated FAQ builder using Postmark inbound email parsing 

# Getting started
The project requires .NET Core 9.x and MongoDB 8.x.
To get started:
- make sure that dotnet is available on the path
- start the required docker containers: enter the `Docker` directory and run `docker compose up`
- open the Mongo Express interface: `http://localhost:8081`
- create a database named `nanotips`

# Development
- to start the backend, launch the C# Web application in you development environment
- to start the frontend, enter the `Source/NanoTips.Client` directory and launch `npm install` then `npm run`