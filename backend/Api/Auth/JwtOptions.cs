namespace Api.Auth;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = "";
    public string Issuer { get; set; } = "LynxGym.Api";
    public string Audience { get; set; } = "LynxGym.App";
    public int ExpirationHours { get; set; } = 24;
}
