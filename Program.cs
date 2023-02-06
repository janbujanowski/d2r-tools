// See https://aka.ms/new-console-template for more information
using D2Rbots;

//var messenger = new MessageLogger();
var service = new TerrorZoneService(args);
service.StartServiceAsync();
service._quitEvent.WaitOne();
Console.WriteLine("Exited service");