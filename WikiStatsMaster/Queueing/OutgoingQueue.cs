using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace WikiStatsMaster.Queueing {
	public static class OutgoingQueue {
		private static IModel _channel;

		public static void Initialize() {
			ConnectionFactory factory = new ConnectionFactory {
				HostName = Properties.Settings.Default.RabbitMQHostname,
				Port = Properties.Settings.Default.RabbitMQPort,
				UserName = Properties.Settings.Default.RabbitMQUsername,
				Password = Properties.Settings.Default.RabbitMQPassword
			};
			IConnection connection = factory.CreateConnection();
			_channel = connection.CreateModel();
			_channel.QueueDeclare(Properties.Settings.Default.RabbitMQQueue, true, false, false, null);
		}

		public static void QueueRaw(String data) {
			var body = Encoding.UTF8.GetBytes(data);
			var properties = _channel.CreateBasicProperties();
			properties.Persistent = true;
			_channel.BasicPublish("", Properties.Settings.Default.RabbitMQQueue, properties, body);
		}

		public static void QueueIrc(String lineToQueue, DateTime dateTime) {
			QueueRaw($"{dateTime:O}|{lineToQueue}");
		}
	}
}
