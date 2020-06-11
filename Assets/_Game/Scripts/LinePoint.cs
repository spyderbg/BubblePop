using UnityEngine;

public class LinePoint : MonoBehaviour
{
    public Vector2    StartPoint;
    public Vector2    NextPoint;
    public GameObject Light;
    
    private const float _speed = 5;

    DrawLine drawLine;

    private void Start()
    {
        transform.position = World.Instance.LaunchPosition;
    }

    public void SetDraw(DrawLine draw_)
    {
        drawLine = draw_;
    }

    private void Update()
    {
        // if (!drawLine._isVisible) return;
        if (StartPoint == NextPoint)
            GetComponent<SpriteRenderer>().enabled = false;

        transform.position = Vector3.MoveTowards(transform.position, NextPoint, _speed * Time.deltaTime);
        if ((Vector2) transform.position == NextPoint)
            transform.position = StartPoint;
    }
}