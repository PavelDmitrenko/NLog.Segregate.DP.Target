using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;


namespace Tests
{
	class Program
	{

		
		static void Main(string[] args)
		{  
			
			var servicesProvider = BuildDi();
			var runner = servicesProvider.GetRequiredService<Runner>();

			runner.DoAction("Action1");
			Console.ReadLine();
		}
		private static ServiceProvider BuildDi()
		{
			return new ServiceCollection()
				.AddLogging(builder => {
					builder.SetMinimumLevel(LogLevel.Trace);
					builder.AddNLog(new NLogProviderOptions {
						CaptureMessageTemplates = true,
						CaptureMessageProperties = true
					});
				})
				.AddTransient<Runner>()
				.BuildServiceProvider();
		}

		public class Runner
		{
			private readonly ILogger<Runner> _logger;

			public Runner(ILogger<Runner> logger)
			{
				_logger = logger;
			}

			public void DoAction(string name)
			{
				Logger logger = NLog.LogManager.GetCurrentClassLogger();
				logger.Info("zzz");
				
			}


		}

	}
}
