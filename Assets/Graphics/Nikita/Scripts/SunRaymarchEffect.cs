using UnityEngine;

[ExecuteAlways, RequireComponent(typeof(Camera))]
public class SecondSkyboxEffect : MonoBehaviour
{
    public Material material;

    void OnEnable()
    {
        var cam = GetComponent<Camera>();
        cam.depthTextureMode |= DepthTextureMode.Depth;      // needed for _CameraDepthTexture
        if (cam.clearFlags == CameraClearFlags.Nothing)
            cam.clearFlags = CameraClearFlags.Skybox;        // ensure skybox renders first
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (material == null) { Graphics.Blit(src, dst); return; }
        Graphics.Blit(src, dst, material, 0);
    }
}
