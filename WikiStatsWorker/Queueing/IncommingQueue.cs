using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WikiStatsWorker.DataManagement;
using WikiStatsWorker.Processor;

namespace WikiStatsWorker.Queueing {
	class IncommingQueue {
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Initializes the incomming RabbitMQ Queue and begins processing IRC Lines.
		/// </summary>
		public IncommingQueue() {
			ConnectionFactory factory = new ConnectionFactory {
				HostName = Properties.Settings.Default.RabbitMQHostname,
				Port = Properties.Settings.Default.RabbitMQPort,
				UserName = Properties.Settings.Default.RabbitMQUsername,
				Password = Properties.Settings.Default.RabbitMQPassword
			};
			var connection = factory.CreateConnection();
			var channel = connection.CreateModel();
			channel.QueueDeclare(Properties.Settings.Default.RabbitMQQueue, true, false, false, null);
			channel.BasicQos(0, 1, false);
			EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
			consumer.Received += (model, ea) => {
				Byte[] body = ea.Body;
				WikiProcessor.Process(Encoding.UTF8.GetString(body));
				//String[] strSplit = message.Split(new[] { '|' }, 2);
				//DateTime ircDateTime;
				//if (DateTime.TryParseExact(strSplit[0], "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out ircDateTime)) {
				//	// Parse the actual IRC line
				//	IrcParser.Parse(strSplit[1], ircDateTime);
				//	// Before moving on to the next line make sure Redis has been fully updated.
				//	DataStore.WaitTasks();
				//} else {
				//	// For whatever reason the line was malformed.
				//	Logger.Info("Bad RabbitMQ Entry: {0}", message);
				//}
				// Finally, acknowledge the line has been fully delivered and processed.
				//DataStore.PrintStats();
				DataStore.Commit();
				channel.BasicAck(ea.DeliveryTag, false);
			};
			channel.BasicConsume(Properties.Settings.Default.RabbitMQQueue, false, consumer);
		}
	}
}
