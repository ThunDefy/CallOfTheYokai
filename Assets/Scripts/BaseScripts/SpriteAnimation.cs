using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] private int frameRate;
    [SerializeField] private bool loop;
    [SerializeField] private bool haveEndSprites;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Sprite[] endSprites;
    [SerializeField] private UnityEvent onComplete;

    private SpriteRenderer _renderer;
    private float secondsPerFrame;
    private int currentSpriteIndex;
    private float nextFrameTime;
    private Sprite[] currentSprites;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        currentSprites = sprites;
    }

    private void OnEnable()
    {
        secondsPerFrame = 1f / frameRate;
        nextFrameTime = Time.time + secondsPerFrame;
        currentSpriteIndex = 0;
    }
    private void Update()
    {
        if (nextFrameTime > Time.time) return;

        if (currentSpriteIndex >= currentSprites.Length)
        {
            if (loop)
            {
                currentSpriteIndex = 0;
            }
            else
            {
                enabled = false;
                onComplete?.Invoke();
                return;
            }
        }
        _renderer.sprite = currentSprites[currentSpriteIndex];
        nextFrameTime += secondsPerFrame;
        currentSpriteIndex++;
    }

    public void EndingAnimation()
    {
        if (haveEndSprites)
        {
            currentSprites = endSprites;
            currentSpriteIndex = 0;
            loop = false;
        }
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
