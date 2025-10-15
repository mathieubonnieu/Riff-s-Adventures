using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomRenderFeature : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        private Material material;
        private RTHandle cameraColorTarget;
        private RTHandle temporaryColorTexture;
        private string profilerTag;

        public CustomRenderPass(Material mat, string tag)
        {
            material = mat;
            profilerTag = tag;
            // Indiquer que cette passe nécessite la profondeur de la caméra.
            ConfigureInput(ScriptableRenderPassInput.Depth);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // Récupérer la cible de couleur de la caméra.
            cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

            // On utilise ici le descriptor de la caméra pour allouer notre texture temporaire.
            // On laisse depthBufferBits à 0 puisque la profondeur sera récupérée via _CameraDepthTexture.
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;

            RenderingUtils.ReAllocateIfNeeded(ref temporaryColorTexture,
                descriptor,
                FilterMode.Bilinear,
                TextureWrapMode.Clamp,
                name: "_TempRT");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null)
            {
                Debug.LogError("Material is null in CustomRenderPass");
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            using (new ProfilingScope(cmd, new ProfilingSampler(profilerTag)))
            {

                // Copier la cible caméra dans notre texture temporaire.
                Blitter.BlitCameraTexture(cmd, cameraColorTarget, temporaryColorTexture);

                Matrix4x4 cameraProj = renderingData.cameraData.GetProjectionMatrix();
                Matrix4x4 invProj = cameraProj.inverse;

                // Transmettre les matrices au shader
                material.SetMatrix("_InverseCameraProjection", invProj);
                // Définir explicitement la texture principale dans le matériau.
                material.SetTexture("_MainTex", temporaryColorTexture);
                // Note : _CameraDepthTexture est automatiquement généré par URP et est accessible depuis le shader

                // Appliquer l'effet et écrire le résultat dans la cible caméra.
                Blitter.BlitCameraTexture(cmd, temporaryColorTexture, cameraColorTarget, material, 0);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // Le nettoyage des RTHandles est géré automatiquement par URP.
        }
    }

    [System.Serializable]
    public class Settings
    {
        public Material effectMaterial;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public Settings settings = new Settings();
    private CustomRenderPass pass;
    private const string k_ProfilerTag = "Custom Fullscreen Effect";

    public override void Create()
    {
        if (settings.effectMaterial == null)
        {
            Debug.LogWarning("Effect material is missing in CustomRenderFeature");
            return;
        }

        pass = new CustomRenderPass(settings.effectMaterial, k_ProfilerTag)
        {
            renderPassEvent = settings.renderPassEvent
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.effectMaterial == null || !renderingData.cameraData.postProcessEnabled)
            return;
        
        // Ne pas activer dans l'éditeur ou si le post-process est désactivé
        if (settings.effectMaterial == null ||
            !renderingData.cameraData.postProcessEnabled ||
            renderingData.cameraData.cameraType == CameraType.SceneView)
        {
            return;
        }

        renderer.EnqueuePass(pass);
    }

    protected override void Dispose(bool disposing)
    {
        pass = null;
    }
}
