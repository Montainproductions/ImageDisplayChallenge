using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Purchasing.MiniJSON;

public class RestClient : MonoBehaviour
{
    public static RestClient instance;
    public static RestClient[] Instances;
    public static RestClient Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<RestClient>();
                //Debug.Log(instance);
                if (instance == null && Application.isPlaying)
                {
                    Debug.Log("Hello");
                    //Debug.Log(instance);
                    GameObject go = new GameObject();
                    go.name = typeof(RestClient).Name;
                    go.hideFlags = HideFlags.HideAndDontSave;
                    instance = go.AddComponent<RestClient>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    public IEnumerator Get(string url, System.Action<Images[]> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError) Debug.Log(www.error);

            if (www.isDone)
            {
                string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);

                Images[] listImages = JsonConvert.DeserializeObject<Images[]>(jsonResult);

                callback(listImages);
            }
        }
    }
}
