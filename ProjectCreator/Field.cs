using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCreator
{
    public class Field
    {
        public string FieldName { get; set; }
        public string FieldType { get; set; }

        public override string ToString()
        {
            return FieldName + " : " + FieldType;
        }
    }
}
