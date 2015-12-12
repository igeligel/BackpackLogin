# BackpackLogin
A Library to handle the login to http://backpack.tf/ in C#.

## How to use
Get the classes and implement the login into your project. In program.cs is a example to use it.
Otherwise here is the code:
```c#
// Create instance of the login.
var backpackLogin = new Login();
// Login with two factor code.
backpackLogin.DoLogin("steamusername", "steampasswort", "twofactorcode");
// Login without two factor code.
backpackLogin.DoLogin("steamusername", "steampassword");
```

## How to contribute
Fork the repository and create a pull request with your changes.
Be sure you are using the C# Conventions. You can find them [here](https://msdn.microsoft.com/en-us/library/ff926074.aspx). 
Otherwise your pull requests will not be accepted.

## Questions
Write an issue or contact me in Steam. I am also open to improvements.

## Donation
If you think my work is awesome and you want to donate, just add me:

![http://steamcommunity.com/profiles/76561198028630048/](http://i.imgur.com/BmdPAEJ.png) [Profile](http://steamcommunity.com/profiles/76561198028630048/)
