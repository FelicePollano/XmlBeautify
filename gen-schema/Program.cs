using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace gen_schema
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Error.WriteLine("generate xsd without funny dataset mess");
			Console.Error.WriteLine("2014 Felice Pollano");
			if (args.Length == 0)
				Console.Error.WriteLine("Specify at least one file to operate on.");
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
						string outfile = Path.ChangeExtension(Path.GetFileNameWithoutExtension(fileName) ,"xsd");
						Console.Error.WriteLine("Make xsd schema for:{0} in {1}", fileName, outfile);
						using(var sw = new StreamWriter(outfile))
						using(var sr = new StreamReader(fileName))
						{
							XmlSchemaInference infer = new XmlSchemaInference();
                            
							var schemaSet = infer.InferSchema(XmlReader.Create(sr));
							foreach (XmlSchema schema in schemaSet.Schemas())
							{
								schema.Write(sw);
							}
						}
					}
					catch (Exception e)
					{
						Console.Error.WriteLine(e.Message);
					}
				}
			}
		}
	}
}
