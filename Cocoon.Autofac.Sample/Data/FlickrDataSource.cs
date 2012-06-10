using System.Collections.Generic;
using Cocoon.Data;

namespace Cocoon.Autofac.Sample.Data
{
    public class FlickrDataSource
    {
        // *** Fields ***

        private FlickrApi flickrApi = new FlickrApi();
        private IList<FlickrPhoto> interestingPhotos;

        // *** Methods ***

        public IList<FlickrPhoto> GetInterestingPhotos()
        {
            // Create a single instance of the interesting photos list
            // This can then be shared between multiple view models

            if (interestingPhotos == null)
            {
                InterestingPhotosDataListSource dataListSource = new InterestingPhotosDataListSource(flickrApi);
                interestingPhotos = (IList<FlickrPhoto>)new VirtualizingDataList<FlickrPhoto>(dataListSource);
            }

            return interestingPhotos;
        }
    }
}
