using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target; // 回転の中心となるターゲット（PylosGameオブジェクト）
    [SerializeField] private float rotationSpeed = 2.0f;
    [SerializeField] private float zoomSpeed = 2.0f;
    [SerializeField] private float minDistance = 2.0f;
    [SerializeField] private float maxDistance = 15.0f;

    private float yaw = 0.0f;
    private float pitch = 40.0f;
    private float distance = 6.0f;

    private float initialYaw;
    private float initialPitch;
    private float initialDistance;

    private Vector3 targetPosition = new Vector3(1.8f, 1.0f, 1.8f);

    void Start()
    {
        // 初期角度の計算（現在のカメラの位置から逆算して保存）
        Vector3 angles = transform.eulerAngles;
        initialYaw = angles.y;
        initialPitch = angles.x;
        yaw = initialYaw;
        pitch = initialPitch;
        
        if (target != null)
        {
            targetPosition = target.position;
        }
        else
        {
            // PylosGameオブジェクトを自動検索
            GameObject go = GameObject.Find("PylosGame");
            if (go != null)
            {
                target = go.transform;
                targetPosition = target.position;
            }
        }
        initialDistance = Vector3.Distance(transform.position, targetPosition);
        distance = initialDistance;
    }

    void LateUpdate()
    {
        // マウスの左ボタンドラッグで回転
        if (Input.GetMouseButton(0))
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed * 5f;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed * 5f;
            pitch = Mathf.Clamp(pitch, 10f, 85f); // あまり真下や真上に行きすぎないように制限
        }

        // マウスホイールでズーム
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed * 5f;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (target != null)
        {
            targetPosition = target.position;
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + targetPosition;

        transform.rotation = rotation;
        transform.position = position;
    }

    // 外部のUIボタンなどから回転させる用
    public void Rotate(float angleAmount)
    {
        yaw += angleAmount;
        UpdatePosition();
    }

    // カメラ位置・ズームの初期化
    public void ResetCamera()
    {
        yaw = initialYaw;
        pitch = initialPitch;
        distance = initialDistance;
        UpdatePosition();
    }
}
