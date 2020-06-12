using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityConstants;
using DarkTonic.PoolBoss;
using DG.Tweening;
using UnityEngine.UI;
using Utils;
using Utils.Extensions;

using Random = UnityEngine.Random;
using BallState = Ball.BallState;

public class World : MonoSingleton<World>
{
    public enum GameState
    {
        Playing,
        BallDrawNext,
        BallMoving,
        BallMerging,
        BallDestroying,
        BoardMovingUp,
        BoardMovingDown,
        Finished,
        Waiting
    }
    
    #region Public fields

    public float BoardLeft   = -3.5f;
    public float BoardTop    = 6f;
    public float BoardRight  = 3.5f;
    public float BoardBottom = -6f;
    public float CellHeight  = 1.1f;
    public float CellWidth   = 1.3f;
    public int Rows          = 10;
    public int Columns       = 6;
    public int MaxLinePoints = 3;

    public Sprite[]    Sprites;
    public GameObject  BallLaunchPoint;
    public GameObject  BallNext;
    public GameObject  TempBall;
    public DrawLine    Line;
    public GameObject  EndGameText;
    
    public GameState  State;

    [HideInInspector] public Executor Executor;
    #endregion

    #region Private fields

    private Camera       _camera;
    private SoundManager _soundManager;

    private Ball[,] _board;
    private Ball    _tempBall;
    private Ball    _launchBall;
    private Ball    _nextBall;

    private bool _isEvenLeft;
    private bool _wasLineVisible;

    private Vector2Int[] leftNeighbors = {
        new Vector2Int(-1, 0),
        new Vector2Int(-1, -1),
        new Vector2Int(0, -1),
        // new Vector2Int(1, -1), 
        new Vector2Int(1, 0),
        // new Vector2Int(1, 1), 
        new Vector2Int(0, 1),
        new Vector2Int(-1, 1)
    };
    
    private Vector2Int[] rightNeighbors = {
        new Vector2Int(-1, 0), 
        // new Vector2Int(-1, -1), 
        new Vector2Int(0, -1), 
        new Vector2Int(1, -1), 
        new Vector2Int(1, 0), 
        new Vector2Int(1, 1), 
        new Vector2Int(0, 1), 
        // new Vector2Int(-1, 1) 
    };
    
    #endregion
    
    #region MonoBehaviour methods

    protected void Awake()
    {
        base.Awake();
        
        Executor = new Executor(this);
    }
    
    private void Start()
    {
        Random.InitState(1);
        
        LoadLevel(Levels.Level_01);
        
        _tempBall = TempBall.GetComponentRequired<Ball>();
        _tempBall.SetState(BallState.None);
        
        _launchBall = BallLaunchPoint.GetComponentRequired<Ball>();
        _launchBall.SetState(BallState.None);
        
        _nextBall = BallNext.GetComponentRequired<Ball>();
        _nextBall.SetState(BallState.Launch);
        _nextBall.SetValue(RandomBallValue());

        State = GameState.BallDrawNext;
    }

    private void OnDestroy()
    {
    }

    private void Update()
    {
        switch (State)
        {
            case GameState.Playing:
                if (_wasLineVisible && !Line.IsVisible)
                    State = GameState.BallMoving;
                _wasLineVisible = Line.IsVisible;
                break;
            
            case GameState.BallDrawNext:
                if (IsGameFinished)
                {
                    State = GameState.Finished;
                }
                else
                {
                    State = GameState.Waiting;
                    StartCoroutine(DrawNextBall());
                }
                break;
            
            case GameState.BallMoving:
                SoundManager.Instance.PlayLaunch();
                State = GameState.Waiting;
                StartCoroutine(MoveBall());
                break;
            
            case GameState.BallMerging:
                State = GameState.Waiting;
                StartCoroutine(MergeBalls(Line.Placeholder.GetComponent<Ball>()));
                break;
            
            case GameState.BallDestroying:
                State = GameState.Waiting;
                StartCoroutine(DestroyBalls());
                break;
            
            case GameState.BoardMovingUp:
                State = GameState.Waiting;
                StartCoroutine(MoveBoardUp());
                break;
        
            case GameState.BoardMovingDown:
                State = GameState.Waiting;
                StartCoroutine(MoveBoardDown());
                break;
            
            case GameState.Finished:
                if (EndGameText.activeInHierarchy)
                {
                    var ac = EndGameText.GetComponent<Animation>();
                    if (!ac.isPlaying && Input.GetMouseButtonDown(0))
                        ac.Play("FadeOut");
                    
                }
                else
                {
                    EndGameText.SetActive(true);
                    
                    var ac = EndGameText.GetComponent<Animation>();
                    if (!ac.isPlaying)
                        EndGameText.GetComponent<Animation>().Play("FadeIn");
                }
                break;
            
            case GameState.Waiting:
                break;
        }
    }
    
