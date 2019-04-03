using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using NLog.Targets;

namespace NLog.Segregate.DP.Target
{

	[Target("Segregate")]
	public class SegregateTarget : TargetWithLayout
	{

		public string logsDirectory { get; set; }
		public int filesToArchive { get; set; }
		private string _directory { get; set; }
		private readonly object threadLock = new object();


		public SegregateTarget()
		{
		}

		protected override void InitializeTarget()
		{
			base.InitializeTarget();
		}

		protected override void Write(LogEventInfo logEvent)
		{
			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			_directory = Path.Combine(baseDirectory, logsDirectory);

			if (!Directory.Exists(_directory))
				Directory.CreateDirectory(_directory);

			string logMessage = this.Layout.Render(logEvent);
			string fileName = $"{DateTime.Now:yyyyMMdd_HHmmss_fffffff}.log";
			File.WriteAllText($"{_directory}{fileName}", logMessage);

			_Archive();

		}

		private void _Archive()
		{
			lock (threadLock)
			{

				DirectoryInfo DirInfo = new DirectoryInfo(_directory);

				var files = DirInfo.EnumerateFiles("*.log")
					.Where(x => x.CreationTime < DateTime.Now.AddSeconds(-5))
						.OrderBy(x => x.CreationTime).Take(filesToArchive).ToList();

				if (files.Count == filesToArchive)
				{
					string zipName = Path.Combine(_directory, $"{DateTime.Now:yyyyMMdd_HHmmss_fffffff}.zip");

					using (ZipArchive newFile = ZipFile.Open(zipName, ZipArchiveMode.Create))
					{
						foreach (FileInfo file in files)
						{
							newFile.CreateEntryFromFile(file.FullName, file.Name, CompressionLevel.Fastest);
						}

					}

					foreach (FileInfo file in files)
					{
						try
						{
							File.Delete(file.FullName);
						}
						catch 
						{
							// nothing
						}
						
					}

				}
			}
		}

		protected override void CloseTarget()
		{
			base.CloseTarget();
		}

	}
}