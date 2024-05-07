using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearAttack : MonoBehaviour
{
    [SerializeField]
    private float _xOffset = 0f;
    private float previousXOffset = 0f;

    public float xOffset
    {
        get { return _xOffset; }
        set
        {
            _xOffset = value;
            SpearLunge();
        }
    }

    private void Update()
    {
        if (xOffset != previousXOffset)
        {
            SpearLunge();
            previousXOffset = xOffset;
        }
    }

    public void SpearLunge()
    {
        transform.position = new Vector2(transform.position.x + xOffset, transform.position.y);
        print(transform.position.x);
    }
}
