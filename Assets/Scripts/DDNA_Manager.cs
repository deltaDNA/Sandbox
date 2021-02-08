using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DeltaDNA;
using System;
using System.IO;
using UnityEngine.UI;
using System.Text;

public class DDNA_Manager : MonoBehaviour
{
    [SerializeField] private Button btnEvents;
    [SerializeField] private Button btnCampaigns;
    [SerializeField] private Button btnADS;
    [SerializeField] private Button btnIAP;
    [SerializeField] private TMP_Text txtStartSDK;
    [SerializeField] private TMP_Text txtParams;
    [SerializeField] private TMP_Text txtValues;
    [SerializeField] private TMP_Text txtEventName;
    [SerializeField] private Toggle sdkOnStart;

    private AdsManager adsManager;
    private RemoteConfigManager remoteConfigManager; 

    // Start is called before the first frame update
    void Start()
    {
        ConfigureDeltadnaSDK();

        adsManager = GetComponent<AdsManager>();
        remoteConfigManager = GetComponent<RemoteConfigManager>();

        //listener on startSDK toggle
        sdkOnStart.onValueChanged.AddListener(delegate {
            ToggleValueChanged(sdkOnStart);
        });

        sdkOnStart.isOn = Preferences.GetToggleStartSDK();

        
        if (DDNA.Instance.HasStarted)
        {
            //simulate a click
            OnStartSDKClicked();
        }
     
        HandleUIForSDK(DDNA.Instance.HasStarted);
      
    }

    private void HandleUIForSDK(bool hasStarted)
    {
        if (hasStarted)
        {
            txtStartSDK.text = "Stop SDK";
        }
        else
        {
            txtStartSDK.text = "Start SDK";
        }
        btnEvents.interactable = hasStarted;
        btnCampaigns.interactable = hasStarted;
        btnIAP.interactable = hasStarted;
        btnADS.interactable = hasStarted;
    }

    public void ToggleValueChanged(UnityEngine.UI.Toggle toggle)
    {
        Preferences.SaveToggleStartSDK(toggle.isOn);
    }
    public void OnStartSDKClicked()
    {
        // Default Configuration points to
        // https://www.deltadna.net/demo-account/sandbox/dev 
        //TODO check config use config instead of UI CONFIG
        if (txtStartSDK.text.StartsWith("Start"))
        {

            DDNA.Instance.StartSDK();
            DDNA.Instance.AndroidNotifications.RegisterForPushNotifications();

            remoteConfigManager.FetchRemoteConfig();


            HandleUIForSDK(DDNA.Instance.HasStarted);            
            txtStartSDK.text = "Stop SDK";            
        }
        else
        {
            DDNA.Instance.StopSDK();
            HandleUIForSDK(false);
        }
        

    }
    public void OnRecordEventClick()
    {
        string[] parameters;
        string[] values;
        parameters = txtParams.text.Split(';');
        values = txtValues.text.Split(';');
        GameEvent gameEvent = null;
        string eventName = txtEventName.text.ToString().Trim();

        for (int i = 0; i <= parameters.Length - 1; i++)
        {
            if (i == 0)
            {
                var utf8 = Encoding.UTF8;
                byte[] utfBytes = utf8.GetBytes(eventName);
                eventName = utf8.GetString(utfBytes, 0, utfBytes.Length);
                gameEvent = new GameEvent(eventName);
            }
                
            gameEvent.AddParam(parameters[i], values[i]);
        }

        if(gameEvent != null)
            DDNA.Instance.RecordEvent(gameEvent);
    }


    private void ConfigureDeltadnaSDK()
    {
        DDNA.Instance.SetLoggingLevel(DeltaDNA.Logger.Level.DEBUG);

        // Hook up callback to fire when DDNA SDK has received session config info, including Event Triggered campaigns.
        DDNA.Instance.NotifyOnSessionConfigured(true);
        DDNA.Instance.OnSessionConfigured += (bool cachedConfig) => ReceivedGameConfig(cachedConfig);


        // Allow multiple game parameter actions callbacks from a single event trigger        
        DDNA.Instance.Settings.MultipleActionsForEventTriggerEnabled = true;

        //Register default handlers for event triggered campaigns. These will be candidates for handling ANY Event-Triggered Campaigns. 
        //Any handlers added to RegisterEvent() calls with the .Add method will be evaluated before these default handlers. 
        DDNA.Instance.Settings.DefaultImageMessageHandler =
            new ImageMessageHandler(DDNA.Instance, imageMessage => {
                // do something with the image message
                myImageMessageHandler(imageMessage);
            });
        DDNA.Instance.Settings.DefaultGameParameterHandler = new GameParametersHandler(gameParameters => {
            // do something with the game parameters
            myGameParameterHandler(gameParameters);
        });

        DDNA.Instance.SetLoggingLevel(DeltaDNA.Logger.Level.DEBUG);
    }



    public void ReceivedGameConfig(bool cachedConfig)
    {
        // Callback indicating that the deltaDNA has downloaded its session configuration, including
        // Event Triggered Campaign actions and logic, is used to record a "sdkConfigured" event 
        // that can be used provision remotely configured parameters. 
        // i.e. deferring the game session config until it knows it has received any info it might need

        Debug.Log("Configuration Loaded, Cached =  " + cachedConfig.ToString());
        Debug.Log("Recording a sdkConfigured event for Event Triggered Campaign to react to");

        // Create an sdkConfigured event object
        var gameEvent = new GameEvent("sdkConfigured")
            .AddParam("clientVersion", DDNA.Instance.ClientVersion)
            .AddParam("unityVersion", Application.unityVersion);

        // Record sdkConfigured event and run default response hander
        DDNA.Instance.RecordEvent(gameEvent).Run();
    }



    private void myImageMessageHandler(ImageMessage imageMessage)
    {
        // Add a handler for the 'dismiss' action.
        imageMessage.OnDismiss += (ImageMessage.EventArgs obj) => {
            Debug.Log("Image Message dismissed by " + obj.ID);

            // NB : parameters not processed if player dismisses action
        };

        // Add a handler for the 'action' action.
        imageMessage.OnAction += (ImageMessage.EventArgs obj) => {
            Debug.Log("Image Message actioned by " + obj.ID + " with command " + obj.ActionValue);

            // Process parameters on image message if player triggers image message action
            if (imageMessage.Parameters != null) myGameParameterHandler(imageMessage.Parameters);
        };

        imageMessage.OnDidReceiveResources += () =>
        {
            Debug.Log("Received Image Message Assets");
        };


        // the image message is already cached and prepared so it will show instantly
        imageMessage.Show();
    }



    private void myGameParameterHandler(Dictionary<string, object> gameParameters)
    {
        // Generic Game Parameter Handler
        Debug.Log("Received game parameters from Engage campaign: " + DeltaDNA.MiniJSON.Json.Serialize(gameParameters));

        // Handle ADS commands received from DDNA
        if (gameParameters.ContainsKey("adProvider")) { 
            adsManager.ProcessAdCommands(gameParameters);

        }

        // Handle Remote Confic Commands received from DDNA
        if (gameParameters.ContainsKey("remoteConfigName") || gameParameters.ContainsKey("remoteConfigSegment"))
        {
            remoteConfigManager.FetchRemoteConfig(gameParameters); 
        }
    }
}

