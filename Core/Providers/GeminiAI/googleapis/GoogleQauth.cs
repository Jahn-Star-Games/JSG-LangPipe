using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Google.Apis.Auth.OAuth2;

namespace JahnStarGames.Langpipe.Providers.GoogleApis
{
    public static class GoogleQauth
    {
        /** 
        <summary>
        generate credential tokens to connect to Google Cloud Vertex AI
        </summary>
        */
        /// <summary>
        /// Get Access Token From JSON Key Async
        /// </summary>
        /// <param name="jsonKeyFilePath">Path to your JSON Key file</param>
        /// <param name="scopes">Scopes required in access token</param>
        /// <returns>Access token as string Task</returns>
        public static async Task<string> GetAccessTokenFromJSONKeyAsync(string jsonKeyFilePath, params string[] scopes)
        {
            string[] _scopes = scopes;
            if (_scopes == null || _scopes.Length == 0) _scopes = new string[] { "https://www.googleapis.com/auth/cloud-platform" };
            using var cancellationSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)); // Set timeout to 10 seconds (adjustable)
            using var stream = new FileStream(jsonKeyFilePath, FileMode.Open, FileAccess.Read);
            return await GoogleCredential
                .FromStream(stream)
                .CreateScoped(_scopes)
                .UnderlyingCredential
                .GetAccessTokenForRequestAsync(cancellationToken: cancellationSource.Token);
        }

        /// <summary>
        /// Get Access Token From P12 Key Async
        /// </summary>
        /// <param name="p12KeyFilePath">Path to your P12 Key file</param>
        /// <param name="serviceAccountEmail">Service Account Email</param>
        /// <param name="keyPassword">Key Password</param>
        /// <param name="scopes">Scopes required in access token</param>
        /// <returns>Access token as string Task</returns>
        public static async Task<string> GetAccessTokenFromP12KeyAsync(string p12KeyFilePath, string serviceAccountEmail, string keyPassword = "notasecret", params string[] scopes)
        {
            string[] _scopes = scopes;
            if (_scopes == null || _scopes.Length == 0) _scopes = new string[] { "https://www.googleapis.com/auth/cloud-platform" };
            using var cancellationSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)); // Set timeout to 10 seconds (adjustable)
            return await new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(serviceAccountEmail)
                {
                    Scopes = scopes
                }.FromCertificate(
                    new System.Security.Cryptography.X509Certificates.X509Certificate2(
                        p12KeyFilePath,
                        keyPassword,
                        System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.Exportable))).GetAccessTokenForRequestAsync(cancellationToken: cancellationSource.Token);
        }
        
        public static async Task<string> GetAccessTokenAsync()
        {
            using var cancellationSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)); // Set timeout to 10 seconds (adjustable)
            var credential = await GoogleCredential.GetApplicationDefaultAsync() ?? throw new Exception ("Default credentials not found. Set GOOGLE_APPLICATION_CREDENTIALS environment variable.");
            if (credential.IsCreateScopedRequired) credential = credential.CreateScoped(new[] { "https://www.googleapis.com/auth/cloud-platform" });
            var token = await ((ITokenAccess)credential).GetAccessTokenForRequestAsync(cancellationToken: cancellationSource.Token);
            return token;
        }
    }
}
