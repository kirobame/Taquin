using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using Random = UnityEngine.Random;

// Classe principale visant à gérer la création de la situation initiale de jeu + Sa résolution.
public class Taquin : MonoBehaviour
{
    // Propriétés utilitaires
    public RectTransform RectTransform => (RectTransform)transform;
    public float Width => RectTransform.rect.width;
    
    // Données de pour la création de la situation de jeu
    [SerializeField] private SlicedImage image; // Représentation des images pour chaque pièce
    [SerializeField] private Image goal; // L'image obtenu une fois le jeu gagné
    
    [Space, SerializeField] private float margin; // De combien les pièces sont séparés des bords du cadre
    [SerializeField] private float spacing; // De combien les pièces sont espéacés entre elles

    [Space, SerializeField] private Part partPrefab; 

    private float Step => partSize + spacing;
    
    // Caching des pièces du jeu selon leur disposition spatiale
    private IPart[,] parts;
    
    // Caching de certaines valeurs utiles
    private Vector2 topLeft;
    private float partSize;
    
    //---INITIALISATION-------------------------------------------------------------------------------------------------/
    
    void Awake() => SetupGame();
    void Start()
    {
        goal.sprite = image.Goal;
        CreateParts();
    }

    // Bootup du manager et Ouvertures des évènements dont l'appel est géré par la classe
    private void SetupGame()
    {
        Manager.ClearEvents();
        Manager.state = GameState.Active;
        
        Manager.Open(TaquinEvent.OnWin);
    }
    
    // Création de la situation initiale de jeu
    private void CreateParts()
    {
        partSize = (Width - margin * 2.0f - spacing * (image.Size - 1)) / image.Size; // Taille d'une pièce
        topLeft = new Vector2(-partSize * image.Size * 0.5f - spacing, partSize * image.Size * 0.5f + spacing);

        ushort index = 0;
        var slices = image.GetSlices();
        parts = new IPart[image.Size, image.Size];

        var fullLength = image.Size * image.Size;
        var spots = new List<Vector2Int>(fullLength); // Enregistrement des indexes 2D
        
        for (var y = 0; y < image.Size; y++)
        {
            for (var x = 0; x < image.Size; x++)
            {
                var localPosition = topLeft + new Vector2(Step * x, -Step * y);

                if (index == fullLength / 2) // Check pour vérifier si il faut placer la case vide. Dans ce cas au centre
                {
                    // Utilisation d'un struct pour utilisation en tant que IPart sans avoir de réprésentation physique dans la scène
                    parts[x, y] = new EmptyPart(localPosition); 
                    parts[x, y].Initialize(this, new Vector2Int(x, y), (ushort)(index + 1), null);

                    index++;
                    continue;
                }

                // Instantion d'une pièce classique avec une représentation physique dans la scène
                var part = Instantiate(partPrefab, RectTransform);
                parts[x, y] = part;

                part.RectTransform.localPosition = localPosition;
                part.RectTransform.sizeDelta = Vector2.one * partSize;
                part.Initialize(this, new Vector2Int(x,y), (ushort)(index + 1), slices[index]);
                
                spots.Add(new Vector2Int(x, y));
                index++;
            }
        }

        var previousSpot = new Vector2Int(image.Size / 2, image.Size / 2); // Indexe 2D de où se trouve la pièce vide

        // Prise d'une pièce au hasard pour placer aléatoirement la pièce vide autre part qu'au centre
        index = (ushort)Random.Range(0, spots.Count);
        var spot = spots[index];
        spots.RemoveAt(index);
        
        // Échange de la pièce choisi & de la pièce vide - Position dans le tableau 2D + Valeurs
        parts.Swap(previousSpot, spot);
        spots.Add(previousSpot);

        // Correction, si nécessaire, de la parité de la pièce vide & du nombre de permutation pour "Shuffle" les pièces
        // Permet d'assurer que le jeu est une solution
        var count = spots.Count;
        if (spot.IsEven() == count.IsEven()) count--;

        // Transpositions selon la méthode d'échange utilisé précedemment
        for (var i = 0; i < count; i++)
        {
            index = (ushort)Random.Range(0, spots.Count);
            spot = spots[index];
            spots.RemoveAt(index);
            
            parts.Swap(previousSpot, spot);
            previousSpot = spot;
        }
    }
    
    //---MÉTHODES UTILITAIRES-------------------------------------------------------------------------------------------/

    public void SetImage(SlicedImage image) => this.image = image;
    public List<Part> GetParts()
    {
        var output = new List<Part>(image.Size * image.Size);

        for (var y = 0; y < image.Size; y++)
        {
            for (var x = 0; x < image.Size; x++)
            {
                if (!(parts[x, y] is Part part)) continue;
                output.Add(part);
            }
        }

        return output;
    }
    
    public Vector2 ConvertWorldToLocal(Vector2 world) => transform.InverseTransformPoint(world);
    public Vector2 ConvertPartToWorldPosition(IPart part) => transform.TransformPoint(part.Position);

    // Méthode échangeant de place 2 pièces dans le tableau 2D sans échanger leurs valeurs
    // L'échange de valeurs ne peut pas se faire complétement car au moment de l'appel, la pièce n'as pas fini de bouger
    public void Exchange(IPart l, IPart r) 
    {
        parts.Swap(l.Index, r.Index, false); // Échange

        // Vérification de la condition de victoire
        // Si toutes les pièces sont ordonnées, le joueur a gagné
        ushort order = 0;
        for (var y = 0; y < image.Size; y++)
        {
            for (var x = 0; x < image.Size; x++)
            {
                if (parts[x, y].Order != order + 1) return;
                order++;
            }
        }

        // Appel de l'event de Victoire & Changement correspondant pour l'état de jeu
        Manager.state = GameState.Won;
        Manager.Call(TaquinEvent.OnWin);
    }

    // Vérifie si la pièce adjacente à "from" est vide. Si oui, un drag pourras s'effectuer
    public bool TryGetTarget(IPart from, Vector2 delta, out IPart part)
    {
        // Détection de la direction dans lequel se fait le drag
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            if (delta.x > 0) return SubFunction(new Vector2Int(from.Index.x + 1, from.Index.y), out part);
            else return SubFunction(new Vector2Int(from.Index.x - 1, from.Index.y), out part);
        }
        else
        {
            if (delta.y > 0) return SubFunction(new Vector2Int(from.Index.x, from.Index.y - 1), out part);
            else return SubFunction(new Vector2Int(from.Index.x, from.Index.y + 1), out part);
        }

        // Vérification de la valeur de la case adjacente
        bool SubFunction(Vector2Int index, out IPart output)
        {
            if (index.x < 0 || index.y < 0 || index.x >= image.Size || index.y >= image.Size)
            {
                output = null;
                return false;
            }
            
            output = parts[index.x, index.y];
            return output is EmptyPart;
        }
    }
}