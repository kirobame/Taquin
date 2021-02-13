using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Classe responsable des interactions joueurs
public class Part : MonoBehaviour, IPart, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Nested Types

    public enum DragState : byte
    {
        Inactive = 0, // La pièce n'est pas entrain d'être bouger
        Active = 1, // La pièce se fait drag
        Finishing = 2  // La pièce finit le déplacement initié par le drag
    }
    #endregion
    
    // Implémentation des données requises par IPart
    public ushort Order { get; private set; }
    public Vector2Int Index { get; set; }
    public Vector2 Position
    {
        get => RectTransform.localPosition;
        set => RectTransform.localPosition = value;
    }
    
    // Propriétés utilitaires
    public RectTransform RectTransform => (RectTransform)transform;
    
    [SerializeField] private Image image;
    [SerializeField] private float dragSmoothing; // Smoothdamp value

    [Space, SerializeField] private Animator animator;
    
    private Taquin taquin;

    // Valeurs nécessaires pour le drag
    private Coroutine dragRoutine;
    private DragState dragState;
    
    private IPart dragTarget; // Quelle pièce que celle ci remplaceras si le drag est finalisé
    private float dragProgress; // Valeur entre 0 & 1 représentant l'avancement du drag
    private Vector2 dragStart; // Caching de la position de cette pièce avant le drag
    private Vector2 dragVelocity; // Smoothdamp value
    
    //---INITIALISATION & CHANGEMENT STRUCTURELLES----------------------------------------------------------------------/
    
    public void Initialize(Taquin taquin, Vector2Int index, ushort order, Sprite sprite)
    {
        this.taquin = taquin;

        Order = order;
        Index = index;
        
        image.sprite = sprite;
    }
    
    // Échange de valeur entre deux pièces pour l'actualisation visuelle & maintien du lien au tableau 2D
    public void SwapWith(IPart part) 
    {
        var positionCopy = Position;
        var indexCopy = Index;
        
        Position = part.Position;
        Index = part.Index;
        
        part.Position = positionCopy;
        part.Index = indexCopy;
    }
    
    //---DRAG-----------------------------------------------------------------------------------------------------------/
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Si le joueur n'as pas encore perdu ou gagné & qu'il drag dans une direction correcte -> Commencer le drag de la pièce
        if (Manager.state != GameState.Active || !taquin.TryGetTarget(this, eventData.delta, out dragTarget)) return;

        var dragOffset = eventData.position - (Vector2)RectTransform.position;
        var dragDestination = taquin.ConvertPartToWorldPosition(dragTarget);

        // Si la finition du mouvement d'un drag etait en cours -> L'annuler et commencer le nouveau
        if (dragRoutine != null ) StopCoroutine(dragRoutine); 
        else dragStart = RectTransform.position;
        
        dragState = DragState.Active;
        dragRoutine = StartCoroutine(DragRoutine(dragOffset, dragDestination));
    }
    
    private IEnumerator DragRoutine(Vector2 dragOffset, Vector2 dragDestination)
    {
        var start = dragStart + dragOffset;
        var destination = dragDestination + dragOffset;

        while (true) // Tant que le drag n'est pas fini, drag la pièce
        {
            // Projection de la position de la souris sur la ligne reliant cette pièce et sa destination de drag
            var projection = (Vector2)Vector3.Project((Vector2)Input.mousePosition - start, destination - start) + start;
            
            // Conversion de la projection en ratio pour définir la target du Smoothdamp
            dragProgress = projection.InverseLerp(start, destination);
            var point = Vector2.Lerp(start, destination, dragProgress) - dragOffset;
        
            RectTransform.position = Vector2.SmoothDamp(RectTransform.position, point, ref dragVelocity, dragSmoothing);
            yield return new WaitForEndOfFrame();
        }
    }
    
    // Callback vide mais nécessaire pour être un candidat valide pour les autres "IDragHandler"
    public void OnDrag(PointerEventData eventData) { }
    public void OnEndDrag(PointerEventData eventData)
    {
        // Ne pas finir le drag si il provient de la CompletionRoutine
        if (dragRoutine == null || dragState != DragState.Active) return;
        
        dragState = DragState.Finishing;
        StopCoroutine(dragRoutine);
        
        if (dragProgress > 0.5) // Si le joueur a suffisament avancé la pièce, finir le mouvement vers la destination
        {
            var goal = taquin.ConvertPartToWorldPosition(dragTarget);
            dragRoutine = StartCoroutine(CompletionRoutine(goal));
                
            dragTarget.Position = taquin.ConvertWorldToLocal(dragStart);
         
            // Échange des valeurs d'indexes mais pas de valeurs sous risque de compromettre la CompletionRoutine
            // Data race condition
            var indexCopy = Index;
            Index = dragTarget.Index;
            dragTarget.Index = indexCopy;
                
            // Notifier le taquin de l'échange pour l'actualisation de la situation de jeu globale
            taquin.Exchange(this, dragTarget);
            dragStart = goal;
        }
        else dragRoutine = StartCoroutine(CompletionRoutine(dragStart)); // Remettre le taquin là où il était avant le drag
    }

    // Finit le mouvement initié par le drag
    private IEnumerator CompletionRoutine(Vector2 destination) 
    {
        while (Vector2.Distance(RectTransform.position, destination) > 0.001f)
        {
            RectTransform.position = Vector2.SmoothDamp(RectTransform.position, destination, ref dragVelocity, dragSmoothing);
            yield return new WaitForEndOfFrame();
        }
        
        dragState = DragState.Inactive;
        dragRoutine = null;
    }
    
    //---FEEDBACKS------------------------------------------------------------------------------------------------------/
    
    public void Disappear() => animator.SetTrigger("Play");
}