using Castle.DynamicProxy;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Core.CrossCuttingConcerns.Logging.FakeLoggerService;

namespace Core.Aspects.Autofac.Performance
{
    public class PerformanceInterceptionAspect : MethodInterception
    {
        private int _interval;
        private Stopwatch _stopwatch;
        private FakeLoggerService _loggerService;
        public PerformanceInterceptionAspect(int interval)
        {
            _interval = interval;
            _stopwatch = Activator.CreateInstance<Stopwatch>();
        }
        public PerformanceInterceptionAspect(int interval, Type loggerType)
        {
            if (loggerType.BaseType != typeof(FakeLoggerService))
            {
                throw new Exception("Logger Type Mismatch.");
            }
            _interval = interval;
            _stopwatch = Activator.CreateInstance<Stopwatch>();
            _loggerService = ServiceTool.ServiceProvider.GetService<FakeLoggerService>();
        }
        protected override void OnBefore(IInvocation invocation)
        {
            _stopwatch.Start();
        }
        protected override void OnAfter(IInvocation invocation)
        {
            string logLine = string.Format($"Performance : {invocation.Method.DeclaringType.FullName}.{invocation.Method.Name} --> {_stopwatch.Elapsed.TotalSeconds}");
            _stopwatch.Stop();
            if (_stopwatch.Elapsed.TotalSeconds > _interval)
            {
                if (_loggerService != null)
                {
                    _loggerService.Log();
                    return;
                }
                Debug.WriteLine(logLine);
            }
            _stopwatch.Reset();
        }
    }
}
