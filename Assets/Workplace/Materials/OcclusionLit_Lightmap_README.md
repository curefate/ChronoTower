# OcclusionLit_Lightmap Shader 使用说明

## 📝 概述

这是一个基于Meta官方 `OcclusionLit.shader` 修改的版本，添加了 **Lightmap（烘焙光照贴图）** 支持。

## ✨ 功能特性

✅ **Meta MR 深度遮挡** - 支持 HARD_OCCLUSION 和 SOFT_OCCLUSION  
✅ **Lightmap 烘焙光照** - 支持Unity的光照贴图系统  
✅ **PBR 材质** - 完整的物理渲染（金属度、光滑度）  
✅ **实时光照** - 支持主光源和额外光源  
✅ **实时阴影** - 支持接收和投射阴影  

## 🔧 主要修改内容

### 1. 添加的 Multi-compile 指令
```hlsl
#pragma multi_compile _ LIGHTMAP_ON
#pragma multi_compile _ DIRLIGHTMAP_COMBINED
```

### 2. 顶点输入结构（Attributes）
```hlsl
float2 staticLightmapUV : TEXCOORD1;  // 新增：光照贴图UV
```

### 3. 顶点到片段传递结构（Varyings）
```hlsl
DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 4);  // 新增：光照贴图或球谐函数
```

### 4. 顶点着色器
```hlsl
// 传递光照贴图UV或计算球谐函数
OUTPUT_LIGHTMAP_UV(input.staticLightmapUV, unity_LightmapST, output.staticLightmapUV);
OUTPUT_SH(output.normalWS.xyz, output.vertexSH);
```

### 5. 片段着色器
```hlsl
// 采样烘焙GI（全局光照）
lightingInput.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.vertexSH, normalWS);
lightingInput.shadowMask = SAMPLE_SHADOWMASK(input.staticLightmapUV);
lightingInput.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
```

### 6. Meta Pass（新增）
添加了 Meta Pass 用于支持光照贴图烘焙过程

## 📦 使用方法

### 1. 创建材质
1. 在Unity中右键 → Create → Material
2. 将 Shader 设置为 `Custom/OcclusionLit_Lightmap`

### 2. 配置材质参数
- **Color** - 基础颜色
- **Albedo (RGB)** - 反照率贴图
- **Smoothness** - 光滑度 (0-1)
- **Metallic** - 金属度 (0-1)
- **Environment Depth Bias** - 深度偏移（用于微调遮挡效果）

### 3. 设置物体
1. 将材质应用到物体上
2. 确保物体的 Mesh Renderer 设置：
   - ✅ **Contribute Global Illumination** 已启用
   - ✅ **Receive Global Illumination** 设为 `Lightmaps`
   - ✅ **Cast Shadows** 已启用

### 4. 烘焙光照贴图
1. 在场景中设置光源标记为 `Baked` 或 `Mixed`
2. Window → Rendering → Lighting
3. 点击 `Generate Lighting` 进行烘焙

### 5. Meta MR 深度遮挡设置
在材质面板的 Shader Keywords 中可以启用：
- `HARD_OCCLUSION` - 硬边缘遮挡（性能更好）
- `SOFT_OCCLUSION` - 软边缘遮挡（效果更自然）

## 🎯 与原版的区别

### Meta 原版 OcclusionLit.shader
- ✅ 深度遮挡
- ✅ 实时光照
- ❌ **不支持** Lightmap

### 修改版 OcclusionLit_Lightmap.shader
- ✅ 深度遮挡
- ✅ 实时光照
- ✅ **支持** Lightmap ⭐

## ⚙️ 技术细节

### Lightmap 工作原理
1. **有光照贴图时**：使用烘焙的光照数据（LIGHTMAP_ON）
2. **无光照贴图时**：使用球谐函数（Spherical Harmonics）提供环境光

### 深度遮挡兼容性
- 深度遮挡在最后阶段应用，不会影响光照计算
- 使用 `META_DEPTH_OCCLUDE_OUTPUT_PREMULTIPLY` 宏处理遮挡

## 🐛 故障排除

### 问题1：看不到烘焙光照效果
**解决方案：**
- 检查物体是否标记为 Lightmap Static
- 检查 Mesh Renderer 的 GI 设置
- 重新烘焙光照贴图

### 问题2：深度遮挡不工作
**解决方案：**
- 确保在材质上启用了 HARD_OCCLUSION 或 SOFT_OCCLUSION
- 检查 Meta XR SDK 是否正确安装
- 确认在 Meta Quest 设备上运行

### 问题3：阴影显示异常
**解决方案：**
- 检查 URP 渲染管线设置中的阴影配置
- 确认光源的阴影设置正确

## 📚 相关文档

- [Meta Depth API Documentation](https://developers.meta.com/horizon/documentation/unity/unity-depthapi-occlusions-advanced-usage/)
- [Unity URP Lighting](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest)
- [Unity Lightmapping](https://docs.unity3d.com/Manual/Lightmapping.html)

## 📄 许可证

基于 Meta Platforms, Inc. 的 Oculus SDK License Agreement  
修改部分遵循相同许可协议

---

**创建日期：** 2026-03-16  
**版本：** 1.0  
**兼容：** Unity 2021.2+ / URP / Meta XR SDK
