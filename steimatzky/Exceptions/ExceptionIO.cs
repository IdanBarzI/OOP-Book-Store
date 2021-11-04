using steimatzky.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steimatzky.Exceptions
{
    class ExceptionIO : Exception
    {
        private string message;

        public ExceptionIO(string msg)
        {
            message = msg;
        }

        public override string ToString()
        {
            return message;
        }

    }
}
