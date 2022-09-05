// See https://aka.ms/new-console-template for more information
using Grpc.Net.Client;
using GrpcContract;
using ProtoBuf.Grpc.Client;

Console.WriteLine("Hello, World!");
using var channel = GrpcChannel.ForAddress("https://localhost:5001");
var client = channel.CreateGrpcService<IGreeterService>();

var reply = await client.SayHelloAsync(
    new HelloRequest { Name = "GreeterClient" });

Console.WriteLine($"Greeting: {reply.Message}");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();