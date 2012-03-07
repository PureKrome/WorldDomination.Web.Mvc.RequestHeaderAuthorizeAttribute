using System.Collections.Specialized;
using System.Web.Mvc;
using Moq;
using WorldDomination.Web.Mvc;
using Xunit;

namespace WorldDomination.Tests
{
    public class RequestHeaderAuthorizeAttributeFacts
    {
        [Fact]
        public void GivenNoTokenHeader_OnAuthorize_ReturnsAnHttpUnauthorizedResult()
        {
            // Arrange.
            var tokenAuthorizeAttribute = new RequestHeaderAuthorizeAttribute
                                          {
                                              Authorization = new InMemoryCustomAuthorization("aaa", "Pew Pew")
                                          };
            AuthorizationContext filterContext =
                new Mock<AuthorizationContext> {DefaultValue = DefaultValue.Mock}.Object;

            // Act.
            tokenAuthorizeAttribute.OnAuthorization(filterContext);

            // Assert.
            Assert.NotNull(filterContext);
            var result = filterContext.Result as HttpUnauthorizedResult;
            Assert.NotNull(result);
            Assert.Equal("The provided token was not authorized.", result.StatusDescription);
        }

        [Fact]
        public void GivenAnInvalidToken_OnAuthorize_ReturnsAnHttpUnauthorizedResult()
        {
            // Arrange.
            var tokenAuthorizeAttribute = new RequestHeaderAuthorizeAttribute
                                          {
                                              Authorization = new InMemoryCustomAuthorization("aaa", "Pew Pew")
                                          };

            var mockfilterContext = new Mock<AuthorizationContext> {DefaultValue = DefaultValue.Mock};
            mockfilterContext.Setup(x => x.HttpContext.Request.Headers)
                .Returns(new NameValueCollection
                         {
                             {tokenAuthorizeAttribute.Header, "omg"}
                         });

            AuthorizationContext filterContext = mockfilterContext.Object;

            // Act.
            tokenAuthorizeAttribute.OnAuthorization(filterContext);

            // Assert.
            Assert.NotNull(filterContext);
            var result = filterContext.Result as HttpUnauthorizedResult;
            Assert.NotNull(result);
            Assert.Equal("The provided token was not authorized.", result.StatusDescription);
        }

        [Fact]
        public void GivenAnValidToken_OnAuthorize_UserIPrincipalIsSet()
        {
            // Arrange.
            var tokenAuthorizeAttribute = new RequestHeaderAuthorizeAttribute
                                          {
                                              Authorization = new InMemoryCustomAuthorization("aaa", "Pew Pew")
                                          };

            var mockfilterContext = new Mock<AuthorizationContext> {DefaultValue = DefaultValue.Mock};
            mockfilterContext.Setup(x => x.HttpContext.Request.Headers)
                .Returns(new NameValueCollection
                         {
                             {tokenAuthorizeAttribute.Header, "aaa"}
                         });

            AuthorizationContext filterContext = mockfilterContext.Object;

            // Act.
            tokenAuthorizeAttribute.OnAuthorization(filterContext);

            // Assert.
            Assert.NotNull(filterContext);
            Assert.NotNull(filterContext.HttpContext.User);
            Assert.NotNull(filterContext.HttpContext.User.Identity);
            Assert.True(filterContext.HttpContext.User.Identity.IsAuthenticated);
            Assert.Equal("Pew Pew", filterContext.HttpContext.User.Identity.Name);
            Assert.Equal("CustomInMemoryTokenAuthorization", filterContext.HttpContext.User.Identity.AuthenticationType);
        }
    }
}