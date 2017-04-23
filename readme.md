# BackpackLogin by <a href="https://github.com/igeligel">igeligel</a>

<div align="center"><a href="https://www.nuget.org/packages/BackpackLogin"><img src="https://img.shields.io/nuget/v/BackpackLogin.svg?style=flat&label=nuget" alt="badge Donate" /></a> <a href="https://www.paypal.me/kevinpeters96/1"><img src="https://img.shields.io/badge/Donate-Paypal-003087.svg?style=flat" alt="badge Donate" /></a> <a href="https://steamcommunity.com/tradeoffer/new/?partner=68364320&token=CzTCv8JM"><img src="https://img.shields.io/badge/Donate-Steam-000000.svg?style=flat" alt="badge Donate" /></a> <a href="https://github.com/igeligel/BackpackLogin/blob/master/LICENSE.md"><img src="https://img.shields.io/badge/License-MIT-1da1f2.svg?style=flat" alt="badge License" /></a> </div>

<div align="center"><img src ="http://i.imgur.com/YJgmXUw.gif" /><img src ="http://i.imgur.com/ffxHpKo.gif" /></div>

## Description

> A [.net standard](https://blogs.msdn.microsoft.com/dotnet/2016/09/26/introducing-net-standard/) class library which reverse engineered the HTTP API of [Backpack.tf](http://backpack.tf/). It will enable you to login easily via giving your Steam credentials to the client and it will let you login.

## Dependencies

Since this library is a .net standard library you need to install .net core or .net framework > 4.6.1 to use this software.

| Dependency | Version |
| -- | -- |
| AngleSharp | 0.9.9 |
| Newtonsoft.Json | 10.0.2 |
| .net standard | 1.4 |


## Installation

To install just you need to have to install a .net version:

| | |
| -- | -- |
| .NET | >= 4.6.1 |
| .NET Standard | >= 1.4 |

You can install this package via nuget or locally. Try to reference it as package in your .csproj file or install it via:

```powershell
PM> Install-Package BackpackLogin
```

or search for ``BackpackLogin`` in your nuget feed.

## How To Use

This project just gives you one API endpoint which is usable.

First you need to create instance of  the ``BackpackLoginClient``.

```csharp
var backpackLoginClient = new BackpackLoginClient();
```

After this you can call a function called ``Login`` with the parameters:

- username
- password
- sharedSecret

After the function gets invoked the login process will begin and the function will return the uhash and Cookies which are needed for API requests.

## Examples

- [Official Console Example](https://github.com/igeligel/BackpackLogin/tree/master/src/BackpackLoginExample)


## Contributing

To contribute please lookup the following styleguides:

- Commits: [here](https://github.com/igeligel/contributing-template/blob/master/commits.md)
- C#: [here](https://github.com/igeligel/contributing-template/blob/master/code-style/csharp.md)

## Resources

### Motivation

Mainly i created this functionality for a friend who wanted to automate some trading at [Backpack.tf](http://backpack.tf/). Since i believe it is useful to someone else i decided to publish it.

### Architecture

The general Workflow is shown in this diagram:

![Workflow](http://svgur.com/i/1Lw.svg)

This is the basic structure of the OpenId Login via Steam to [Backpack.tf](http://backpack.tf/).

## Contact

<p align="center">
  <a href="https://discord.gg/HS57euF"><img src="https://img.shields.io/badge/Contact-Discord-7289da.svg" alt="Discord server of Kevin Peters"></a>
  <a href="https://twitter.com/kevinpeters_"><img src="https://img.shields.io/badge/Contact-Twitter-1da1f2.svg" alt="Twitter of Kevin Peters"></a>
  <a href="http://steamcommunity.com/profiles/76561198028630048"><img src="https://img.shields.io/badge/Contact-Steam-000000.svg" alt="Steam Profile of Kevin Peters"></a>
</p>


## License

*BackpackLogin* is realeased under the MIT License.

<h2>Contributors</h2>

<table><thead><tr><th align="center"><a href="https://github.com/igeligel"><img src="https://avatars2.githubusercontent.com/u/12736734?v=3" width="100px;" style="max-width:100%;"><br><sub>igeligel</sub></a><br><p>Contributions: 19</p></th></tbody></table>

## This readme is powered by vue-readme

Check out vue-readme [[Website](https://igeligel.github.io/vue-readme) | [GitHub](https://github.com/igeligel/vue-readme)]
