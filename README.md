# lifecheck-client - Windows verification service

## Overview

Lifecheck is a set of applications that work together to regularly confirm that a person has 'checked-in', thus indicating that the person is still alive. It has been developed to ensure that friends and family are notified if a certain time period has elapsed without a successful verification. The original intention of Lifecheck was to provide a safety net to ensure that household pets receive the care they need from friends and family if their owners should suddenly pass away.

This repository works in conjunction with the other Lifecheck repository:
* `lifecheck-aws`: Server-side functionality hosted on AWS that performs verification checks and sends notifications.

This `lifecheck-client` project contains .NET C# code that will send a POST request to the AWS verification endpoint when Windows starts. This will allow automatic verification for people that restart their Windows operating system on a daily basis.

## Installation and usage

1. Follow the instructions in the `lifecheck-aws` repository to setup the required AWS functionality.

## Prerequisites

* **Git:** Ensure Git is installed on your Windows machine.
* **Visual Studio Code:** Ensure Visual Studio Code, or another IDE / text editor, is installed.
* **.NET SDK:** Install the .NET SDK via the following link: https://dotnet.microsoft.com/en-us/download/dotnet/9.0

## Setup and deployment

1. **Clone the repository:**
   * Open Git Bash.
   * Navigate to the directory where you want to store the project.
   * Clone the repository: `git clone git@github.com:andrewmelvin-dev/lifecheck-client.git`

2. **Navigate to the project directory:**
   * `cd lifecheck-client`

3. **Update the configuration:**
   * In Visual Studio Code edit the Worker.cs file and change the following:
      * Set the `_url` property to the value of the `LifecheckVerificationUrl` parameter that was output from the deployment of the `lifecheck-aws` project.
      * Set the `_apiKey` property to the value of the `LifecheckApiKey` parameter that was output from the deployment of the `lifecheck-aws` project.

4. **Build the service:**
   * Open PowerShell in your project directory.
   * Run the following command to build the service: `dotnet build -c Release`
      * Set the `_url` property to the value of the `LifecheckVerificationUrl` parameter that was output from the deployment of the `lifecheck-aws` project.
      * Set the `_apiKey` property to the value of the `LifecheckApiKey` parameter that was output from the deployment of the `lifecheck-aws` project.

5. **Install the service:**
   * Open PowerShell in your project directory.
   * Run the following command to install the service: `New-Service -Name "LifecheckClient" -Binary ".\bin\Release\netcoreapp3.1\win-x64\publish\LifecheckClient.exe"`

6. **Run the service:**
   * Open PowerShell.
   * Run the following command to start the service: `Set-Service -Name "LifecheckClient" -StartupType Automatic`

## File overview ##

### Worker.cs ###

This is the class that performs a POST request to the AWS verification endpoint when the service starts.
