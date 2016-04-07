using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NLog;
using WikiStatsWorker.DataManagement;
using WikiStatsWorker.Queueing;

namespace WikiStatsWorker {
	class Program {
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		static void Main(string[] args) {
			Logger.Info("WikiStatsWorker v" + Assembly.GetExecutingAssembly().GetName().Version + " started.");
			DataStore.Connect(Properties.Settings.Default.RedisConnectString);
			IncommingQueue incommingQueue = new IncommingQueue();

			Logger.Info("Waiting for User Input before exiting.");
			Console.ReadLine();
		}
	}
}
