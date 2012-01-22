using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

using Spring.WindowsPhoneQuickStart.ViewModel;

namespace Spring.WindowsPhoneQuickStart.Views
{
    public partial class Dropbox : PhoneApplicationPage
    {
        public Dropbox()
        {
            InitializeComponent();

            this.DataContext = this.ViewModel;
            this.ViewModel.PropertyChanged += new PropertyChangedEventHandler(ViewModel_PropertyChanged);
        }

        public DropboxViewModel ViewModel
        {
            get { return App.Current.DropboxViewModel; }
        }

        // Overrided methods

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (this.ViewModel.IsAuthenticated)
            {
                this.ViewModel.ShowProfile();
            }
            else
            {
                this.ViewModel.Authenticate();
            }
        }

        // Event methods

        void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // WebBrowser.Source property not bindable
            if (e.PropertyName == "AuthenticateUri")
            {
                this.WebBrowser.Navigate(this.ViewModel.AuthenticateUri);
            }
        }

        private void WebBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            if (this.ViewModel != null)
            {
                string url = e.Uri.ToString();
                if (url.StartsWith(DropboxViewModel.CallbackUrl, StringComparison.OrdinalIgnoreCase))
                {
                    string verifier = url.Substring(url.LastIndexOf("oauth_token=") + 15);
                    this.ViewModel.AuthenticateCallback(verifier);
                }
            }
        }
    }
}