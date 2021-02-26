using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private TMP_Text subTitle;

    [Header("Panel Objects")]
    [SerializeField] private GameObject panelSetup;
    [SerializeField] private GameObject panelEvents;
    [SerializeField] private GameObject panelMain;
    [SerializeField] private GameObject panelADS;
    [SerializeField] private GameObject panelIAP;
    [SerializeField] private GameObject panelCampaigns;
    [SerializeField] private GameObject panelSimpleEvent;
    [SerializeField] private GameObject panelDynamicEvent;
    [SerializeField] private Button btnSimpleEvent;
    [SerializeField] private Button btnDynamicEvent;
    





    // Start is called before the first frame update
    void Start()
    {
        ActivatePanel(panelMain.name);
        subTitle.text = "menu";
    }
       
    void ActivatePanel(string panel)
    {
        panelMain.SetActive(panel.Equals(panelMain.name));
        panelSetup.SetActive(panel.Equals(panelSetup.name));
        panelEvents.SetActive(panel.Equals(panelEvents.name));
        panelADS.SetActive(panel.Equals(panelADS.name));
        panelIAP.SetActive(panel.Equals(panelIAP.name));
        panelCampaigns.SetActive(panel.Equals(panelCampaigns.name));
    }

    void ActivateSubPanel(string panel)
    {
        panelSimpleEvent.SetActive(panel.Equals(panelSimpleEvent.name));
        panelDynamicEvent.SetActive(panel.Equals(panelDynamicEvent.name));
    }

    public void OnEventsClick()
    {
        ActivatePanel(panelEvents.name);
        subTitle.text = "events";
        OnSimpleEventClick();

    }
    public void OnConfigClick()
    {
        ActivatePanel(panelSetup.name);
        subTitle.text = "configuration";
    }
    public void OnBackClick()
    {
        ActivatePanel(panelMain.name);
        subTitle.text = "menu";
    }

    public void OnADSClick()
    {
        ActivatePanel(panelADS.name);
        subTitle.text = "ADS";
    }

    public void OnCampaignsClick()
    {
        ActivatePanel(panelCampaigns.name);
        subTitle.text = "Campaigns";
    }

    public void OnIAPClick()
    {
        ActivatePanel(panelIAP.name) ;
        subTitle.text = "IAP";
    }

    public void OnDynamicEventClick()
    {
        ActivateSubPanel(panelDynamicEvent.name);
        EventSystem.current.SetSelectedGameObject(btnDynamicEvent.gameObject);
    }

    public void OnSimpleEventClick()
    {
        ActivateSubPanel(panelSimpleEvent.name);
        EventSystem.current.SetSelectedGameObject(btnSimpleEvent.gameObject);
    }







}
