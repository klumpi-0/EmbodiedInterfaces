using UnityEngine;
using static UnityStandardAssets.ImageEffects.BloomOptimized;

public class CreateMixNMatchFill : MonoBehaviour
{
    public static CreateMixNMatchFill Instance;
    public PuzzleData puzzleData;
    public int[] solution;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns>LowImages, MiddleImages, HighImages, CorrectSolution</returns>
    public (Sprite[], Sprite[], Sprite[], int[]) CreateNewFill(PuzzleData data)
    {
        this.puzzleData = data;
        puzzleData.correctSolutionSites = CreateCorrectSolution();
        puzzleData.arrowSolutionSites = CalculateArrowSolutions(puzzleData.correctSolutionSites);
        solution = puzzleData.correctSolutionSites;
        Sprite[] cubeLow = CreateRow(
            puzzleData.correctSprites[0],
            puzzleData.lowSprites,
            solution[0]);

        Sprite[] cubeMiddle = CreateRow(
            puzzleData.correctSprites[1],
            puzzleData.middleSprites,
            solution[1]);

        Sprite[] cubeHigh = CreateRow(
            puzzleData.correctSprites[2],
            puzzleData.highSprites,
            solution[2]);

        return (cubeLow, cubeMiddle, cubeHigh, solution);
    }

    private int[] CreateCorrectSolution()
    {
        return new int[3] { Random.Range(0, 3), Random.Range(0, 3), Random.Range(0, 3) };
    }

    private Sprite[] CreateRow(Sprite correctSprite, Sprite[] diversionSprites, int correctIndex)
    {
        Sprite[] result = new Sprite[4];

        // Korrektes Bild platzieren
        result[correctIndex] = correctSprite;

        // Ablenkungsbilder mischen
        Sprite[] shuffledDiversions = (Sprite[])diversionSprites.Clone();

        for (int i = 0; i < shuffledDiversions.Length; i++)
        {
            int randomIndex = Random.Range(i, shuffledDiversions.Length);

            (shuffledDiversions[i], shuffledDiversions[randomIndex]) =
                (shuffledDiversions[randomIndex], shuffledDiversions[i]);
        }

        // Restliche Plätze füllen
        int diversionIndex = 0;

        for (int i = 0; i < result.Length; i++)
        {
            if (i == correctIndex)
                continue;

            result[i] = shuffledDiversions[diversionIndex++];
        }

        return result;
    }

    private int[] CalculateArrowSolutions(int[] correctSolution)
    {
        int[] result = new int[3];
        for (int i = 0;i < correctSolution.Length;i++)
        {
            result[i] = (correctSolution[i] + 2) % 4;
        }
        return result;
    }
}
