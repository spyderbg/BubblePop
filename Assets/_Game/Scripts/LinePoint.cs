using UnityEngine;
using Utils;
using Utils.Extensions;
using Utils.Extensions;

public class LinePoint : MonoBehaviour
{
    public enum LinePointState
    {
        Enabled,
        Disabled
    }
    
    public float           Speed = 3;
    public Vector2         StartPoint;
    public Vector2         NextPoint;
    public GameObject      Light;

    private SpriteRenderer _spriteRenderer;
    private LinePointState _state;
    private Vector2        _statrtPoint;
    private Vector2        _nextPoint;

    public LinePointState State
    {
        get => _state;
        set {
            _state = value;

            var isEnabled = _state == LinePointState.Enabled;
            _spriteRenderer.enabled = isEnabled;
            Light.SetActive(isEnabled);
        }
    }
    
    public Color PointColor
    {
        get => _spriteRenderer.color;
        set => _spriteRenderer.color = value;
    }

    private void Awake()
    {
        _spriteRenderer = gameObject.GetComponentRequired<SpriteRenderer>();
    }
    
    private void Start()
    {
        transform.position = World.Instance.LaunchPosition;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, NextPoint, Speed * Time.deltaTime);
        if ((Vector2) transform.position == NextPoint)
            transform.position = StartPoint;
    }
    
}