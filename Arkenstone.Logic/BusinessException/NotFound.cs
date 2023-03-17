namespace Arkenstone.Logic.BusinessException
{
    public class NotFound : System.Exception
    {
        public NotFound(string Type) : base(Type + " are not found")
        {

        }
    }
}
