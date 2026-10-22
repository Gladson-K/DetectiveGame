#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ContentPostprocessors : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        string path = assetPath.Replace('\\', '/');
        if (path.StartsWith("Assets/Resources/Portraits/"))
        {
            var importer = (TextureImporter)assetImporter;
            if (importer.textureType != TextureImporterType.Sprite || importer.spriteImportMode != SpriteImportMode.Single)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.maxTextureSize = 1024;
            }
        }
    }
}
#endif
