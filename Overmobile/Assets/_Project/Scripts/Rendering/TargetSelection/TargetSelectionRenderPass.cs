using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Rendering.TargetSelection
{
    sealed class TargetSelectionRenderPass : ScriptableRenderPass
    {
        private static readonly ShaderTagId MeshShaderTagId = new ShaderTagId("UniversalForward");
        private static readonly ShaderTagId SpriteShaderTagId = new ShaderTagId("SRPDefaultUnlit");
        private static readonly Vector4 CompositeBlitScaleBias = new Vector4(x: 1f, y: 1f, z: 0f, w: 0f);

        private readonly Material _compositeMaterial;
        private readonly Shader _meshMaskShader;
        private readonly Shader _spriteMaskShader;

        public TargetSelectionRenderPass(
            Material compositeMaterial,
            Shader meshMaskShader,
            Shader spriteMaskShader)
        {
            _compositeMaterial = compositeMaterial;
            _meshMaskShader = meshMaskShader;
            _spriteMaskShader = spriteMaskShader;
            requiresIntermediateTexture = true;
            ConfigureInput(ScriptableRenderPassInput.Depth);
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (_compositeMaterial == null)
            {
                throw new MissingTargetSelectionMaterialException(nameof(TargetSelectionRenderPass));
            }

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            TextureHandle sourceColor = resourceData.activeColorTexture;

            if (!sourceColor.IsValid())
            {
                return;
            }

            UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            TextureDesc fillDesc = renderGraph.GetTextureDesc(sourceColor);
            fillDesc.name = "TargetSelectionFill";
            fillDesc.colorFormat = GraphicsFormat.R8G8B8A8_UNorm;
            fillDesc.depthBufferBits = 0;
            fillDesc.msaaSamples = MSAASamples.None;
            fillDesc.clearBuffer = true;
            fillDesc.clearColor = Color.clear;
            TextureHandle fillTexture = renderGraph.CreateTexture(fillDesc);

            TextureDesc outlineDesc = fillDesc;
            outlineDesc.name = "TargetSelectionOutline";
            TextureHandle outlineTexture = renderGraph.CreateTexture(outlineDesc);

            TextureDesc targetDepthDesc = fillDesc;
            targetDepthDesc.name = "TargetSelectionTargetDepth";
            targetDepthDesc.colorFormat = GraphicsFormat.R32_SFloat;
            TextureHandle targetDepthTexture = renderGraph.CreateTexture(targetDepthDesc);

            RendererListHandle meshRendererListHandle = CreateMaskRendererList(
                renderGraph,
                renderingData,
                cameraData,
                TargetSelectionRenderingLayers.Mesh,
                _meshMaskShader,
                MeshShaderTagId,
                cameraData.defaultOpaqueSortFlags
            );

            RendererListHandle spriteRendererListHandle = CreateMaskRendererList(
                renderGraph,
                renderingData,
                cameraData,
                TargetSelectionRenderingLayers.Sprite,
                _spriteMaskShader,
                SpriteShaderTagId,
                SortingCriteria.CommonTransparent
            );

            using (IRasterRenderGraphBuilder maskBuilder = renderGraph.AddRasterRenderPass(
                "Target Selection Mask",
                out MaskPassData maskPassData
            ))
            {
                maskPassData.MeshRendererListHandle = meshRendererListHandle;
                maskPassData.SpriteRendererListHandle = spriteRendererListHandle;

                maskBuilder.SetRenderAttachment(fillTexture, index: 0);
                maskBuilder.SetRenderAttachment(outlineTexture, index: 1);
                maskBuilder.SetRenderAttachment(targetDepthTexture, index: 2);
                maskBuilder.UseRendererList(meshRendererListHandle);
                maskBuilder.UseRendererList(spriteRendererListHandle);
                maskBuilder.AllowPassCulling(false);

                maskBuilder.SetRenderFunc(static (MaskPassData data, RasterGraphContext context) =>
                    {
                        context.cmd.DrawRendererList(data.MeshRendererListHandle);
                        context.cmd.DrawRendererList(data.SpriteRendererListHandle);
                    }
                );
            }

            TextureDesc compositeDesc = renderGraph.GetTextureDesc(sourceColor);
            compositeDesc.name = "TargetSelectionComposite";
            compositeDesc.clearBuffer = false;
            compositeDesc.msaaSamples = MSAASamples.None;
            compositeDesc.depthBufferBits = 0;
            TextureHandle compositeTexture = renderGraph.CreateTexture(compositeDesc);

            using (IRasterRenderGraphBuilder compositeBuilder = renderGraph.AddRasterRenderPass(
                "Target Selection Composite",
                out CompositePassData compositePassData
            ))
            {
                compositePassData.SourceColor = sourceColor;
                compositePassData.FillTexture = fillTexture;
                compositePassData.OutlineTexture = outlineTexture;
                compositePassData.TargetDepthTexture = targetDepthTexture;
                compositePassData.CompositeMaterial = _compositeMaterial;

                TextureHandle depthTexture = resourceData.cameraDepthTexture;

                if (!depthTexture.IsValid())
                {
                    depthTexture = resourceData.activeDepthTexture;
                }

                compositeBuilder.SetRenderAttachment(compositeTexture, index: 0);
                compositeBuilder.UseTexture(sourceColor);
                compositeBuilder.UseTexture(fillTexture);
                compositeBuilder.UseTexture(outlineTexture);
                compositeBuilder.UseTexture(targetDepthTexture);

                if (depthTexture.IsValid())
                {
                    compositeBuilder.UseTexture(depthTexture);
                }

                compositeBuilder.AllowPassCulling(false);

                compositeBuilder.SetRenderFunc(static (CompositePassData data, RasterGraphContext context) =>
                    {
                        data.CompositeMaterial.SetTexture(TargetSelectionShaderProperties.FillTexture, data.FillTexture);
                        data.CompositeMaterial.SetTexture(TargetSelectionShaderProperties.OutlineTexture, data.OutlineTexture);

                        data.CompositeMaterial.SetTexture(
                            TargetSelectionShaderProperties.TargetDepthTexture,
                            data.TargetDepthTexture
                        );

                        Blitter.BlitTexture(
                            context.cmd,
                            data.SourceColor,
                            CompositeBlitScaleBias,
                            data.CompositeMaterial,
                            pass: 0
                        );
                    }
                );
            }

            resourceData.cameraColor = compositeTexture;
        }

        private RendererListHandle CreateMaskRendererList(
            RenderGraph renderGraph,
            UniversalRenderingData renderingData,
            UniversalCameraData cameraData,
            uint renderingLayerMask,
            Shader overrideShader,
            ShaderTagId shaderTagId,
            SortingCriteria sortingCriteria)
        {
            RendererListDesc rendererListDesc = new RendererListDesc(shaderTagId, renderingData.cullResults, cameraData.camera)
            {
                renderQueueRange = RenderQueueRange.all,
                renderingLayerMask = renderingLayerMask,
                overrideShader = overrideShader,
                sortingCriteria = sortingCriteria
            };

            return renderGraph.CreateRendererList(rendererListDesc);
        }

        private sealed class MaskPassData
        {
            public RendererListHandle MeshRendererListHandle;
            public RendererListHandle SpriteRendererListHandle;
        }

        private sealed class CompositePassData
        {
            public TextureHandle SourceColor;
            public TextureHandle FillTexture;
            public TextureHandle OutlineTexture;
            public TextureHandle TargetDepthTexture;
            public Material CompositeMaterial;
        }
    }
}
