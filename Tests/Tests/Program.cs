using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Fluent;
using NLog.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;


namespace Tests
{
	class Program
	{

		
		static void Main(string[] args)
		{  
			GlobalDiagnosticsContext.Set("DeviceID", "SomeDeviceID");
			Logger logger = LogManager.GetCurrentClassLogger();

			while (true)
			{
				logger.Info(DateTime.Now);
				System.Threading.Thread.Sleep(500);
			}


			Console.ReadLine();
		}

	}
}
