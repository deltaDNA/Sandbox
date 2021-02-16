using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using DeltaDNA;

 
namespace DeltaDNA
{
    public class DDNA_Def_Authenticator
    {
        // Start is called before the first frame update
        // on start

         
        private class APIAuthToken
        {
            public string idToken;
        }


        private class APIKeyObject
        {
            public string key;
            public string password;
            public APIKeyObject()
            {
                key = "";
                password = "";
            }
        }

        public void tryAuthenticate()
        {
            var cfg = DeltaDNA.APIConfiguration.GetAssetInstance();




            //TODO: delete the KeyObjectClass, not needed
            //TODO: probably will need an EventClass and a Parameter class

            var formattedAPIDeets = new APIKeyObject();
            formattedAPIDeets.key = cfg.ApiKey;
            formattedAPIDeets.password = cfg.ApiPassword;

            var requestBody = JsonUtility.ToJson(formattedAPIDeets);

            string Uri = "https://api.deltadna.net/api/authentication/v1/authenticate";//String.Format("https://api.deltadna.net/api/authentication/v1/authenticate");


            //Why a put? https://forum.unity.com/threads/unitywebrequest-post-url-jsondata-sending-broken-json.414708/#post-2719900
            /* UnityWebRequest is a very poor implementation of an HTTP client.

            This might be your problem, depending on what is consuming your request: For some reason UnityWebRequest applies URL encoding to POST message payloads. URL encoding the payload is an ASP.NET webform thing. It is not part of the HTML standard (and if it were, it still shouldn't matter to an HTTP client), and it certainly is not part of the HTTP standard.

            To get an unmangled-POST through, create UnityWebRequest as a PUT, then in your own code change the Method property to POST after the fact.
            */
            UnityWebRequest WebRequest = UnityWebRequest.Put(Uri, requestBody);
            WebRequest.method = "Post";
            WebRequest.SetRequestHeader("Content-Type", "application/json");
            UnityWebRequestAsyncOperation asyncAction = WebRequest.SendWebRequest();
            asyncAction.completed += receiveAuthenticate;


        }
        private void receiveAuthenticate(AsyncOperation asyncOperation)
        {
            /* take the generic async operation task and cast it to its correct specific class
                * validate everything was received okay
                * update the configuation object
                * 
                */
            var cfg = DeltaDNA.APIConfiguration.GetAssetInstance();

            UnityWebRequest request = ((UnityWebRequestAsyncOperation)asyncOperation).webRequest;
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
            }

            APIAuthToken returnObj = new APIAuthToken();

            try
            {
                returnObj = JsonUtility.FromJson<APIAuthToken>(request.downloadHandler.text);
                cfg.setAuthToken(returnObj.idToken); //authentication is now a success
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse JSON from API authentication: {e.Message}");
                Debug.Log($"The json that failed to be parsed is: {request.downloadHandler.text}");
            }

        }
    }

}