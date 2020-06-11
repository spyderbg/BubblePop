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
    private bool         _isMouseDown = false;
    
    private GameObject[,] _pointers;
    private GameObject[,] _pointers2;
    
   /////////////////////////////////////////////////////// 
    
    Color col;
    public GameObject pointer;

    Vector3 lastMousePos;
    private bool startAnim;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        Objects = new GameObject[20];
        Waypoints = new Vector3[World.Instance.MaxLinePoints];
        _pointers = new GameObject[World.Instance.MaxLinePoints,15];
        _pointers2 = new GameObject[World.Instance.MaxLinePoints,25];

        World.Instance.TraceRaySimple(Vector3.zero, ref Waypoints);
        
        for (var i = 0; i < World.Instance.MaxLinePoints; i++)
        {
            GeneratePoints(i);
            GeneratePositionsPoints(Waypoints, i);
            // HidePoints(i);
        }
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
            DrawSimple(direction);
    }

    #endregion

    #region Public methods

    #endregion
    
    #region Private methods
    
    private void GetReversSquare(Vector2 firstPos, Vector2 endPos, int num)
    {
        Debug.DrawLine (firstPos, endPos);
        
        var hit = Physics2D.LinecastAll(firstPos, endPos, 1 << 10);
        foreach (var item in hit)
        {
            // if (item.collider.gameObject.GetComponent<Square>().Busy != null) continue;
            
            // mainscript.Instance.reverseMesh[num] = item.collider.GetComponent<Square>();
                
            Debug.DrawLine (firstPos, item.collider.transform.position, Color.green);
                
            return;
        }

        // mainscript.Instance.reverseMesh[num] = null;
    }
    
    private void GeneratePoints(int num = 0)
    {
        for (var i = 0; i < _pointers.GetLength(1); i++)
        {
            _pointers[num, i] = Instantiate(pointer, transform.position, transform.rotation);
            _pointers[num, i].transform.parent = transform;
            _pointers[num, i].GetComponent<LinePoint>().Light.SetActive(false);
            _pointers[num, i].GetComponent<LinePoint>().SetDraw(this);
        }

        for (var i = 0; i < _pointers2.GetLength(1); i++)
        {
            _pointers2[num, i] = Instantiate(pointer, transform.position, transform.rotation);
            _pointers2[num, i].transform.parent = transform;
            _pointers2[num, i].GetComponent<LinePoint>().Light.SetActive(false);
            _pointers2[num, i].GetComponent<LinePoint>().SetDraw(this);
        }
    }

    private void GeneratePositionsPoints(Vector3[] waypoints, int num = 0)
    {
        // if (mainscript.Instance.boxCatapult.GetComponent<Square>().Busy != null)
        {
            // col = mainscript.Instance.boxCatapult.GetComponent<Square>().Busy.GetComponent<SpriteRenderer>().sprite
                // .texture.GetPixelBilinear(0.6f, 0.6f);
            col.a = 1;
        }

        HidePoints(num);

        var ab = (waypoints[1] - waypoints[0]).normalized;
        for (var i = 0; i < _pointers.GetLength(1); i++)
        {
            var step = i / 1.5f;
            var newPos = waypoints[0] + (step * ab);
            if (step >= (waypoints[1] - waypoints[0]).magnitude)
            {
                newPos = waypoints[1];
            }

            _pointers[num, i].transform.position = newPos;
            _pointers[num, i].GetComponent<SpriteRenderer>().enabled = true;
            _pointers[num, i].GetComponent<SpriteRenderer>().color = col;
            _pointers[num, i].GetComponent<LinePoint>().Light.SetActive(true);
            _pointers[num, i].GetComponent<LinePoint>().StartPoint = _pointers[num, i].transform.position;
            _pointers[num, i].GetComponent<LinePoint>().NextPoint = _pointers[num, i].transform.position;
            if (i > 0)
                _pointers[num, i - 1].GetComponent<LinePoint>().NextPoint = _pointers[num, i].transform.position;
        }

        ab = (waypoints[2] - waypoints[1]).normalized;
        for (var i = 0; i < World.Instance.MaxLinePoints; i++)
        {
            var step = i / 2f;

            if (step < (waypoints[2] - waypoints[1]).magnitude)
            {
                _pointers2[num, i].transform.position = waypoints[1] + (step * ab);
                _pointers2[num, i].GetComponent<SpriteRenderer>().enabled = true;
                _pointers2[num, i].GetComponent<SpriteRenderer>().color = col;
                _pointers2[num, i].GetComponent<LinePoint>().Light.SetActive(true);
                _pointers2[num, i].GetComponent<LinePoint>().StartPoint = _pointers2[num, i].transform.position;
                _pointers2[num, i].GetComponent<LinePoint>().NextPoint = _pointers2[num, i].transform.position;
                if (i > 0)
                    _pointers2[num, i - 1].GetComponent<LinePoint>().NextPoint = _pointers2[num, i].transform.position;
            }
        }
    }

    private void HidePoints(int num = 0)
    {
        for (var i = 0; i < _pointers.GetLength(1); i++)
        {
            _pointers[num, i].GetComponent<SpriteRenderer>().enabled = false;
            _pointers[num, i].GetComponent<LinePoint>().Light.SetActive(false);
        }

        for (var i = 0; i < _pointers2.GetLength(1); i++)
        {
            _pointers2[num, i].GetComponent<SpriteRenderer>().enabled = false;
            _pointers2[num, i].GetComponent<LinePoint>().Light.SetActive(false);
        }
    }

    private void HideAllPoints()
    {
        IsVisible = false;
        _lineRenderer.enabled = false;

        // for (var i = 0; i < World.Instance.MaxLinePoints; i++)
            // HidePoints(i);
    }

    private void EnableBoostLight()
    {
        for (var i = 0; i < World.Instance.MaxLinePoints; i++)
        {
            for (var j = 0; j < _pointers.GetLength(1); j++)
            {
                _pointers[i, j].GetComponent<LinePoint>().Light.SetActive(true);
            }

            for (var j = 0; j < _pointers2.GetLength(1); j++)
            {
                _pointers2[i, j].GetComponent<LinePoint>().Light.SetActive(true);
            }
        }
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
        Debug.DrawLine(World.Instance.BallLaunchPoint.transform.position, direction, Color.green);
        
        direction.z = 0;
        
        startAnim = lastMousePos == direction;
        lastMousePos = direction;

        World.Instance.TraceRaySimple(direction, ref Waypoints);
        
        // if (!startAnim)
            // GeneratePositionsPoints(Waypoints, 0);
    }

    #endregion
}