using System.IO;

namespace Code2.Tools.Csv
{
	public class CsvReaderFactory : ICsvReaderFactory
	{
		public ICsvReader Create(string filePath, CsvReaderOptions? options = null)
			=> new CsvReader(filePath, options);


		public ICsvReader<T> Create<T>(string filePath, CsvReaderOptions? options = null) where T : class, new()
			=> new CsvReader<T>(filePath, options);


		public ICsvReader Create(TextReader textReader, CsvReaderOptions? options = null)
			=> new CsvReader(textReader, options);


		public ICsvReader<T> Create<T>(TextReader textReader, CsvReaderOptions? options) where T : class, new()
			=> new CsvReader<T>(textReader, options);

	}
}
