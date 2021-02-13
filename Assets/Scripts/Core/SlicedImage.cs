using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSlicedImage", menuName = "Test/Sliced Image")]
// Classe intermédiare définissant une image, ses morceaux & la taille souhaité du tableau 2D concernant l'utilisation des morceaux
public class SlicedImage : ScriptableObject
{
    public ushort Size => size;
    public Sprite Goal => goal;
    
    [SerializeField] private ushort size;
    [SerializeField] private Sprite[] slices;
    [SerializeField] private Sprite goal;

    public Sprite[] GetSlices()
    {
        var output = new Sprite[size * size];
        if (slices.Length != output.Length) Debug.LogError($"Invalid data exception for {this} ! The number of slices does not match the wanted size");
        
        Array.Copy(slices, output, output.Length);
        return output;
    }
}