using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeltaDNA;

public class DDNA_Manager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Default Configuration points to
        // https://www.deltadna.net/demo-account/sandbox/dev 
        DDNA.Instance.SetLoggingLevel(DeltaDNA.Logger.Level.DEBUG);
        DDNA.Instance.StartSDK(); 
    }


}
