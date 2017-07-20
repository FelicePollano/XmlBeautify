using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace XmlBeautify
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Error.WriteLine("Make xml file formatted in a readable way");
			Console.Error.WriteLine("2014 Felice Pollano");
			if (args.Length == 0)
				Console.Error.WriteLine("Specify at least one file to beautify.");
			foreach (String argString in args)
			{
				// Split into path and wildcard
				int lastBackslashPos = argString.LastIndexOf('\\') + 1;
				var path = argString.Substring(0, lastBackslashPos);
				var filenameOnly = argString.Substring(lastBackslashPos,
										   argString.Length - lastBackslashPos);
				if (string.IsNullOrEmpty(path))
					path = ".";
				String[] fileList = System.IO.Directory.GetFiles(path, filenameOnly);
				foreach (String fileName in fileList)
				{
					try
					{
						string outfile = Path.ChangeExtension(Path.GetFileNameWithoutExtension(fileName).Trim('.') + "_o",Path.GetExtension(fileName));
						Console.Error.WriteLine("Make beauty xml for:{0} in {1}", fileName, outfile);
						XmlDocument doc = new XmlDocument();
						doc.Load(fileName);
						Beautify(doc, outfile);
					}
					catch (Exception e)
					{
						Console.Error.WriteLine(e.Message);
					}
				}
			}
		}
		static public string Beautify(XmlDocument doc,string outfile)
		{
			StringBuilder sb = new StringBuilder();
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.IndentChars = "  ";
			settings.NewLineChars = "\r\n";
			settings.NewLineHandling = NewLineHandling.Replace;
			using (XmlWriter writer = XmlWriter.Create(outfile, settings))
			{
				doc.Save(writer);
			}
			return sb.ToString();
		}
	}
}
