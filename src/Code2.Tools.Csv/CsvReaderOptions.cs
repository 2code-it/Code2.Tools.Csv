namespace Code2.Tools.Csv
{
	public class CsvReaderOptions
	{
		public string[]? Header { get; set; }
		public bool HasHeaderRow { get; set; }
		public char Delimiter { get; set; } = ',';
		public bool KeepEnclosureQuotes { get; set; }
		public bool Explicit { get; set; }
		public string? DateFormat { get; set; }
		public bool IgnoreEmptyWhenDeserializing { get; set; }
	}
}
