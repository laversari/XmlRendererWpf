using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlRendererWpf.Models
{
    public class InsertData
    {
        public InsertData(string column, string value)
        {
            Column = column;
            Value = value;
        }

        public string Column { get; private set; }
        public string Value { get; private set; }
    }
}
