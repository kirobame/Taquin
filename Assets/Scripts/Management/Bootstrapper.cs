using System;
using System.Linq;
using UnityEngine;

// Classe s'occupant d'assigner la SlicedImage correspondant à la plateforme cible si possible
// Valeurs disponibles :
// - Android
// - Apple
public class Bootstrapper : MonoBehaviour
{
    #region Nested Types

    [Serializable]
    public class KeyValuePair
    {
        public RuntimePlatform Platform => platform;
        [SerializeField] private RuntimePlatform platform;

        public SlicedImage Image => image;
        [SerializeField] private SlicedImage image;
    }
    #endregion
    
    [SerializeField] private Taquin taquin;
    [SerializeField] private KeyValuePair[] keyValuePairs;

    void Awake()
    {
        var result = keyValuePairs.SingleOrDefault(kvp => kvp.Platform == Application.platform);
        if (result != null) taquin.SetImage(result.Image);
    }
}