using System;
using System.ComponentModel;
using System.IO.IsolatedStorage;

using Spring.Social.OAuth1;
using Spring.Social.Dropbox.Api;

namespace Spring.WindowsPhoneQuickStart.ViewModel
{
    public class DropboxViewModel : INotifyPropertyChanged
    {
        private const string OAuthTokenKey = "OAuthToken";
        public const string CallbackUrl = "http://localhost/Dropbox/Callback";

        private OAuthToken requestOAuthToken;
        private Uri authenticateUri;
        private DropboxProfile profile;

        public IOAuth1ServiceProvider<IDropbox> DropboxServiceProvider { get; set; }

        public bool IsAuthenticated
        {
            get
            {
                return this.OAuthToken != null;
            }
        }

        public OAuthToken OAuthToken
        {
            get
            {
                return this.LoadSetting<OAuthToken>(OAuthTokenKey, null);
            }
            set
            {
                this.SaveSetting(OAuthTokenKey, value);
                NotifyPropertyChanged("IsAuthenticated");
            }
        }

        public Uri AuthenticateUri
        {
            get
            {
                return this.authenticateUri;
            }
            set
            {
                this.authenticateUri = value;
                NotifyPropertyChanged("AuthenticateUri");
            }
        }

        public DropboxProfile Profile
        {
            get
            {
                return this.profile;
            }
            set
            {
                this.profile = value;
                NotifyPropertyChanged("Profile");
            }
        }

        public void Authenticate()
        {
            this.DropboxServiceProvider.OAuthOperations.FetchRequestTokenAsync(CallbackUrl, null,
                r =>
                {
                    this.requestOAuthToken = r.Response;
                    OAuth1Parameters parameters = new OAuth1Parameters();
                    parameters.CallbackUrl = CallbackUrl;
                    this.AuthenticateUri = new Uri(this.DropboxServiceProvider.OAuthOperations.BuildAuthenticateUrl(r.Response.Value, parameters));
                });
        }

        public void AuthenticateCallback(string verifier)
        {
            AuthorizedRequestToken authorizedRequestToken = new AuthorizedRequestToken(this.requestOAuthToken, verifier);
            this.DropboxServiceProvider.OAuthOperations.ExchangeForAccessTokenAsync(authorizedRequestToken, null,
                r =>
                {
                    this.OAuthToken = r.Response;
                    this.ShowProfile();
                });
        }

        public void ShowProfile()
        {
            IDropbox dropboxClient = this.DropboxServiceProvider.GetApi(this.OAuthToken.Value, this.OAuthToken.Secret);
            dropboxClient.GetUserProfileAsync(
                r =>
                {
                    if (r.Error == null)
                    {
                        this.Profile = r.Response;
                    }
                });
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private void SaveSetting(string key, object value)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(key))
            {
                settings[key] = value;
            }
            else
            {
                settings.Add(key, value);
            }
        }

        private T LoadSetting<T>(string key, T defaultValue)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (!settings.Contains(key))
            {
                settings.Add(key, defaultValue);
            }
            return (T)settings[key];
        }
    }
}
