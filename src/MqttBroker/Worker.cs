using MQTTnet.Server;
using MQTTnet;
using System.Text;

namespace MqttBroker
{
    public class MqttWorker : BackgroundService
    {
        private readonly ILogger<MqttWorker> _logger;

        public MqttWorker(ILogger<MqttWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            /*
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }*/
            // create and start broker
            await StartBroker();
           
        }
        IMqttServer mqttServer;
        async Task StartBroker()
        {
            // Create the options for MQTT Broker
            var option = new MqttServerOptionsBuilder()
                //Set endpoint to localhost
                .WithDefaultEndpoint()
                //Add Interceptor for logging incoming messages
                .WithApplicationMessageInterceptor(OnNewMessage);

            // Create a new mqtt server 
            mqttServer = new MqttFactory().CreateMqttServer();
            await mqttServer.StartAsync(option.Build());
            // Keep application running until user press a key
            _logger.LogInformation("MQTT Server is Starting: {time}", DateTimeOffset.Now);
        }

        void OnNewMessage(MqttApplicationMessageInterceptorContext context)
        {
            // Convert Payload to string
            var payload = context.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(context.ApplicationMessage?.Payload);

            _logger.LogInformation(" TimeStamp: {0} -- Message: ClientId = {1}, Topic = {2}, Payload = {3}, QoS = {4}, Retain-Flag = {5}",

                DateTime.Now,
                context.ClientId,
                context.ApplicationMessage?.Topic,
                payload,
                context.ApplicationMessage?.QualityOfServiceLevel,
                context.ApplicationMessage?.Retain);
            /*
            Console.WriteLine(
                " TimeStamp: {0} -- Message: ClientId = {1}, Topic = {2}, Payload = {3}, QoS = {4}, Retain-Flag = {5}",

                DateTime.Now,
                context.ClientId,
                context.ApplicationMessage?.Topic,
                payload,
                context.ApplicationMessage?.QualityOfServiceLevel,
                context.ApplicationMessage?.Retain);
            */
        }

    }
}