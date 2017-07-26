using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class Bloom : MonoBehaviour 
{
	Material _stencilMaskMaterial;
    Material stencilMaskMaterial
    {
        get
        {
            if (_stencilMaskMaterial == null)
                _stencilMaskMaterial = new Material(stencilMaskShader);

            return _stencilMaskMaterial;
        }
    }

    Material _bloomMaterial;
    Material bloomMaterial
    {
        get
        {
            if(_bloomMaterial == null)
                _bloomMaterial = new Material(bloomShader);

            return _bloomMaterial;
        }
    }

    RenderTexture cameraRenderTexture;
	RenderTexture stencilMask;

    Camera camera;

    int bloomTex_id;

    [SerializeField]
    Camera renderCamera;
    [SerializeField]
    Shader bloomShader;
    [SerializeField]
    Shader stencilMaskShader;

    [SerializeField]
    [Range(0, 4f)]
    float bloomIntencity;

    [SerializeField]
    [Range(1,3)]
    int stencil;

    [SerializeField]
    [Range(1, 4)]
    int blurSpread;
    [SerializeField]
    [Range(0, 10)]
    int blurIterations;

    public void Start()
    {
        bloomTex_id = Shader.PropertyToID("_BloomTexture");

        camera = gameObject.AddComponent<Camera>();
        camera.allowHDR = false;
        camera.allowMSAA = false;
        camera.clearFlags = CameraClearFlags.Nothing;

        cameraRenderTexture = new RenderTexture (Screen.width, Screen.height, 24);
		stencilMask = new RenderTexture (Screen.width, Screen.height, 24);

        renderCamera.targetTexture = cameraRenderTexture;
	}

    void OnPostRender()
    {
        GL.Clear(true, true, Color.black);

        CommandBuffer cmdBuffer = new CommandBuffer();

        Graphics.SetRenderTarget (stencilMask.colorBuffer, cameraRenderTexture.depthBuffer);

        stencilMaskMaterial.SetFloat("_Stencil", stencil);
        Graphics.Blit(cameraRenderTexture, stencilMaskMaterial);

        //Apply Bloom Effect
        DownSample(cmdBuffer, stencilMask, blurSpread);
        Blur(cmdBuffer, stencilMask.width >> blurSpread, stencilMask.height >> blurSpread, bloomTex_id, blurIterations);

        RenderTexture.active = null;

        Graphics.ExecuteCommandBuffer(cmdBuffer);

        bloomMaterial.SetFloat("_Intensity", bloomIntencity);

        Graphics.Blit(cameraRenderTexture, bloomMaterial, 4);
    }


    public void DownSample(CommandBuffer buffer, RenderTexture src, int lod)
    {
        int width = src.width;
        int height = src.height;
        for (var i = 0; i < lod; i++)
        {
            width = width >> 1;
            height = height >> 1;

            buffer.GetTemporaryRT(Shader.PropertyToID("downSample1_" + i), width, height, 0, FilterMode.Bilinear);

            if(i != 0)
                buffer.Blit(Shader.PropertyToID("downSample0_" + (i - 1)), Shader.PropertyToID("downSample1_" + i), bloomMaterial, 0);
            else
                buffer.Blit(src, Shader.PropertyToID("downSample1_" + i), bloomMaterial, 0);

            buffer.GetTemporaryRT(Shader.PropertyToID("downSample0_" + i), width, height, 0, FilterMode.Bilinear);
            buffer.Blit(Shader.PropertyToID("downSample1_" + i), Shader.PropertyToID("downSample0_" + i));
        }

        //buffer.SetGlobalTexture(outputId, Shader.PropertyToID("downSample0_" + (lod - 1)));
        //Blur(buffer, src.width >> lod, src.height >> lod, outputId, blurIterations);
    }

    public void Blur(CommandBuffer buffer, int width, int height, int outputId, int nIterations)
    {
        buffer.GetTemporaryRT(Shader.PropertyToID("blur0"), width, height, 0, FilterMode.Bilinear);
        buffer.GetTemporaryRT(Shader.PropertyToID("blur1"), width, height, 0, FilterMode.Bilinear);

        var iters = Mathf.Clamp(nIterations, 0, 10);

        //Grab Texture
        buffer.Blit(null, Shader.PropertyToID("blur0"));

        for (var i = 0; i < iters; i++)
        {
            for (var pass = 2; pass < 4; pass++)
            {
                buffer.Blit(Shader.PropertyToID("blur0"), Shader.PropertyToID("blur1"), bloomMaterial, pass);
                Swap(buffer, Shader.PropertyToID("blur0"), Shader.PropertyToID("blur1"), width, height);
            }
        }

        buffer.SetGlobalTexture(outputId, Shader.PropertyToID("blur0"));
    }

    private void Swap(CommandBuffer buffer, RenderTargetIdentifier src, RenderTargetIdentifier dst, int width, int height)
    {
        int swapTemp = Shader.PropertyToID("swap_temp");
        buffer.GetTemporaryRT(swapTemp, width, height, 0, FilterMode.Bilinear);

        buffer.Blit(src, swapTemp);
        buffer.Blit(dst, src);
        buffer.Blit(swapTemp, dst);
    }
}
