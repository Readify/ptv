.NET Portable Class Library for Public Transport Victoria API (Ptv.dll)
=======================================================================

Ptv is a Portable Class Library which provides .NET-based wrapper around the Public Transport Victoria APIs that have been published at http://data.vic.gov.au.

Usage
-----

Documentation is pretty light on at the moment (we are in Alpha), but if you want to get started quickly just use NuGet to pull down the package with the following command:

```PowerShell
Install-Package Ptv
```

Then in your code add the following:

```C#
var developerID = "12345";
var securityKey = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";

var client = new TimetableClient(
    developerID,
    securityKey,
    (input, key) =>
    {
		// Unfortunately the APIs exposed to .NET PCLs does not include an implementation
		// of the HMACSHA1 algorithm which the PTV API requires to generate signatures, so
		// rather than take a dependency on another library, for now the API defines a
		// delegate (TimetableClientHasher) which takes the key, and a sequence of bytes
		// to be hashed which can then be passed into the underlying platforms APIs.
        var provider = new HMACSHA1(key);
        var hash = provider.ComputeHash(input);
        return hash;
    });

var results = await client.SearchAsync("South Melbourne");
```