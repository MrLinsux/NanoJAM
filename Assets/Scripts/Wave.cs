using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    private MaterialPropertyBlock _propertyBlock;
    private float _elapsedTime;
    private SpriteRenderer _renderer;

    public float Duration;
    [Space]
    public AnimationCurve ScaleProgress;
    public Vector3 ScalePower;
    [Space]
    public AnimationCurve PropertyProgress;
    public float PropertyPower;
    [Space]
    public AnimationCurve AlphaProgress;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _propertyBlock = new MaterialPropertyBlock();
    }

    private void OnEnable()
    {
        _elapsedTime = 0f;
    }

    public IEnumerator ZaWarudo()
    {
        while (_elapsedTime < Duration)
        {
                var progress = _elapsedTime / Duration;

                var scale = ScaleProgress.Evaluate(progress) * ScalePower;
                var property = PropertyProgress.Evaluate(progress) * PropertyPower;
                var alpha = AlphaProgress.Evaluate(progress);

                transform.localScale = scale;
                //_renderer.material.SetFloat("_DisplacementPower", property);
                _renderer.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetFloat("_DisplacementPower", property / transform.localScale.x);
                _renderer.SetPropertyBlock(_propertyBlock);

                var color = _renderer.color;
                color.a = alpha;
                _renderer.color = color;

                _elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        while (_elapsedTime > 0)
        {
            var progress = _elapsedTime / Duration;

            var scale = ScaleProgress.Evaluate(progress) * ScalePower;
            var property = PropertyProgress.Evaluate(progress) * PropertyPower;
            var alpha = AlphaProgress.Evaluate(progress);

            transform.localScale = scale;
            //_renderer.material.SetFloat("_DisplacementPower", property);
            _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat("_DisplacementPower", property / transform.localScale.x);
            _renderer.SetPropertyBlock(_propertyBlock);

            var color = _renderer.color;
            color.a = alpha;
            _renderer.color = color;

            _elapsedTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _renderer.color = _renderer.color - new Color(0,0,0,1);
    }
}
