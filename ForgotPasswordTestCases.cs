using CodeReviewAI.Controllers;
using CodeReviewAI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReviewAITesting
{
   [TestFixture]
    public class ForgotPasswordTestCases
    {
        private Mock<IForgotPasswordService> MockForgotPasswordService;
        private ForgotPasswordController ForgotPasswordController;

        [SetUp]
        public void SetUp()
        {
            MockForgotPasswordService = new Mock<IForgotPasswordService>();
            ForgotPasswordController = new ForgotPasswordController(MockForgotPasswordService.Object);
        }

        [TearDown] 
        public void TearDown() { 
            MockForgotPasswordService.VerifyAll();
            ForgotPasswordController.Dispose();
        }

        [Test]
        public async Task CheckTheUserEmail_IfExist_ReturnsUserID()
        {
            var email = "test@123.com";
            var userId = "test@!2";
            MockForgotPasswordService.Setup((s) => s.CheckTheUserExists(email)).ReturnsAsync(userId);

            var result = await ForgotPasswordController.CheckUseEmailIsExistorNot(email) as ViewResult;
            Assert.AreEqual(userId, result.ViewName);
        }

        [Test]
        public async Task CheckTheUserEmail_IfNotExist_ReturnExceptionMessage()
        {
            var email = "test@123.com";
            MockForgotPasswordService.Setup((s)=>s.CheckTheUserExists(email)).ThrowsAsync(new Exception("Invalid Email! Create new account"));

            var result = await ForgotPasswordController.CheckUseEmailIsExistorNot(email);
            
        }

        [Test]
        public async Task ValidateOTP_IfOTPMatched_ReturnsTrue()
        {
            var otp = "234567";
            MockForgotPasswordService.Setup((s)=>s.ValidateOTP(otp)).ReturnsAsync(true);

            var result = await ForgotPasswordController.ValidateTheOTPIfUserExist(otp) as RedirectToActionResult;

            Assert.AreEqual(nameof(ForgotPasswordController.ResetThePassword), result.ActionName);
        }
    }
}
