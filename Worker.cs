namespace lifecheck_client;

using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class Worker : BackgroundService {
	private readonly string _url = "";
	private readonly string _apiKey = "";

	private readonly ILogger<Worker> _logger;
	private readonly HttpClient _httpClient;
	private readonly int MAX_ATTEMPTS = 3;
	private readonly int DELAY_BETWEEN_ATTEMPTS = 360000;

	public Worker(ILogger<Worker> logger) {
		_logger = logger;
		_httpClient = new HttpClient();
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
		_logger.LogInformation("Service running at: {time}", DateTimeOffset.Now);

		if (_url.Length == 0 || _apiKey.Length == 0) {
			_logger.LogError("Required parameters do not exist: _url='{_url}' _apiKey='{_apiKey}'", _url, _apiKey);
		} else {
			_logger.LogInformation("Attempting to send POST request to _url='{_url}' using _apiKey='{_apiKey}'", _url, _apiKey);

			int attempt = 0;
			while (!stoppingToken.IsCancellationRequested && ++attempt <= MAX_ATTEMPTS) {
				var response = await SendPostRequest();

				// Stop the service if the response was successful
				if (response.IsSuccessStatusCode) {
					_logger.LogInformation("Successfully received a valid response. Stopping service.");
					break;
				} else {
					_logger.LogWarning("Request failed. Retrying...");
				}

				// Delay 5 minutes before retrying
				await Task.Delay(DELAY_BETWEEN_ATTEMPTS, stoppingToken);
			}
			if (attempt > MAX_ATTEMPTS) {
				_logger.LogError("Maximum attempts exceeded");
			}
		}

		_logger.LogInformation("Service execution completed.");
	}

	private async Task<HttpResponseMessage> SendPostRequest() {
		var data = new {
			apiKey = _apiKey
		};
		var json = JsonConvert.SerializeObject(data);
		var content = new StringContent(json, Encoding.UTF8, "application/json");

		try {
			var response = await _httpClient.PostAsync(_url, content);
			return response;
		} catch (Exception ex) {
			_logger.LogError($"Error sending POST request: {ex.Message}");
			throw;
		}
	}
}
