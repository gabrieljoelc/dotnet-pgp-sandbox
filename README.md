# PgpCore Sandbox

This is an attempt to use the PgpCore library to encrypt/decrypt a file at a specified path.

## Getting started

Prerequesites:
- .NET Core 3.1 SDK
- .NET CLI

1. Create `.env` file in root
2. Add these environment variables to the `.env` file:
```
SecurityKeySource__PrivateKeySource=<path to private key file relative to root>
SecurityKeySource__PublicKeySource=<path to public key file relative to root>
SecurityKeySource__PassPhrase=<your passphrase>
FilePathes__Input=<file to encrypt/decrypt>
FilePathes__Input=<path of encrypted/decrypted file to read relative to root of solution>
FilePathes__Output=<path of written encrypted/decrypted file relative to root of solution>
```
3. 
4. While in the root of the solution, run these commands to start the application:
```
# to encrypt a file
env -S "`cat .env`" dotnet run -- encrypt
# to decrypt a file
env -S "`cat .env`" dotnet run -- decrypt
```
