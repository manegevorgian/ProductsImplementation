using ProductsCommon.Models;
using ProductsService;
using ProductsCommon.Repositories;
using ProductsCommon.Services;

IHost host = Host.CreateDefaultBuilder(args)
   .ConfigureAppConfiguration((hostingContext, config) =>
   {
       config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
   })
   .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();

        IConfiguration configuration = hostContext.Configuration;
        var systemSettings = configuration.GetSection("SystemSettings").Get<SystemSettings>();

        services.AddSingleton(systemSettings);


        services.AddTransient<IProductRepository, ProductRepository>();
        services.AddTransient<IXmlService, XmlService>();

    })
    .Build();

await host.RunAsync();
