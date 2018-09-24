# AdobeUMAPI.FSharp
This sample client should get you started with accessing the Adobe User Management API in .NET / F#

To some degree it is based on https://github.com/adobe-apiplatform/umapi-documentation/blob/master/samples/JWTExchange.py

## Setup
Please follow [this guide](https://www.adobe.io/apis/cloudplatform/console/authentication/jwt_workflow.html) to setup your api credentials. Be sure to include a valid yaml config file based on the included sample in your local environment.

If you have trouble with certificates (Invalid algorithm exceptions) in .NET, have a [look at this](https://hintdesk.com/2011/07/29/c-how-to-fix-invalid-algorithm-specified-when-signing-with-sha256/).
