using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameBox : MonoBehaviour
{
    private List<BoxEffect> _effects;
    private MeshRenderer _mr;
    private MaterialPropertyBlock _mat;
    private Vector2 _fallTarget;
    private Vector2 _fallOrigin;
    private bool _isAlive;
    private bool _isReadyToPerform;
    private bool _isPerforming;

    public BoxStateContainer State { get; private set; }
    public float PerformDelay { get; set; }
    public Vector2Int PositionOnGrid { get; set; }
    public bool CanBeInQueue { get; set; }

    public void Initialize()
    {
        _mat = new MaterialPropertyBlock();
        _mat.SetColor(Settings.GameBoxCol, Color.white);
        _mr = GetComponent<MeshRenderer>();
        _mr.SetPropertyBlock(_mat);
    }
    
    public void Born(Vector2Int posOnGrid)
    {
        _mat.SetColor(Settings.GameBoxCol, Color.white);
        _mr.SetPropertyBlock(_mat);
        
        State = new BoxStateContainer();
        State.BoxMeshRenderer = _mr;
        State.BoxMatBlock = _mat;
        
        _isAlive = true;
        CanBeInQueue = true;
        _isReadyToPerform = false;
        _isPerforming = false;
        
        gameObject.transform.localScale = Vector3.one;
        PositionOnGrid = posOnGrid;
        _fallTarget = transform.position;
        transform.localRotation = Quaternion.Euler(Vector3.zero);

        IsOnButton();
        Core.Coroutine.Start(Fall);
        Core.Coroutine.Start(Life);
    }

    private IEnumerator Life()
    {
        yield return new WaitUntil(() => Core.Observer.Get<Field>().queueInProgress);
        yield return new WaitUntil(IsOnButton);
        var lifeTimeOnButton = Settings.Get.lifeTimeOnBottom;
        while (_isAlive && lifeTimeOnButton > 0)
        { lifeTimeOnButton -= Time.deltaTime; yield return null; }
        yield return ShakeAnimation();
        if (_isPerforming || _isReadyToPerform) { yield break; }
        yield return DeathAnimation();
        Die();
    }

    private bool IsOnButton()
    {
        if (PositionOnGrid.y > 0) return false;
        CanBeInQueue = false;
        var currentColor = _mat.GetColor(Settings.GameBoxCol);
        var newCol = new Color(currentColor.r, currentColor.g, currentColor.b, 0.6f);
        ChangeColor(newCol, true);
        return true;
    }

    public void SetEffects(BoxEffect[] effects)
    { _effects = effects.ToList(); foreach (var effect in _effects) { effect.Init(this); } }

    public void GetReady()
    { ChangeColor(Color.yellow); _isReadyToPerform = true; }
    
    public IEnumerator Perform()
    {
        _isReadyToPerform = false;
        _isPerforming = true;
        ChangeColor(Color.red);
        yield return WaitDelay();
        yield return PerformEffects();
        yield return DeathAnimation();
        Die();
    }

    private IEnumerator WaitDelay()
    { var delay = PerformDelay; while (delay > 0) { delay -= Time.deltaTime; yield return null; } }

    private IEnumerator PerformEffects()
    {
        var prioritizedEffects = _effects.OrderByDescending(e => e.priority).ToList();
        foreach (var effect in prioritizedEffects)
        { yield return effect.Perform(); }
    }
    
    public void UpdateFallTarget(Vector2 worldTarget, Vector2 fallOrigin)
    { _fallTarget = worldTarget; _fallOrigin = fallOrigin; }

    private IEnumerator Fall()
    {
        var baseSpeed = Settings.Get.fallAnimationSpeed;
        var maxSpeed = Settings.Get.fallAnimationMaxSpeed;
        var minSpeed = Settings.Get.fallAnimationMinSpeed;
        while (_isAlive)
        {
            yield return new WaitUntil(IsTargetFar);
            var origin = new Vector2(transform.localPosition.x, _fallOrigin.y);
            var dist = Vector2.Distance(transform.position, origin) * 0.3f;
            while (IsTargetFar())
            {
                var speed = baseSpeed / dist;
                speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
                transform.localPosition += Vector3.down * speed * Time.deltaTime;
                if (transform.position.y < _fallTarget.y) break;
                yield return null;
            }
            transform.localPosition = _fallTarget;
            yield return null;
        }
    }

    private bool IsTargetFar() => Vector3.Distance(transform.position, _fallTarget) > 0.01f;

    private IEnumerator ShakeAnimation()
    {
        var baseRotation = Quaternion.Euler(Vector3.zero);
        const float amplitude = 20f;
        const float frequency = 18f;
        const float duration = 0.5f;
        var time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            var progress = time / duration;
            var damping = 1f - progress;
            var angle = Mathf.Sin(time * frequency) * amplitude * damping;
            transform.rotation = baseRotation * Quaternion.Euler(0,0,angle);
            yield return null;
        }

        transform.rotation = baseRotation;
    }

    private IEnumerator DeathAnimation()
    {
        CanBeInQueue = false;
        var speed = Settings.Get.deathAnimationSpeed;
        while (_isAlive && gameObject.transform.localScale.x > 0.1f)
        {
            var scale = Time.deltaTime * speed;
            gameObject.transform.localScale -= new Vector3(scale, scale, scale);
            yield return null;
        }
    }

    public void ChangeColor(Color color, bool setAlpha = false)
    {
        var alpha = setAlpha ? color.a : _mat.GetColor(Settings.GameBoxCol).a;
        var newColor = new Color(color.r, color.g, color.b, alpha);
        _mat.SetColor(Settings.GameBoxCol, newColor);
        _mr.SetPropertyBlock(_mat);
    }

    public void ForceDie()
    { Die(false); }
    
    private void Die(bool sendEvent = true)
    {
        Core.Coroutine.Stop(Life);
        Core.Coroutine.Stop(Fall);
        
        _effects = null;
        _isPerforming = false;
        _isReadyToPerform = false;
        _isAlive = false;
        
        _mat = new MaterialPropertyBlock();
        _mr.SetPropertyBlock(_mat);
        State.BoxMatBlock = _mat;
        
        if (!sendEvent) return;
        Core.Event.Invoke(new GameBoxDestroyedEvent {DestroyedGb = this});
    }
}

public class BoxStateContainer
{
    public MeshRenderer BoxMeshRenderer;
    public MaterialPropertyBlock BoxMatBlock;
    
    public int TextureSpotsUsed;
}