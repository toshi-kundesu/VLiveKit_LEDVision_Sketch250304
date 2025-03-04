using UnityEngine;

public class FieldOfViewOscillator : MonoBehaviour
{
    private Camera mainCamera; // 通常のCameraの参照
    private float initialFOV;
    public float amplitude = 10f;
    public float frequency = 1f;

    void Start()
    {
        // アタッチされているGameObjectからCameraを取得
        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
        {
            Debug.LogError("このGameObjectにCameraがアタッチされていません。");
            return;
        }

        // 初期のFieldOfViewを取得
        initialFOV = mainCamera.fieldOfView;
    }

    void Update()
    {
        if (mainCamera == null) return;

        // 時間に基づいてFieldOfViewを振動させる
        float fovOffset = amplitude * Mathf.Sin(Time.time * frequency);
        mainCamera.fieldOfView = initialFOV + fovOffset;
    }
}
