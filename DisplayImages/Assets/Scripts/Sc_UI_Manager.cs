using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Purchasing.MiniJSON;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;
using UnityEngine.Networking;

public class Sc_UI_Manager : MonoBehaviour
{
    public static Sc_UI_Manager Instance { get; private set; }

    [SerializeField]
    private string WebURL = ""; //Link to get information from

    private Images[] albumEntry; //All entries

    [SerializeField]
    private GameObject entireDisplay, individualDisplay; //Menu options

    [SerializeField]
    private int retryAttempts; //Amount of attempts to load an image before trying again

    [SerializeField]
    private int timePerImageLoad; //Amount of time per attempt to load an image before trying again.

    private bool singleImageShowing; //Showing individual Image;

    [SerializeField]
    private Sc_Menu_Grouped_Images groupedImages; //Menu scripts

    [SerializeField]
    private Sc_Menu_Individual individualImages;

    //Awake method
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Destroy");
            Destroy(this);
        }
        else
        {
            Debug.Log("Create Instance");
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        singleImageShowing = false;

        /* Activate either menu */
        entireDisplay.SetActive(!singleImageShowing);
        individualDisplay.SetActive(singleImageShowing);
    }

    //Download an image given a link and place it in an image.
    public IEnumerator DownloadImage(string url, RawImage imagePosition)
    {
        int attempts = 0; //Current attempt to download image
        while (attempts < retryAttempts)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url)) //Attempt to get request 
            {
                request.timeout = timePerImageLoad; //Amount of time to try to download image
                yield return request.SendWebRequest();

                //If request works then load image
                if (request.result == UnityWebRequest.Result.Success)
                {
                    byte[] imageBytes = request.downloadHandler.data;
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageBytes); //Load Image

                    imagePosition.texture = texture;
                }
                else if (request.responseCode == 504) // 504 Gateway Timeout error
                {
                    Debug.LogWarning("Timeout occurred, retrying...");
                    attempts++;
                    yield return new WaitForSeconds(2);
                }
                else if (request.responseCode == 502) // 502 Bad Gateway error
                {
                    Debug.LogWarning("Bad Gateway, retrying...");
                    attempts++;
                    yield return new WaitForSeconds(2);
                }
                {
                    Debug.LogError("Failed to download image: " + request.error);
                    yield break;
                }
            }
        }

        if(attempts < retryAttempts){
            Debug.Log("Finished loading image");
        }else{
            Debug.Log("To many attempts made. Moving to next Image.");
        }
    }

    #region WorkingWithData
    // Load Data
    public void GetAlbumEntries()
    {
        StartCoroutine(RestClient.Instance.Get(WebURL, SetImageToArray));
        Debug.Log("Album Loaded");
    }

    //Gets alll Images Information into a new array. Meant to keep in more centralized instead of the grouped Images script.
    public void SetImageToArray(Images[] listImages)
    {
        albumEntry = listImages;

        groupedImages.ImagesDisplayed();
    }

    //Prints all information about album entry.
    public void OutputAlbumEntries()
    {
        if(albumEntry == null) Debug.Log("No album loaded"); return;

        foreach (Images image in albumEntry)
        {
            Debug.Log("Image ID: " + image.id);
            Debug.Log("Image Album ID: " + image.albumId);
            Debug.Log("Image Title: " + image.title);
            Debug.Log("Image URL" + image.url);
            Debug.Log("Image Thumbnail URL: " + image.thumbnailUrl);
        }
    }

    //Clear all the information in the array albumEntry
    public void ClearAlbumEntry()
    {
        Array.Clear(albumEntry, 0, albumEntry.Length);
        Debug.Log("Album Cleared");
    }
    #endregion

    //Switches between displaying an individual image and the entire display
    public void ChangingMenu(bool individual, int imageId)
    {
        singleImageShowing = individual;
        entireDisplay.SetActive(!individual);
        individualDisplay.SetActive(individual);

        if(individual){
            individualImages.ImageChossen(imageId, albumEntry[imageId]);
        }
    }

    //Returns the array albumEntry
    public Images[] ReturnWholeAlbum()
    {
        return albumEntry;
    }

    //Returns the information for a image in a position
    public Images ReturnSpecificImage(int imagePos){
        return albumEntry[imagePos];
    }

    //Returns the size of albumEntry
    public int ReturnAlbumLength()
    {
        return albumEntry.Length;
    }
}
