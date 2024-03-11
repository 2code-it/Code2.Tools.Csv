using System;

namespace Code2.Tools.Csv
{
	public interface ICsvReader<T> : ICsvReader
		where T : class
	{
		Func<string[], int, T> Deserializer { get; set; }

		T ReadObject();
		T[] ReadObjects(int amount);
	}
}