using System;
using UnityEngine;
using Utils.Extensions;

namespace Utils {
    
public class ScreenFitter : MonoBehaviour
{
    public enum AspectModeEnum
    {
        None = 0,
        WidthControlsHeight = 1,
        HeightControlsWidth = 2
    }

    public enum AnchorPointEnum
    {
        None = 0,
        Left,
        Top,
        Right,
        Bottom,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public Camera Camera; 
    
    [SerializeField] private AspectModeEnum _aspectMode = AspectModeEnum.None;
    
    [SerializeField] private AnchorPointEnum _anchorPoint = AnchorPointEnum.None;
    private AnchorPointEnum _prevAnchorPoint = AnchorPointEnum.None;

    public AspectModeEnum AspectMode {
        get => _aspectMode;
        set {
            if (_aspectMode == value) return;
            _aspectMode = value;
            SetDirty();
        }
    }

    private float _aspectRatio = 1f;
    
    [SerializeField]
    public float AspectRation
    {
        get => _aspectRatio;
        set {
            if (Math.Abs(_aspectRatio - value) > 0.0001f) return;
            _aspectRatio = value;
            SetDirty();
        }
    }

    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer SpriteRenderer {
        get {
            if (_spriteRenderer == null);
            _spriteRenderer = gameObject.GetComponentRequired<SpriteRenderer>();
            return _spriteRenderer;
        }
    }

    private void Start()
    {
        _spriteRenderer = gameObject.GetComponentRequired<SpriteRenderer>();
    }

    public virtual bool IsActive()
    {
        return isActiveAndEnabled;
    }
    
    protected void OnEnable()
    {
        if(Camera == null) Camera = Camera.main;

        _prevAnchorPoint = _anchorPoint;
    
        SetDirty();
    }

    public void SetDirty()
    {
        AdjustAspectRatio();
        UpdateSize();
        UpdatePosition();
    }

    private void AdjustAspectRatio()
    {
        var screenHeight = Camera.orthographicSize * 2f;
        var screenWidth = screenHeight * Screen.width / Screen.height;
        _aspectRatio = screenWidth / screenHeight;
    }
    
    private void UpdateSize()
    {
        if (!IsActive() || !Camera.orthographic) return;

        var screenHeight = Camera.orthographicSize * 2f;
        var screenWidth = screenHeight * _aspectRatio;

        var localScale = transform.localScale;
        var bounds = SpriteRenderer.bounds;
        var width = bounds.size.x;
        var height = bounds.size.y;
        var scaleFactor = 1f;
        
        if (_aspectMode == AspectModeEnum.WidthControlsHeight)
            scaleFactor = screenWidth / width;
        else if (_aspectMode == AspectModeEnum.HeightControlsWidth)
            scaleFactor = screenHeight / height;

        transform.localScale = localScale * scaleFactor;
    }

    private void UpdatePosition()
    {
        if (!IsActive() || !Camera.orthographic || _anchorPoint == AnchorPointEnum.None) return;
        
        var screenHeight = Camera.orthographicSize * 2f;
        var screenWidth = screenHeight * _aspectRatio;
        
        var localScale = transform.localScale;
        var bounds = SpriteRenderer.bounds;
        var width = bounds.size.x;
        var height = bounds.size.y;

        var pos = gameObject.transform.position;
        
        if (_anchorPoint == AnchorPointEnum.Bottom)
        {
            var bottom = Camera.ScreenToWorldPoint(new Vector3(0f, 0f, 0f)).y;
            pos.y = bottom + height / 2f;
        }
        else if(_anchorPoint == AnchorPointEnum.Top)
        {
            var top = Camera.ScreenToWorldPoint(new Vector3(0f, Screen.height, 0f)).y;
            pos.y = top - height / 2f;
        }
        // TODO: implement other anchor types
        else
        {
            // ...
        }
        gameObject.transform.position = pos;
    }


    void Update()
    {
        if (_prevAnchorPoint != _anchorPoint)
            SetDirty();

        _prevAnchorPoint = _anchorPoint;
        
        if (Input.GetKey(KeyCode.F))
        {
            UpdateSize();
        }
    }
}

} // Utils
