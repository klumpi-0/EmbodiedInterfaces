using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetImagesOnMixNMatch : MonoBehaviour
{
    public static SetImagesOnMixNMatch Instance;

    [Header("Puzzle Data")]
    [SerializeField] private PuzzleData currentPuzzle;

    [Header("LowCube")]
    [SerializeField] private Image[] lowImages = new Image[4];

    [Header("MiddleCube")]
    [SerializeField] private Image[] middleImages = new Image[4];

    [Header("HighCube")]
    [SerializeField] private Image[] highImages = new Image[4];

    [Header("More Sprites")]
    [SerializeField] private Sprite arrowSprite;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        //ApplyImages(currentPuzzle);
    }

    //public void ApplyImages(PuzzleData puzzle)
    //{
    //    if (puzzle == null)
    //    {
    //        Debug.LogError("No PuzzleData assigned!");
    //        return;
    //    }

    //    ApplyToCube(lowImages, puzzle.lowSprites);
    //    ApplyToCube(middleImages, puzzle.middleSprites);
    //    ApplyToCube(highImages, puzzle.highSprites);
    //}

    public void ApplyImages(Sprite[] low, Sprite[] middle, Sprite[] high)
    {
        ApplyToCube(lowImages, low);
        ApplyToCube(middleImages, middle);
        ApplyToCube(highImages, high);
    }

    private void ApplyToCube(Image[] images, Sprite[] sprites)
    {
        int count = Mathf.Min(images.Length, sprites.Length);

        for (int i = 0; i < count; i++)
        {
            if (images[i] != null && sprites[i] != null)
            {
                images[i].sprite = sprites[i];
                //images[i].SetNativeSize(); // optional
            }
        }
    }
    public void ApplySingleImageToAllSides(Sprite[] sprites)
    {
        SetAllSides(lowImages, sprites[0]);
        SetAllSides(middleImages, sprites[1]);
        SetAllSides(highImages, sprites[2]);
    }

    private void SetAllSides(Image[] images, Sprite sprite)
    {
        if (images == null || sprite == null)
            return;

        foreach (Image image in images)
        {
            if (image != null)
            {
                image.sprite = sprite;
            }
        }
    }

    public void SetupMoreInformationImages(int[] positionsArrowImage, Sprite[] otherSprites)
    {
        ApplySingleImageToAllSides(otherSprites);
        SetSingleImage(0, positionsArrowImage[0], arrowSprite);
        SetSingleImage(1, positionsArrowImage[1], arrowSprite);
        SetSingleImage(2, positionsArrowImage[2], arrowSprite);
    }

    public void SetSingleImage(int cube, int index, Sprite sprite)
    {
        Image[] target = GetCube(cube);

        if (target == null || index < 0 || index >= target.Length)
            return;

        target[index].sprite = sprite;
    }

    private Image[] GetCube(int cube)
    {
        return cube switch
        {
            0 => lowImages,
            1 => middleImages,
            2 => highImages,
            _ => null
        };
    }
}