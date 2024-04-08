using Code2.Tools.Csv.Internals;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Code2.Tools.Csv
{
	public class CsvReader<T> : CsvReader, ICsvReader<T>
		where T : class, new()
	{
		public CsvReader(string filePath) : this(filePath, new CsvReaderOptions(), new FileSystem()) { }
		public CsvReader(string filePath, CsvReaderOptions options) : this(filePath, options, new FileSystem()) { }
		internal CsvReader(string filePath, CsvReaderOptions options, IFileSystem fileSystem) : this(fileSystem.FileOpenText(filePath), options, disposeReader: true) { }
		public CsvReader(TextReader reader) : this(reader, new CsvReaderOptions()) { }
		public CsvReader(TextReader reader, CsvReaderOptions options, bool disposeReader = false) : base(reader, options, disposeReader)
		{
			Deserializer = DefaultDeserializer;
		}

		private PropertyInfo?[]? _properties;

		public event EventHandler<UnhandledExceptionEventArgs>? Error;
		public Func<string[], int, T> Deserializer { get; set; }

		public T[] ReadObjects(int amount)
		{
			int start = CurrentLineNumber + 1;
			string[][] lines = ReadLines(amount);
			return lines.AsParallel().AsOrdered().Select((x, i) => Deserializer(x, start + i)).ToArray();
		}

		public T ReadObject()
		{
			string[]? line = ReadLine();
			if (line is null) return default!;
			return Deserializer(line, CurrentLineNumber);
		}

		protected virtual T DefaultDeserializer(string[] line, int lineNumber)
		{
			PropertyInfo?[] properties = GetProperties();

			T newObject = Activator.CreateInstance<T>();
			for (int i = 0; i < properties.Length && i < line.Length; i++)
			{
				if (properties[i] is null) continue;
				SetValueOrThrow(properties[i]!, line[i], newObject, lineNumber);
			}
			return newObject;
		}

		private void SetValueOrThrow(PropertyInfo property, string cellValue, object instance, int lineNumber)
		{
			Type type = property.PropertyType;
			Type? innerType = Nullable.GetUnderlyingType(type);
			bool isNullable = innerType is not null;
			type = innerType ?? type;
			object? value = null;
			string? errorMessage = null;

			if (!(isNullable && cellValue == string.Empty))
			{
				if (type == typeof(string))
				{
					value = cellValue;
				}
				else if (type == typeof(DateTime) && Options.DateFormat is not null)
				{
					if (DateTime.TryParseExact(cellValue, Options.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
					{
						value = dateTime;
					}
					else
					{
						errorMessage = "Failed to parse datetime";
					}
				}
				else
				{
					try
					{
						value = Convert.ChangeType(cellValue, property.PropertyType);
					}
					catch (Exception ex)
					{
						errorMessage = ex.Message;
					}
				}
			}
			if (errorMessage is not null)
			{
				OnError($"Error setting value for {property.Name}, csv line number {lineNumber}, reason:{errorMessage}");
			}

			property.SetValue(instance, value);
		}

		private PropertyInfo?[] GetProperties()
		{
			if (_properties is null)
			{
				PropertyInfo[] properties = typeof(T).GetProperties().Where(x => x.CanWrite).ToArray();
				_properties = Options.Header is null ?
				properties :
					Options.Header.Select(x => x.ToLower()).Select(x => x == string.Empty ? null :
						properties.FirstOrDefault(p => new[] { x, x.Replace("_", "") }.Contains(p.Name, StringComparer.InvariantCultureIgnoreCase))
					).ToArray();
			}
			return _properties;
		}

		private void OnError(string errorMessage)
		{
			InvalidOperationException exception = new InvalidOperationException(errorMessage);
			if (Error is null) throw exception;
			Error.Invoke(this, new UnhandledExceptionEventArgs(exception, false));
		}
	}
}
