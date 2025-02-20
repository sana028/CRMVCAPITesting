using CodeReviewAI.Controllers;
using CodeReviewAI.Interfaces;
using CodeReviewAI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CodeReviewAITesting
{
    [TestFixture]
    public class LoginTestCases
    {
        private Mock<IAuthService> AuthServiceMock;
        private LoginController LoginController;

        [SetUp]
        public void SetUp()
        {
            AuthServiceMock = new Mock<IAuthService>();
            LoginController = new LoginController(AuthServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            LoginController?.Dispose();
        }

        [Test]
        public async Task Login_ValidCredentials_ReturnsViewWithToken()
        {
            var loginRequest = new LoginRequest { Email = "test", Password = "testab12" };
            var mockAccessToken = "2233DErtggg%kkk";
            var res = AuthServiceMock.Setup((s) => s.AuthenticateUser(loginRequest)).ReturnsAsync(mockAccessToken);

            var result = await LoginController.Login(loginRequest);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(mockAccessToken, viewResult.ViewName);
        }

        [Test]
        public async Task Login_InvalidCredentials_RedirectsToLogin()
        {
            var loginRequest = new LoginRequest { Email = "test", Password = "test222" };
            AuthServiceMock.Setup((s) => s.AuthenticateUser(loginRequest)).ThrowsAsync(new Exception("Invalid Credentials"));

            var result = await LoginController.Login(loginRequest);
            var redirectResult = result as RedirectToActionResult;
            Assert.NotNull(redirectResult);
            Assert.AreEqual(nameof(LoginController.Login), redirectResult.ActionName);
        }
    }
}