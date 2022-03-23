using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Helpers
{
    public class EditorHelper
    {
        public static void Register<T, TC>()
        {
            Attribute[] attribute = new Attribute[1];
            TypeConverterAttribute typeConverterAttribute = new TypeConverterAttribute(typeof(TC));
            attribute[0] = typeConverterAttribute;
            TypeDescriptor.AddAttributes(typeof(T), attribute);
        }
    }
}
