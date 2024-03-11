using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Code2.Tools.Csv.Internals;

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

		private PropertyInfo[] _properties;

		public Func<string[], int, T> Deserializer { get; set; }

		public T[] ReadObjects(int amount)
		{
			int start = CurrentLineNumber+1;
			string[][] lines = ReadLines(amount);
			return lines.AsParallel().AsOrdered().Select((x, i) => Deserializer(x, start + i)).ToArray();
		}

		public T ReadObject()
		{
			string[] line = ReadLine();
			if (line is null) return default;
			return Deserializer(line, CurrentLineNumber);
		}

		protected virtual T DefaultDeserializer(string[] line, int lineNumber)
		{
			PropertyInfo[] properties = GetProperties();

			T newObject = Activator.CreateInstance<T>();
			for (int i = 0; i < properties.Length && i < line.Length; i++)
			{
				if (properties[i] is null || string.IsNullOrEmpty(line[i])) continue;
				TrySetValue(properties[i], line[i], newObject, lineNumber);
			}
			return newObject;
		}

		private void TrySetValue(PropertyInfo property, string cellValue, object instance, int lineNumber)
		{
			try
			{
				object value = property.PropertyType == typeof(string) ? cellValue : Convert.ChangeType(cellValue, property.PropertyType);
				property.SetValue(instance, value);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Error setting value for {property.Name}, csv line number {lineNumber}, reason: {ex.Message}");
			}
		}

		private PropertyInfo[] GetProperties()
		{
			if (_properties is null)
			{
				PropertyInfo[] properties = typeof(T).GetProperties().Where(x => x.CanWrite).ToArray();
				_properties = Options.Header is null ?
				properties :
					Options.Header.Select(x => x.ToLower()).Select(x => x == string.Empty ? null :
						properties.FirstOrDefault(p => new[] { x, x.Replace("_", "") }.Contains(p.Name.ToLower()))
					).ToArray();
			}
			return _properties;
		}
	}
}
