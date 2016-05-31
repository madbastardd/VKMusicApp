using System;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Exception;

namespace VKMusic {
    public class VKConnector {
        static ulong ID = 5485784;
        VkApi api;

        MainWindow window;
        string username;
        string password;
        public string KeyValue  { get; set; }

        public VkApi VK {
            get {
                return api;
            }
        }
        public VKConnector(MainWindow window, string username, string password) {
            api = new VkApi();

            this.username = username;
            this.password = password;
            this.window = window;
        }

        public bool OneAuth() {
            ApiAuthParams authParams = new ApiAuthParams {
                ApplicationId = ID,
                Login = username,
                Password = password,
                Settings = Settings.Audio,
            };

            return Auth(authParams);
        }

        public bool TwoAuth() {
            ApiAuthParams authParams = new ApiAuthParams {
                ApplicationId = ID,
                Login = username,
                Password = password,
                Settings = Settings.Audio,
                TwoFactorAuthorization = () => KeyValue
            };

            return Auth(authParams);
        }

        private bool Auth(ApiAuthParams authParams) {
            try {
                Authorization(authParams);

                window.errorMessage.Dispatcher.Invoke(() => window.errorMessage.Text = "ALL OK");

                HideInfo();

                return true;
            }
            catch (Exception e) when (e is VkApiException || e is VkApiAuthorizationException) {
                return false;
            }
        }

        private void Authorization(ApiAuthParams param) {
            api.Authorize(param);
        }

        private void HideInfo() {
            window = null;
            username = password = null;
        }
    }
}
