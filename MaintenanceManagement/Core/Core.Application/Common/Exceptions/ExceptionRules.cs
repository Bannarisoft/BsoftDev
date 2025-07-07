using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Exceptions
{
    public class ExceptionRules : Exception
    {
        public ExceptionRules(string message) : base(message) { }
    }
}