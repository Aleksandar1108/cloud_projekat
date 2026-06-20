using SmartGrid.ITSimulator.Models;
using System;
using System.Net.Http.Json;

namespace SmartGrid.ITSimulator.Services
{
    public class SmartMeterClient
    {
        private readonly HttpClient _httpClient;

        public SmartMeterClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<SmartMeterDto>> GetAllAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<SmartMeterDto>>("https://localhost:7262/api/SmartMeters");

            return result ?? new List<SmartMeterDto>();
        }
    }
}