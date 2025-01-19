using System.Security.Claims;

namespace GoldenRaspberryAwards.Core.Interfaces;

public interface IAspNetUser
{
    string Name { get; }
    Guid GetUserId();
    string GetUserName();
    string GetUserEmail();
    bool IsAuthenticated();
    bool IsInRole(string role);
    IEnumerable<Claim> GetClaimsIdentity();
}
