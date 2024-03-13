using System.IO;

namespace Code2.Tools.Csv.Internals
{
	internal interface IFileSystem
	{
		StreamReader FileOpenText(string path);
	}
}