using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace check_schema
{
    class Logger
    {
            StringBuilder sb;
            public Logger()
            {
                sb = new StringBuilder();
            }
            public void Error(string s)
            {
                sb.AppendLine("ERROR:" + s);
            }
            public void Warn(string s)
            {
                sb.AppendLine("WARN:" + s);
            }
            public override string ToString()
            {
                return sb.ToString();
            }
     }
}
