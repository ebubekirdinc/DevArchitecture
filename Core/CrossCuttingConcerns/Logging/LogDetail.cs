using System;
using System.Collections.Generic;
using System.Text;

namespace Core.CrossCuttingConcerns.Logging
{
    public class LogDetail
    {
        public string ActionLog { get;set;}
        public string MethodName { get; set; }
        public List<LogParameter> Parameters { get; set; }
    }
}
