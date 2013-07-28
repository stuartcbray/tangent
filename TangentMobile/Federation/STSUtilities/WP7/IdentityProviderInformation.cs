using System;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;

namespace TheFactorM.Federation
{
    /// <summary>
    /// DataContract for IdentityProviderInformation returned by the Identity Provider Discover Service
    /// </summary>

    public partial class IdentityProviderInformation
    {
        private BitmapImage _image;

        /// <summary>
        /// The image populated by calling LoadImageFromImageUrl
        /// </summary>
        public BitmapImage Image
        {
            get
            {
                return _image;
            }
        }

        /// <summary>
        /// Retieves the image from ImageUrl
        /// </summary>
        /// <returns>The image from the url as a BitmapImage</returns>
        public BitmapImage LoadImageFromImageUrl()
        {
            _image = null;

            if (string.Empty != ImageUrl)
            {
                BitmapImage imageBitmap = new BitmapImage();
                Uri imageUrlUri = new Uri(ImageUrl);
                imageBitmap.UriSource = imageUrlUri;
                _image = imageBitmap;
            }

            return _image;
        }

    }
}
