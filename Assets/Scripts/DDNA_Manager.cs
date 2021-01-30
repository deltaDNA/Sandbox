using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DeltaDNA;
public class DDNA_Manager : MonoBehaviour
{
    [SerializeField] private TMP_Text btnStartSDK;
    [SerializeField] private TMP_Text txtParams;
    [SerializeField] private TMP_Text txtValues;
    [SerializeField] private TMP_Text txtEventName;

    private AdsManager adsManager;
    


    // Start is called before the first frame update
    void Start()
    {
        adsManager = GetComponent<AdsManager>();

        if (DDNA.Instance.HasStarted)
        {
            btnStartSDK.text = "Stop SDK";
        }
    }
    
   
    public void OnStartSDKClicked()
    {
        // Default Configuration points to
        // https://www.deltadna.net/demo-account/sandbox/dev 
        //TODO check config use config instead of UI CONFIG
        if (btnStartSDK.text.StartsWith("Start"))
        {
            ConfigureDeltadnaSDK();

            DDNA.Instance.SetLoggingLevel(DeltaDNA.Logger.Level.DEBUG);
            DDNA.Instance.StartSDK();
            DDNA.Instance.AndroidNotifications.RegisterForPushNotifications();
            btnStartSDK.text = "Stop SDK";            
        }
        else
        {
            DDNA.Instance.StopSDK();
            btnStartSDK.text = "Start SDK";
        }
        

    }
    public void OnRecordEventClick()
    { 
        string[] parameters;
        string[] values;
        parameters = txtParams.text.Split(';');
        values = txtValues.text.Split(';');
        GameEvent gameEvent = null;

        for (int i = 0; i < parameters.Length-1; i++)
        {
            if(i==0)
                gameEvent = new GameEvent(txtEventName.text);
            gameEvent.AddParam(parameters[i], values[i]);
        }
          
        DDNA.Instance.RecordEvent(gameEvent);
    }

    private void ConfigureDeltadnaSDK()
    {
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
        if (gameParameters.ContainsKey("adProvider"))
        {
            adsManager.ProcessAdCommands(gameParameters);
        }
    }
}