    #endregion

    #region Public methods

    public Vector3 LaunchPosition => BallLaunchPoint.transform.position;

    public bool IsLineBlocked => State != GameState.Playing;

    public void RestartGame()
    {
        EndGameText.SetActive(false);
        LoadRandomLevel();
        State = GameState.BallDrawNext;
    }

    public void LoadRandomLevel()
    {
        var level = Random.Range(1, 6);

        Debug.Log($"Load level {level}");
        
        LoadLevel(Levels.GetLevel(level));
    }
    
    public void LoadLevel(int[,] level)
    {
        if(level == null) return;
        
        ResetBoard(true);
        for (var y = 0; y < level.GetLength(0); y++)
        {
            for (var x = 0; x < level.GetLength(1); x++)
            {
                if (level[y, x] == 0)
                {
                    _board[y, x].SetState(BallState.None);
                }
                else
                {
                    _board[y, x].SetState(BallState.Active);
                    _board[y, x].SetValue(level[y, x]);
                }
            }
        }
    }
    
    public int TraceRay(Vector3 origin, Vector3 direction, ref Vector3[] points, ref GameObject[] gameObjects)
    {
        var num = 0;
        const int layerMask = Layers.BorderMask | Layers.TopBorderMask | Layers.BallMask;
        const int layerMaskBalls = Layers.BallMask | Layers.LinkedBallMask;

        Debug.DrawLine(origin, origin + direction, Color.red);
        var hit = Physics2D.Raycast(origin, direction, 100f, layerMask);
        if (hit.collider == null) 
            return 0;
        
        points[0] = origin;
        points[1] = hit.point;
        points[2] = hit.point;

        var balls = Physics2D.LinecastAll(points[0], points[1], layerMaskBalls);
        for (var i = 0; i < balls.Length; i++)
            gameObjects[num++] = balls[i].collider.gameObject.transform.parent.gameObject;

        if (hit.collider.gameObject.layer == Layers.Border)
        {
            // TODO: add extension method 'reflect'
            if (hit.collider.gameObject.layer == Layers.Border)
                direction.x *= -1f;
            else
                direction.y *= -1f;
                
            Debug.DrawLine(points[1], points[1] + direction, Color.red);
            var hits = Physics2D.RaycastAll(points[1], direction, 100f, layerMask);
            if (!hits.Any()) 
                return num;
            
            foreach (var item in hits)
            {
                if (points[2] == item.point.WithZ(0f))
                    continue;
                    
                points[2] = item.point;
                
                balls = Physics2D.LinecastAll(points[1], points[2], layerMaskBalls);
                for (var i = 0; i < balls.Length; i++)
                    gameObjects[num++] = balls[i].collider.gameObject.transform.parent.gameObject;
                    
                break;
            }
        }

        return num;
    }

    public RaycastHit2D[] TraceRaySimple(Vector3 direction, ref Vector3[] points)
    {
        var hits = new RaycastHit2D[MaxLinePoints];

        points[0] = LaunchPosition;
        points[1] = new Vector3(2.5f, -0.5f, 0f);
        points[1] = new Vector3(-2.6f, 5f, 0f);

        return hits;
    }
    
    #endregion

    #region Private methods

    private bool IsGameFinished =>
        _board.Cast<Ball>().Any(ball => ball.GetValue() == 512);
    
