using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Preferences class will be used to save UI options
/// </summary>
public class Preferences 
{
    public static void SaveToggleStartSDK(bool toggle)
    {
        PlayerPrefs.SetInt("ToggleStatSDK", toggle ? 1 : 0);
    }
    public static bool GetToggleStartSDK()
    {
       return PlayerPrefs.GetInt("ToggleStatSDK") == 1 ? true:false ;
        
    }
}
