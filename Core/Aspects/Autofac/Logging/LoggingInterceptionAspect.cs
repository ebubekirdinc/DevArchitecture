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
        private bool _logOnAfter;
        public LoggingInterceptionAspect(Type loggerType, bool logOnAfter)
        {
            if (loggerType.BaseType != typeof(LoggerService))
                throw new Exception("Wrong Logger Type");

            _loggerService = (LoggerService)Activator.CreateInstance(loggerType);
            _logOnAfter = logOnAfter;
        }
        protected override void OnBefore(IInvocation invocation)
        {
        

            _loggerService.Error(GetLogDetail(invocation,"OnBefore"));
        }

        protected override void OnAfter(IInvocation invocation)
        {
            if (!_logOnAfter) return;

            _loggerService.Error(GetLogDetail(invocation, "OnAfter"));
            
        }

        LogDetail GetLogDetail (IInvocation invocation, string actionLog)
        {
            var logParameters = invocation.Arguments.Select(x => new LogParameter
            {
                Type = x.GetType().Name,
                Value = x
            }).ToList();

            var logDetail = new LogDetail()
            {
                ActionLog = actionLog,
                MethodName = invocation.Method.Name,
                Parameters = logParameters
            };

            return logDetail;
        }
    }
}
