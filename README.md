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

You can think of your API key as another user in your Onspring instance and therefore it is subject to all the same permission considerations as any other user when it comes to it's ability to access a file and download it. The API key you use with this tool needs to have all the correct permissions within your instance to access the record where a file is held and the field where the file is held. Things to think about in this context are role permissions, content security, and field security.

## API Usage Considerations

This tool uses version 2 of the Onspring API to retrieve the attachments you want to download. Currently this version of the Onspring API does not provide any endpoints to perform bulk operations for retrieving attachments or the attachments information.

Therefore two API requests must be made for each attachment you want to retrieve from your instance - one request to get the attachment's information and another to get the attachments actual content. There are also additional requests made, the number of which depends on the source you use to identify the records from which attachments should be downloaded, to locate all the proper attachment ids.

But as an example if you were to use this tool to download 100 attachments from your instance the tool would make at least 200 requests against the Onspring API to accomplish the task.

That all being said it's important you take into consideration and have an idea about the number of attachments you are going to retrieve and how that will translate into a number of requests against the Onspring API. If the quantity is quite considerable I'd encourage you to consult with your Onspring representative to understand what if any limits there are to your usage of the Onspring API.

## Installation

The tool is published as a release where you can download it as a single executable file for the following operating systems:

+ win-x64
+ linux-x64
+ osx-x64 (Minimum OS version is macOS 10.12 Sierra)

You are also welcome to clone this repository and run the tool using the .NET 6.0 tooling and runtime. As well as modify the tool further for your specific needs.

## General Usage

When starting the tool you will always be prompted for the following:

+ `Api Key` - This is the api key that has all the proper permsissions to download the files from your instance.
+ `App Id` - This is the id of the app where your files are held. Note this can be retrieved via...
  + the [Onspring API's](https://api.onspring.com/swagger/index.html) [/Apps](/Apps) endpoint
  + By looking at the url of the app in your browser.
  ![app-id-url-example.png](/READMEimages/app-id-url-example.png)

+ `File Field Ids` - These are the ids for the fields where the attachments you want to download are held. This should be a list of ids delimited by commas. These ids can be retrieved via... 
  + The [Onspring API's](https://api.onspring.com/swagger/index.html) /[Fields/appId/{appId}](Fields/appId/{appId}) endpoint
  + An app's Fields & Objects report
  ![field-and-objects-report-example.png](/READMEimages/field-and-objects-report-example.png)
  + By opening the fields configurations within Onspring.
  ![field-id-example.png](/READMEimages/field-id-example.png)
+ `Source` - This tells the tool how you plan to identify which records you want to get attachments from. There are currently three valid options: App, Report, or Records.
  + `App` - If you choose the app source option the tool will retrieve all files from all records in the app from the fields whose ids you entered.
  + `Report` - If you choose the report source option the tool will prompt you to provide the id for the report which identifies all the records from which you want to download attachments from the fields whose ids you entered.
  + `Records` - If you choose the records source option the tool will prompt you to provide the ids for the records from which you want to download attachments from the fields whose ids you entered.

### Getting Attachments for Specific Records

### Getting Attachments for Records in a Report

### Getting Attachments for Records in an App

## Log
