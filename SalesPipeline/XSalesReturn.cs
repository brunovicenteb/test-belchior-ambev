using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmbevWeb.RulePipeline
{
    public enum XSalesResult
    {
        Sucess,
        Error
    }

    public class XSalesReturn
    {
        public XSalesReturn(string pMessage, XSalesResult pResult)
        {
            Message = pMessage;
            Result = pResult;
        }

        public readonly string Message;
        public readonly XSalesResult Result;
    }
}