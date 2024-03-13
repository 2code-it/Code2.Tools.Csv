using System.IO;

namespace Code2.Tools.CsvTests
{
	public class CsvUtil
	{
		public static StreamReader GetReaderFromLines(string[] lines)
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
