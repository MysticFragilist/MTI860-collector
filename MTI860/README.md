# MTI860-collector
An API Function app to collect info on ongoing VR research.

## Installation
Create a new function app and link your repo to auto-deploy to it.

1. In your local shell execute this:
```
$ dotnet user-secrets set ConnectionStrings:CosmosDbConnectionString "AccountEndpoint=<ACCOUNT_ENDPOINT>;AccountKey=<ACCOUNT_KEY>"
```