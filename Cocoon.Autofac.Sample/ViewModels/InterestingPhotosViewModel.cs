using System.Collections.Generic;
using System.ComponentModel;
using Cocoon.Autofac.Sample.Data;
using Windows.UI.Xaml;

namespace Cocoon.Autofac.Sample.ViewModels
{
    public class InterestingPhotosViewModel : INotifyPropertyChanged
    {
        private IList<FlickrPhoto> interestingPhotos;

        public event PropertyChangedEventHandler PropertyChanged;

        public InterestingPhotosViewModel(FlickrDataSource flickrDataSource)
        {
            if (HasValidApiKey)
                InterestingPhotos = flickrDataSource.GetInterestingPhotos();
        }

        public bool HasValidApiKey
        {
            get
            {
                return FlickrApi.FLICKR_API_KEY != "ENTER YOUR FLICKR API KEY HERE";
            }
        }

        public Visibility HasInvalidApiKeyVisibility
        {
            get
            {
                return HasValidApiKey ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public IList<FlickrPhoto> InterestingPhotos
        {
            get
            {
                return interestingPhotos;
            }
            set
            {
                if (interestingPhotos != value)
                {
                    interestingPhotos = value;
                    OnPropertyChanged("InterestingPhotos");
                }
            }
        }

        // *** Protected Methods ***

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
