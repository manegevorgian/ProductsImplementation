using ProductsCommon.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace ProductsCommon.Services
{
    public class XmlService : IXmlService
    {
        private string _directoryPath;
        private readonly string _folderName = "Discounts";

        public XmlService(SystemSettings settings)
        {
            _directoryPath = settings.fileDirectory;

            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }
        }
        public void SaveXmlStringAsFile(string xmlContent)
        {
            XDocument xmlDoc;

            try
            {
                xmlDoc = XDocument.Parse(xmlContent);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("Provided string is not valid XML.", exception);
            }

            string dateString = DateTime.Now.ToString("ddMMyyyy");
            string fileName = $"{dateString}.xml";
            string filePath = Path.Combine(_directoryPath, fileName);

            xmlDoc.Save(filePath);
        }

        public (string type, decimal value) GetTodaysDiscount()
        {
            string dateString = DateTime.Now.ToString("ddMMyyyy");
            string fileName = $"{dateString}.xml";
            string filePath = Path.Combine(_directoryPath, fileName);

            if (!File.Exists(filePath)) return ("Fix",0);

            XDocument xmlDoc = XDocument.Load(filePath);

            XElement discountElement = xmlDoc.Element("ProductInfo")?.Element("Discount");
            if (discountElement != null)
            {
                if (discountElement.Element("Percent") != null)
                {
                    decimal percentValue = decimal.Parse(discountElement.Element("Percent").Value);
                    return ("Percent", percentValue);
                }
                else if (discountElement.Element("Fix") != null)
                {
                    decimal fixValue = decimal.Parse(discountElement.Element("Fix").Value);
                    return ("Fix", fixValue);
                }
            }

            throw new InvalidOperationException("No valid discount information found.");
        }
    }
}
