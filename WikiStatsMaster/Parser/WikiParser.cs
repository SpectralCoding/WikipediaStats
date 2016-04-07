using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using NLog;
using WikiStatsMaster.Queueing;

namespace WikiStatsMaster.Parser {
	public static class WikiParser {
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public static void Parse(String filename) {
			XmlReaderSettings xmlSettings = new XmlReaderSettings {IgnoreWhitespace = true};
			Int64 count = 0;
			using (XmlReader reader = XmlReader.Create(filename, xmlSettings)) {
				while (reader.Read()) {
					if ((reader.NodeType == XmlNodeType.Element) && (reader.Name.ToLower() == "page")) {
						OutgoingQueue.QueueRaw(reader.ReadOuterXml());
						count++;
						if (count % 1000 == 0) {
							Logger.Debug($"Sent Page: {count:N0}");
						}
					}
				}
			}
		}

	}
}
