﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2Rbots
{
    internal class TerrorZoneService
    {
        private string _discordWebhookUrl = "https://discord.com/api/webhooks/";
        private string _d2runewizardRequestUrl = "https://d2runewizard.com/api/terror-zone?token=";
        Dictionary<string, string> _customRunewizardAuthHeaders;
        private string[] _startArgs;
        private DateTime _nextScheduledTZUpdate;
        public TerrorZoneService(string discordTokenArg, string d2runewizardTokenArg, string[] args)
        {
            if (string.IsNullOrEmpty(discordTokenArg) || string.IsNullOrEmpty(d2runewizardTokenArg))
            {
                Console.WriteLine($"Invalid token value. discord={discordTokenArg} d2runewizardToken={d2runewizardTokenArg}");
                throw new ArgumentNullException();
            }
            _discordWebhookUrl = _discordWebhookUrl + discordTokenArg;
            _d2runewizardRequestUrl = _d2runewizardRequestUrl + d2runewizardTokenArg;
            _customRunewizardAuthHeaders = new Dictionary<string, string>()
            {
                { "D2R-Contact", "jan.bujanowski@gmail.com" },
                { "D2R-Platform", "Discord" },
                { "D2R-Repo", "https://github.com/janbujanowski/d2r-tools" }
            };
            this._startArgs = args;
            SetUpQuitEvents();
        }
        public async Task StartServiceAsync()
        {
            SetUpRoutineCheck();
        }

        private async Task SetUpRoutineCheck()
        {
            var counter = 0;
            var max = _startArgs.Length != 0 ? Convert.ToInt32(_startArgs[0]) : -1;
            while (max == -1 || counter < max)
            {
                ConsoleLogMessage($"Routine check: {++counter} | Next scheduled update : {_nextScheduledTZUpdate}");
                CheckForApiCalls();
                await Task.Delay(TimeSpan.FromMilliseconds(10_000));
            }
        }

        private void CheckForApiCalls()
        {
            if (DateTime.UtcNow > _nextScheduledTZUpdate)
            {
                try
                {
                    var response = GetCurrentTZStatus();
                    if (response.terrorZone.highestProbabilityZone.amount > 0)
                    {
                        _nextScheduledTZUpdate = DateTime.UtcNow.AddMinutes(1);
                    }
                    if (response.terrorZone.highestProbabilityZone.amount > 3)
                    {
                        _nextScheduledTZUpdate = GetNextFullHour();
                    }
                    if (response.terrorZone.highestProbabilityZone.probability == 0)
                    {
                        _nextScheduledTZUpdate = DateTime.UtcNow.AddSeconds(35);
                    }
                    else
                    {
                        PostToDiscord(response);
                    }
                    ConsoleLogMessage($"Scheduling next update to {_nextScheduledTZUpdate}");

                }
                catch (Exception ex)
                {
                    MessageLogger.WriteLine($"Error during scheduled update. Exception : {ex.Message}");
                    _nextScheduledTZUpdate = DateTime.UtcNow.AddMinutes(1);//retry in 1 mins
                }
            }
        }

        private DateTime GetNextFullHour()
        {
            var current = DateTime.UtcNow;
            current = current.AddHours(1);

            return new DateTime(current.Year, current.Month, current.Day, current.Hour, 0, 45, 0);
        }

        public ManualResetEvent _quitEvent = new ManualResetEvent(false);
        private void SetUpQuitEvents()
        {
            Console.CancelKeyPress += (s, e) =>
            {
                _quitEvent.Set();
                e.Cancel = true;
            };
        }

        private void ConsoleLogMessage(string message)
        {
            MessageLogger.WriteLine(message);
        }

        private D2runewizardTZRespone? GetCurrentTZStatus()
        {
            ConsoleLogMessage("Requesting update from runewizard");
            D2runewizardTZRespone? tryParseResponse = new D2runewizardTZRespone();
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, _d2runewizardRequestUrl);
                foreach (var header in _customRunewizardAuthHeaders)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
                var httpResponse = httpClient.SendAsync(request).Result;
                var response = JObject.Parse(httpResponse.Content.ReadAsStringAsync().Result);
                tryParseResponse = response.ToObject<D2runewizardTZRespone>();
            }
            return tryParseResponse;
        }

        private bool PostToDiscord(D2runewizardTZRespone tzUpdate)
        {
            ConsoleLogMessage("Posting to discord");
            DiscordWebhookRequest? request = new DiscordWebhookRequest(tzUpdate);
            var stringPayload = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            using (var httpClient = new HttpClient())
            {
                var httpResponse = httpClient.PostAsync(_discordWebhookUrl, httpContent).Result;
            }
            return true;
        }
    }
}
