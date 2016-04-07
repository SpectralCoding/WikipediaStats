using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NLog;
using WikiStatsWorker.MediaWiki;
using WikiStatsWorker.DataManagement;

namespace WikiStatsWorker.Processor {
	public static class WikiProcessor {
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public static void Process(String xml) {
			//Logger.Debug(xml);
			XmlSerializer serializer = new XmlSerializer(typeof(PageType));
			PageType pageContents = (PageType)serializer.Deserialize(new StringReader(xml));
			Logger.Debug($"Processing Page: {pageContents.title}");
			CatalogTitle(pageContents.title);
			CatalogNamespace(pageContents.ns);
			foreach (Object curPageItem in pageContents.Items) {
				Logger.Debug(curPageItem.GetType().ToString());
				if (curPageItem.GetType() == typeof(RevisionType)) {
					CatalogRevision(curPageItem as RevisionType);
				} else {
					throw new NotImplementedException();
				}
			}

		}

		private static void CatalogTitle(String title) {
			DataStore.IncrementOrSet("TitleLength", title.Length.ToString());
			DataStore.IncrementOrSet("TitleCharFreq", CountCharacterFrequency(title));
			DataStore.IncrementOrSet("TitleWordFreq", CountWordFrequency(title));
		}

		private static void CatalogNamespace(String ns) {

		}

		private static void CatalogRevision(RevisionType revision) {
			CatalogRevisionId(revision.id);
			CatalogRevisionParentId(revision.parentid);
			CatalogRevisionTimestamp(revision.timestamp);
			CatalogRevisionContributor(revision.contributor);
			CatalogRevisionComment(revision.comment);
			CatalogRevisionModel(revision.model);
			CatalogRevisionFormat(revision.format);
			CatalogRevisionText(revision.text);
			CatalogRevisionSha1(revision.sha1);
		}

		private static void CatalogRevisionId(String id) {
			
		}
		private static void CatalogRevisionParentId(String parentId) {

		}
		private static void CatalogRevisionTimestamp(DateTime timestamp) {

		}
		private static void CatalogRevisionContributor(ContributorType contributor) {

		}
		private static void CatalogRevisionComment(CommentType comment) {

		}
		private static void CatalogRevisionModel(String model) {

		}
		private static void CatalogRevisionFormat(String format) {

		}
		private static void CatalogRevisionText(TextType text) {

		}
		private static void CatalogRevisionSha1(String sha1) {

		}


		private static Dictionary<String, Int64> CountCharacterFrequency(String text, Boolean caseSensitive = false) {
			Dictionary<String, Int64> returnDict = new Dictionary<String, Int64>();
			if (!caseSensitive) { text = text.ToUpper(); }
			foreach (Char curChar in text) {
				String encoded = DataStore.Base64Encode(curChar);
				if (returnDict.ContainsKey(encoded)) {
					returnDict[encoded]++;
				} else {
					returnDict.Add(encoded, 1);
				}
			}
			return returnDict;
		}

		private static Dictionary<String, Int64> CountWordFrequency(String text, Boolean caseSensitive = false) {
			Dictionary<String, Int64> returnDict = new Dictionary<String, Int64>();
			if (!caseSensitive) { text = text.ToUpper(); }
			foreach (String curWord in text.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries)) {
				String encoded = DataStore.Base64Encode(curWord);
				if (returnDict.ContainsKey(encoded)) {
					returnDict[encoded]++;
				} else {
					returnDict.Add(encoded, 1);
				}
			}
			return returnDict;
		}
	}
}
