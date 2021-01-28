using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    public void OnEventsClick()
    {
        ActivatePanel(panelEvents.name);
        subTitle.text = "events";

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



}
