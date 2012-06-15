using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cocoon.Navigation
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ViewModelExportAttribute : ExportAttribute
    {
        // *** Constructors ***

        public ViewModelExportAttribute(string pageName)
            : base("CocoonViewModel", typeof(object))
        {
            this.PageName = pageName;
        }

        // *** Properties ***

        public string PageName { get; private set; }
    }
}
