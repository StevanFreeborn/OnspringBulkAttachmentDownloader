# Onspring Bulk Attachment Downloader

A tool built for helping Onspring customers download multiple attachments at once from one or more attachment/image fields from one or more records based on record ids provided from individual ids, a report, or an app.

**Note:**
This is an unofficial Onspring tool. It was not built in consultation with Onspring Technologies LLC or a member of their development team. This tool was developed indepdently using Onspring's existing [API .NET SDK](https://github.com/onspring-technologies/onspring-api-sdk).

## API Key

This tool makes use of version 2 of the Onspring API. Therefore you will need an API key to be able to utilize the tool. API keys may be obtained by an Onspring user with permissions to atleast read API Keys for your instance, using the following instructions:

1. Within Onspring, navigate to [/Admin/Security/ApiKey](/Admin/Security/ApiKey).
2. On the list page, add a new API Key (requires Create permissions) or click an existing API Key to view its details.
3. On the details page for an API Key, click on the Developer Information tab. 
4. Copy the X-ApiKey Header value from this section.

**Important:**

+ An API key must have a status of Enabled in order to be used.
+ Each API key must have an assigned Role. This role controls the permissions for requests that are made by this tool to retrieve files from fields on records in an app. If the API key does not have sufficient permissions the attachment will not be downloaded.

## Permission Considerations

## API Usage Considerations

## Installation
