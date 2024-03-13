using System.IO;

namespace Code2.Tools.Csv.Internals
{
	internal class FileSystem : IFileSystem
	{
		public StreamReader FileOpenText(string path)
		  => File.OpenText(path);

	}
}
