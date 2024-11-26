namespace lifecheck_client;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class Worker : BackgroundService {
	private readonly string _url = "";
	private readonly string _apiKey = "";

	private readonly IHostApplicationLifetime _hostApplicationLifetime;
	private readonly ILogger<Worker> _logger;
	private readonly HttpClient _httpClient;
	private readonly int MAX_ATTEMPTS = 3;
	private readonly int DELAY_BETWEEN_ATTEMPTS = 360000;

	public Worker(ILogger<Worker> logger, IHostApplicationLifetime hostApplicationLifetime) {
		_hostApplicationLifetime = hostApplicationLifetime;
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
				HttpResponseMessage response = await SendPostRequest();

				// Stop the service if the response was successful
				if (response.IsSuccessStatusCode) {
					_logger.LogInformation("Successfully received a valid response. Stopping service.");
					break;
				} else {
					_logger.LogWarning("Request failed. Retrying...");
					// Delay 5 minutes before retrying
					await Task.Delay(DELAY_BETWEEN_ATTEMPTS, stoppingToken);
				}
			}
			if (attempt > MAX_ATTEMPTS) {
				_logger.LogError("Maximum attempts exceeded");
			}
		}

		_logger.LogInformation("Service execution completed.");
		_hostApplicationLifetime.StopApplication();
	}

	private async Task<HttpResponseMessage> SendPostRequest() {
		try {
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _url);

			// Include the API key in the "x-api-key" header
			request.Headers.Add("x-api-key", _apiKey);

			// Send the request
			HttpResponseMessage response = await _httpClient.SendAsync(request);
			return response;
		} catch (Exception ex) {
			_logger.LogError($"Error sending POST request: {ex.Message}");
			throw;
		}
	}
}
