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
    [Header("MAIN PANEL")]
    [SerializeField] private Button btnEvents;
    [SerializeField] private Button btnCampaigns;
    [SerializeField] private Button btnADS;
    [SerializeField] private Button btnIAP;
    [SerializeField] private TMP_Text txtStartSDK;
    [SerializeField] private Toggle sdkOnStart;

    [Header("EVENT PANEL")]
    [SerializeField] private InputField txtEvenName;
    [SerializeField] private InputField txtParams;
    [SerializeField] private InputField txtValues;

    [SerializeField] private Dropdown cboEvent;
    [SerializeField] private Dropdown cboParams;
    [SerializeField] private InputField txtValuesDynamics;

    [SerializeField] private GameObject objParamValues;

    [Header("CAMPAIGN PANEL")]
    [SerializeField] private InputField txtDecisionPointName;
    [SerializeField] private InputField txtDPValues;
    [SerializeField] private InputField txtDPParameters;
    [SerializeField] private InputField txtJSON;







    // Start is called before the first frame update
    void Start()
    {
        //listener on startSDK toggle
        sdkOnStart.onValueChanged.AddListener(delegate {
            ToggleValueChanged(sdkOnStart);
        });

        txtEvenName.onValueChanged.AddListener(delegate {
            InputValueChanged(txtEvenName);
        });

        txtParams.onValueChanged.AddListener(delegate {
            InputValueChanged(txtParams);
        });

        txtValues.onValueChanged.AddListener(delegate {
            InputValueChanged(txtValues);
        });

        txtDecisionPointName.onValueChanged.AddListener(delegate {
            InputValueChanged(txtValues);
        });

        cboEvent.onValueChanged.AddListener(delegate
        {
            DynamicLoadEvent();
        });

       

        LoadSavedValues();

        if (sdkOnStart.isOn)
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

    private void InputValueChanged(InputField field)
    {
        Preferences.SaveInputField(field);
    }

    private void LoadSavedValues()
    {
        sdkOnStart.isOn = Preferences.GetToggleStartSDK();
        txtEvenName.text = Preferences.GetInputField(txtEvenName);
        txtParams.text = Preferences.GetInputField(txtParams);
        txtValues.text = Preferences.GetInputField(txtValues);
        txtDecisionPointName.text = Preferences.GetInputField(txtDecisionPointName);
    }

    /// <summary>
    ///  Dynamic load of events through platformapi
    /// </summary>
    private void DynamicLoadEvent()
    {
        //TODO Load event from json

        //Clear all existing events and params
        RemoveDynamicParams();
        DynamicLoadParams();
    }

    /// <summary>
    /// Once an event is dynamically picked we will load all valid parameters
    /// </summary>
    private void DynamicLoadParams()
    {
        GameObject temp = null;
        //TODO find what event has been chosen from cboEvents

        //For each param in the event create a new objparam from prefabs + the height + 15
        for (int i = 0; i <= 10 - 1; i++) //todo get the length from the json
        {
            if (i==0)
            {
                temp = Instantiate(objParamValues, new Vector3(objParamValues.transform.position.x, cboParams.transform.position.y - 125, cboEvent.transform.position.z), Quaternion.identity, cboEvent.transform.parent);
            }
            else
            {
                temp = Instantiate(objParamValues, new Vector3(objParamValues.transform.position.x, temp.transform.position.y - 125, cboEvent.transform.position.z), Quaternion.identity, cboEvent.transform.parent);
            }
            temp.tag = "dynamic";//tag fo later removal;

        }
    }

    private void RemoveDynamicParams()
    {
        GameObject[] objectsToDelete;
        objectsToDelete = GameObject.FindGameObjectsWithTag("dynamic");

        foreach(GameObject obj in objectsToDelete)
        {
            Destroy(obj);
        }
    }

    public void OnStartSDKClicked()
    {
        // Default Configuration points to
        // https://www.deltadna.net/demo-account/sandbox/dev 
        //TODO check config use config instead of UI CONFIG
        if (txtStartSDK.text.StartsWith("Start"))
        {
            ConfigureDeltadnaSDK();

            DDNA.Instance.SetLoggingLevel(DeltaDNA.Logger.Level.DEBUG);
            DDNA.Instance.StartSDK();
            DDNA.Instance.AndroidNotifications.RegisterForPushNotifications();
            HandleUIForSDK(DDNA.Instance.HasStarted);
            Debug.Log(DDNA.Instance.ClientVersion);
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
        String eventName = txtEvenName.text.ToString().Trim();

        for (int i = 0; i <= parameters.Length - 1; i++)
        {
            if (i == 0)
            {
                //var utf8 = Encoding.Unicode;
                //byte[] utfBytes = utf8.GetBytes(eventName);
                //eventName = utf8.GetString(utfBytes, 0, utfBytes.Length);
                gameEvent = new GameEvent(eventName);
            }

            gameEvent.AddParam(parameters[i], values[i]);
        }

        if (gameEvent != null)
            DDNA.Instance.RecordEvent(gameEvent);
    }



    #region DeltaDNA Functions
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

    public void OnEngagementDPClick()
    {
        string decisionPoint = txtDecisionPointName.text;
        string[] parameters = txtDPParameters.text.Split(';');
        string[] values = txtDPValues.text.Split(';');
    

        if (decisionPoint != "")
        {
            if (parameters[0] != "")
            {
                Params customParams = null;
             
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    if (i == 0)
                    {
                        customParams = new Params();
                    }

                    customParams.AddParam(parameters[i], values[i]);
                }

                DDNA.Instance.EngageFactory.RequestGameParameters(decisionPoint, customParams,(gameParameters) => {
                    txtJSON.text = DeltaDNA.MiniJSON.Json.Serialize(gameParameters);
                });
            }
            else
            {
                DDNA.Instance.EngageFactory.RequestGameParameters(decisionPoint, (gameParameters) => {
                    txtJSON.text = DeltaDNA.MiniJSON.Json.Serialize(gameParameters);
                });
            }
            
        }
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
      
        imageMessage.OnStore += eventArgs =>
        {
            Application.OpenURL(eventArgs.ActionValue);
        };
    
       
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
        if (gameParameters.ContainsKey("<yourParameter>"))
        {
            // Do something
        }
    }
    #endregion


}

