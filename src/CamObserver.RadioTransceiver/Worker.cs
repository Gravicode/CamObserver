using CamObserver.Models;
using CamObserver.RadioTransceiver.Data;
using CamObserver.RadioTransceiver.Helpers;
using CamObserver.RadioTransceiver.Models;
using Newtonsoft.Json;

namespace CamObserver.RadioTransceiver
{
    public class Worker : BackgroundService
    {
        const int DelayTime = 5000;
        private readonly ILogger<Worker> _logger;
        static Xbee xbee;
        WeatherDataService weatherDataService;
        DataCounterService dataCounterService;
        public Worker(ILogger<Worker> logger)
        {
            xbee = new Xbee(AppConstants.COM_PORT);
            xbee.DataReceived += Xbee_DataReceived;
            _logger = logger;
            weatherDataService = new WeatherDataService();
            dataCounterService = new();
            Console.WriteLine("Cam Observer - Radio Transreceiver Service is running");
        }

        private void Xbee_DataReceived(object sender, Xbee.DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data)) return;
            try
            {
                var obj = JsonConvert.DeserializeObject<SensorData>(e.Data);
                if (obj != null)
                {
                    var newItem = new WeatherData()
                    {
                        BarPressure = obj.BarPressure,
                        Humidity = obj.Humidity,
                        RainfallOneDay = obj.RainfallOneDay,
                        RainfallOneHour = obj.RainfallOneHour,
                        Tanggal = DateTime.Now,
                        Temperature =
                     obj.Temperature,
                        WindDirection = obj.WindDirection,
                        WindSpeedAverage = obj.WindSpeedAverage,
                        WindSpeedMax = obj.WindSpeedMax
                    };
                    var res = weatherDataService.InsertData(newItem);
                    if (res)
                    {
                        _logger.LogInformation($"[{DateTimeOffset.Now}] - Data received and inserted to DB: {e.Data}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //throw;
            }
            


        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                var data = dataCounterService.GetPersonAndBicycle();
                if (data != null)
                {
                    var json = JsonConvert.SerializeObject(data);
                    var res = xbee.SendMessage(json);
                    if (res)
                    {
                        _logger.LogInformation($"[{DateTimeOffset.Now}] - Data transmit to Xbee: {json}");
                    }
                }
                await Task.Delay(DelayTime, stoppingToken);
            }
        }
    }
}