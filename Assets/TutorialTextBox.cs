using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialTextBox : MonoBehaviour
{
    [SerializeField] AnimationCurve growCurve;
    [SerializeField] float time = 1.2f, range;
    [SerializeField] Sound startSound;
    [SerializeField] bool canHide;
    float timeLeft;
    float width;
    Transform player;
    bool moving, open;
    
    void Start()
    {
        startSound = Instantiate(startSound);
        width = transform.GetChild(0).localScale.x;
        transform.GetChild(0).localScale = new Vector2(0, transform.GetChild(0).localScale.y);
        player = FindObjectOfType<PlayerController>().transform;
    }

    void Update()
    {
        if (canHide && open && Vector2.Distance(player.position, transform.position) > range) {
            StartCoroutine(Activate(false));
        }
        if (!open && Vector2.Distance(player.position, transform.position) < range) {
            startSound.Play();
            StartCoroutine(Activate(true));
        }
    }

    IEnumerator Activate(bool open)
    {
        if (moving) yield break;
        this.open = open;
        moving = true;

        transform.GetChild(0).localScale = new Vector2(open ? 0 : width, transform.GetChild(0).localScale.y);        
        timeLeft = time;
        while (timeLeft > 0) {
            float progress = (timeLeft / time);
            if (open) progress = 1 - progress;
            float widthProgress = growCurve.Evaluate(progress);
            timeLeft -= Time.deltaTime;
            transform.GetChild(0).localScale = new Vector2(widthProgress * width, transform.GetChild(0).localScale.y);

            yield return new WaitForEndOfFrame();
        }

        transform.GetChild(0).localScale = new Vector2(open ? width : 0, transform.GetChild(0).localScale.y); 
        moving = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
