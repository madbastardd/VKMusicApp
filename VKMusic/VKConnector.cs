using System;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Exception;

namespace VKMusic {
    public class VKConnector {
        static ulong ID = 5485784;   //application ID
        VkApi api;            //VKApi

        string username;       //username
        string password;       //password
        public string keyValue { get; set; }           //key from SMS

        public VkApi VK {
            //read only property that returns VKApi
            get {
                return api;
            }
        }
        public VKConnector(string username, string password) {
            //constrcutor
            api = new VkApi();

            this.username = username;
            this.password = password;
        }

        public bool OneAuth() {
            //auth without SMS
            ApiAuthParams authParams = new ApiAuthParams {
                ApplicationId = ID,
                Login = username,
                Password = password,
                Settings = Settings.Audio,
            };

            return Auth(authParams);
        }

        public bool TwoAuth() {
            //auth with SMS
            ApiAuthParams authParams = new ApiAuthParams {
                ApplicationId = ID,
                Login = username,
                Password = password,
                Settings = Settings.Audio,
                TwoFactorAuthorization = () => keyValue
            };

            return Auth(authParams);
        }

        private bool Auth(ApiAuthParams authParams) {
            //try auth
            //if all OK - return true
            //else - false
            try {
                Authorization(authParams);

                HideInfo();

                return true;
            }
            catch (Exception e) when (e is VkApiException || e is VkApiAuthorizationException) {
                return false;
            }
        }

        private void Authorization(ApiAuthParams param) {
            //auth with parameters
            api.Authorize(param);
        }

        private void HideInfo() {
            //delete username and password after
            //success auth
            username = password = keyValue = null;
        }
    }
}
