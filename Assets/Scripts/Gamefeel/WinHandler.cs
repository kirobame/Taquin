using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

// Classe lancant une suite de feedbacks sur le callback de Victoire
public class WinHandler : MonoBehaviour
{
    [SerializeField] private Taquin taquin;
    [SerializeField] private Animator goal;
    [SerializeField] private Animator banner;
    
    void Start() => Manager.SubscribeTo(TaquinEvent.OnWin, OnWin);
    void OnDestroy() => Manager.UnsubscribeFrom(TaquinEvent.OnWin, OnWin);

    void OnWin() => StartCoroutine(ClearRoutine());
    private IEnumerator ClearRoutine()
    {
        var parts = taquin.GetParts();

        while (parts.Count > 0)
        {
            var count = Mathf.Clamp(Random.Range(0, 2), 0, parts.Count);
            for (var i = 0; i < count; i++)
            {
                var index = Random.Range(0, parts.Count);
                var part = parts[index];
                parts.RemoveAt(index);
                
                part.Disappear();
            }
            
            yield return new WaitForSeconds(Random.Range(0.025f, 0.075f));
        }
        
        yield return new WaitForSeconds(0.15f);
        goal.SetTrigger("Play");
        
        yield return new WaitForSeconds(1f);
        banner.SetTrigger("Play");
    }
}