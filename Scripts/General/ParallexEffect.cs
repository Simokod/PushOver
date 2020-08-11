using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallexEffect : MonoBehaviour
{
    public GameObject cam;
    public float XparallexEffect = 0.5f;
    public float YparallexEffect = 0.5f;
    private float length;
    private Vector2 startingPos;
    void Start() {
        startingPos = transform.position;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    void Update() {
        float temp = cam.transform.position.x * (1 - XparallexEffect);
        float distX = cam.transform.position.x * XparallexEffect;
        float distY = cam.transform.position.y * YparallexEffect;
        transform.position = new Vector2(startingPos.x + distX, startingPos.y + distY);
        if (temp > startingPos.x + length)
            startingPos.x += length;
        else
            if (temp < startingPos.x - length)
                startingPos.x -= length;        
    }
}
