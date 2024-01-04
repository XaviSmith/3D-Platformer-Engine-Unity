using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public GemColours GemColour;
    [SerializeField] bool unlocked;
    [SerializeField] Material defaultMaterial; //For if we ever want to go back to the default for whatever reason
    [SerializeField] Material unlockedMaterial;
    [SerializeField] MeshRenderer mesh;

    void Start()
    {
        LoadGem();
    }

    public void Unlock()
    {
        unlocked = true;
        SetMaterial();

        PlayerPrefs.SetInt(GemColour.ToString() + "_unlocked", 1);
    }

    public void SetMaterial()
    {
        mesh.material = unlocked ? unlockedMaterial : defaultMaterial;
    }

    void LoadGem()
    {
        if (PlayerPrefs.HasKey(GemColour.ToString() + "_unlocked"))
        {
            Unlock();
        }
    }
}
