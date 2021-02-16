//
// Copyright (c) 2018 deltaDNA Ltd. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Xml.Serialization;
using UnityEngine;


//see Confugration.cs as my template
//not really sure how ScriptableObjects work, but it seems we create some baseline values
//and either fetch an asset with values set by the user,  
//or create a blank asset for the user to set values to?
//pretty cool either way. C'tri 2021-02-09
namespace DeltaDNA {  
    [Serializable, XmlRoot("APIConfiguration")]
    //[CreateAssetMenu()]
    public class APIConfiguration : ScriptableObject {

        public const string RUNTIME_RSRC_PATH = "ddnaAPI_configuration";
        public const string RESOURCES_CONTAINER = "Assets";
        public const string RESOURCES_DIRECTORY = "Resources";
        public const string ASSET_DIRECTORY = RESOURCES_CONTAINER + "/" + RESOURCES_DIRECTORY;
        public const string FULL_ASSET_PATH = ASSET_DIRECTORY + "/" + RUNTIME_RSRC_PATH + ".asset";


        public string ApiKey = "";
        public string ApiPassword = "";
        string AuthToken = "";
        private bool isAuthenticated = false;
        private DDNA_Def_Authenticator authenticator = new DDNA_Def_Authenticator();
        //TODO: api key only lasts an hour. Record when it was captured and such.

        /***
         * Intent seems to be, fetch the cached asset if it exists.
         * If it does not exist, create it with the public parameters above
         * question - where does GetAssetInstance get called from? is it inherited?
         */
        public static APIConfiguration GetAssetInstance()
        {
            APIConfiguration cfg = Resources.Load<APIConfiguration>(RUNTIME_RSRC_PATH);

            if (cfg == null)
            {
                cfg = ScriptableObject.CreateInstance<APIConfiguration>();
            }

            return cfg;
        }

        public string getAuthToken()
        {
            return AuthToken;
        }
        public void setAuthToken(string newAuthToken)
        {
            this.AuthToken = newAuthToken;
            this.isAuthenticated = true;
        }
        public Boolean IsAuthenticated()
        {
            return isAuthenticated;
        }
    }

}