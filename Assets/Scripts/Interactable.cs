using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] Gun contents;
    [SerializeField] GameObject promptParent;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>()) promptParent.SetActive(true);
    }

    private void Start()
    {
        promptParent.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>()) promptParent.SetActive(false);
    }

    private void Update()
    {
        if (promptParent.activeInHierarchy && Input.GetKeyDown(KeyCode.E)) {
            GameManager.i.SpawnGunPickup(contents, transform.position);
            Destroy(gameObject);
        }
    }
}
