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
        public XSalesReturn(string pMessage, XSalesResult pResult, object pArtifact = null)
        {
            Message = pMessage;
            Result = pResult;
            Artifact = pArtifact;
        }

        public readonly string Message;
        public readonly XSalesResult Result;
        public readonly object Artifact;
    }
}