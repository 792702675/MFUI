using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Configuration
{
    public class TypeAttribute : Attribute
    {
        public string Value { get; set; }
        public TypeAttribute(string value)
        {
            Value = value;
        }
    }

    public class TextAreaAttribute : TypeAttribute
    {
        public TextAreaAttribute() : base("TextArea") { }
    }
}
