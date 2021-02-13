using UnityEngine;

public interface IPart : ISwappable<IPart>
{
    ushort Order { get; } // Quelle est l'index + 1 de ce morceau d'image
    
    Vector2Int Index { get; set; } // Index dans le tableau 2D de la classe Taquin, permet de bypass des "Find"
    Vector2 Position { get; set; } // Position locale de la pièce

    void Initialize(Taquin taquin, Vector2Int index, ushort order, Sprite sprite);
}