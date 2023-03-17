namespace Arkenstone.Logic.BusinessException
{
    public class NotAuthentified : System.Exception
    {
        public NotAuthentified() : base("You are not authentified")
        {

        }
    }
}
