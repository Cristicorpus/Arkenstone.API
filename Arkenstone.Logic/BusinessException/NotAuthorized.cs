namespace Arkenstone.Logic.BusinessException
{
    public class NotAuthorized : System.Exception
    {
        public NotAuthorized() : base("You are not authorized")
        {

        }
    }
}
