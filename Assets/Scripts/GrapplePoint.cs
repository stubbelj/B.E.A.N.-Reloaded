using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    [SerializeField] Sprite activeSprite;
    Sprite defaultSprite;
    SpriteRenderer srend => GetComponent<SpriteRenderer>();
    PlayerGrapple pGrapple => FindAnyObjectByType<PlayerGrapple>();
    [HideInInspector] public bool hovered;

    private void Start()
    {
        defaultSprite = srend.sprite;
    }

    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10);

        hovered = Vector2.Distance(mousePos, transform.position) < pGrapple.GetHoverDist();
        if (hovered) pGrapple.SetPoint(this);
        srend.sprite = hovered ? activeSprite : defaultSprite;
    }
}
