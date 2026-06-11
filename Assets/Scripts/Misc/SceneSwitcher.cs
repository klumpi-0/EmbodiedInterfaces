using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    if(OVRInput.GetDown(OVRInput.Button.One))
        {
            SceneManager.LoadScene("Scenes/ParticleSampleScene");
        }
    if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            SceneManager.LoadScene("Scenes/SampleScene");
        }
    }


}
