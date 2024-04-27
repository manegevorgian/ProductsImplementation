using System;
using System.Collections.Generic;
using ProductsCommon.Models;
using ProductsCommon.Repositories;
using ProductsCommon.Services;
using Microsoft.AspNetCore.Mvc;


namespace ProductsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly XmlService _xmlService;
        private readonly SystemSettings _settings;

        public ProductsController(IProductRepository productRepository, XmlService xmlService, SystemSettings settings)
        {
            _productRepository = productRepository;
            _xmlService = xmlService;
            _settings = settings;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetAllProducts()
        {
            return Ok(_productRepository.GetAllProducts());
        }

        [HttpPost]
        public IActionResult CreateOrUpdateProduct([FromBody] Product product)
        {
            _productRepository.CreateOrUpdate(product);
            return NoContent(); 
        }
        [HttpPost("xml/save")]
        public IActionResult SaveXmlFile([FromBody] string xmlContent)
        {
            try
            {
                _xmlService.SaveXmlStringAsFile(xmlContent);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);
            }

            return Ok("XML file saved successfully.");
        }
    }
}
