using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code2.Tools.Csv.Internals
{
	internal class FileSystem : IFileSystem
	{
		public StreamReader FileOpenText(string path)
		  => File.OpenText(path);

	}
}
