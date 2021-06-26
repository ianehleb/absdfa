using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmBot : MonoBehaviour
{
    private BoxCollider2D col;

    public float speed = 1f;
    public LayerMask obstacleMask;
    public LayerMask playerMask;
    public List<AlarmBot> senders;

    bool alarm = false;

    public event EventHandler botEvent;
    public event EventHandler alarmEvent;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();

        foreach (AlarmBot sender in senders)
        {
            sender.botEvent += Flip;
            sender.alarmEvent += Alarm;
        }
    }

    private void FixedUpdate() => Move();
    private void Move()
    {
        transform.Translate(Vector2.right * speed * Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        CheckFlip();
        CheckAlarm();
    }       
    private void CheckFlip()
    {
        var rayOrigin = new Vector2(col.bounds.extents.x, 0);
        rayOrigin = transform.TransformPoint(rayOrigin);

        RaycastHit2D ray = Physics2D.Raycast(rayOrigin, transform.right, speed * Time.fixedDeltaTime, obstacleMask);
        if (ray.collider)
        {
            FlipEvent();
            //return;
        }
        else
        {
            ray = Physics2D.Raycast(rayOrigin, -transform.up, col.bounds.extents.y + 0.2f, obstacleMask);
            if (!ray.collider)
            {
                FlipEvent();
                //return;
            }
        }
    }
    private void CheckAlarm()
    {
        if (alarm)
            return;
        RaycastHit2D ray = Physics2D.Raycast(transform.position, transform.right, 0.2f, playerMask);
        if (ray.collider)
            AlarmEvent();
    }

    private void FlipEvent()
    {
        Flip(this, null);

        if (botEvent != null)
            botEvent(this, null);
    }
    private void AlarmEvent()
    {
        Alarm(this, null);

        if (alarmEvent != null)
            alarmEvent(this, null);
    }

    private void Flip(object sender, EventArgs e)
    {
        transform.Rotate(new Vector2(0, 180));
    }
    private void Alarm(object sender, EventArgs e)
    {
        alarm = true;
        Debug.Log("Alarm");
    }
}
