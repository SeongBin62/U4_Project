using UnityEngine;
using DG.Tweening;

public class DamageMove : MonoBehaviour
{
    private Tween moveTween;
    private Transform cachedTransform;

    private void Awake()
    {
        cachedTransform = transform;
    }

    public void Play()
    {
        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
            moveTween = null;
        }

        Vector3 startPos = cachedTransform.position;
        Vector3 targetPos = startPos + Vector3.up * 1f;

        moveTween = cachedTransform.DOMove(targetPos, 0.5f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => gameObject.SetActive(false));
    }

    private void OnDisable()
    {
        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
            moveTween = null;
        }
    }
}
