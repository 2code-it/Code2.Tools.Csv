using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Code2.Tools.Csv.Tests.Assets
{
	internal class CsvService
	{
		public StreamReader GetStreamReader(IEnumerable<string> lines)
		{
			MemoryStream ms = new MemoryStream();
			StreamWriter writer = new StreamWriter(ms);
			foreach (string line in lines)
			{
				writer.WriteLine(line);
			}
			writer.Flush();

			ms.Position = 0;

			return new StreamReader(ms);
		}
	}
}
