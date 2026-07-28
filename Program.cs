using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LocalStack.Client.Extensions;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
class Program
{
    static async Task Main(string[] args)
    {
        IConfiguration Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddCommandLine(args)
            .Build();

        var isUsingLocalStack = Configuration.GetValue<bool>("LocalStack:UseLocalStack");
        Console.WriteLine($"Using LocalStack: {isUsingLocalStack}");
        var awsOptions = Configuration.GetAWSOptions();
        Console.WriteLine($"AWS Region: {awsOptions.Region}");

        var hostBuilder = Host.CreateDefaultBuilder(args);
        hostBuilder.ConfigureServices((hostContext, services) =>
        {
            services.AddLocalStack(Configuration);
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAwsService<IAmazonS3>();
            services.AddSingleton<BucketLister>();
        });
        var host = hostBuilder.Build();
        var myService = host.Services.GetRequiredService<BucketLister>();
        await myService.ListAsync();
    }
}

class BucketLister(IAmazonS3 service)
{
    public async Task ListAsync()
    {
        Console.WriteLine("Listing Buckets...");
        await service.EnsureBucketExistsAsync("awstest-my-bucket");
        var result = await service.ListBucketsAsync();
        Console.WriteLine($"Buckets: {string.Join(", ", result.Buckets.Select(b => b.BucketName))}");
    }
}