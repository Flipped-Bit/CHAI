using CHAI.Extensions;
using CHAI.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Text.Json;
using System.Web;
using System.Windows;

namespace CHAI.Views
{
    /// <summary>
    /// Interaction logic for <see cref="LoginWindow"/>.xaml.
    /// </summary>
    public partial class LoginWindow : Window
    {
        private static readonly NameValueCollection ClientData = (NameValueCollection)ConfigurationManager.GetSection("AppSettings/clientData");

        private static readonly NameValueCollection Endpoints = (NameValueCollection)ConfigurationManager.GetSection("AppSettings/endpoints");

        private static readonly NameValueCollection Scopes = (NameValueCollection)ConfigurationManager.GetSection("AppSettings/scopes");

        /// <summary>
        /// The injected <see cref="ILogger{LoginWindow}"/>.
        /// </summary>
        private readonly ILogger _loginWindowLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginWindow"/> class.
        /// </summary>
        /// <param name="logger">The injected <see cref="ILogger"/>.</param>
        public LoginWindow(ILogger logger)
        {
            _loginWindowLogger = logger;
            InitializeComponent();
            VerifyConfigLoaded();
            Browser.Source = GenerateOauthUrl();
            Browser.CoreWebView2InitializationCompleted += BrowserInitializationCompleted;
            DataContext = Owner;
        }

        /// <summary>
        /// Method for getting Token from redirect Url.
        /// </summary>
        /// <param name="sender">The sender of <see cref="Browser_TitleChanged"/> event.</param>
        /// <param name="e">Arguments from <see cref="Browser_TitleChanged"/> event.</param>
        private void Browser_TitleChanged(object sender, object e)
        {
            if (Browser.CoreWebView2.DocumentTitle.StartsWith("localhost"))
            {
                var settingsWindow = Owner as SettingsWindow;
                var url = HttpUtility.HtmlDecode(Browser.CoreWebView2.Source);
                var token = GetTokenFromUrl(url);
                settingsWindow.CurrentUser = GetUserForToken(token);
                Browser.CoreWebView2.CookieManager.DeleteAllCookies();
                Close();
            }
        }

        /// <summary>
        /// Method for setting up events once <see cref="Browser"/> is initialised.
        /// </summary>
        /// <param name="sender">The sender of <see cref="BrowserInitializationCompleted"/> event.</param>
        /// <param name="e">Arguments from <see cref="BrowserInitializationCompleted"/> event.</param>am>
        private void BrowserInitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            Browser.CoreWebView2.DocumentTitleChanged += Browser_TitleChanged;
        }

        /// <summary>
        /// Method for creating OAuth request Url.
        /// </summary>
        /// <returns>OAuth Url.</returns>
        private Uri GenerateOauthUrl()
        {
            return new Uri(string.Join(
                '&',
                $"{Endpoints.Get("Auth")}?response_type=token",
                $"client_id={ClientData.Get("Id")}",
                $"redirect_uri={Endpoints.Get("Redirect")}",
                $"scope={Scopes.Get("Bits")}"));
        }

        /// <summary>
        /// Method for extracting Token from URL parameters.
        /// </summary>
        /// <param name="url">url to extract token from.</param>
        /// <returns>Auth token.</returns>
        private string GetTokenFromUrl(string url)
        {
            var header = "#access_token=";
            var headerIndex = url.IndexOf(header) + header.Length;
            return url[headerIndex..url.IndexOf("&")];
        }

        /// <summary>
        /// Method for validating an Auth Token.
        /// </summary>
        /// <param name="token">Token to validate.</param>
        /// <returns><see cref="User"/> associated with the token.</returns>
        private User GetUserForToken(string token)
        {
            User user = default;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Endpoints.Get("Validate"));
            request.Method = "GET";
            request.Headers.Add("Authorization", $"OAuth {token}");
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var result = response.GetResponseStream()
                    .ReadToEnd();
                user = JsonSerializer.Deserialize<User>(result);
                user.Token = token;
            }
            catch (WebException ex)
            {
                var result = ex.Response.GetResponseStream()
                    .ReadToEnd();
                _loginWindowLogger.LogError(result);
            }
            catch (Exception ex)
            {
                _loginWindowLogger.LogError(ex.Message);
            }

            return user;
        }

        /// <summary>
        /// Method for Verify the config has loaded successfully.
        /// </summary>
        private void VerifyConfigLoaded()
        {
            if (ClientData == null)
            {
                _loginWindowLogger.LogError("Client Data not loaded correctly");
            }

            if (Endpoints == null)
            {
                _loginWindowLogger.LogError("Endpoints not loaded correctly");
            }

            if (Scopes == null)
            {
                _loginWindowLogger.LogError("Scopes not loaded correctly");
            }
        }
    }
}
