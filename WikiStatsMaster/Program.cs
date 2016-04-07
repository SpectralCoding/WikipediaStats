using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NLog;
using WikiStatsMaster.Parser;
using WikiStatsMaster.Queueing;

namespace WikiStatsMaster {
	class Program {
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		static void Main(string[] args) {
			Logger.Info("WikiStatsMaster v" + Assembly.GetExecutingAssembly().GetName().Version + " started.");
			OutgoingQueue.Initialize();
			WikiParser.Parse(Properties.Settings.Default.WikiExportPath);
			Console.ReadLine();
		}
	}
}
