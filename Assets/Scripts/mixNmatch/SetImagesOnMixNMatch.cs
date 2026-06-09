using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetImagesOnMixNMatch : MonoBehaviour
{
    public static SetImagesOnMixNMatch Instance;

    [Header("LowCube")]
    [SerializeField] private Image imgLow01;
    [SerializeField] private Image imgLow02;
    [SerializeField] private Image imgLow03;
    [SerializeField] private Image imgLow04;

    [Header("MiddleCube")]
    [SerializeField] private Image imgMiddle01;
    [SerializeField] private Image imgMiddle02;
    [SerializeField] private Image imgMiddle03;
    [SerializeField] private Image imgMiddle04;

    [Header("HighCube")]
    [SerializeField] private Image imgHighe01;
    [SerializeField] private Image imgHighe02;
    [SerializeField] private Image imgHighe03;
    [SerializeField] private Image imgHighe04;

    private List<Image> lowImages = new List<Image>();
    private List<Image> middleImages = new List<Image>();
    private List<Image> highImages = new List<Image>(); 
    
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        AddImagesToList();

    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="cube">0 = low, 1 = middle, 2 = high</param>
    /// <param name="numImage"></param>
    public void SetImages(Sprite sprite, int cube, int numImage)
    {
        List<Image> targetList = GetCubeList(cube);

        if (targetList == null)
        {
            Debug.LogError("Invalid cube index!");
            return;
        }

        SetImageOnCube(targetList, sprite, numImage);
    }

    private List<Image> GetCubeList(int cube)
    {
        return cube switch
        {
            0 => lowImages,
            1 => middleImages,
            2 => highImages,
            _ => null
        };
    }

    private void SetImageOnCube(List<Image> imageList, Sprite sprite, int index)
    {
        if (index < 0 || index >= imageList.Count)
        {
            Debug.LogError("Index out of range!");
            return;
        }

        imageList[index].sprite = sprite;
    }

    private void AddImagesToList()
    {
        lowImages = new List<Image>
        {
            imgLow01,
            imgLow02,
            imgLow03,
            imgLow04
        };

        middleImages = new List<Image>
        {
            imgMiddle01,
            imgMiddle02,
            imgMiddle03,
            imgMiddle04
        };

        highImages = new List<Image>
        {
            imgHighe01,
            imgHighe02,
            imgHighe03,
            imgHighe04
        };
    }
}
