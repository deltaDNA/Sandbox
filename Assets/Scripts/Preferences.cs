using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Preferences class will be used to save UI options
/// </summary>
public class Preferences 
{
    /// <summary>
    /// Handles saving the value based on true or false
    /// </summary>
    /// <param name="toggle">Boolean value that will be sent if start sdk on load or not</param>
    public static void SaveToggleStartSDK(bool toggle)
    {
        PlayerPrefs.SetInt("ToggleStartSDK", toggle ? 1 : 0);
    }

    /// <summary>
    /// GetToggleStartSDK get the value saved in playerprefs
    /// </summary>
    /// <returns>boolean</returns>
    public static bool GetToggleStartSDK()
    {
       return PlayerPrefs.GetInt("ToggleStartSDK") == 1 ? true:false ;
        
    }
}
