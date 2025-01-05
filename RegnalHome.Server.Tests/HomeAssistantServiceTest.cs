using Moq;
using RegnalHome.Server.Http.HttpClients;
using RegnalHome.Server.Http.Options;
using RegnalHome.Server.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegnalHome.Server.Tests
{
    public class HomeAssistantServiceTest
    {
        [Theory]
        [InlineData(null, "12/31/2025", "2/28/2025", "12/30/2025")]
        [InlineData(null, "1/4/2025", "2/28/2024", "1/3/2025")]
        [InlineData(null, "2/23/2025", "2/28/2024", "2/22/2025")]
        [InlineData(2024, "2/23/2025", "2/28/2024", "2/28/2025")]
        [InlineData(2023, "3/23/2025", "2/28/2023", "2/28/2024")]
        public void GetDateFromTo(int? year, string today, string from, string to)
        {
            // Arrange
            var options = new EgdOptions();

            var service = new HomeAssistantService(Mock.Of<IEgdHttpClient>(), options);

            // Act
            var result = service.GetDateFromTo(year, DateTime.Parse(today, CultureInfo.InvariantCulture));

            // Assert
            Assert.Equal(from ?? new DateTime(DateTime.Today.Year, options.BeginDateTime.Month, options.BeginDateTime.Day).ToShortDateString(), result.DateFrom.ToShortDateString());
            Assert.Equal(to, result.DateTo.ToShortDateString());
        }
    }
}
