using System;
using System.IO;
using System.Linq;
using System.Collections;

using UnityEngine;
using UnityEngine.Assertions;

public class SimpleThumbnail : MonoBehaviour
{
#if UNITY_EDITOR
    /// <summary>
    /// From which camera the pixels will be read.
    /// </summary>
    [SerializeField] private Camera _camera;

    /// <summary>
    /// Which key code will be used to take a screenshot in editor play mode.
    /// </summary>
    [SerializeField] private KeyCode _editorTakeKeyCode = KeyCode.P;

    /// <summary>
    /// Thumbnail size.
    /// </summary>
    [SerializeField, Range(.1f, 3)] private float _multiplier = 1;

    // Methods

    private void Update()
    {
        if (_editorTakeKeyCode != KeyCode.None && Input.GetKeyDown(_editorTakeKeyCode))
        {
            StartCoroutine(Take(_camera, tex2D =>
            {
                string path = $"{Application.persistentDataPath}/{Screen.width}x{Screen.height}_{DateTime.Now.ToString("yyyyMMddhhmmss")}_{_camera.name}.png";
                File.WriteAllBytes(path, tex2D.EncodeToPNG());
                Debug.Log(path);
            }, _multiplier));
        }
    }
#endif

    public static void Focus(Camera camera, GameObject target, float marginPercentage)
    {
        Bounds bounds = GetBounds(target);
        float maxExtent = bounds.extents.magnitude;
        float minDistance = (maxExtent * marginPercentage) / Mathf.Sin(Mathf.Deg2Rad * camera.fieldOfView / 2f);
        camera.transform.position = target.transform.position - Vector3.forward * minDistance;
        camera.nearClipPlane = minDistance - maxExtent;
    }

    private static Bounds GetBounds(GameObject gameObject)
    {
        Renderer parentRenderer = gameObject.GetComponent<Renderer>();
        Renderer[] childrenRenderers = gameObject.GetComponentsInChildren<Renderer>();
        Bounds bounds = parentRenderer != null ? parentRenderer.bounds : childrenRenderers.FirstOrDefault(x => x.enabled).bounds;

        if (childrenRenderers.Length > 0)
            foreach (Renderer renderer in childrenRenderers)
                if (renderer.enabled)
                    bounds.Encapsulate(renderer.bounds);

        return bounds;
    }

    /// <summary>
    /// Take a screenshot.
    /// </summary>
    /// <param name="camera">Target camera.</param>
    /// <param name="callback">Result.</param>
    /// <param name="multiplier">Output texture size.</param>
    public static IEnumerator Take(Camera camera, Action<Texture2D> callback, float multiplier = 1)
    {
        Assert.IsTrue(multiplier >= .1f);

        RenderTexture renderTexture = new RenderTexture((int)(Screen.width * multiplier), (int)(Screen.height * multiplier), 24, RenderTextureFormat.ARGB32, 0);
        camera.targetTexture = renderTexture;

        yield return null;

        renderTexture.Release();
        camera.Render();

        Texture2D tex2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        RenderTexture.active = renderTexture;
        tex2D.ReadPixels(new Rect(camera.pixelRect), 0, 0, false);
        tex2D.Apply();
        RenderTexture.active = null;

        yield return null;

        camera.targetTexture = null;

        DestroyImmediate(renderTexture);

        callback?.Invoke(tex2D);
    }

    /// <summary>
    /// Remove transparent margins and leave only visible pixels.
    /// </summary>
    /// <param name="target">Target texture.</param>
    /// <param name="square">Save the result as square (transparent margins are added if necessary).</param>
    /// <param name="callback">Result.</param>
    public static IEnumerator Crop(Texture2D target, bool square, Action<Texture2D> callback)
    {
        RectOffset offset = new RectOffset(target.width, 0, 0, target.height);

        Color32[] pixels = target.GetPixels32();
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a != 0)
            {
                int y = i / target.width;
                int x = i % target.width;

                if (x < offset.left) offset.left = x;
                if (x > offset.right) offset.right = x;
                if (y > offset.top) offset.top = y;
                if (y < offset.bottom) offset.bottom = y;
            }
        }

        yield return null;

        int width = offset.right - offset.left;
        int height = offset.top - offset.bottom;

        Texture2D tex2D;
        if (square)
        {
            int max = Mathf.Max(width, height);

            Color32[] transparent = new Color32[max * max];
            for (int i = 0; i < transparent.Length; i++)
                transparent[i] = Color.clear;

            tex2D = new Texture2D(max, max);
            tex2D.SetPixels32(transparent);

            if (width > height)
                tex2D.SetPixels(0, width / 2 - height / 2, width, height, target.GetPixels(offset.left, offset.bottom, width, height));
            else
                tex2D.SetPixels(height / 2 - width / 2, 0, width, height, target.GetPixels(offset.left, offset.bottom, width, height));
        }
        else
        {
            tex2D = new Texture2D(width, height);
            tex2D.SetPixels(target.GetPixels(offset.left, offset.bottom, width, height));
        }

        tex2D.Apply();

        callback?.Invoke(tex2D);
    }
}
