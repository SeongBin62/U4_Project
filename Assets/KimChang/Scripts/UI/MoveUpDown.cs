using UnityEngine;
using DG.Tweening;

public class MoveUpDown : MonoBehaviour
{
    private float moveDistance = 0.2f;
    private float moveDuration = 0.5f;

    private Tween moveTween;
    private Vector3 startPos;

    private void OnEnable()
    {
        startPos = transform.position;

        moveTween?.Kill();

        moveTween = transform.DOMoveY(startPos.y + moveDistance, moveDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void OnDisable()
    {
        moveTween?.Kill();
    }
}
