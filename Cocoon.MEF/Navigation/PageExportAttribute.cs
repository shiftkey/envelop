using System;
using System.Composition;

namespace Cocoon.Navigation
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PageExportAttribute : ExportAttribute
    {
        // *** Constructors ***

        public PageExportAttribute(string pageName)
            : base("CocoonPage", typeof(object))
        {
            this.PageName = pageName;
        }

        // *** Properties ***

        public string PageName { get; private set; }
    }
}
