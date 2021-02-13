using UnityEngine;

// Classe permettant d'éviter l'instantiation d'une pièce "vide" tout en maintenant le fonctionement homogène 
// au sein du tableau 2D de la classe Taquin
public struct EmptyPart : IPart
{
    public EmptyPart(Vector2 position)
    {
        Position = position;
        Index = Vector2Int.zero;
        Order = 0;
    }

    public ushort Order { get; private set; }
    public Vector2Int Index { get; set; }
    public Vector2 Position { get; set; }

    public void Initialize(Taquin taquin, Vector2Int index, ushort order, Sprite sprite)
    {
        Order = order;
        Index = index;
    }
    
    // Implémentation de la fonction swap pour correctement fonctionner avec les autres implémentations d'IPart
    public void SwapWith(IPart part)
    {
        var positionCopy = Position;
        var indexCopy = Index;
        
        Position = part.Position;
        Index = part.Index;
        
        part.Position = positionCopy;
        part.Index = indexCopy;
    }
}