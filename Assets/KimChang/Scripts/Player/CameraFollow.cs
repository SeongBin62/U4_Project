using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    private float followSpeed = 2f;

    private float lookDuration = 0.5f;
    private float lookOffsetY = 4f;

    private float upPressedTime = 0f;
    private float downPressedTime = 0f;

    private bool isLookingUp = false;
    private bool isLookingDown = false;

    private Bounds mapBounds;
    private Camera cam;

    private void OnEnable()
    {
        if (cam == null)
            cam = GetComponent<Camera>() ?? Camera.main;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (mapBounds.size == Vector3.zero)
        {
            GameObject mapObject = GameObject.FindGameObjectWithTag("Map");
            if (mapObject != null)
            {
                Collider2D col = mapObject.GetComponent<Collider2D>();
                if (col != null)
                {
                    mapBounds = col.bounds;
                }
                else if (mapObject.TryGetComponent(out Renderer rend))
                {
                    mapBounds = rend.bounds;
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.Space))
        {
            ResetLookState();
        }

        if (Input.GetKey(KeyCode.W))
        {
            upPressedTime += Time.deltaTime;
            if (upPressedTime >= lookDuration)
                isLookingUp = true;
        }
        else
        {
            upPressedTime = 0f;
            isLookingUp = false;
        }

        if (Input.GetKey(KeyCode.S))
        {
            downPressedTime += Time.deltaTime;
            if (downPressedTime >= lookDuration)
                isLookingDown = true;
        }
        else
        {
            downPressedTime = 0f;
            isLookingDown = false;
        }
    }

    void LateUpdate()
    {
        if (player != null)
            FollowPlayer();
    }
    void FollowPlayer()
    {
        if (cam == null) cam = Camera.main;

        Vector3 targetPos = player.position;

        if (isLookingUp)
            targetPos.y += lookOffsetY;
        else if (isLookingDown)
            targetPos.y -= lookOffsetY;

        targetPos.z = transform.position.z;

        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        float minX = mapBounds.min.x + horzExtent;
        float maxX = mapBounds.max.x - horzExtent;
        float minY = mapBounds.min.y + vertExtent;
        float maxY = mapBounds.max.y - vertExtent;

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        // 중앙 고정: followSpeed가 0 이상이면 부드럽게 따라가고, 아니면 바로 이동
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
    }

    private void ResetLookState()
    {
        upPressedTime = 0f;
        downPressedTime = 0f;
        isLookingUp = false;
        isLookingDown = false;
    }
    public void SetSceneContext(Transform newPlayer)
    {
        player = newPlayer;
        UpdateMapBounds();

        Vector3 targetPos = player.position;

        targetPos.z = transform.position.z;

        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        float minX = mapBounds.min.x + horzExtent;
        float maxX = mapBounds.max.x - horzExtent;
        float minY = mapBounds.min.y + vertExtent;
        float maxY = mapBounds.max.y - vertExtent;

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        transform.position = targetPos;
    }


    public void UpdateMapBounds()
    {
        GameObject mapObject = GameObject.FindGameObjectWithTag("Map");
        if (mapObject != null)
        {
            Collider2D col = mapObject.GetComponent<Collider2D>();
            if (col != null)
            {
                mapBounds = col.bounds;
            }
            else if (mapObject.TryGetComponent(out Renderer rend))
            {
                mapBounds = rend.bounds;
            }
        }
    }
    

}
