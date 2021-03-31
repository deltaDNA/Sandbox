using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        return PlayerPrefs.GetInt("ToggleStartSDK") == 1 ? true : false;

    }

    public static void SaveToggleSandbox(bool toggle)
    {
        PlayerPrefs.SetInt("ToggleSandBox", toggle ? 1 : 0);
    }

    public static bool GetToggleSandbox()
    {
        return PlayerPrefs.GetInt("ToggleStartSDK") == 1 ? true : false;
    }

    public static void SaveToggleUseEditor(bool toggle)
    {
        PlayerPrefs.SetInt("ToggleEditor", toggle ? 1 : 0);
    }

    public static bool GetToggleUseEditor()
    {
        return PlayerPrefs.GetInt("ToggleEditor") == 1 ? true : false;
    }

    public static void SaveInputField(InputField fieldName)
    {
        PlayerPrefs.SetString(fieldName.name, fieldName.text);
    }
    public static void SaveInputField(InputField fieldName, string overrideValue)
    {
        PlayerPrefs.SetString(fieldName.name, overrideValue);
    }
    public static string GetInputField(InputField fieldName)
    {
        return PlayerPrefs.GetString(fieldName.name);
    }


    public static void SaveEnvironment(int value)
    {
        PlayerPrefs.SetInt("SelectedEnvironment", value);
    }

    public static int GetEnvironment()
    {
        return PlayerPrefs.GetInt("SelectedEnvironment");
    }
}
