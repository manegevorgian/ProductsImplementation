namespace ProductsCommon.Services
{
    public interface IXmlService
    {
        (string type, decimal value) GetTodaysDiscount();
        void SaveXmlStringAsFile(string xmlContent);
    }
}