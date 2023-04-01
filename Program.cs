// See https://aka.ms/new-console-template for more information
using D2Rbots;

var runewizardtoken = Environment.GetEnvironmentVariable("runewizardtoken");
var discordtoken = Environment.GetEnvironmentVariable("discordtoken");
var service = new TerrorZoneService(discordtoken, runewizardtoken, args);
service.StartServiceAsync();
service._quitEvent.WaitOne();
Console.WriteLine("Exited service");