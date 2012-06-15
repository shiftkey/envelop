using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Cocoon.Sample.Data
{
    public partial class FlickrApi
    {
        // *** IMPORTANT : YOU MUST ENTER YOUR API KEY BELOW! ***
        public const string FLICKR_API_KEY = "ENTER YOUR FLICKR API KEY HERE";

        public async Task<FlickrPhotoPage> GetInterestingPhotos(int page, int perPage)
        {
            // Construct the request URI

            string flickrUri = string.Format("http://api.flickr.com/services/rest/?method=flickr.interestingness.getList&api_key={0}&page={1}&per_page={2}",
                                                FLICKR_API_KEY, page, perPage);

            // Call the Flickr web service

            HttpClient httpClient = new HttpClient();
            using (HttpResponseMessage response = await httpClient.GetAsync(flickrUri))
            {
                // Parse the response message from XML

                Stream responseStream = await response.Content.ReadAsStreamAsync();
                XDocument responseDocument = XDocument.Load(responseStream);
                XElement rspElement = responseDocument.Root;

                // Check if there was an error

                if (rspElement.Attribute("stat").Value == "fail")
                {
                    throw new NotImplementedException();
                }

                // Otherwise parse the result

                XElement payloadElement = rspElement.Elements().First();

                XmlSerializer serializer = new XmlSerializer(typeof(FlickrPhotoPage));
                object result = serializer.Deserialize(payloadElement.CreateReader());

                return (FlickrPhotoPage)result;
            }
        }
    }
}
