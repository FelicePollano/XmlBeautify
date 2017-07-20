using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace check_schema
{
    class Program
    {
        static Logger logger;
        
        static void Main(string[] args)
        {
            logger = new Logger();
            Console.Error.WriteLine("check xml against xsd");
            Console.Error.WriteLine("2017 Felice Pollano");
            if (args.Length == 0)
                Console.Error.WriteLine("Specify at least one file to operate on. xsd files will automatically treated as schema!");
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
                XmlSchemaSet set = new XmlSchemaSet();
                List<string> toValidate = new List<string>();
                foreach (String fileName in fileList)
                {
                    try
                    {
                        if (string.Compare(Path.GetExtension(fileName), ".xsd", true) == 0)
                        {
                            Console.Error.WriteLine("Collecting {0} as schema file", fileName);
                            set.Add(XmlSchema.Read(XmlReader.Create(fileName), new ValidationEventHandler((ss, e) => OnValidateReadSchema(ss, e))));
                        }
                        else
                        {
                            toValidate.Add(fileName);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e.Message);
                    }
                }
                //validate
                foreach (string fileName in fileList)
                {
                    ValidateOne(set, fileName);
                }
                Console.Write(logger.ToString());
            }
        }

        private static void ValidateOne(XmlSchemaSet set, string fileName)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.ValidationEventHandler += new ValidationEventHandler((o, e) => OnValidate(o, e));  // Your callback...
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas.Add(set);
            settings.ValidationFlags =
              XmlSchemaValidationFlags.ReportValidationWarnings |
              XmlSchemaValidationFlags.ProcessIdentityConstraints |
              XmlSchemaValidationFlags.ProcessInlineSchema |
              XmlSchemaValidationFlags.ProcessSchemaLocation;

            // Wrap document in an XmlNodeReader and run validation on top of that
            try
            {
                using(var fs = new FileStream(fileName,FileMode.Open,FileAccess.Read))
                using (XmlReader validatingReader = XmlReader.Create(fs, settings))
                {
                    while (validatingReader.Read()) { /* just loop through document */ }
                }
            }
            catch (XmlException e)
            {
                logger.Error(string.Format("Errore in lettura Linea:{0} Posizione:{1}", e.LineNumber, e.LinePosition));
            }
            
        }

        private static void OnValidate(object o, ValidationEventArgs vae)
        {
            if (vae.Severity == XmlSeverityType.Error)
                logger.Error(vae.Message);
            else
                logger.Warn(vae.Message);
            
        }

        private static void OnValidateReadSchema(object ss, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Error)
                logger.Error(e.Message);
            else
                logger.Warn(e.Message);
        }
    }
}
