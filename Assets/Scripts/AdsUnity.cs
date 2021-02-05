using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using DeltaDNA; 

public class AdsUnity : MonoBehaviour, IUnityAdsListener
{
    AdsManager adsManager;
    
     
    // Unit Ads
    [Header("Game IDs")]
    [SerializeField] private string androidGameId = "3992843";
    [SerializeField] private string iosGameId = "3992842";

    [Header("Placements")]    
    [SerializeField] private string rewardedPlacementId = "rewardedVideo";
    [SerializeField] private string interstitialPlacementId = "interstitialVideo";

    [Header("Settings")]
    [SerializeField] private bool testMode = false;

    private ShowOptions showOptions; 
    private Ad currentAd ; 

    public void Start()
    {
        adsManager = gameObject.GetComponent<AdsManager>();

        // Initialise Unity Ads
        Advertisement.AddListener(this);
        #if UNITY_ANDROID || UNITY_EDITOR
            Advertisement.Initialize(androidGameId, testMode);
        #else
            Advertisement.Initialize(iosGameId, testMode);
        #endif

        ConfigureUnityAdsMetaData();
    }

    public void ConfigureUnityAdsMetaData()
    {
        // Send the deltaDNA userID + Metadata to Unity Ads to recveive adRevenue event
        // back in DDNA if Laurie's server based adRevenue script is enabled      
        if (DDNA.Instance.HasStarted)
        {
            showOptions.gamerSid = DDNA.Instance.UserID;

            MetaData metaData = new MetaData("DDNA");
            metaData.Set("sessionID", DDNA.Instance.SessionID);
            metaData.Set("collectURL", DDNA.Instance.CollectURL);
            metaData.Set("environmentKey", DDNA.Instance.EnvironmentKey);

            Advertisement.SetMetaData(metaData);
        }
    }

    public void ShowAd(Ad ad)
    {
        currentAd = ad;
        if (showOptions == null)
        {
            showOptions = new ShowOptions();
            ConfigureUnityAdsMetaData();
        }

        currentAd.PlacementId = currentAd.AdType == "INTERSTITIAL" ? interstitialPlacementId : rewardedPlacementId;        
        Advertisement.Show( currentAd.PlacementId , showOptions);
    }



    #region Unity Ads Listeners
    // Unity Ads Listeners
    public void OnUnityAdsReady(string placementId)
    {
        adsManager.State = AdState.Ready; 
        Debug.Log("Unity Ad Ready for PlacementID: " + placementId);
    }
    public void OnUnityAdsDidError(string message)
    {
        adsManager.currentAd = currentAd; 
        adsManager.State = AdState.Error;
        Debug.Log("Unity Ads Error: " + message);
    }
    public void OnUnityAdsDidStart(string placementId)
    {
        Debug.Log("Unity Ad Started for PlacementID: " + placementId);
        adsManager.State = AdState.Watching; 
    }
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        adsManager.State = showResult == ShowResult.Finished ? AdState.Completed : AdState.Finished;            
    }
    #endregion
}
