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
        public async Task DeleteAccount_shouldReturnOK_WhenAccountIdIsPresent()
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

        [Theory]
        [InlineData(50010,50,"Depsot")]
        public async Task Update_ShouldReturnNotOKResponse_WhenTransTypeIsAbsent(int accountId, decimal amount, String transType)
        {

            //Arrange
            var accountData = _fixture.Create<AccountModel>();
            
            _accountServiceMock.Setup(x => x.GetAccount(accountId)).ReturnsAsync(accountData);

            //Act
            var result = await _controller.UpdateAccount(accountId, amount, transType);

            Assert.IsType<BadRequestObjectResult>(result);
  
        }

        [Fact]
        public async Task UpdateAccount_Success_WhenValidTransType_Deposit_Present()
        {
            //Arrange
             var transData = _fixture.Create<TransactionModel>();
             var accountId = _fixture.Create<int>();
             var amount = _fixture.Create<decimal>();

            _accountServiceMock.Setup(x => x.UpdateAccount(accountId, amount, UtilsConstants.TRANSTYPE_DEPOSIT)).ReturnsAsync(transData);

            //Act
            var result = await _controller.UpdateAccount(accountId, amount, UtilsConstants.TRANSTYPE_DEPOSIT);

            //Assert
            Assert.IsType<OkObjectResult>(result);

        }

        [Fact]
        public async Task UpdateAccount_Success_WhenValidTransType_Withdraw_Present()
        {

            //Arrange
            var transData = _fixture.Create<TransactionModel>();
            var accountId = _fixture.Create<int>();
            var amount = _fixture.Create<decimal>();

            _accountServiceMock.Setup(x => x.UpdateAccount(accountId, amount, UtilsConstants.TRANSTYPE_WITHDRAW)).ReturnsAsync(transData);

            //Act
            var result = await _controller.UpdateAccount(accountId, amount, UtilsConstants.TRANSTYPE_WITHDRAW);

            //Assert
            var transResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateAccount_Fail_When_Withdraw_MaxLimitBreach()
        {

            //Arrange
            var transData = _fixture.Create<TransactionModel>();
            var errorData = _fixture.Create<ErrorModel>();
            errorData.Code = 4002;
            transData.Error = errorData;
            var accountId = _fixture.Create<int>();
            var amount = _fixture.Create<decimal>();

            _accountServiceMock.Setup(x => x.UpdateAccount(accountId, amount, UtilsConstants.TRANSTYPE_WITHDRAW)).ReturnsAsync(transData);

            //Act
            var result = await _controller.UpdateAccount(accountId, amount, UtilsConstants.TRANSTYPE_WITHDRAW);

            //Assert
            var transResult = Assert.IsType<OkObjectResult>(result);
            var fetchtrans = (TransactionModel)transResult.Value;
            Assert.Equal(4002, fetchtrans.Error.Code);
            
        }

        [Fact]
        public async Task UpdateAccount_Fail_When_Withdraw_MinBalBreach()
        {

            //Arrange
            var transData = _fixture.Create<TransactionModel>();
            var errorData = _fixture.Create<ErrorModel>();
            errorData.Code = 4001;
            transData.Error = errorData;
            var accountId = _fixture.Create<int>();
            var amount = _fixture.Create<decimal>();

            _accountServiceMock.Setup(x => x.UpdateAccount(accountId, amount, UtilsConstants.TRANSTYPE_WITHDRAW)).ReturnsAsync(transData);

            //Act
            var result = await _controller.UpdateAccount(accountId, amount, UtilsConstants.TRANSTYPE_WITHDRAW);

            //Assert
            var transResult = Assert.IsType<OkObjectResult>(result);
            var fetchtrans = (TransactionModel)transResult.Value;
            Assert.Equal(4001, fetchtrans.Error.Code);

        }


    }
}