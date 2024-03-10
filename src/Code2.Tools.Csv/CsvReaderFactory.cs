using System.IO;

namespace Code2.Tools.Csv
{
	public class CsvReaderFactory : ICsvReaderFactory
	{
		public ICsvReader Create(string filePath, CsvReaderOptions? options = null)
			=> new CsvReader(filePath, options ?? new CsvReaderOptions());
		

		public ICsvReader<T> Create<T>(string filePath, CsvReaderOptions? options = null)
			=> new CsvReader<T>(filePath, options ?? new CsvReaderOptions());
		

		public ICsvReader Create(TextReader textReader, CsvReaderOptions? options = null)
			=> new CsvReader(textReader, options ?? new CsvReaderOptions());
		

		public ICsvReader<T> Create<T>(TextReader textReader, CsvReaderOptions? options)
			=> new CsvReader<T>(textReader, options ?? new CsvReaderOptions());
		
	}
}
