using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using rest_api.Controllers;
using rest_api.DTO.Temperature;
using rest_api.Models;
using rest_api.Services.TemperatureService;
using Xunit;

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
            _mockService.Setup(service => service.GetAllTemperatures())
                .ReturnsAsync(new ServiceResponse<List<GetTemperatureDTO>>
                {
                    Data = new List<GetTemperatureDTO>()
                });

            var result = await _controller.Get();
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetSingle_ReturnsOkResult()
        {
            int id = 1;
            _mockService.Setup(service => service.GetTemperatureById(id))
                .ReturnsAsync(new ServiceResponse<GetTemperatureDTO>
                {
                    Data = new GetTemperatureDTO()
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
                    Data = new GetTemperatureDTO()
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
                    Data = new GetTemperatureDTO()
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
                    Data = null
                });

            var result = await _controller.DeleteTemperature(id);
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
        [Fact]
        public async Task Get_ReturnsNotFoundResult_WhenNoData()
        {
            _mockService.Setup(service => service.GetAllTemperatures())
                .ReturnsAsync(new ServiceResponse<List<GetTemperatureDTO>>
                {
                    Data = null,
                    Success = true
                });

            var result = await _controller.Get();
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetSingle_ReturnsNotFoundResult_ForInvalidId()
        {
            int invalidId = 99;
            _mockService.Setup(service => service.GetTemperatureById(invalidId))
                .ReturnsAsync(new ServiceResponse<GetTemperatureDTO>
                {
                    Success = true,
                    Data = null
                });

            var result = await _controller.GetSingle(invalidId);
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateTemperature_ReturnsBadRequestResult_WhenFailedToAdd()
        {
            var newTemperature = new AddTemperatureDTO();
            _mockService.Setup(service => service.AddTemperature(newTemperature))
                .ReturnsAsync(new ServiceResponse<GetTemperatureDTO>
                {
                    Success = false,
                    Message = "Failed to add"
                });

            var result = await _controller.CreateTemperature(newTemperature);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteTemperature_ReturnsBadRequestResult_WhenServiceReportsError()
        {
            int id = 1;
            _mockService.Setup(service => service.DeleteTemperatureById(id))
                .ReturnsAsync(new ServiceResponse<GetTemperatureDTO>
                {
                    Success = false,
                    Message = "Failed to delete"
                });

            var result = await _controller.DeleteTemperature(id);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}
