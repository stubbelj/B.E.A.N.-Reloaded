using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrapple : MonoBehaviour
{
    [SerializeField] float hoverDist = 4, moveSpeed, autoBreakDist = 1.5f, timeSinceGrappleEnd;
    [SerializeField] GrapplePoint point;
    Vector3 currentTargetPoint;
    [SerializeField] bool movingToPoint, launchedFromGrapple;
    [SerializeField] KeyCode activateKey = KeyCode.E;

    PlayerCombat pCombat => GetComponent<PlayerCombat>();
    PlayerController pMove => GetComponent<PlayerController>();
    LineRenderer line => GetComponent<LineRenderer>();
    Rigidbody2D rb => GetComponent<Rigidbody2D>();

    public void EndGrappleLaunch()
    {
        launchedFromGrapple = false;
    }

    public void SetPoint(GrapplePoint point)
    {
        this.point = point;
    }

    public float GetHoverDist()
    {
        return hoverDist;
    }

    public bool LaunchFromGrapple()
    {
        return launchedFromGrapple;
    }

    private void Update()
    {
        if (Input.GetKeyDown(activateKey)) GrappleInteract(); 
        if (launchedFromGrapple && pMove.isOnGround && timeSinceGrappleEnd > 0.5f) launchedFromGrapple = false;

        if (!movingToPoint) {
            timeSinceGrappleEnd += Time.deltaTime;
            return;
        }
        DrawLine();
        

        MoveToPoint();
    }

    void DrawLine()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, currentTargetPoint);
    }

    void MoveToPoint()
    {
        var dist = Vector2.Distance(transform.position, currentTargetPoint);
        if (dist <= autoBreakDist) {
            EndGrapple();
            return;
        }

        var dir = currentTargetPoint - transform.position;
        rb.velocity = dir.normalized * moveSpeed;
    }

    void GrappleInteract()
    {
        if (movingToPoint) EndGrapple();
        else StartGrapple();
    }

    void EndGrapple()
    {
        movingToPoint = false;
        line.enabled = false;
        launchedFromGrapple = true;
        timeSinceGrappleEnd = 0;

        pMove.RefreshJump(1);
        ToggleMoveAndFight(true);
    }

    void StartGrapple()
    {
        if (point == null || !point.hovered || pMove.isSlamming()) return;

        currentTargetPoint = point.transform.position;
        movingToPoint = true;
        line.enabled = true;
        

        ToggleMoveAndFight(false);
    }

    void ToggleMoveAndFight(bool active)
    {
        pCombat.enabled = active;
        pMove.enabled = active;
    }
}
