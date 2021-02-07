using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DeltaDNA; 

public class AdsPanel : MonoBehaviour
{
    

    private AdsManager adsManager;
    private bool adsReady; 


    [Header("Buttons")]
    [SerializeField] private Button btnInterstitial;
    [SerializeField] private Button btnRewarded;

    public Text TxtCooldown ; 


    // Start is called before the first frame update
    void Start()
    {
        adsManager = GameObject.Find("DDNA_Manager").GetComponent<AdsManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        int remainingCoolDown = adsManager.RemainingCooldownSeconds();
        adsReady = adsManager.State == AdState.Ready &&  remainingCoolDown == 0  && DDNA.Instance.HasStarted;

        TxtCooldown.text = string.Format ("Next Ad Ready : {0}",adsReady);

        if (remainingCoolDown > 0 )
            TxtCooldown.text = string.Format("Cooldown {0} seconds", remainingCoolDown);
        else if (!DDNA.Instance.HasStarted) 
            TxtCooldown.text = string.Format("Please Start DDNA SDK!");
        
        btnInterstitial.interactable = adsReady;
        btnRewarded.interactable = adsReady;
    }
}
