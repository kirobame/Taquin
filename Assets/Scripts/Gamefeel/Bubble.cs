using System;
using UnityEngine;
using UnityEngine.UI;

public class Bubble : MonoBehaviour
{
    public event Action<Bubble> onOutOfBounds;

    public RectTransform RectTransform => (RectTransform)transform;
    
    [SerializeField] private Image image;

    private float limit;
    private float speed;

    public void Initialize(float limit, float speed, Color color)
    {
        this.limit = limit;
        this.speed = speed;

        image.color = color;
    }

    void Update()
    {
        var yMin = RectTransform.position.y - RectTransform.sizeDelta.y * 0.5f;
        if (yMin > limit)
        {
            onOutOfBounds?.Invoke(this);
            return;
        }

        transform.position += Vector3.up * (Time.deltaTime * speed);
    }
}