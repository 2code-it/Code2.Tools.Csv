using Microsoft.VisualStudio.TestTools.UnitTesting;
using Code2.Tools.Csv.Tests.Assets;
using System.IO;
using System;

namespace Code2.Tools.Csv.Tests
{
	[TestClass]
	public class CsvReaderOfTTests
	{
		private readonly CsvService _csvService = new CsvService();

		[TestMethod]
		public void ReadObject_When_CommentLine_Expect_LineSkipped()
		{
			string[] lines = new[]
			{
				"1,first1,last0,20",
				"#next line",
				"2,first1,last2,10",
			};
			using StreamReader reader = _csvService.GetStreamReader(lines);
			CsvReader<TestItem> csvReader = new CsvReader<TestItem>(reader);

			var items = csvReader.ReadObjects(10);

			Assert.AreEqual(2, items.Length);
			Assert.AreEqual(3, csvReader.CurrentLineNumber);
		}

		[TestMethod, ExpectedException(typeof(AggregateException))]
		public void ReadObject_When_NonMatchingValues_Expect_Exception()
		{
			string[] lines = new[]
			{
				"first1,last0,20,1",
				"first1,last2,10,2",
			};
			using StreamReader reader = _csvService.GetStreamReader(lines);
			CsvReader<TestItem> csvReader = new CsvReader<TestItem>(reader);

			var items = csvReader.ReadObjects(10);
		}

		[TestMethod]
		public void ReadObject_When_NonMatchingValuesAndCorrectingHeader_Expect_ItemCount()
		{
			string[] lines = new[]
			{
				"first1,last0,20,1",
				"first1,last2,10,2",
			};
			using StreamReader reader = _csvService.GetStreamReader(lines);
			CsvReader<TestItem> csvReader = new CsvReader<TestItem>(reader);
			csvReader.Options.Header = new[] { "Firstname", "Lastname", "Total", "Id" };

			var items = csvReader.ReadObjects(10);

			Assert.AreEqual(2, items.Length);
		}

		[TestMethod]
		public void ReadObject_When_UsingCustomDeserializer_Expect_CustomItem()
		{
			string[] lines = new[]
			{
				"1,first1,last0,20",
				"2,first1,last2,10",
			};
			using StreamReader reader = _csvService.GetStreamReader(lines);
			CsvReader<TestItem> csvReader = new CsvReader<TestItem>(reader);
			csvReader.Deserializer = (line, lineNumber) =>
			{
				TestItem item = new TestItem();
				item.Id = Convert.ToInt32(line[0]);
				item.Firstname = line[1];
				item.Lastname = line[2];
				item.Total = Convert.ToInt32(line[3]);
				return item;
			};

			var items = csvReader.ReadObjects(10);

			Assert.AreEqual(2, items.Length);
			Assert.AreEqual(1, items[0].Id);
		}
	}
}