using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LocalStack.Client.Extensions;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
class Program
{
    static void Main(string[] args)
    {
        IConfiguration Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        var y = Configuration.GetValue<bool>("LocalStack:UseLocalStack");
        Console.WriteLine($"Using LocalStack: {y}");

        var hostBuilder = Host.CreateDefaultBuilder(args);
        hostBuilder.ConfigureServices((hostContext, services) =>
        {
            services.AddLocalStack(Configuration);
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAwsService<IAmazonS3>();
            services.AddSingleton<MyService>();
        });
        var host = hostBuilder.Build();
        var myService = host.Services.GetRequiredService<MyService>();
        myService.DoSomething();
    }
}

class MyService(IAmazonS3 service)
{
    public void DoSomething()
    {
        Console.WriteLine("MyService is doing something...");
    }
}