using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using StackExchange.Redis;

namespace WikiStatsWorker.DataManagement {
	public static class DataStore {
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		public const String Delimiter = ":";
		private static ConnectionMultiplexer _redis;
		public static Dictionary<String, Dictionary<String, Int64>> Statistics = new Dictionary<String, Dictionary<String, Int64>>();
		public static List<Task> Tasks = new List<Task>();

		public static ConnectionMultiplexer Redis
		{
			get { return _redis; }
			set { _redis = value; }
		}

		public static void Connect(String connectString) {
			_redis = ConnectionMultiplexer.Connect(connectString);
		}

		public static void WaitTasks() {
			//Stopwatch stopWatch = new Stopwatch();
			//stopWatch.Start();
			Task.WaitAll(Tasks.ToArray());
			Tasks.Clear();
			//stopWatch.Stop();
			//TimeSpan ts = stopWatch.Elapsed;
		}

		public static void IncrementOrSet(String ht, String field, Int64 change = 1) {
			if (!Statistics.ContainsKey(ht)) {
				Statistics.Add(ht, new Dictionary<String, Int64>());
			}
			if (Statistics.ContainsKey(field)) {
				Statistics[ht][field] += change;
			} else {
				Statistics[ht].Add(field, change);
			}
		}

		public static void IncrementOrSet(String ht, Dictionary<String, Int64> items) {
			if (!Statistics.ContainsKey(ht)) {
				Statistics.Add(ht, new Dictionary<String, Int64>());
			}
			foreach (KeyValuePair<String, Int64> curKVP in items) {
				if (Statistics.ContainsKey(curKVP.Key)) {
					Statistics[ht][curKVP.Key] += curKVP.Value;
				} else {
					Statistics[ht].Add(curKVP.Key, curKVP.Value);
				}
			}
		}


		public static void PrintStats() {
			foreach (KeyValuePair<String, Dictionary<String, Int64>> topKVP in Statistics) {
				Logger.Debug(topKVP.Key);
				foreach (KeyValuePair<String, Int64> subKVP in topKVP.Value) {
					Logger.Debug($"\t{subKVP.Key} = {subKVP.Value}");
                }
			}
		}

		public static void Commit() {
			Logger.Debug("Committing Entries:");
			IDatabase db = Redis.GetDatabase();
			while (Statistics.Count > 0) {
				String firstHashtable = Statistics.Keys.ToArray()[0];
                while (Statistics[firstHashtable].Count > 0) {
					String firstEntry = Statistics[firstHashtable].Keys.ToArray()[0];
					db.HashIncrement(firstHashtable, firstEntry, Statistics[firstHashtable][firstEntry]);
					Statistics[firstHashtable].Remove(firstEntry);
				}
				Statistics.Remove(firstHashtable);
			}
		}


		public static string Base64Encode(String plainText) {
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}
		public static string Base64Encode(Char plainText) {
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText.ToString());
			return System.Convert.ToBase64String(plainTextBytes);
		}
		public static string Base64Decode(String encodedText) {
			var base64EncodedBytes = System.Convert.FromBase64String(encodedText);
			return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
		}

	}
}
