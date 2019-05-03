using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Logging;
using Core.CrossCuttingConcerns.Logging.Log4Net;
using Core.Utilities.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Aspects.Autofac.Logging
{

    public class LoggingInterceptionAspect : MethodInterception
    {
        private LoggerService _loggerService;
        public LoggingInterceptionAspect(Type loggerType)
        {
            if (loggerType.BaseType != typeof(LoggerService))
                throw new Exception("Wrong Logger Type");

            _loggerService = (LoggerService)Activator.CreateInstance(loggerType);
        }
        protected override void OnBefore(IInvocation invocation)
        {
            var logParameters = invocation.Arguments.Select(x => new LogParameter
            {
                Type = x.GetType().Name,
                Value = x
            }).ToList();

            var logDetail = new LogDetail()
            {
                FullName = invocation.TargetType.Name,
                MethodName = invocation.Method.Name,
                Parameters = logParameters
            };

            _loggerService.Error(logDetail);
        }
    }
}
