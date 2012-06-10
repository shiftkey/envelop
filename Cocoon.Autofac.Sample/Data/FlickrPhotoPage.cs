using System.Xml.Serialization;

namespace Cocoon.Autofac.Sample.Data
{
    [XmlRoot("photos")]
    public class FlickrPhotoPage
    {
        // *** Fields ***

        [XmlAttribute("page")]
        public int Page;

        [XmlAttribute("pages")]
        public int Pages;

        [XmlAttribute("perpage")]
        public int PerPage;

        [XmlElement("photo")]
        public FlickrPhoto[] Photos;

        [XmlAttribute("total")]
        public int Total;
    }
}