    private IEnumerable<Vector2Int> GetNeighbors(int y) 
        => y.IsEven() ^ _isEvenLeft ? rightNeighbors : leftNeighbors;
    
    private bool IsInBoard(Vector2 pos) =>
        0 <= pos.y && pos.y < Rows &&
        0 <= pos.x && pos.x < Columns;
    
    private int TopLeftValue(int y, int x)
    {
        if (y < 0 || Rows <= y) throw new ArgumentException(nameof(y));
        if (x < 0 || Columns <= x) throw new ArgumentException(nameof(x));
        
        return x == 0 || y == 0 ? 0 : _board[y - 1, x - 1].IsActiveInt();
    }

    private int TopValue(int y, int x)
    {
        if (y < 0 || Rows <= y) throw new ArgumentException(nameof(y));
        if (x < 0 || Columns <= x) throw new ArgumentException(nameof(x));
        
        return y == 0 ? 0 : _board[y - 1, x].IsActiveInt();
    }

    private int TopRightValue(int y, int x)
    {
        if (y < 0 || Rows <= y) throw new ArgumentException(nameof(y));
        if (x < 0 || Columns <= x) throw new ArgumentException(nameof(x));
        
        return x == Columns - 1 || y == 0 ? 0 : _board[y - 1, x + 1].IsActiveInt();
    }
    
    private int LeftValue(int y, int x)
    {
        if (y < 0 || Rows <= y) throw new ArgumentException(nameof(y));
        if (x < 0 || Columns <= x) throw new ArgumentException(nameof(x));
        
        return x == 0 ? 0 : _board[y, x - 1].IsActiveInt();
    }
    
    private int RightValue(int y, int x)
    {
        if (y < 0 || Rows <= y) throw new ArgumentException(nameof(y));
        if (x < 0 || Columns <= x) throw new ArgumentException(nameof(x));
        
        return x == Columns - 1 ? 0 : _board[y, x + 1].IsActiveInt();
    }
    
    private bool IsCellEmpty(int y, int x)
    {
        return _board[y, x].GetState() != BallState.Active;
    }
    
    private bool IsBallLinked(int y, int x)
    {
        if (y < 0 || Rows <= y) throw new ArgumentException(nameof(y));
        if (x < 0 || Columns <= x) throw new ArgumentException(nameof(x));
        
        var tl = TopLeftValue(y, x);
        var t = TopValue(y, x);
        var tr = TopRightValue(y, x);
        var l = LeftValue(y, x);
        var r = RightValue(y, x);

        return y.IsEven() && _isEvenLeft
            ? (l + r + t + tl) > 0
            : (l + r + t + tr) > 0;
    }
    
    private void ResetBoard(bool complete = false)
    {
        _isEvenLeft = true;
        if(_board == null)
            _board = new Ball[Rows, Columns];

        PoolBoss.DespawnAllPrefabs();
        
        for (var y = 0; y < Rows; y++)
        {
            var offsetX = y.IsEven() && _isEvenLeft ? 0f : CellWidth / 2; 
            for(var x = 0; x < Columns; x++)
            {
                var pos = new Vector3(BoardLeft + x * CellWidth + offsetX, BoardTop + CellHeight - y * CellHeight, 0f);
                var ball = PoolBoss.SpawnInPool("ball", pos, Quaternion.identity).GetComponent<Ball>();
                ball.BoardX = x;
                ball.BoardY = y;
                ball.SetValue(RandomBallValue());
                ball.SetState(y > 4 || complete ? BallState.None : BallState.Active);
                
                _board[y, x] = ball;
            }
        }
    }

    private void SetCollisionBalls()
    {
        for (var y = 0; y < Rows; y++)
        {
            for (var x = 0; x < Columns; x++)
            {
                if(!IsCellEmpty(y, x)) continue;
                
                var state = IsBallLinked(y, x) ? BallState.Linked : BallState.None;
                _board[y, x].SetState(state);
            }
        }
    }

