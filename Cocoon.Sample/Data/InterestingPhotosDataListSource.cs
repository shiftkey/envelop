using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocoon.Data;

namespace Cocoon.Sample.Data
{
    public class InterestingPhotosDataListSource : PagedDataListSource<FlickrPhoto>
    {
        // *** Fields ***

        private FlickrApi flickrApi;

        // *** Constructors ***

        public InterestingPhotosDataListSource(FlickrApi flickrApi)
        {
            this.flickrApi = flickrApi;
        }

        // *** Overriden Base Methods ***

        protected override Task<DataListPageResult<FlickrPhoto>> FetchCountAsync()
        {
            // Since we need to get the first page from Flickr to obtain the count, just pull back the first page

            return FetchPageAsync(1);
        }

        protected override Task<DataListPageResult<FlickrPhoto>> FetchPageSizeAsync()
        {
            // As for FetchCountAsync(), just pull back the first page

            return FetchPageAsync(1);
        }

        protected async override Task<DataListPageResult<FlickrPhoto>> FetchPageAsync(int pageNumber)
        {
            // Call the Flickr API to get the requested page (asynchronous)
            // NB: The number of items per page is restricted to 20

            FlickrPhotoPage flickrPage = await flickrApi.GetInterestingPhotos(pageNumber, 20);

            // Create a DataListPageResult<FlickrPhoto> to return containing
            //    - The total number of items in the list
            //    - The number of items returned per page
            //    - The page number that is being returned
            //    - The items on that page

            return new DataListPageResult<FlickrPhoto>(flickrPage.Total, flickrPage.PerPage, flickrPage.Page, flickrPage.Photos);
        }
    }
}
