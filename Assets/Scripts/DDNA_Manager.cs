using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DeltaDNA;
public class DDNA_Manager : MonoBehaviour
{
    [SerializeField] private TMP_Text btnStartSDK;
    [SerializeField] private  TMP_Text txtParams;
    [SerializeField] private TMP_Text txtValues;
    [SerializeField] private TMP_Text txtEventName;
    // Start is called before the first frame update
    void Start()
    {
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
}