    private void UpdateLinkedBalls()
    {
        // not fully accurate
        for (var y = 0; y < Rows; y++)
        {
            for (var x = 0; x < Columns; x++)
            {
                _board[y, x].IsLinked =
                    y == 0 ||
                    _board[y - 1, x].IsLinked ||
                    ((y.IsEven() && _isEvenLeft) && (x < Columns - 1) && _board[y - 1, x + 1].IsLinked) ||
                    (!(y.IsEven() && _isEvenLeft) && x > 0 && _board[y - 1, x - 1].IsLinked);
            }
        }
    }

    private void UpdateLinkedBalls2()
    {
        // recursive more accurate
        void LinkBall(int yy, int xx)
        {
            _board[yy, xx].IsLinked = true;

            foreach (var n in GetNeighbors(yy))
            {
                var x = xx + n.x;
                var y = yy + n.y;
                
                if ( 0 <= y && y < Rows && 0 <= x && x < Columns && _board[y, x].IsActive() && !_board[y, x].IsLinked)
                    LinkBall(y, x);
            }
        }

        foreach (var ball in _board)
            ball.IsLinked = false;

        LinkBall(0, 0);
    }

    private bool IsMoveUpAvailable()
    {
        return true;
    }
    
    private bool IsMoveDownAvailable()
    {
        for (var i = 0; i < Columns; i++)
            if (_board[Rows - 1, i].IsActive())
                return false;
        return true;
    }
    
    ///////////////////////////////////////////////////////////

    private int RandomBallValue()
    {
        return ((int)Math.Pow(2.0, Random.Range(1, 6)));
    }
    
    private IEnumerator DrawNextBall()
    {
        yield return new WaitForEndOfFrame();
        
        _tempBall.CopyFrom(_nextBall);
        _tempBall.SetState(BallState.Launch);

        _launchBall.SetState(BallState.None);
        _nextBall.SetValue(RandomBallValue());
        _nextBall.SetState(BallState.Launch);
        
        const float duration = 0.3f;
        var tween = DOTween.Sequence()
            .Append(_tempBall.transform.DOMove(_launchBall.transform.position, duration))
            .Insert(0f, _tempBall.transform.DORotate(_launchBall.transform.localRotation.eulerAngles, duration))
            .Insert(0f, _tempBall.transform.DOScale(_launchBall.transform.localScale, duration));
        yield return tween.WaitForCompletion();
        
        _launchBall.SetValue(_tempBall.GetValue());
        _launchBall.SetState(BallState.Launch);
        _tempBall.SetState(BallState.None);

        State = GameState.Playing;
    }
    
    private IEnumerator MoveBall()
    {
        yield return new WaitForEndOfFrame();

        var finalPos = Line.Placeholder.transform.position;
        
        _tempBall.CopyFrom(_launchBall);
        _tempBall.SetState(BallState.Active);
        _launchBall.SetState(BallState.None);

        Tween tween;
        if (Line.HasBoardHit)
        {
            tween = _tempBall.transform.DOMove(Line.Waypoints[1], 0.2f);
            yield return tween.WaitForCompletion();
        }
            
        tween = _tempBall.transform.DOMove(finalPos, 0.2f);
        yield return tween.WaitForCompletion();

        var ballPlaceholder = Line.Placeholder.GetComponentRequired<Ball>();
        ballPlaceholder.CopyFrom(_tempBall);
        _tempBall.SetState(BallState.None);

        // move neighbors
        var boardPos = new Vector2Int(ballPlaceholder.BoardX, ballPlaceholder.BoardY);
        var nTrasnform = ballPlaceholder.transform;
        
        foreach (var n in GetNeighbors(ballPlaceholder.BoardY))
        {
            var pos = boardPos + n;
            if(!IsInBoard(pos) || !_board[pos.y, pos.x].IsActive()) 
                continue;
                
            var t = _board[pos.y, pos.x].transform;
            var dir = (t.position - nTrasnform.position).normalized;

            t.DOMove(t.position + dir * 0.1f, 0.05f);
        }
        yield return new WaitForSeconds(0.1f);
        
        foreach (var n in GetNeighbors(ballPlaceholder.BoardY))
        {
            var pos = boardPos + n;
            if(!IsInBoard(pos) || !_board[pos.y, pos.x].IsActive()) 
                continue;
                
            var t = _board[pos.y, pos.x].transform;
            var dir = (t.position - nTrasnform.position).normalized;

            t.DOMove(t.position - dir * 0.1f, 0.05f);
        }
        yield return new WaitForSeconds(0.1f);
        
        State = GameState.BallMerging;
    }

