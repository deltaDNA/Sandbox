using System;
using UnityEngine;
using UnityEngine.Networking;
using DeltaDNA;
using System.Collections.Generic;



public class DDNA_DefnitionFetcher : MonoBehaviour
{




    // Start is called before the first frame update
    void Awake()
    {

    }

    private object requestEvents()
    { 

        int returnedCount = 1;
        int currentPage = 0;
        int i = 0; 
        while (returnedCount > 0 && i < 50)

        {
            i++;

            string Uri = String.Format("https://api.deltadna.net/api/events/v1/event-parameters?limit=100&page={0}", currentPage);

            UnityWebRequest WebRequest = UnityWebRequest.Get(Uri);
            //WebRequest.SetRequestHeader("Authorization", String.Format("Bearer {0}",tokenObj.idToken));

            WebRequest.SendWebRequest();
            if (WebRequest.isNetworkError || WebRequest.isHttpError)
            {
                Debug.LogError(WebRequest.error);
                break;
                return null;
            }
            string fetchedText = WebRequest.downloadHandler.text;
            string wrappedText = String.Concat(new List<string> { "{\"parameters\":[", fetchedText, "]}" });

            var parameters = JsonUtility.FromJson<DDNA_ParameterBlock>(fetchedText);
            //TODO - parse here >:(
            currentPage++;
        }


        return null;
    }

    private void receiveEvents(AsyncOperation asyncOperation)
    {
        //validate the received data, and then pass it on to the actual handler
        //make sure to wrap it first. They use <ArrayResponseWrapper<T>>

    }

    private class DDNA_ParameterBlock<T>
    {
        public T[] parameters;
    }


    


    

}
