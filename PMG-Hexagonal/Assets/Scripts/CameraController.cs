using UnityEngine;

public class CameraController : MonoBehaviour
{
    public void FitCameraToMap(float totalWidth, float totalHeight, float spriteWidth, float spriteHeight)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null || !mainCamera.orthographic) return;

        mainCamera.transform.position = new Vector3(0f, 0f, -10f);

        float mapWidthWithPadding = totalWidth + spriteWidth;
        float mapHeightWithPadding = totalHeight + spriteHeight;

        float sizeBasedOnHeight = mapHeightWithPadding / 2f;
        float screenAspect = (float)Screen.width / Screen.height;
        float sizeBasedOnWidth = (mapWidthWithPadding / 2f) / screenAspect;

        mainCamera.orthographicSize = Mathf.Max(sizeBasedOnHeight, sizeBasedOnWidth);
    }
}
