using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Classe s'occupant de spawn un certain nombre de "Particule" et de faire boucler leur comportement
public class Background : MonoBehaviour
{
    public RectTransform RectTransform => (RectTransform)transform;
    
    [SerializeField] private Bubble prefab;
    [SerializeField] private int count;
    [SerializeField] private Vector2 spawnDelayRange;
    
    [Space, SerializeField] private Vector2 sizeRange;
    [SerializeField] private Vector2 speedRange;
    [SerializeField] private Color minColor;
    [SerializeField] private Color maxColor;

    private List<Bubble> bubbles;
    
    void Awake()
    {
        bubbles = new List<Bubble>(count);
        StartCoroutine(SpawnRoutine());
    }
    private IEnumerator SpawnRoutine()
    {
        for (var i = 0; i < count; i++)
        {
            var bubble = Instantiate(prefab, transform);
            bubble.onOutOfBounds += SetupBubble;
            SetupBubble(bubble);
            
            yield return new WaitForSeconds(Random.Range(spawnDelayRange.x, spawnDelayRange.y));
        }
    }

    public void SetupBubble(Bubble bubble)
    {
        var rect = RectTransform.rect;
        rect.center = RectTransform.TransformPoint(rect.center);
        
        var ratio = Random.Range(0f, 1f);
        
        var x = Random.Range(rect.xMin, rect.xMax);    
        var size = Mathf.Lerp(sizeRange.y, sizeRange.x, ratio);

        bubble.RectTransform.sizeDelta = Vector2.one * size;
        bubble.RectTransform.position = new Vector2(x, rect.yMin - bubble.RectTransform.sizeDelta.y * 0.5f);
        
        var speed = Mathf.Lerp(speedRange.y, speedRange.x, ratio);
        var color = Color.Lerp(maxColor, minColor, ratio);

        bubble.Initialize(rect.yMax, speed, color);
    }
}