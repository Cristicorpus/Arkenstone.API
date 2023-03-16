
namespace Arkenstone.API.Models
{
    public class ErrorModel
    {
        public ErrorModel(string message)
        {
            this.message = message;
        }
        public string message { get; set; }
    }
}
