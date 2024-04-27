using Code2.Tools.CsvTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Code2.Tools.Csv.Tests
{
	[TestClass]
	public class CsvReaderOfTTests
	{
		[TestMethod]
		public void ReadObject_When_CommentLine_Expect_LineSkipped()
		{
			string[] lines = new[]
			{
				"1,first1,last0,20",
				"#next line",
				"2,first1,last2,10",
			};
			using StreamReader reader = CsvUtil.GetReaderFromLines(lines);
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
			using StreamReader reader = CsvUtil.GetReaderFromLines(lines);
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
			using StreamReader reader = CsvUtil.GetReaderFromLines(lines);
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
			using StreamReader reader = CsvUtil.GetReaderFromLines(lines);
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

		[TestMethod]
		public void ReadObject_When_UsingMatchingDateFormat_Expect_DeserializationSuccess()
		{
			string[] lines = new[]
			{
				"1,first1,last0,20,20010101T00:00:00",
				"2,first1,last2,10,20000102T23:00:00",
			};
			using StreamReader reader = CsvUtil.GetReaderFromLines(lines);
			CsvReaderOptions options = new CsvReaderOptions { DateFormat = "yyyyMMddTHH:mm:ss" };
			CsvReader<TestItem> csvReader = new CsvReader<TestItem>(reader, options);

			var items = csvReader.ReadObjects(10);

			Assert.AreEqual(2, items.Length);
			Assert.AreEqual(1, items[0].Id);
		}

		[TestMethod]
		[ExpectedException(typeof(AggregateException))]
		public void ReadObject_When_UsingNonMatchingDateFormat_Expect_DeserializationException()
		{
			string[] lines = new[]
			{
				"1,first1,last0,20,20010101T00:00:00",
				"2,first1,last2,10,20000102T23:00:00",
			};
			using StreamReader reader = CsvUtil.GetReaderFromLines(lines);
			CsvReaderOptions options = new CsvReaderOptions { DateFormat = "yyyyMMdd HH:mm:ss" };
			CsvReader<TestItem> csvReader = new CsvReader<TestItem>(reader, options);

			var items = csvReader.ReadObjects(10);

			Assert.AreEqual(2, items.Length);
			Assert.AreEqual(1, items[0].Id);
		}

		[TestMethod]
		public void ReadObject_When_IgnoreEmptyOptionSet_Expect_EmptyCellsIgnored()
		{
			string[] lines = new[]
			{
				"1,first1,,20,20010101T00:00:00",
				"2,first1,last2,,20000102T23:00:00",
			};
			using StreamReader reader = CsvUtil.GetReaderFromLines(lines);
			CsvReaderOptions options = new CsvReaderOptions { DateFormat = "yyyyMMddTHH:mm:ss", IgnoreEmptyWhenDeserializing = true };
			CsvReader<TestItem> csvReader = new CsvReader<TestItem>(reader, options);

			var items = csvReader.ReadObjects(10);

			Assert.IsNull(items[0].Lastname);
			Assert.AreEqual(0, items[1].Total);
		}

		private class TestItem
		{
			public int Id { get; set; }
			public string? Firstname { get; set; }
			public string? Lastname { get; set; }
			public int Total { get; set; }
			public DateTime? Created { get; set; }
		}
	}
}