---
services: active-directory
platforms: dotnet
author: onecode
---

# How to authenticate AAD once for multiple resources

In some scenarios, our AAD application needs to involve both AAD Graph API and Azure Management API (and other AAD resources).

As we all know, when AAD authentication process is running, requests need to contain "resource" parameter to indicate the authentication target.

Due to AAD limitation, different AAD resources need different authentication tokens. In other words, clients need to authenticate several times for multiple resources. Unfortunately, clients could lack in experiences in handling multiple authentication processes.

This sample demonstrates how to let clients authenticate only once for multiple resources.

## How to Run This Sample

To run this sample, you will need:
- Visual Studio 2013
- An Internet connection
- An Azure Active Directory (Azure AD) tenant. For more information about how to get an Azure AD tenant, please see [How to get an Azure AD tenant](https://azure.microsoft.com/en-us/documentation/articles/active-directory-howto-tenant/)
- A user account in your Azure AD tenant. This sample will not work with a Microsoft account, so if you signed in to the Azure portal with a Microsoft account and have never created a user account in your directory before, you will have to do that now.

### Step 1:  Clone or download this repository

From your shell or command line:

`git clone https://github.com/Azure-Samples/active-directory-dotnet-client-authentication-once-multiple-resource`

### Step 2:  Register the sample with your Azure Active Directory tenant

There is one project in this sample that needs to be registered separately in your Azure AD tenant.

1. Sign in to the [Azure portal](https://portal.azure.com).
2. On the top bar, click on your account. Under the **Directory** list, choose the Active Directory tenant wihch you wish to register your application.
3. Click on **More Services** in the left-hand navigator, and choose **Azure Active Directory**.
4. Click on **App registrations** and choose **Add**.
5. Enter a friendly name for the application, for example 'testnativeapplication' and select 'Native Application' as the Application Type. For the sign-on URL, enter the base URL for the sample, which is by default `http://myapplication.com`. For the App ID URI, enter https://[your_tenant_name]/testnativeapplication by replacing [your_tenant_name] with your Azure AD tenant name. Click **OK** to complete the registration. Click on **Create** to create the application.
6. While still in the Azure portal, choose your application, click on **Settings** and choose **Configure**.
7. Find the Application ID value and copy it to the clipboard.
8. Scroll to the bottom, in "Permissions to other applications", add "Microsoft Graph" and "Windows Azure Service Management API" applications.
9. Check all "Delegated Permissions" and save settings.

### Step 3:  Configure the sample to use your Azure AD tenant

1. Open the solution in Visual Studio 2013.
2. Open the `Program.cs` file.
3. Modify `SubscriptionTenant`, `NativeClientId` and `subscriptionId` variables for your AAD settings.

### Step 4:  Run the sample

In this sample, method `GetGraphAccessResult` needs client authentication process to input username and password.

Method `GetManagementAccessResult` doesn't need client authentication process. It will be automatically finished by refreshing the token which can be generated from method `GetGraphAccessResult`.

Method `InvokeManagementApi` uses the authentication token generated from method `GetManagementAccessResult` to invoke sample Azure management API.

Press F5 to run this sample. Check authentication token and Azure management API result.
