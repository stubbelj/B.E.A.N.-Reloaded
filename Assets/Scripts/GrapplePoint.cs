using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    [SerializeField] Sprite activeSprite;
    Sprite defaultSprite;
    SpriteRenderer srend;  
    PlayerGrapple pGrapple; 
    [HideInInspector] public bool hovered;

    private void Start()
    {
        srend = GetComponent<SpriteRenderer>();
        defaultSprite = srend.sprite;
    }

    private void Update()
    {
        if (pGrapple == null) {
            pGrapple = FindObjectOfType<PlayerGrapple>();
            return;
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10);

        hovered = Vector2.Distance(mousePos, transform.position) < pGrapple.GetHoverDist();
        if (hovered) pGrapple.SetPoint(this);
        if (srend) srend.sprite = hovered ? activeSprite : defaultSprite;
    }
}
