using CodeReviewAI.Controllers;
using CodeReviewAI.Interfaces;
using CodeReviewAI.Models;
using CodeReviewAI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReviewAITesting
{
    [TestFixture]
    public class SignUpTestcase
    {
        private Mock<IAuthService> MockAuthService;
        private SignUpController SignUpController;
        private TempDataDictionary TempData;

        [SetUp]
        public void Setup()
        {
            MockAuthService = new Mock<IAuthService>();
            SignUpController = new SignUpController(MockAuthService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            SignUpController?.Dispose();
        }

        [Test]
        public async Task AddNewUserProfile_SuccessfulSignup_RedirectsToOtpVerification()
        {
      
            var signUpRequest = new SignUpRequest { UserId= "tese@23", Email = "test@example.com", Password = "Password123!" };
            MockAuthService.Setup(s => s.AddNewUser(signUpRequest)).ReturnsAsync("OTP sent to email");

            var result = await SignUpController.AddNewUserProfile(signUpRequest) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.AreEqual("OtpVerification", result.ActionName);
        }

        [Test]
        public async Task AddNewUserProfile_FailedSignup_ReturnsViewWithMessage()
        {
            var signUpRequest = new SignUpRequest { UserId = null, Email = "test@example.com", Password = "Password123!" };
            MockAuthService.Setup(s => s.AddNewUser(signUpRequest)).ReturnsAsync(string.Empty);

            var result = await SignUpController.AddNewUserProfile(signUpRequest) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual("Failed to send OTP. Please try again.", result.ViewData["Message"]);
        }

        [Test]
        public async Task AddNewUserProfile_Exception_ReturnsViewWithErrorMessage()
        {
            var signUpRequest = new SignUpRequest {UserId = null, Email = "test@example.com", Password = "Password123!" };
            MockAuthService.Setup(s => s.AddNewUser(signUpRequest)).ThrowsAsync(new Exception("Service failure"));

            var result = await SignUpController.AddNewUserProfile(signUpRequest) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual("An error occurred while processing your request: Service failure", result.ViewData["Message"]);
        }

        [Test]
        public async Task VerifyUserEmailWithOTP_SuccessfulVerification_ReturnsViewWithMessage()
        {
            // Arrange
            var otpModel = new OtpModel { Email = "test@example.com", Otp = "123456" };
            MockAuthService.Setup(s => s.VerifyOTP(otpModel)).ReturnsAsync("OTP Verified");

            // Act
            var result = await SignUpController.VerifyUserEmailWithOTP(otpModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("OTP Verified", result.ViewData["Message"]);
        }

        [Test]
        public async Task VerifyUserEmailWithOTP_FailedVerification_ReturnsViewWithMessage()
        {
            // Arrange
            var otpModel = new OtpModel { Email = "test@example.com", Otp = "000000" };
            MockAuthService.Setup(s => s.VerifyOTP(otpModel)).ReturnsAsync("Invalid OTP");

            // Act
            var result = await SignUpController.VerifyUserEmailWithOTP(otpModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("Invalid OTP", result.ViewData["Message"]);
        }

        [Test]
        public async Task VerifyUserEmailWithOTP_Exception_ReturnsViewWithErrorMessage()
        {

            var otpModel = new OtpModel { Email = "test@example.com", Otp = "123456" };
            MockAuthService.Setup(s => s.VerifyOTP(otpModel)).ThrowsAsync(new Exception("Service failure"));

            var result = await SignUpController.VerifyUserEmailWithOTP(otpModel) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual("Service failure", result.ViewData["Message"]);
        }
    }
}
