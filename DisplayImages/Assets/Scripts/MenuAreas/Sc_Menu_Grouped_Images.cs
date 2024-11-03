using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Sc_Menu_Grouped_Images : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown imagesPerPageComponent;

    [SerializeField]
    private GameObject imagesTo50; // Displaying X amount of Images
    private bool displayingFull50; //If 50 images would be displayed or not. Currently only have 25 and 50 as options but it can be adjusted depending on designers.

    [SerializeField]
    private RawImage[] displayImagesGroup; //All images where a thumbnail would appear

    [SerializeField]
    private int imagesPerPage; //Amount of pages to display per page

    [SerializeField]
    private List<string> imagesPerPagesOpt; //List of options for how many images to show

    private int startPageIndex, endPageIndex; //Starting and final indexes to determine which set of images to display. Used to find the position in array
    
    private int currentPage, totalPages; // Current Pages and total amount of Pages Info

    [SerializeField]
    private TMP_Text pagesText; //Text to show current page and total pages

    // Start is called before the first frame update
    void Start()
    {
        /* Showing Groups Images */
        SetAmountImagesOptions();
        imagesPerPageComponent.onValueChanged.AddListener(UpdateImagesPerPage);
        currentPage = 0;
        displayingFull50 = false;
    }

    //Sets how many images are shown per page.
    public void SetAmountImagesOptions()
    {
        imagesPerPageComponent.AddOptions(imagesPerPagesOpt);
        imagesTo50.SetActive(false);
    }

    //Updates how many images will be displayed. Based on dropdown menu in scene.
    public void UpdateImagesPerPage(int index)
    {
        imagesPerPage = Int32.Parse(imagesPerPageComponent.options[index].text);
        currentPage = 0;

        ImagesDisplayed();
    }

    //In case Images didnt reload.
    public void ReloadImagesDisplayed(){
        ImagesDisplayed();
    }

    //Displays the images on the corisponding positions
    public void ImagesDisplayed()
    {
        //Amount of images displayed. Either option 1 or 2
        if(imagesPerPage == Int32.Parse(imagesPerPagesOpt[0]) ){
            displayingFull50 = false;
            imagesPerPage = Int32.Parse(imagesPerPagesOpt[0]);
        }
        else{
            displayingFull50 = true;
            imagesPerPage = Int32.Parse(imagesPerPagesOpt[1]);
        }
        imagesTo50.SetActive(displayingFull50); //Will set active or not the entire image set.

        startPageIndex = imagesPerPage * currentPage; //Calculates what is the first image that needs to be displayed
        endPageIndex = Mathf.Min(startPageIndex + imagesPerPage, Sc_UI_Manager.Instance.ReturnAlbumLength()); //Determines what is the last image to be displayed. Either the whole set or the end of the list.

        int displayPos;

        //Loops through each index and gets the info for the image in that position of the array. Then shows image
        for (int i = startPageIndex; i <= endPageIndex; i++)
        {
            Images newImage = Sc_UI_Manager.Instance.ReturnSpecificImage(i); //Saves current Image information
            if(startPageIndex<imagesPerPage){
                displayPos = i;
            }else{
                displayPos = i - startPageIndex;
            }

            StartCoroutine(Sc_UI_Manager.Instance.DownloadImage(newImage.thumbnailUrl, displayImagesGroup[displayPos]));
        }
        
        //Determines what page it is currently in and how in total are there
        PagesCalculus();
    }

    //Goes to the next page
    public void GetNextPage()
    {
        if (currentPage + 1 < totalPages)
        {
            currentPage++;
            ImagesDisplayed();
        }
    }

    //Goes to the previous page
    public void GetPreviousPage()
    {
        if (currentPage + 1 > 1)
        {
            currentPage--;
            ImagesDisplayed();
        }
    }

    //Calculates the current total pages that exist and in which page the user is currently in
    public void PagesCalculus(){
        totalPages = Sc_UI_Manager.Instance.ReturnAlbumLength() / imagesPerPage;
        string pagesRatio = (currentPage+1) + "/" + totalPages;
        pagesText.text = pagesRatio;
    }

    //Choose a specific image and its positon.
    public void ChooseImage(int posChossen)
    {
        int currentImagePosId = startPageIndex + posChossen + 1;
        Sc_UI_Manager.Instance.ChangingMenu(true, currentImagePosId);
    }
}
