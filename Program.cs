// See https://aka.ms/new-console-template for more information
using D2Rbots;

//var messenger = new MessageLogger();
var service = new TerrorZoneService(args);
service.StartServiceAsync();
service._quitEvent.WaitOne();
var runewizardtoken = Environment.GetEnvironmentVariable("runewizardtoken");
var discordtoken = Environment.GetEnvironmentVariable("discordtoken");
var service = new TerrorZoneService(discordtoken, runewizardtoken, args);
Console.WriteLine("Exited service");