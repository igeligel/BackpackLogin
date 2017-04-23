namespace HedgehogSoft.BackpackLogin.Interfaces
{
    internal interface IOpenIdParameters
    {
        string Action { get; set; }
        string OpenIdMode { get; set; }
        string OpenIdParams { get; set; }
        string Nonce { get; set; }
    }
}
