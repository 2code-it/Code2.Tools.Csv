namespace Code2.Tools.Csv
{
	public interface ICsvReader
	{
		int CurrentLineNumber { get; }
		bool EndOfStream { get; }
		CsvReaderOptions Options { get; }

		string[]? ReadLine();
		string[][] ReadLines(int amount = 1000);
	}
}