// using Moq;
// using AutoMapper;
// using Microsoft.EntityFrameworkCore;
// using RestApi.Data;
// using RestApi.Services.TemperatureService;
// using RestApi.DTO.Temperature;
// using RestApi.Models;
// using RestApi;
// using System.Linq.Expressions;
// using static RestApi.Extensions.QueryableExtensions;

// namespace TemperatureApi.Tests
// {



//     public class TemperatureServiceTests
//     {
//         private readonly TemperatureService _service;
//         private readonly Mock<DataContext> _mockContext;
//         private readonly IQueryable<Temperature> _mockData;

//         public TemperatureServiceTests()
//         {
//             var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));
//             var mapper = new Mapper(mapperConfig);
//             _mockContext = new Mock<DataContext>(new DbContextOptions<DataContext>());
//             _service = new TemperatureService(mapper, _mockContext.Object);

//             // Mock data
//             _mockData = new List<Temperature>
//             {
//                 new() { Id = 1, Temp = 22.5 },
//                 new() { Id = 2, Temp = 23.5 },
//                 new() { Id = 3, Temp = 24.5 }
//             }.AsQueryable();
//         }

//         [Fact]
//         public async Task GetAllTemperatures_ReturnsListOfTemperatures()
//         {
//             var mockSet = new Mock<DbSet<Temperature>>();
//             mockSet.As<IAsyncEnumerable<Temperature>>()
//                     .Setup(m => m.GetAsyncEnumerator(new CancellationToken()))
//                     .Returns(new InMemoryAsyncEnumerator<Temperature>(_mockData.GetEnumerator()));

//             // Use TestAsyncQueryProvider<Temperature>
//             mockSet.As<IQueryable<Temperature>>()
//                     .Setup(m => m.Provider)
//                     .Returns(new TestAsyncQueryProvider<Temperature>(_mockData.Provider));

//             mockSet.As<IQueryable<Temperature>>().Setup(m => m.Expression).Returns(_mockData.Expression);
//             mockSet.As<IQueryable<Temperature>>().Setup(m => m.ElementType).Returns(_mockData.ElementType);
//             mockSet.As<IQueryable<Temperature>>().Setup(m => m.GetEnumerator()).Returns(_mockData.GetEnumerator());

//             _mockContext.Setup(c => c.Temperatures).Returns(mockSet.Object);

//             // Act
//             var result = await _service.GetAllTemperatures(page: 1, pageSize: 3, sortBy: "Id", ascending: false);
//             Console.WriteLine(result.Message);

//             // Assert
//             Assert.True(result.Success);
//             Assert.IsType<List<GetTemperatureDTO>>(result.Data);
//             Assert.Equal(3, result.Data.Count);  // Mock data has 3 items
//         }


//         [Fact]
//         public async Task GetTemperatureById_ReturnsTemperature()
//         {
//             // Arrange
//             var mockSet = new Mock<DbSet<Temperature>>();
//             mockSet.As<IQueryable<Temperature>>().Setup(m => m.Provider).Returns(_mockData.AsQueryable().Provider);
//             mockSet.As<IQueryable<Temperature>>().Setup(m => m.Expression).Returns(_mockData.AsQueryable().Expression);
//             mockSet.As<IQueryable<Temperature>>().Setup(m => m.ElementType).Returns(_mockData.AsQueryable().ElementType);
//             mockSet.As<IQueryable<Temperature>>().Setup(m => m.GetEnumerator()).Returns(_mockData.AsQueryable().GetEnumerator());

//             _mockContext.Setup(m => m.Temperatures).Returns(mockSet.Object);

//             // Act
//             var result = await _service.GetTemperatureById(1);  // Assuming 1 is an ID that exists

//             // Assert
//             Assert.True(result.Success);
//             Assert.IsType<GetTemperatureDTO>(result.Data);
//             Assert.Equal(22.5, result.Data.Temp);  // Mock data for ID=1 has a temperature value of 22.5
//         }
//     }
// }
