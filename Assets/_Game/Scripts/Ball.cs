using System;
using DarkTonic.PoolBoss;
using UnityEngine;
using TMPro;
using UnityConstants;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    public enum BallState
    {
        None = 0,
        Linked,
        Placeholder,
        Launch,
        Fall,
        Active
    }
    
    public enum BallType
    {
        Val2 = 1,
        Val4,
        Val8,
        Val16,
        Val32,
        Val64,
        Val128,
        Val256,
        Val512
    }

    public GameObject DebugElement;
    
    public TMP_Text NumberTxt;
    public TMP_Text NumberTxtBgr;
    
    public BallType Type;
    public int      BoardX;
    public int      BoardY;

    public GameObject StateNone;
    public GameObject StateLinked;
    public GameObject StatePlaceholder;
    public GameObject StateActive;
    public GameObject Trail;

    [SerializeField] 
    private BallState        _ballState;
    private SpriteRenderer   _spriteRenderer;
    private Rigidbody2D      _rigidbody2D;
    private bool             _isLinked; 

    public bool IsActive() => _ballState == BallState.Active;
    public int IsActiveInt() => IsActive() ? 1 : 0;

    public bool IsLinked {
        get => _isLinked;
        set {
            _isLinked = value;
            DebugElement.SetActive(_isLinked);
        }
    }
    
    public BallState GetState() => _ballState;
    
    public void SetState(BallState state)
    {
        StateNone.SetActive(false);
        StateLinked.SetActive(false);
        StatePlaceholder.SetActive(false);
        StateActive.SetActive(false);
        
        switch (state)
        {
            case BallState.None:
                StateNone.SetActive(true);
                break;
            case BallState.Linked:
                StateLinked.SetActive(true);
                break;
            case BallState.Placeholder:
                StatePlaceholder.SetActive(true);
                StatePlaceholder.GetComponent<SpriteRenderer>().color = 
                    World.Instance.BallLaunchPoint.GetComponent<Ball>().GetColor();
                break;
            case BallState.Launch:
                StateActive.SetActive(true);
                Trail.SetActive(false);
                break;
            case BallState.Fall:
                StateActive.SetActive(true);
                Trail.SetActive(false);
                _rigidbody2D.isKinematic = false;
                break;
            case BallState.Active:
                StateActive.SetActive(true);
                Trail.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        _ballState = state;
    }
    
    public int GetValue()
    {
        switch (Type)
        {
            case BallType.Val2:   return 2;
            case BallType.Val4:   return 4;
            case BallType.Val8:   return 8;
            case BallType.Val16:  return 16;
            case BallType.Val32:  return 32;
            case BallType.Val64:  return 64;
            case BallType.Val128: return 128;
            case BallType.Val256: return 256;
            case BallType.Val512: return 512;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetValue(int value)
    {
        switch (value)
        {
            case 2:   SetType(BallType.Val2);   break;
            case 4:   SetType(BallType.Val4);   break;
            case 8:   SetType(BallType.Val8);   break;
            case 16:  SetType(BallType.Val16);  break;
            case 32:  SetType(BallType.Val32);  break;
            case 64:  SetType(BallType.Val64);  break;
            case 128: SetType(BallType.Val128); break;
            case 256: SetType(BallType.Val256); break;
            case 512: SetType(BallType.Val512); break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public Color GetColor()
    {
        switch (Type)
        {
            case BallType.Val2:   return new Color(1f, 0.27f, 0.49f);
            case BallType.Val4:   return new Color(1f, 0.14f, 0.16f);
            case BallType.Val8:   return new Color(0.5f, 0.32f, 0.05f);
            case BallType.Val16:  return new Color(1f, 0.56f, 0.1f);
            case BallType.Val32:  return new Color(0.1f, 0.5f, 0.34f);
            case BallType.Val64:  return new Color(0.31f, 1f, 0.55f);
            case BallType.Val128: return new Color(0.13f, 0.75f, 1f);
            case BallType.Val256: return new Color(0.24f, 0.26f, 1f);
            case BallType.Val512: return new Color(1f, 0.91f, 0.14f);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void CopyFrom(Ball ball)
    {
        IsLinked = ball.IsLinked;
        SetState(ball.GetState());
        SetValue(ball.GetValue());
        gameObject.transform.position = ball.transform.position;
        gameObject.transform.localRotation = ball.transform.localRotation;
        gameObject.transform.localScale = ball.transform.localScale;
    }

    public void Explode()
    {
        var color = GetColor();
        
        var go = PoolBoss.SpawnInPool("BubbleExplosion", transform.position, Quaternion.identity);
        var m = go.GetComponent<ParticleSystem>().main;
        m.startColor = new ParticleSystem.MinMaxGradient(color);
        
        foreach (var ps in go.transform.GetComponentsInChildren<ParticleSystem>())
        {
            var main = ps.GetComponent<ParticleSystem>().main;
            main.startColor = new ParticleSystem.MinMaxGradient(color);
        }
    }

    private void Awake()
    {
        _spriteRenderer = StateActive.GetComponent<SpriteRenderer>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.isKinematic = true;
        IsLinked = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer != Layers.BottomBorder)
        {
            Debug.Log($"Ball collision {gameObject.layer} error {other.gameObject.layer}");
        }
        
        SetState(BallState.None);
        PoolBoss.Despawn(transform);
    }

    private void SetType(BallType type)
    {
        Type = type;

        try {
            var sprite = World.Instance.Sprites[GetSpriteIndex()];
            _spriteRenderer.sprite = sprite;
        }
        catch (IndexOutOfRangeException)
        {
            Debug.Log($"Error index out of range {GetSpriteIndex()}");
        }

        NumberTxt.text = GetValue().ToString();
        NumberTxtBgr.text = GetValue().ToString();
    }

    private int GetSpriteIndex()
    {
        return (int)Type - 1;
    }
}
