using System.Security.Principal;

namespace WorldDomination.Web.Mvc
{
    public interface ICustomAuthorization
    {
        string AuthenticationType { get; }
        bool TryAuthorize(string token, out IPrincipal principal);
    }
}