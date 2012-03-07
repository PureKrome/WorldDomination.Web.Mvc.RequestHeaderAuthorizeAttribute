using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace WorldDomination.Web.Mvc
{
    public class InMemoryCustomAuthorization : ICustomAuthorization
    {
        private readonly IDictionary<string, string> _tokensAndNames;

        public InMemoryCustomAuthorization(string token, string name)
            : this(new Dictionary<string, string> {{token, name}})
        {
        }

        public InMemoryCustomAuthorization(IDictionary<string, string> tokenAndNames)
        {
            if (tokenAndNames == null)
            {
                throw new ArgumentNullException("tokenAndNames");
            }

            _tokensAndNames = tokenAndNames;
        }

    #region ICustomAuthorization Members

    public string AuthenticationType
    {
        get { return "CustomInMemoryTokenAuthorization"; }
    }

    public bool TryAuthorize(string token, out IPrincipal principal)
    {
        principal = null;

        if (!_tokensAndNames.ContainsKey(token))
        {
            return false;
        }

        var identity = new GenericIdentity(_tokensAndNames[token], AuthenticationType);
        principal = new GenericPrincipal(identity, null);

        return true;
    }

    #endregion
    }
}