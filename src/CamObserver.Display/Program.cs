using CamObserver.Tools;
using Grpc.Net.Client.Web;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using CamObserver.Display.Helpers;
using CamObserver.Display.Data;

namespace CamObserver.Display
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            ReadConfig();
            Setup();
            Application.Run(new MainDisplay());
        }


        static void Setup()
        {
            var channel = GrpcChannel.ForAddress(
              AppConstants.GrpcUrl, new GrpcChannelOptions
              {
                  MaxReceiveMessageSize = 8 * 1024 * 1024, // 5 MB
                  MaxSendMessageSize = 8 * 1024 * 1024, // 2 MB                
                  HttpHandler = new GrpcWebHandler(new HttpClientHandler())
              });
            ObjectContainer.Register<GrpcChannel>(channel);
            ObjectContainer.Register<DataCounterService>(new DataCounterService(channel));
            ObjectContainer.Register<CCTVService>(new CCTVService(channel));
            ObjectContainer.Register<GatewayService>(new GatewayService(channel));
        }

        static void ReadConfig()
        {
            try
            {
                var builder = new ConfigurationBuilder()
   .SetBasePath(Directory.GetCurrentDirectory())
   .AddJsonFile("config.json", optional: false);

                IConfiguration Configuration = builder.Build();

                AppConstants.GrpcUrl = Configuration["App:GrpcUrl"];
                AppConstants.Gateway = Configuration["App:Gateway"];
                AppConstants.Lokasi = Configuration["App:Lokasi"];
                AppConstants.CCTVName = Configuration["App:CCTVName"];
                AppConstants.SyncDelay = int.Parse(Configuration["App:SyncDelay"]);
                AppConstants.AutoStart = bool.Parse(Configuration["App:AutoStart"]);
                Tracker.DistanceLimit = int.Parse(Configuration["App:DistanceLimit"]);
                Tracker.TimeLimit = int.Parse(Configuration["App:TimeLimit"]);
                //AppConstants.Label = Configuration["NetworkModel:Label"];
                //AppConstants.Weights = Configuration["NetworkModel:Weights"];
                //AppConstants.Cfg = Configuration["NetworkModel:Cfg"];

                //AppConstants.Cctv1 = Configuration["CameraUrls:CCTV1"];
            }
            catch (Exception ex)
            {
                Console.WriteLine("read config failed:" + ex);
            }
            Console.WriteLine("Read config successfully.");
        }
    }
}