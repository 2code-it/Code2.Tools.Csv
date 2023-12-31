﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Code2.Tools.Csv.Tests.Assets;

namespace Code2.Tools.Csv.Tests
{
	[TestClass]
	public class CsvReaderTests
	{
		private readonly CsvService _csvService = new CsvService();

		[TestMethod]
		public void ReadLine_When_CommentLine_Expect_LineSkipped()
		{
			string[] lines = new[]
			{
				"1,2,3",
				"2,1,3",
				"# comment line",
				"3,1,2"
			};
			using StreamReader reader = _csvService.GetStreamReader(lines);
			CsvReader csvReader = new CsvReader(reader);
			int count = 0;
			while (!csvReader.EndOfStream)
			{
				csvReader.ReadLine();
				count++;
			}

			Assert.AreEqual(3, count);
			Assert.AreEqual(4, csvReader.CurrentLineNumber);
		}

		[TestMethod]
		public void ReadLine_When_QuotedAndContainsNewLine_Expect_NewLineInCell()
		{
			string[] lines = new[]{"1,2,3,\"test1\r\ntest2\""};
			using StreamReader reader = _csvService.GetStreamReader(lines);
			CsvReader csvReader = new CsvReader(reader);
			string[] line = csvReader.ReadLine()!;

			Assert.AreEqual(4, line.Length);
			Assert.AreNotEqual(-1, line[3].IndexOf("\r\n"));
		}

		[TestMethod]
		public void ReadLine_When_QuotedAndQuoteInCell_Expect_QuoteInCell()
		{
			string[] lines = new[]{"1,2,3,\"test1\"\"test2\""};
			using StreamReader reader = _csvService.GetStreamReader(lines);
			CsvReader csvReader = new CsvReader(reader);
			string[] line = csvReader.ReadLine()!;

			Assert.AreEqual(4, line.Length);
			Assert.AreNotEqual(-1, line[3].IndexOf("\""));
		}

		[TestMethod]
		public void ReadLine_When_NotQuotedAndQuoteInCell_Expect_SingleLine()
		{
			string[] lines = new[]
			{
				"1,2,3,test1\"test2",
				"1,2,3,test1",
			};
			using StreamReader reader = _csvService.GetStreamReader(lines);
			CsvReader csvReader = new CsvReader(reader);
			string[][] csvlines = csvReader.ReadLines();

			Assert.AreEqual(1, csvlines.Length);
		}

		[TestMethod]
		public void ReadLine_When_QuotedAndKeepQuotes_Expect_QuotedCell()
		{
			string[] lines = new[]{"1,2,3,\"test1\"\"test2\""};
			using StreamReader reader = _csvService.GetStreamReader(lines);
			CsvReader csvReader = new CsvReader(reader);
			csvReader.Options.KeepEnclosureQuotes = true;
			string[] line = csvReader.ReadLine()!;

			Assert.AreEqual('"', line[3].First());
			Assert.AreEqual('"', line[3].Last());
		}

		[TestMethod]
		public void ReadLine_When_UsingAlternateDelimiter_Expect_CellCount()
		{
			string[] lines = new[]{"1|2|3|\"test1\"\"test2\""};
			using StreamReader reader = _csvService.GetStreamReader(lines);
			CsvReader csvReader = new CsvReader(reader);
			csvReader.Options.Delimiter = '|';
			string[] line = csvReader.ReadLine()!;

			Assert.AreEqual(4, line.Length);
		}

		[TestMethod, ExpectedException(typeof(InvalidOperationException))]
		public void ReadLine_When_OptionsExplicitAndNonMatchingHeader_Expect_Exception()
		{
			string[] lines = new[] { "1,2,3,4" };
			using StreamReader reader = _csvService.GetStreamReader(lines);
			CsvReader csvReader = new CsvReader(reader);
			csvReader.Options.Explicit = true;
			csvReader.Options.Header = "a,b,c,d,e,f".Split(',');

			string[] line = csvReader.ReadLine()!;
		}

		[TestMethod]
		public void ReadLine_When_OptionsExplicitAndNoHeader_Expect_NoException()
		{
			string[] lines = new[] { "1,2,3,4" };
			using StreamReader reader = _csvService.GetStreamReader(lines);
			CsvReader csvReader = new CsvReader(reader);
			csvReader.Options.Explicit = true;

			string[] line = csvReader.ReadLine()!;
		}


	}
}