    private Ball FindForMerge(Ball ball)
    {
        var boardPos = new Vector2Int(ball.BoardX, ball.BoardY);
        var value = ball.GetValue();

        foreach (var t in GetNeighbors(boardPos.y))
        {
            var pos = boardPos + t;
            if(!IsInBoard(pos) || !_board[pos.y, pos.x].IsActive()) 
                continue;

            if (value == _board[pos.y, pos.x].GetValue())
                return _board[pos.y, pos.x];
        }

        return null;
    }

    private IEnumerator MergeBalls(Ball placeholder)
    {
        var srcBall = placeholder;
        var dstBall = FindForMerge(srcBall);
        
        if(dstBall != null)
            SoundManager.Instance.PlayPop();
            
        while(dstBall != null)
        {
            _tempBall.CopyFrom(srcBall);
            srcBall.SetState(BallState.None);
            srcBall.Explode();
            
            var tween = _tempBall.transform.DOMove(dstBall.transform.position, 0.1f);
            yield return tween.WaitForCompletion(); 
            
            dstBall.SetValue(dstBall.GetValue() + _tempBall.GetValue());
            _tempBall.SetState(BallState.None);

            srcBall = dstBall;
            dstBall = FindForMerge(srcBall);
        } 
        
        State = GameState.BallDestroying;
        // State = GameState.BallDrawNext;
    }

    private IEnumerator DestroyBalls()
    {
        yield return new WaitForEndOfFrame();

        var nextState = GameState.BallDrawNext;

        UpdateLinkedBalls2();
        
        foreach (var ball in _board)
        {
            if (!ball.IsActive() || ball.IsLinked) 
                continue;
            
            nextState = GameState.BoardMovingDown;
                
            ball.SetState(BallState.None);
                
            var ballFall = PoolBoss.SpawnInPool("ball", ball.transform.position, Quaternion.identity).GetComponent<Ball>();
            ballFall.CopyFrom(ball);
            ballFall.SetState(BallState.Fall);
        }
        
        if(nextState == GameState.BoardMovingDown)
            SoundManager.Instance.PlayFall();

        State = nextState;
        // State = GameState.BallDrawNext;
    }

    private IEnumerator MoveBoardUp()
    {
        yield return new WaitForEndOfFrame();

        if (IsMoveUpAvailable())
        {
            
        }

        State = GameState.BallDrawNext;
    }
    
    private IEnumerator MoveBoardDown()
    {
        yield return new WaitForEndOfFrame();

        if (IsMoveDownAvailable())
        {
            var tempBalls = new Ball[Columns];
            for (var x = 0; x < Columns; x++)
                tempBalls[x] = _board[Rows - 1, x];
            
            // move down all balls
            for (var y = Rows - 2; y >= 0; y--)
            {
                for (var x = 0; x < Columns; x++)
                {
                    var ball = _board[y, x];
                        
                    var pos = ball.transform.position;
                    pos.y -= CellHeight;
                    ball.transform.DOMove(pos, 0.2f);
                    ball.BoardY += 1;

                    _board[ball.BoardY, ball.BoardX] = ball;
                }
            }

            yield return new WaitForSeconds(0.2f);
            
            // fill line '0' with random balls
            _isEvenLeft = !_isEvenLeft;
            var offsetX = _isEvenLeft ? 0f: CellWidth / 2f; 
            
            for (var x = 0; x < Columns; x++)
            {
                var ball = tempBalls[x];
                ball.IsLinked = true;
                ball.BoardX = x;
                ball.BoardY = 0;
                ball.SetState(BallState.Active);
                ball.SetValue(RandomBallValue());
                ball.transform.position = new Vector3(BoardLeft + x * CellWidth + offsetX, BoardTop + CellHeight, 0f);

                _board[0, x] = ball;
            }
        }
        
        State = GameState.BallDrawNext;
    }

    #endregion
}
