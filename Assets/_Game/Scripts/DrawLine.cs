using DarkTonic.PoolBoss;
using UnityConstants;
using UnityEngine;
using Utils.Extensions;

using BallState = Ball.BallState;

public class DrawLine : MonoBehaviour
{
    public float LowerAngle = -2.5f;

    public int          ObjectsNum;
    public GameObject[] Objects;
    public GameObject   Placeholder;
    public Vector3[]    Waypoints;
    public bool         IsVisible;

    public bool HasBoardHit => Waypoints[2] != Waypoints[1];

    [HideInInspector] private Vector3 HitPoint => Waypoints[2];
    [HideInInspector] private Vector3 LastPoint => Waypoints[2] != Waypoints[1] ? Waypoints[2] : Waypoints[2];
    
    private LineRenderer _lineRenderer;
    private bool         _isMouseDown;
    private Vector3      _lastDirection;
    
    private LinePoint[]  _pointers;
    private LinePoint[]  _pointers2;
    
   /////////////////////////////////////////////////////// 
    
    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        Objects = new GameObject[20];
        Waypoints = new Vector3[World.Instance.MaxLinePoints];
        _pointers = new LinePoint[15];
        _pointers2 = new LinePoint[25];

        World.Instance.TraceRaySimple(World.Instance.LaunchPosition, Vector3.zero, ref Waypoints);
        
        GeneratePoints();
        UpdatePoints();
        HidePoints();
    }

    #region MonoBahaviour methods

    private void Update()
    {
        if (World.Instance.IsLineBlocked)
            return;
        
        // update mouse left button state
        if (Input.GetMouseButtonDown(0))
        {
            _isMouseDown = true;

            ObjectsNum = 0;
            Placeholder = null;
            for (var i = 0; i < Waypoints.Length; i++)
                Waypoints[i] = World.Instance.LaunchPosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isMouseDown = false;
        }

        // draw line
        var direction = Vector3.zero;
        
        if (_isMouseDown)
        {
            var mousePoistion = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (mousePoistion.y < LowerAngle)
                direction = Vector3.zero;
            else
                direction = (mousePoistion.WithZ(0f) - World.Instance.LaunchPosition.WithZ(0f)).normalized;
        }
        
        if(direction == Vector3.zero)
            HideAllPoints();
        else
            DrawAnimated(direction);
            // DrawSimple(direction);
    }

    #endregion

    #region Public methods

    #endregion
    
    #region Private methods
    
    private void GeneratePoints()
    {
        var t = transform;
        
        for (var i = 0; i < _pointers.Length; i++)
        {
            var go = PoolBoss.Spawn("pointer", t.position, t.rotation, t).gameObject;
            _pointers[i] = go.GetComponentRequired<LinePoint>();
            _pointers[i].State = LinePoint.LinePointState.Disabled;
        }

        for (var i = 0; i < _pointers2.Length; i++)
        {
            var go = PoolBoss.Spawn("pointer", t.position, t.rotation, t).gameObject;
            _pointers2[i] = go.GetComponentRequired<LinePoint>();
            _pointers2[i].State = LinePoint.LinePointState.Disabled;
        }
    }

    private void UpdatePoints()
    {
        HidePoints();

        // update points to first hit
        var color = World.Instance.BallLaunchPoint.GetComponent<Ball>().GetColor();
        var dir = (Waypoints[1] - Waypoints[0]).normalized;
        var magnitude = (Waypoints[1] - Waypoints[0]).magnitude;
        
        for (var i = 0; i < _pointers.Length; i++)
        {
            var step = i / 1.5f;
            var pos = Waypoints[0] + step * dir;
            if (step >= magnitude)
                pos = Waypoints[1];

            _pointers[i].transform.position = pos;

            var linePoint = _pointers[i];
            linePoint.State = LinePoint.LinePointState.Enabled;
            linePoint.PointColor = color;
            linePoint.StartPoint = _pointers[i].transform.position;
            linePoint.NextPoint =  _pointers[i].transform.position;
            
            if (i > 0)
                _pointers[i - 1].NextPoint = _pointers[i].transform.position;
        }

        // update points on second line if exists
        if (!HasBoardHit) return;

        dir = (Waypoints[2] - Waypoints[1]).normalized;
        magnitude = (Waypoints[1] - Waypoints[0]).magnitude;
        
        for (var i = 0; i < 3; i++)
        {
            var step = i / 2f;
            if (!(step < magnitude)) 
                continue;
            
            var linePoint = _pointers2[i];
            linePoint.transform.position = Waypoints[1] + step * dir;
            linePoint.State = LinePoint.LinePointState.Enabled;
            linePoint.PointColor = color;
            linePoint.StartPoint = _pointers2[i].transform.position;
            linePoint.NextPoint = _pointers2[i].transform.position;
            
            if (i > 0)
                _pointers2[i - 1].NextPoint = _pointers2[i].transform.position;
        }
    }

    private void HidePoints()
    {
        for (var i = 0; i < _pointers.Length; i++)
        {
            _pointers[i].GetComponent<SpriteRenderer>().enabled = false;
            _pointers[i].GetComponent<LinePoint>().Light.SetActive(false);
        }

        for (var i = 0; i < _pointers2.Length; i++)
        {
            _pointers2[i].GetComponent<SpriteRenderer>().enabled = false;
            _pointers2[i].GetComponent<LinePoint>().Light.SetActive(false);
        }
    }

    private void HideAllPoints()
    {
        IsVisible = false;
        _lineRenderer.enabled = false;

        HidePoints();
    }

    private void EnableBoostLight()
    {
        for (var j = 0; j < _pointers.Length; j++)
            _pointers[j].GetComponent<LinePoint>().Light.SetActive(true);

        for (var j = 0; j < _pointers2.Length; j++)
            _pointers2[j].GetComponent<LinePoint>().Light.SetActive(true);
    }

    private void DrawSimple(Vector3 direction)
    {
        IsVisible = true;
        _lineRenderer.enabled = true;

        try {
            ObjectsNum = World.Instance.TraceRay(World.Instance.LaunchPosition, direction, ref Waypoints, ref Objects);
            for (var i = ObjectsNum - 1; i >= 0; i--)
            {
                var ball = Objects[i].GetComponent<Ball>();
                if (ball == null || ball.GetState() == BallState.Active) 
                    continue;

                if (Placeholder != null)
                    Placeholder.GetComponent<Ball>().SetState(BallState.None);
                    
                Placeholder = ball.gameObject;
                ball.SetState(BallState.Placeholder);
                break;
            }
        }
        catch {}

        _lineRenderer.SetPositions(Waypoints);
    }
    
    private void DrawAnimated(Vector3 direction)
    {
        IsVisible = true;
        
        try {
            ObjectsNum = World.Instance.TraceRay(World.Instance.LaunchPosition, direction, ref Waypoints, ref Objects);
            // ObjectsNum = World.Instance.TraceRaySimple(World.Instance.LaunchPosition, direction, ref Waypoints);
            for (var i = ObjectsNum - 1; i >= 0; i--)
            {
                var ball = Objects[i].GetComponent<Ball>();
                if (ball == null || ball.GetState() == BallState.Active) 
                    continue;

                if (Placeholder != null)
                    Placeholder.GetComponent<Ball>().SetState(BallState.None);
                    
                Placeholder = ball.gameObject;
                ball.SetState(BallState.Placeholder);
                break;
            }
        }
        catch {}
        
        if (_lastDirection != direction)
            UpdatePoints();

        _lastDirection = direction;
    }

    #endregion
}