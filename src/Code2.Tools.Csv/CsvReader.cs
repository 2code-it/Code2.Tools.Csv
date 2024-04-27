using Code2.Tools.Csv.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Code2.Tools.Csv
{
	public class CsvReader : ICsvReader
	{
		public CsvReader(string filePath, CsvReaderOptions? options = null) : this(filePath, options, new FileSystem()) { }
		internal CsvReader(string filePath, CsvReaderOptions? options, IFileSystem fileSystem) : this(fileSystem.FileOpenText(filePath), options, true) { }
		public CsvReader(TextReader reader, CsvReaderOptions? options = null, bool disposeReader = false)
		{
			_reader = reader;
			_disposeReader = disposeReader;
			CopyOptionProperties(options, _options);
		}

		private readonly TextReader _reader;
		private readonly CsvReaderOptions _options = new CsvReaderOptions();


		private readonly List<string> _line = new List<string>();
		private readonly StringBuilder _cell = new StringBuilder();
		private readonly bool _disposeReader;

		private int _currentChar;
		private bool _isQuoteOpen;


		public int CurrentLineNumber { get; private set; }
		public bool EndOfStream { get; private set; }
		public CsvReaderOptions Options => _options;


		public string[][] ReadLines(int amount = 1000)
		{
			List<string[]> lines = new List<string[]>();
			while (!EndOfStream)
			{
				string[]? line = ReadLine();
				if (line is null) continue;
				lines.Add(line);
				if (lines.Count == amount) break;
			}
			return lines.ToArray();
		}

		public string[]? ReadLine()
		{
			CurrentLineNumber++;

			if (_reader.Peek() == '#')
			{
				_reader.ReadLine();
				return ReadLine();
			}

			_cell.Clear();
			_line.Clear();
			_isQuoteOpen = false;

			while ((_currentChar = _reader.Read()) != -1)
			{
				if (!_isQuoteOpen && _currentChar == Options.Delimiter)
				{
					_line.Add(_cell.ToString());
					_cell.Clear();
				}
				else if (!_isQuoteOpen && (_currentChar == '\r' || _currentChar == '\n'))
				{
					if (_currentChar == '\r' && _reader.Peek() == '\n') _reader.Read();
					_line.Add(_cell.ToString());
					_cell.Clear();
					break;
				}
				else if (_currentChar == '"')
				{
					if (_isQuoteOpen && _reader.Peek() == '"')
					{
						_cell.Append((char)_currentChar);
						_reader.Read();
					}
					else
					{
						_isQuoteOpen = !_isQuoteOpen;
						if (Options.KeepEnclosureQuotes) _cell.Append((char)_currentChar);
					}
				}
				else
				{
					_cell.Append((char)_currentChar);
				}
			}

			EndOfStream = _reader.Peek() == -1;
			if (Options.Explicit && Options.Header is not null && Options.Header.Length != _line.Count)
			{
				throw new InvalidOperationException($"Header mismatch line: {CurrentLineNumber}");
			}
			if(CurrentLineNumber == 1 && Options.HasHeaderRow)
			{
				Options.Header ??= _line.ToArray();
				return ReadLine();
			}
			return _line.Count == 0 ? null : _line.ToArray();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize", Justification = "Cascade call to reader")]
		public void Dispose()
		{
			if (_disposeReader)
			{
				_reader.Dispose();
			}
		}

		private void CopyOptionProperties(CsvReaderOptions? source, CsvReaderOptions target)
		{
			if (source is null) return;
			var properties = typeof(CsvReaderOptions).GetProperties().Where(x => x.CanWrite);
			foreach (var property in properties)
			{
				property.SetValue(target, property.GetValue(source));
			}
		}
	}
}
