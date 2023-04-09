using AutoFixture;
using AutoMapper;
using accolite_bank_application.Models;
using accolite_bank_application.Entities;
using accolite_bank_application.Repositories;
using accolite_bank_application.Services;
using Moq;
using Xunit;
using accolite_bank_application.Controllers;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using accolite_bank_application.Constants;

namespace accolite_bank_application.Tests
{
    public class ServiceUnitTests
    {

        private readonly IFixture _fixture;
        private readonly Mock<IAccountServices> _accountServiceMock;
        private readonly AccountController _controller;


        public ServiceUnitTests()
        {
            _fixture = new Fixture();
            
            _accountServiceMock = _fixture.Freeze<Mock<IAccountServices>>();
            _controller = new AccountController(_accountServiceMock.Object);
        }

        [Fact]
        public async Task DeleteAccount_WhenAccountIdIsPresent()
        {
            //Arrange
            int accountId = 50001;

            _accountServiceMock.Setup(x => x.DeleteAccount(accountId)).ReturnsAsync(true);

            //Act
            var result = await _controller.DeleteAccount(accountId);

            //Assert
            result.Should().NotBeNull().Equals(true);

            _accountServiceMock.Verify(x => x.DeleteAccount(accountId), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldReturnNotOKResponse_WhenTransTypeIsAbsent()
        {

            //Arrange
           
            var accountData = _fixture.Create<AccountModel>();
            accountData.accountId = 50010;
            //accountData.balance = 100;
            int accountNumber = 50010;
            decimal amount = 50;


            _accountServiceMock.Setup(x => x.GetAccount(accountNumber)).ReturnsAsync(accountData);

            //Act
            var result = await _controller.UpdateAccount(accountNumber, amount, "Deosit");

            Assert.IsType<BadRequestObjectResult>(result);
  
        }

        [Fact]
        public async Task UpdateAccount_Success_WhenValidTransType_Deposit_Present()
        {

            //Arrange
             var transData = _fixture.Create<TransactionModel>();
            transData.accountId = 50010;
            transData.transAmount = 50;
            transData.postBalance = 150;
             int accountNumber = 50010;
            decimal amount = 50;


            _accountServiceMock.Setup(x => x.UpdateAccount(accountNumber, amount, UtilsConstants.TRANSTYPE_DEPOSIT)).ReturnsAsync(transData);

            //Act
            var result = await _controller.UpdateAccount(accountNumber, amount, UtilsConstants.TRANSTYPE_DEPOSIT);


            var transResult = Assert.IsType<OkObjectResult>(result);
            var transModel = (TransactionModel)transResult.Value;

            Assert.Equal(transModel.postBalance, transData.postBalance);


        }

        [Fact]
        public async Task UpdateAccount_Success_WhenValidTransType_Withdraw_Present()
        {

            //Arrange
            var transData = _fixture.Create<TransactionModel>();
            transData.accountId = 50010;
            transData.transAmount = 50;
            transData.postBalance = 50;
            int accountNumber = 50010;
            decimal amount = 50;


            _accountServiceMock.Setup(x => x.UpdateAccount(accountNumber, amount, UtilsConstants.TRANSTYPE_WITHDRAW)).ReturnsAsync(transData);

            //Act
            var result = await _controller.UpdateAccount(accountNumber, amount, UtilsConstants.TRANSTYPE_WITHDRAW);


            var transResult = Assert.IsType<OkObjectResult>(result);
            var trans1 = (TransactionModel)transResult.Value;

            Assert.Equal(trans1.postBalance, transData.postBalance);


        }

        [Fact]
        public async Task UpdateAccount_Fail_When_Withdraw_MaxLimitBreach()
        {

            //Arrange
            var transData = _fixture.Create<TransactionModel>();
            transData.accountId = 50010;
            transData.transAmount = 10001;
            transData.postBalance = 150;

            int accountNumber = 50010;
            decimal amount = 10001;

            _accountServiceMock.Setup(x => x.UpdateAccount(accountNumber, amount, UtilsConstants.TRANSTYPE_WITHDRAW)).ReturnsAsync(transData);


            //Act
            var result = await _controller.UpdateAccount(accountNumber, amount, UtilsConstants.TRANSTYPE_WITHDRAW);


            var transResult = Assert.IsType<OkObjectResult>(result);
            var trans1 = (TransactionModel)transResult.Value;
            trans1.message.StartsWith(ErrorConstants.TRANS_MAXLIMIT_BREACH_MESSAGE);
        }

        [Fact]
        public async Task UpdateAccount_Fail_When_Withdraw_MinBalBreach()
        {

            //Arrange
            var transData = _fixture.Create<TransactionModel>();
            transData.accountId = 50010;
            transData.transAmount = 51;
            transData.postBalance = 100;

            int accountNumber = 50010;
            decimal amount = 150;

            _accountServiceMock.Setup(x => x.UpdateAccount(accountNumber, amount, UtilsConstants.TRANSTYPE_WITHDRAW)).ReturnsAsync(transData);


            //Act
            var result = await _controller.UpdateAccount(accountNumber, amount, UtilsConstants.TRANSTYPE_WITHDRAW);


            var transResult = Assert.IsType<OkObjectResult>(result);
            var trans1 = (TransactionModel)transResult.Value;
            trans1.message.StartsWith(ErrorConstants.TRANS_MINBAL_BREACH_MESSAGE);


        }


    }
}