using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using NLog.Internal;
using NLog.Layouts;
using NLog.Targets;

namespace NLog.Segregate.DP.Target
{

	[Target("Segregate")]
	public class SegregateTarget : FileTarget
	{

		public Layout segregateFileName { get; set; }
		public int filesToArchive { get; set; }
		private readonly object _threadLock = new object();

		public Layout segregateArchiveFileName{ get; set; }

		public SegregateTarget()
		{
			CreateDirs = true;
			FileName = "";
		}

		protected override void InitializeTarget()
		{
			FileName = segregateFileName;
			base.InitializeTarget();
		}

		protected override void Write(LogEventInfo logEvent)
		{
			base.Write(logEvent);
			_Archive();
		}

		private void _Archive()
		{
			lock (_threadLock)
			{

				string archiveName = CleanupInvalidFilePath(segregateArchiveFileName.Render(new LogEventInfo()));
				string logDirectory = Path.GetDirectoryName(Path.GetFullPath(FileName.Render(new LogEventInfo())));
				string logExtension = Path.GetExtension(FileName.Render(new LogEventInfo()));

				DirectoryInfo DirInfo = new DirectoryInfo(logDirectory);

				List<FileInfo> files = DirInfo.EnumerateFiles($"*{logExtension}")
					.Where(x => x.CreationTime < DateTime.Now.AddMilliseconds(100))
						.OrderBy(x => x.CreationTime).Take(filesToArchive).ToList();

				if (files.Count == filesToArchive)
				{
					string zipName = Path.GetFullPath(archiveName);

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

		// From FilePathLayout.cs
		private static readonly HashSet<char> InvalidFileNameChars = new HashSet<char>((IEnumerable<char>)Path.GetInvalidFileNameChars());
		private static string CleanupInvalidFilePath(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
				return filePath;

			int num = filePath.LastIndexOfAny(new[] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar});

			char[] chArray = (char[])null;
			for (int index = num + 1; index < filePath.Length; ++index)
			{
				if (InvalidFileNameChars.Contains(filePath[index]))
				{
					if (chArray == null)
						chArray = filePath.Substring(num + 1).ToCharArray();

					chArray[index - (num + 1)] = '_';
				}
			}

			if (chArray != null)
				return Path.Combine(num > 0 ? filePath.Substring(0, num + 1) : string.Empty, new string(chArray));

			return filePath;
		}
	}

}
