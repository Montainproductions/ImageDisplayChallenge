using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Sc_Menu_Individual : MonoBehaviour
{
    //Looping through Image
    private bool loopingThroughImages; //Whether or not the images are looping

    [SerializeField]
    private float timeBetweenLoops; //Time it takes before going to the next image while looping

    private int currentImagePosId; //The position of the current image

    private Images imageInformation; //The information on the current image

    [SerializeField]
    private RawImage imageDisplay; //Display area

    [SerializeField]
    private TMP_Text titleText; //Title text component

    [SerializeField]
    private TMP_Text iDText; //Id Value text component

    // Start is called before the first frame update
    void Start()
    {
        /* Showing Individual Images */
        loopingThroughImages = false;

        //currentImagePosId = 0;
    }

    //Gets the image chossen based on their id position in the array
    public void ImageChossen(int imageId, Images newImageInformation){
        currentImagePosId = imageId; //Sets the current image id
        imageInformation = newImageInformation; //Sets the current image information

        titleText.text = imageInformation.title; //Sets title name
        iDText.text = imageInformation.id.ToString(); //Sets id value

        StartCoroutine(Sc_UI_Manager.Instance.DownloadImage(imageInformation.url, imageDisplay)); //Downloads and sets image
    }

    //Leave the individual image menu
    public void ExitChoosingImage()
    {
        currentImagePosId = 0;

        Sc_UI_Manager.Instance.ChangingMenu(false, currentImagePosId);
    }

    //Button to see if it loops through images
    public void LoopingImages()
    {
        loopingThroughImages = !loopingThroughImages;

        if(loopingThroughImages){
            StartCoroutine(LoopImage());
        }
    }

    //Corutine to loop through all the Images
    public IEnumerator LoopImage()
    {
        currentImagePosId++;
        ImageChossen(currentImagePosId, Sc_UI_Manager.Instance.ReturnSpecificImage(currentImagePosId));
        yield return new WaitForSeconds(timeBetweenLoops);
        if(loopingThroughImages){ //Continue looping through the images
            StartCoroutine(LoopImage());
        }else{
            yield return null;
        }
    }

    //Pressing the button to get the next image
    public void GetNextImage(){
        if(currentImagePosId + 1 < Sc_UI_Manager.Instance.ReturnAlbumLength()){
            currentImagePosId++;
            ImageChossen(currentImagePosId, Sc_UI_Manager.Instance.ReturnSpecificImage(currentImagePosId));
        }
    }

    //Pressing the button to get the previous image
    public void GetPreviousImage(){
        if(currentImagePosId - 1 > 1){
            currentImagePosId--;
            ImageChossen(currentImagePosId, Sc_UI_Manager.Instance.ReturnSpecificImage(currentImagePosId));
        }
    }
}
