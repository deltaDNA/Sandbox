using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeltaDNA; 

public enum AdState
{
    Waiting,
    Ready,
    Watching, 
    Error,
    Completed,
    Finished
}

public class Ad
{
    public Ad()
    {
    }

    private string placementId;
    public string PlacementId { get => placementId; set => placementId= value; }

    private string provider;
    public string Provider { get => provider; set => provider = value; }

    private string adType; 
    public string AdType { get => adType; set => adType = value; }

    private bool completed ;
    public bool Completed { get => completed; set => completed = value; }
}

public class AdsManager : MonoBehaviour
{
    private AdsUnity adsUnity;

    [Header("Properties")]
    [SerializeField] private string adProvider = "ANY"; 
    [SerializeField] private int adsPerSession = 10;
    [SerializeField] private int adCooldownSeconds = 60;

    [Header("Rewards")]
    [SerializeField] private int rewardValue = 50;
    [SerializeField] private string rewardType = "COINS";

    private AdState state = AdState.Waiting;
    public AdState State { get => state; set => UpdateAdState(value); }

    private int adCounter = 0;
    public int AdCounter {   get => adCounter; set => adCounter = value; }

    private DateTime lastAdTimestamp;
    public DateTime LastAdTimestamp { get => lastAdTimestamp; set => lastAdTimestamp = value; }
       

    public int RemainingCooldownSeconds()
    {
        // Return the number of seconds remaining until next ad can be shown
        DateTime nextAdTimestamp = lastAdTimestamp.AddSeconds(adCooldownSeconds);
        double secondsRemaining = nextAdTimestamp.Subtract(DateTime.Now).TotalSeconds; 

        return secondsRemaining > 0 ? Convert.ToInt32(secondsRemaining) : 0 ;           
       } 

    public Ad currentAd; 

    // Start is called before the first frame update
    void Start()
    {
        LastAdTimestamp = DateTime.MinValue;
        adsUnity = gameObject.GetComponent<AdsUnity>();
    }
    
    void UpdateAdState(AdState s)
    {
        state = s;

        switch (state)
        {
            case AdState.Waiting:
                break;
            case AdState.Ready:
                break;
            case AdState.Watching:
                AdCounter++;
                break;
            case AdState.Error:
                currentAd = null;
                UpdateAdState(AdState.Waiting);
                break;
            case AdState.Completed:
                currentAd.Completed = true;
                // Economy deliver reward
                if (currentAd.AdType == "REWARDED")
                {
                    //TODO Send Reward 
                    // EconomyManager.PlayerReceive(rewardType,rewardValue)
                }
                UpdateAdState(AdState.Finished);
                break;
            case AdState.Finished:

                RecordAdImpressionEvent();
                LastAdTimestamp = DateTime.Now;
                currentAd = null;

                UpdateAdState(AdState.Waiting);               
                break;
        }                    
    }

    public void ProcessAdCommands( Dictionary<string,object> gameParameters)
    {
        // Game Parameters, sent by Decision Point or Event Triggered Campaigns are 
        // used to control Interstitial and Rewarded Ads
        
        // REWARDS
        if (gameParameters.ContainsKey("adRewardValue") && gameParameters.ContainsKey("adRewardType"))
        {
            rewardType = gameParameters["adRewardType"].ToString();
            rewardValue = System.Convert.ToInt32(gameParameters["adRewardValue"]);
        }

        // FREQUENCY
        if (gameParameters.ContainsKey("adPerSessionLimit"))
        {            
            adsPerSession = System.Convert.ToInt32(gameParameters["adPerSessionLimit"]);
        }
        if (gameParameters.ContainsKey("adCooldownSeconds"))
        {        
            adCooldownSeconds = System.Convert.ToInt32(gameParameters["adCooldownSeconds"]);
        }
        
        // PROVIDER
        // ANY / UNITY / MOPUB / IRONSOURCE        
        if (gameParameters.ContainsKey("adProvider") && gameParameters.ContainsKey("adType"))
        {
            adProvider = gameParameters["adProvider"].ToString();
            
            // Interstitial Ads are shown immediately when commanded (dynamic placement)
            // Else we wait for the player to initiate a rewarded ad
            if (gameParameters["adType"].ToString() == "INTERSTITAL")
            {
                ShowInterstitialAd();
            }                      
        }
    }

    public void ShowInterstitialAd()
    {
        ShowAd("INTERSTITIAL");
    }

    public void ShowRewardedAd()
    {
        ShowAd("REWARDED");
    }

    private void ShowAd(string adType)
    { 
        currentAd = new Ad();
        currentAd.Provider = adProvider;
        currentAd.AdType = adType;
        currentAd.Completed = false;

    
        // Is it OK to show an Ad? 
        if (AdCounter < adsPerSession && RemainingCooldownSeconds() == 0)
        {
            // Which Ad Provider to use
            if (currentAd.Provider == "UNITY" || currentAd.Provider == "ANY")
            {

                adsUnity.ShowAd(currentAd);
            }
        }
    }

   public void RecordAdImpressionEvent()
    {
        GameEvent adEvent = new GameEvent("adImpression")
        .AddParam("adCompletionStatus", currentAd.Completed ? "COMPLETED" : "INCOMPLETE")
        .AddParam("adProvider", currentAd.Provider)
        .AddParam("placementType", currentAd.AdType)
        .AddParam("placementId", currentAd.PlacementId)
        .AddParam("placementName", currentAd.PlacementId);

        DDNA.Instance.RecordEvent(adEvent).Run();
    }
}

