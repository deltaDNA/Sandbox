using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;
using TMPro;
using DeltaDNA;

public class RemoteConfigManager : MonoBehaviour
{

    public struct UserAttributes
    {
        
        // These two variables will be used in remoteConfig rules to fetch specific remoteConfig settings
        // The values of these parameters will be supplied in deltaDNA GameParameter Actions, based on DDNA Segmentation
        public string configName { get; set; }
        public string configSegment { get; set; }
/*
        public void Set(string ConfigName, string ConfigSegment)
        {
            configName = ConfigName;
            configSegment = ConfigSegment;
        }
        */

    }
    public UserAttributes userAttributes;
    
    public struct AppAttributes
    {
        // Optionally declare variables for any custom app attributes:
    }
    public AppAttributes appAttributes;

    // Optionally declare a unique assignmentId if you need it for tracking:
    public string assignmentId;
    public string environmentID;
    public string lastResponse; 
  

    // Retrieve and apply the current key-value pairs from the service on Awake:
    void Awake()
    {
        // Add a listener to apply settings when successfully retrieved:
        ConfigManager.FetchCompleted += ApplyRemoteSettings;

  

        userAttributes = new UserAttributes();
        appAttributes = new AppAttributes();

        // Fetch previous configuration
        Load();

    }
    private void OnDestroy()
    {
        ConfigManager.FetchCompleted -= ApplyRemoteSettings;        
    }

    public void FetchRemoteConfig(Dictionary<string, object> dict)
    {                
        if (dict.ContainsKey("remoteConfigName"))
        {
            userAttributes.configName = dict["remoteConfigName"].ToString();
        }
        if (dict.ContainsKey("remoteConfigSegment"))
        {
            userAttributes.configSegment = dict["remoteConfigSegment"].ToString();
        }

        FetchRemoteConfig();
    }

    public void FetchRemoteConfig(string configEnvironment, string configName, string configSegment)
    {
        if (!string.IsNullOrEmpty(configEnvironment))
        {
            // Set the remote Config environment ID:
            ConfigManager.SetEnvironmentID(configEnvironment);
        }
        userAttributes.configName = configName;
        userAttributes.configSegment = configSegment;
        
        FetchRemoteConfig();
    }

    public void FetchRemoteConfig()
    {
        // Set the user’s unique ID:
        ConfigManager.SetCustomUserID(DDNA.Instance.UserID);    

        // Fetch configuration setting from the remote service:
        Debug.Log(string.Format("Fetching Remote Config - configName : {0} configSegment : {1}", userAttributes.configName, userAttributes.configSegment));
        ConfigManager.FetchConfigs<UserAttributes, AppAttributes>(userAttributes, appAttributes);
    }

    // Create a function to set your variables to their keyed values:
    private void ApplyRemoteSettings(ConfigResponse configResponse)
    {
               
        // Conditionally update settings, depending on the response's origin:
        switch (configResponse.requestOrigin)
        {
            case ConfigOrigin.Default:
                Debug.Log("No settings loaded this session; using default values.");
                break;
            case ConfigOrigin.Cached:
                Debug.Log("No settings loaded this session; using cached values from a previous session.");
                break;
            case ConfigOrigin.Remote:
                
                assignmentId = ConfigManager.appConfig.assignmentID;
                environmentID =  ConfigManager.appConfig.environmentID;
                lastResponse = ConfigManager.appConfig.config.ToString();
                Debug.Log(string.Format("Remote Config Received : \nassignemnt {0} \nenvironment {1} \nconfig {2}",assignmentId,environmentID, lastResponse));

                GameObject txtRemoteConfigResponse = GameObject.Find("txtRemoteConfigResponse");
                
                if (txtRemoteConfigResponse)
                {
                    
                    txtRemoteConfigResponse.GetComponent<TMP_Text>().SetText(ConfigManager.appConfig.config.ToString());
                }
                RecordRemoteConfigReceivedEvent();
                Save();
                
                break;
        }
    }

    public void RecordRemoteConfigReceivedEvent()
    {
        GameEvent e = new GameEvent("remoteConfigReceived");
        if (!string.IsNullOrEmpty(userAttributes.configName))
            e.AddParam("remoteConfigName", userAttributes.configName);
        if (!string.IsNullOrEmpty(userAttributes.configSegment))
            e.AddParam("remoteConfigSegment", userAttributes.configSegment);

        DDNA.Instance.RecordEvent(e).Run();
    }

    private void Save()
    {
        // Save the last remote config details
        PlayerPrefs.SetString("remoteConfigEnvironment", environmentID);
        PlayerPrefs.SetString("remoteConfigName", userAttributes.configName);
        PlayerPrefs.SetString("remoteConfigSegment", userAttributes.configSegment);
        PlayerPrefs.SetString("remoteConfigResponse", lastResponse);
    }

    private void Load()
    {
        // Save the last remote config details
        environmentID =  PlayerPrefs.GetString("remoteConfigEnvironment", "");
        userAttributes.configName = PlayerPrefs.GetString("remoteConfigName", "");
        userAttributes.configSegment = PlayerPrefs.GetString("remoteConfigSegment", "");
        lastResponse = PlayerPrefs.GetString("remoteConfigResponse", "");
    }




}

