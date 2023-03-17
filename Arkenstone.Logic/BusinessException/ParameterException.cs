using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkenstone.Logic.BusinessException
{
    public class ParameterException : System.Exception
    {
        public ParameterException(string Type) : base(Type + " is incorrect")
        {

        }
    }
}
