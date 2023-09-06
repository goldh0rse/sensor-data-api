using Microsoft.AspNetCore.Mvc;
using Moq;
using RestApi.Controllers;
using RestApi.DTO.Temperature;
using RestApi.Models;
using RestApi.Services.TemperatureService;

namespace TemperatureApi.Tests
{
    public class TemperatureControllerTests
    {
        private readonly Mock<ITemperatureService> _mockService;
        private readonly TemperatureController _controller;

        public TemperatureControllerTests()
        {
            _mockService = new Mock<ITemperatureService>();
            _controller = new TemperatureController(_mockService.Object);
        }

        [Fact]
        public async Task Get_ReturnsOkResult()
        {
            _mockService.Setup(service => service.GetAllTemperatures(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new ServiceResponse<List<GetTemperatureDTO>>
                {
                    Data = new List<GetTemperatureDTO>(),
                    Success = true
                });

            var result = await _controller.Get();
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task Get_ReturnsNotFoundResult_WhenNoData()
        {
            _mockService.Setup(service => service.GetAllTemperatures(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(new ServiceResponse<List<GetTemperatureDTO>>
            {
                Data = null,
                Success = true
            });

            var result = await _controller.Get();
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetSingle_ReturnsOkResult()
        {
            int id = 1;
            _mockService.Setup(service => service.GetTemperatureById(id))
                .ReturnsAsync(new ServiceResponse<GetTemperatureDTO>
                {
                    Data = new GetTemperatureDTO(),
                    Success = true
                });

            var result = await _controller.GetSingle(id);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateTemperature_ReturnsOkResult()
        {
            var newTemperature = new AddTemperatureDTO();
            _mockService.Setup(service => service.AddTemperature(newTemperature))
                .ReturnsAsync(new ServiceResponse<GetTemperatureDTO>
                {
                    Data = new GetTemperatureDTO(),
                    Success = true
                });

            var result = await _controller.CreateTemperature(newTemperature);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteTemperature_ReturnsOkResult()
        {
            int id = 1;
            _mockService.Setup(service => service.DeleteTemperatureById(id))
                .ReturnsAsync(new ServiceResponse<GetTemperatureDTO>
                {
                    Data = new GetTemperatureDTO(),
                    Success = true
                });

            var result = await _controller.DeleteTemperature(id);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteTemperature_ReturnsNotFoundResult()
        {
            int id = 1;
            _mockService.Setup(service => service.DeleteTemperatureById(id))
                .ReturnsAsync(new ServiceResponse<GetTemperatureDTO>
                {
                    Data = null,
                    Success = true
                });

            var result = await _controller.DeleteTemperature(id);
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Get_ReturnsBadRequestResult_WhenServiceFails()
        {
            _mockService.Setup(service => service.GetAllTemperatures(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new ServiceResponse<List<GetTemperatureDTO>>
                {
                    Data = null,
                    Success = false
                });

            var result = await _controller.Get();
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetSingle_ReturnsNotFoundResult_WhenInvalidId()
        {
            int id = 0; // Invalid id
            _mockService.Setup(service => service.GetTemperatureById(id))
                .ReturnsAsync(new ServiceResponse<GetTemperatureDTO>
                {
                    Data = null,
                    Success = true
                });

            var result = await _controller.GetSingle(id);
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateTemperature_ReturnsBadRequestResult_WhenServiceFails()
        {
            var newTemperature = new AddTemperatureDTO();
            _mockService.Setup(service => service.AddTemperature(newTemperature))
                .ReturnsAsync(new ServiceResponse<GetTemperatureDTO>
                {
                    Data = null,
                    Success = false
                });

            var result = await _controller.CreateTemperature(newTemperature);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteTemperature_ReturnsBadRequestResult_WhenServiceFails()
        {
            int id = 1;
            _mockService.Setup(service => service.DeleteTemperatureById(id))
                .ReturnsAsync(new ServiceResponse<GetTemperatureDTO>
                {
                    Data = null,
                    Success = false
                });

            var result = await _controller.DeleteTemperature(id);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}
