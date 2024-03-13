using System.IO;

namespace Code2.Tools.Csv
{
	public interface ICsvReaderFactory
	{
		ICsvReader Create(string filePath, CsvReaderOptions? options = null);
		ICsvReader<T> Create<T>(string filePath, CsvReaderOptions? options = null) where T : class, new();
		ICsvReader Create(TextReader textReader, CsvReaderOptions? options = null);
		ICsvReader<T> Create<T>(TextReader textReader, CsvReaderOptions? options = null) where T : class, new();
	}
}
