namespace Arkenstone.Logic.BusinessException
{
    public class NoContent : System.Exception
    {
        public NoContent(string Type) : base(Type + " no content")
        {

        }
    }
}
