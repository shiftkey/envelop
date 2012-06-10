using System.Xml.Serialization;

namespace Cocoon.Autofac.Sample.Data
{
    public class FlickrPhoto
    {
        // *** Fields ***

        [XmlAttribute("farm")]
        public string Farm;

        [XmlAttribute("id")]
        public string Id;

        [XmlAttribute("secret")]
        public string Secret;

        [XmlAttribute("server")]
        public string Server;

        // *** Properties ***

        public string Image
        {
            get
            {
                return string.Format("http://farm{0}.static.flickr.com/{1}/{2}_{3}_m.jpg", Farm, Server, Id, Secret);
            }
        }

        [XmlAttribute("title")]
        public string Title
        {
            get;
            set;
        }
    }
}
