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

namespace accolite_bank_application.Tests
{
    public class UnitTests
    {

        private readonly IFixture _fixture;
        private readonly Mock<IAccountServices> _accountServiceMock;
        private readonly AccountController _controller;


        public UnitTests()
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
        public async Task DepositTest()
        {

            //Arrange
             var transData = _fixture.Create<TransactionModel>();
            transData.accountId = 50010;
            transData.transAmount = 50;
            transData.postBalance = 150;
             int accountNumber = 50010;
            decimal amount = 50;


            _accountServiceMock.Setup(x => x.UpdateAccount(accountNumber, amount, "Deposit")).ReturnsAsync(transData);

            //Act
            var result = await _controller.UpdateAccount(accountNumber, amount, "Deposit");


            var transResult = Assert.IsType<OkObjectResult>(result);
            var trans1 = (TransactionModel)transResult.Value;

            Assert.Equal(trans1.postBalance, transData.postBalance);


        }

        [Fact]
        public async Task WithdrawTest()
        {

            //Arrange
            var transData = _fixture.Create<TransactionModel>();
            transData.accountId = 50010;
            transData.transAmount = 50;
            transData.postBalance = 50;
            int accountNumber = 50010;
            decimal amount = 50;


            _accountServiceMock.Setup(x => x.UpdateAccount(accountNumber, amount, "Withdraw")).ReturnsAsync(transData);

            //Act
            var result = await _controller.UpdateAccount(accountNumber, amount, "Withdraw");


            var transResult = Assert.IsType<OkObjectResult>(result);
            var trans1 = (TransactionModel)transResult.Value;

            Assert.Equal(trans1.postBalance, transData.postBalance);


        }

        [Fact]
        public async Task BusinessRule1Test()
        {

            //Arrange
            var transData = _fixture.Create<TransactionModel>();
            transData.accountId = 50010;
            transData.transAmount = 10001;
            transData.postBalance = 150;

            int accountNumber = 50010;
            decimal amount = 10001;

            _accountServiceMock.Setup(x => x.UpdateAccount(accountNumber, amount, "Withdraw")).ReturnsAsync(transData);


            //Act
            var result = await _controller.UpdateAccount(accountNumber, amount, "Withdraw");


            var transResult = Assert.IsType<OkObjectResult>(result);
            var trans1 = (TransactionModel)transResult.Value;
            trans1.message.StartsWith("Invalid Transaction: Withdrawl amount is greater than $10000");
        }

        [Fact]
        public async Task BusinessRule2Test()
        {

            //Arrange
            var transData = _fixture.Create<TransactionModel>();
            transData.accountId = 50010;
            transData.transAmount = 51;
            transData.postBalance = 100;

            int accountNumber = 50010;
            decimal amount = 150;

            _accountServiceMock.Setup(x => x.UpdateAccount(accountNumber, amount, "Withdraw")).ReturnsAsync(transData);


            //Act
            var result = await _controller.UpdateAccount(accountNumber, amount, "Withdraw");


            var transResult = Assert.IsType<OkObjectResult>(result);
            var trans1 = (TransactionModel)transResult.Value;
            trans1.message.StartsWith("Invalid Transaction : Withdrawl amount will make available balance below mandatory limit of $100");


        }


    }
}