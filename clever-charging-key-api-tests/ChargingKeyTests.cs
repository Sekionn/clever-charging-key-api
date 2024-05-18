using clever_charging_key_api.Controllers;
using clever_charging_key_api.Models;
using clever_charging_key_api.Providers;
using clever_charging_key_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace clever_charging_key_api_tests
{
    [TestClass]
    public class ChargingKeyTests
    {
        [TestMethod]
        public void CreateChargingKey_ShouldReturnCreatedKeysId()
        {
            //Arrange
            var context = new ChargingKeyContext(createDatabaseOptions());
            var provider = new Mock<ChargingKeyContextProvider>(context);
            var service = new Mock<ChargingKeyService>(provider.Object);
            var controller = new ChargingKeyController(service.Object);

            //Act
            var actionResult = controller.CreateKey(((KeyTag)new Random().Next(0, 3)).ToString()).Result;

            //Assert
            var result = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.StatusCode == 200);
            var resultData = result.Value as ChargingkeyCreatedDTO;
            Assert.IsNotNull(resultData);
            Assert.IsTrue((resultData.Id ?? "").Contains("dk-{"));
            Assert.IsTrue((resultData.Id ?? "").Contains("}-clever"));
        }

        [TestMethod]
        public void CreateChargingKey_ShouldReturnException_ForWrongKey()
        {
            //Arrange
            var context = new ChargingKeyContext(createDatabaseOptions());
            var provider = new Mock<ChargingKeyContextProvider>(context);
            var service = new Mock<ChargingKeyService>(provider.Object);
            var controller = new ChargingKeyController(service.Object);

            //Act
            var actionResult = controller.CreateKey("jdfhngjkh").Result;

            //Assert
            var result = actionResult.Result as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.StatusCode == 400);
            var resultData = result.Value as string;
            Assert.IsTrue((resultData ?? "").Contains("Not a valid key type"));
        }

        [TestMethod]
        public async Task VerifyChargingKey_ShouldReturnExistingIsTrueAndBlockedIsFalse()
        {
            //Arrange
            var context = new ChargingKeyContext(createDatabaseOptions());
            var provider = new Mock<ChargingKeyContextProvider>(context);
            var service = new Mock<ChargingKeyService>(provider.Object);
            var controller = new ChargingKeyController(service.Object);
            string id = "dk-{3606220562}-clever";

            //Act
            await provider.Object.SaveItemToDb(new ChargingKey(id, KeyTag.CleverBox));

            var actionResult = controller.VerifyKey(id).Result;

            //Assert
            var result = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.StatusCode == 200);
            var resultData = result.Value as ChargingkeyVerifyDTO;
            Assert.IsNotNull(resultData);
            Assert.IsTrue(resultData.Exists);
            Assert.IsFalse(resultData.Blocked);
        }

        [TestMethod]
        public async Task VerifyChargingKey_ShouldReturnExistingIsTrueAndBlockedIsTrue()
        {
            //Arrange
            var context = new ChargingKeyContext(createDatabaseOptions());
            var provider = new Mock<ChargingKeyContextProvider>(context);
            var service = new Mock<ChargingKeyService>(provider.Object);
            var controller = new ChargingKeyController(service.Object);
            string id = "dk-{3606220563}-clever";
            ChargingKey chargingkey = new(id, KeyTag.CleverBox)
            {
                Blocked = true
            };

            //Act
            await provider.Object.SaveItemToDb(chargingkey);

            var actionResult = controller.VerifyKey(id).Result;

            //Assert
            var result = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.StatusCode == 200);
            var resultData = result.Value as ChargingkeyVerifyDTO;
            Assert.IsNotNull(resultData);
            Assert.IsTrue(resultData.Exists);
            Assert.IsTrue(resultData.Blocked);
        }

        [TestMethod]
        public void VerifyChargingKey_ShouldReturnNotFound()
        {
            //Arrange
            var context = new ChargingKeyContext(createDatabaseOptions());
            var provider = new Mock<ChargingKeyContextProvider>(context);
            var service = new Mock<ChargingKeyService>(provider.Object);
            var controller = new ChargingKeyController(service.Object);
            
            //Act
            var actionResult =  controller.VerifyKey("fghsdfg").Result;

            //Assert
            var result = actionResult.Result as NotFoundResult;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.StatusCode == 404);
        }

        [TestMethod]
        public void VerifyChargingKey_ShouldReturnNullException()
        {
            //Arrange
            var context = new ChargingKeyContext(createDatabaseOptions());
            var provider = new Mock<ChargingKeyContextProvider>(context);
            var service = new Mock<ChargingKeyService>(provider.Object);
            var controller = new ChargingKeyController(service.Object);

            //Act
            var actionResult = controller.VerifyKey("");

            //Assert
            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Status == TaskStatus.Faulted);
        }

        [TestMethod]
        public async Task BlockChargingKey_ShouldReturnChargingKeyWithBlockedAsTrue()
        {
            //Arrange
            var context = new ChargingKeyContext(createDatabaseOptions());
            var provider = new Mock<ChargingKeyContextProvider>(context);
            var service = new Mock<ChargingKeyService>(provider.Object);
            var controller = new ChargingKeyController(service.Object);
            string id = "dk-{3606220564}-clever";
            ChargingKey chargingkey = new(id, KeyTag.CleverBox);

            //Act
            await provider.Object.SaveItemToDb(chargingkey);

            var actionResult = controller.BlockKey(id).Result;

            //Assert
            var result = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.StatusCode == 200);
            var resultData = result.Value as ChargingkeyBlockDTO;
            Assert.IsNotNull(resultData);
            Assert.IsTrue(resultData.Id == id);
            Assert.IsTrue(resultData.Blocked);
        }

        [TestMethod]
        public async Task BlockChargingKey_AllreadyBlocked_ShouldReturnChargingKeyId_WithBlockedAsTrue()
        {
            //Arrange
            var context = new ChargingKeyContext(createDatabaseOptions());
            var provider = new Mock<ChargingKeyContextProvider>(context);
            var service = new Mock<ChargingKeyService>(provider.Object);
            var controller = new ChargingKeyController(service.Object);
            string id = "dk-{3606220565}-clever";
            ChargingKey chargingkey = new(id, KeyTag.CleverBox)
            {
                Blocked = true
            };

            //Act
            await provider.Object.SaveItemToDb(chargingkey);

            var actionResult = controller.BlockKey(id).Result;

            //Assert
            var result = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.StatusCode == 200);
            var resultData = result.Value as ChargingkeyBlockDTO;
            Assert.IsNotNull(resultData);
            Assert.IsTrue(resultData.Id == id);
            Assert.IsTrue(resultData.Blocked);
        }

        [TestMethod]
        public async Task BlockChargingKey_WrongId_ShouldReturnNullException()
        {
            //Arrange
            var context = new ChargingKeyContext(createDatabaseOptions());
            var provider = new Mock<ChargingKeyContextProvider>(context);
            var service = new Mock<ChargingKeyService>(provider.Object);
            var controller = new ChargingKeyController(service.Object);
            string id = "dk-{3606220566}-clever";
            ChargingKey chargingkey = new(id, KeyTag.CleverBox);

            //Act
            await provider.Object.SaveItemToDb(chargingkey);

            var actionResult = controller.BlockKey("fglkhjdfltgk");

            //Assert
            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Status == TaskStatus.Faulted);
        }

        [TestMethod]
        public void BlockChargingKey_NoId_ShouldReturnNullException()
        {
            //Arrange
            var context = new ChargingKeyContext(createDatabaseOptions());
            var provider = new Mock<ChargingKeyContextProvider>(context);
            var service = new Mock<ChargingKeyService>(provider.Object);
            var controller = new ChargingKeyController(service.Object);
            
            //Act
            var actionResult = controller.BlockKey("");

            //Assert
            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Status == TaskStatus.Faulted);
        }

        [TestMethod]
        public async Task ChargingKeyExists_ShouldReturnTrue()
        {
            //Arrange
            var context = new ChargingKeyContext(createDatabaseOptions());
            var provider = new Mock<ChargingKeyContextProvider>(context);
            var service = new Mock<ChargingKeyService>(provider.Object);
            string id = "dk-{3606220567}-clever";
            ChargingKey chargingkey = new(id, KeyTag.CleverBox);

            //Act
            await provider.Object.SaveItemToDb(chargingkey);

            var result = service.Object.ChargingKeyExists(id);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ChargingKeyExists_WrongId_ShouldReturnFalse()
        {
            //Arrange
            var context = new ChargingKeyContext(createDatabaseOptions());
            var provider = new Mock<ChargingKeyContextProvider>(context);
            var service = new Mock<ChargingKeyService>(provider.Object);
            string id = "dk-{3606220568}-clever";
            ChargingKey chargingkey = new(id, KeyTag.CleverBox);

            //Act
            await provider.Object.SaveItemToDb(chargingkey);

            var result = service.Object.ChargingKeyExists("edjkfgbh");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CreateNewKey_ReturnsCorrectIdFormat_And_CorrectTag()
        {
            //Arrange
            var context = new ChargingKeyContext(createDatabaseOptions());
            var provider = new Mock<ChargingKeyContextProvider>(context);
            var service = new Mock<ChargingKeyService>(provider.Object);
            KeyTag tag = (KeyTag)new Random().Next(0, 3);

            //Act
            var result = service.Object.CreateNewKey(tag);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Tag == tag);
            Assert.IsTrue((result.Id ?? "").Contains("dk-{"));
            Assert.IsTrue((result.Id ?? "").Contains("}-clever"));
        }

        [TestMethod]
        public void GenerateId_ReturnsCorrectIdFormat()
        {
            //Arrange
            var context = new ChargingKeyContext(createDatabaseOptions());
            var provider = new Mock<ChargingKeyContextProvider>(context);
            var service = new Mock<ChargingKeyService>(provider.Object);

            //Act
            var result = service.Object.GenerateId();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("dk-{"));
            Assert.IsTrue(result.Contains("}-clever"));
        }

        [TestMethod]
        public void RandomIdString_ReturnsStringOfCharLength12()
        {
            //Arrange
            var context = new ChargingKeyContext(createDatabaseOptions());
            var provider = new Mock<ChargingKeyContextProvider>(context);
            var service = new Mock<ChargingKeyService>(provider.Object);

            //Act
            var result = service.Object.RandomIdString();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length == 12);
        }

        [TestMethod]
        public async Task FindKeyById_ShouldReturnCorrectChargingKey()
        {
            //Arrange
            var context = new ChargingKeyContext(createDatabaseOptions());
            var provider = new Mock<ChargingKeyContextProvider>(context);
            var service = new Mock<ChargingKeyService>(provider.Object);
            string id = "dk-{3606220569}-clever";
            ChargingKey chargingkey = new(id, KeyTag.CleverBox);

            //Act
            await provider.Object.SaveItemToDb(chargingkey);

            var result = service.Object.FindKeyById(id).Result;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result == chargingkey);
        }

        [TestMethod]
        public async Task FindKeyById_WrongId_ShouldReturnNull()
        {
            //Arrange
            var context = new ChargingKeyContext(createDatabaseOptions());
            var provider = new Mock<ChargingKeyContextProvider>(context);
            var service = new Mock<ChargingKeyService>(provider.Object);
            string id = "dk-{3606220570}-clever";
            ChargingKey chargingkey = new(id, KeyTag.CleverBox);

            //Act
            await provider.Object.SaveItemToDb(chargingkey);

            var result = service.Object.FindKeyById("edjkfgbh").Result;

            //Assert
            Assert.IsNull(result);
        }

        DbContextOptions<ChargingKeyContext> createDatabaseOptions()
        {
           return new DbContextOptionsBuilder<ChargingKeyContext>()
                .UseInMemoryDatabase("ChargingKeyList").Options;
        }
    }
}