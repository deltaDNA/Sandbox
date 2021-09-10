using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using DeltaDNA; 


public class RemoteConfigPanel : MonoBehaviour
{

    [SerializeField] private Button bttnFetchRemoteConfig;
    [SerializeField] public TMP_InputField txtConfigEnvironment;
    [SerializeField] public TMP_InputField txtConfigName;
    [SerializeField] public TMP_InputField txtConfigSegment;
    [SerializeField] public TMP_Text txtLastConfigResponse;

    RemoteConfigManager remoteConfigManager;

    private void Start()
    {
        remoteConfigManager = GameObject.Find("DDNA_Manager").GetComponent<RemoteConfigManager>();

        txtConfigEnvironment.SetTextWithoutNotify(remoteConfigManager.environmentID);
        txtConfigName.SetTextWithoutNotify(remoteConfigManager.userAttributes.configName);
        txtConfigSegment.SetTextWithoutNotify(remoteConfigManager.userAttributes.configSegment);
        txtLastConfigResponse.SetText(remoteConfigManager.lastResponse);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            bttnFetchRemoteConfig.interactable = DDNA.Instance.HasStarted;            
        }
    }

    public void BtnFetchClicked()
    {
        Debug.Log("Fetching " + txtConfigName.text);
        
        if (remoteConfigManager)
        {           
            remoteConfigManager.FetchRemoteConfig(txtConfigEnvironment.text, txtConfigName.text, txtConfigSegment.text);
        }    
    }

}
