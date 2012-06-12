using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocoon.Sample.Data;
using Windows.UI.Xaml;
using Windows.Foundation.Collections;
using System.Collections;
using Windows.UI.Xaml.Data;
using Cocoon.Navigation;
using System.Composition;

namespace Cocoon.Sample.ViewModels
{
    [ViewModelExport(SpecialPageNames.Home)]
    public class InterestingPhotosViewModel : INotifyPropertyChanged
    {
        // *** Fields ***

        private IList<FlickrPhoto> interestingPhotos;

        // *** Events ***

        public event PropertyChangedEventHandler PropertyChanged;

        // *** Constructors ***

        [ImportingConstructor]
        public InterestingPhotosViewModel(FlickrDataSource flickrDataSource)
        {
            if (HasValidApiKey)
                this.InterestingPhotos = flickrDataSource.GetInterestingPhotos();
        }

        // *** Properties ***

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
