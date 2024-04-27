using ProductsCommon.Models;
using ProductsCommon.Repositories;
using ProductsCommon.Services;

namespace ProductsService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly SystemSettings _settings;
        private readonly IXmlService _xmlService;
        private readonly IProductRepository _productRepository;

        public Worker (
            ILogger<Worker> logger, 
            SystemSettings settings, 
            IXmlService xmlService, 
            IProductRepository productRepository
            )
        {
            _logger = logger;
            _settings = settings;
            _xmlService = xmlService;
            _productRepository = productRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                TimeSpan timeStart = new TimeSpan(_settings.timeStart, 0, 0);

                DateTime now = DateTime.Now;
                TimeSpan currentTime = now.TimeOfDay;  

                if (currentTime >= timeStart)
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                    try
                    {
                        (string, decimal) dic = _xmlService.GetTodaysDiscount();
                        _productRepository.UpdateDiscount(dic);

                    }
                    catch (Exception ex) 
                    {
                        _logger.LogInformation($"Worker running Exception time {DateTimeOffset.Now}: Exception {ex.Message}");

                    }
                    await Task.Delay(24*60*60*1000);
                }
               
            }
        }
    }
}
