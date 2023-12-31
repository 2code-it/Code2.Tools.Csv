﻿namespace Code2.Tools.Csv
{
	public interface ICsvReaderFactory
	{
		ICsvReader Create(string filePath, CsvReaderOptions? options = null);
		ICsvReader<T> Create<T>(string filePath, CsvReaderOptions? options = null);
		ICsvReader Create(TextReader textReader, CsvReaderOptions? options = null);
		ICsvReader<T> Create<T>(TextReader textReader, CsvReaderOptions? options = null);
	}
}
