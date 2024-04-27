using Code2.Tools.CsvTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace Code2.Tools.Csv.Tests
{
	[TestClass]
	public class CsvReaderTests
	{
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
			using StreamReader reader = CsvUtil.GetReaderFromLines(lines);
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
			string[] lines = new[] { "1,2,3,\"test1\r\ntest2\"" };
			using StreamReader reader = CsvUtil.GetReaderFromLines(lines);
			CsvReader csvReader = new CsvReader(reader);

			string[] line = csvReader.ReadLine()!;

			Assert.AreEqual(4, line.Length);
			Assert.AreNotEqual(-1, line[3].IndexOf("\r\n"));
			
		}

		[TestMethod]
		public void ReadLine_When_QuotedAndQuoteInCell_Expect_QuoteInCell()
		{
			string[] lines = new[] { "1,2,3,\"test1\"\"test2\"" };
			using StreamReader reader = CsvUtil.GetReaderFromLines(lines);			
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
			using StreamReader reader = CsvUtil.GetReaderFromLines(lines);
			CsvReader csvReader = new CsvReader(reader);

			string[][] csvlines = csvReader.ReadLines();

			Assert.AreEqual(1, csvlines.Length);
		}

		[TestMethod]
		public void ReadLine_When_QuotedAndKeepQuotes_Expect_QuotedCell()
		{
			string[] lines = new[] { "1,2,3,\"test1\"\"test2\"" };
			using StreamReader reader = CsvUtil.GetReaderFromLines(lines);			
			CsvReader csvReader = new CsvReader(reader);
			csvReader.Options.KeepEnclosureQuotes = true;

			string[] line = csvReader.ReadLine()!;

			Assert.AreEqual('"', line[3].First());
			Assert.AreEqual('"', line[3].Last());
		}

		[TestMethod]
		public void ReadLine_When_UsingAlternateDelimiter_Expect_CellCount()
		{
			string[] lines = new[] { "1|2|3|\"test1\"\"test2\"" };
			using StreamReader reader = CsvUtil.GetReaderFromLines(lines);
			CsvReader csvReader = new CsvReader(reader);
			csvReader.Options.Delimiter = '|';

			string[] line = csvReader.ReadLine()!;

			Assert.AreEqual(4, line.Length);
		}

		[TestMethod, ExpectedException(typeof(InvalidOperationException))]
		public void ReadLine_When_OptionsExplicitAndNonMatchingHeader_Expect_Exception()
		{
			string[] lines = new[] { "1,2,3,4" };
			using StreamReader reader = CsvUtil.GetReaderFromLines(lines);			
			CsvReader csvReader = new CsvReader(reader);
			csvReader.Options.Explicit = true;
			csvReader.Options.Header = "a,b,c,d,e,f".Split(',');

			string[] line = csvReader.ReadLine()!;
		}

		[TestMethod]
		public void ReadLine_When_OptionsExplicitAndNoHeader_Expect_NoException()
		{
			string[] lines = new[] { "1,2,3,4" };
			using StreamReader reader = CsvUtil.GetReaderFromLines(lines);
			CsvReader csvReader = new CsvReader(reader);
			csvReader.Options.Explicit = true;

			string[] line = csvReader.ReadLine()!;
		}

		[TestMethod]
		public void ReadLine_When_OptionsHasHeaderRowSet_Expect_HeaderOptionSet()
		{
			string[] lines = new[] { "first,second,third,fourth", "1,2,3,4" };
			using StreamReader reader = CsvUtil.GetReaderFromLines(lines);
			CsvReader csvReader = new CsvReader(reader);
			csvReader.Options.HasHeaderRow = true;

			string[] line = csvReader.ReadLine()!;

			Assert.AreEqual("second", csvReader.Options.Header?[1]);
			Assert.AreEqual("3", line[2]);
		}


	}
}