# Prototype Genie Website Generator

A rudementary implemenation that allows to generate static webpages from url requests. The results are served via azure blob storage.

## Get started locally

Add the following connection strings to appsettings.json

 "AzureBlobStorage": {
   "ConnectionString": "..."
 },
 "AzureCosmosDB": {
   "ConnectionString": "..."
 }


## TODO

- Come up with the payload schema
- Integrate Azure AI .NET SDK
- Add authentication 
