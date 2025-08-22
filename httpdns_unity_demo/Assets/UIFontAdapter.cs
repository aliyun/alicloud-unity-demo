using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI字体自适应脚本 - 简化版本，避免高级语法
/// 专门为Android设备优化字体大小
/// </summary>
public class UIFontAdapter : MonoBehaviour
{
    public float fontScale = 1.2f;
    
    void Start()
    {
        AdaptFontSizes();
    }
    
    void AdaptFontSizes()
    {
        float scale = fontScale;
        
#if UNITY_ANDROID && !UNITY_EDITOR
        // Android设备适配
        scale = 1.5f;
#endif
        
        ApplyFontScaling(scale);
        
        Debug.Log("[UIFontAdapter] 应用字体缩放: " + scale);
    }
    
    void ApplyFontScaling(float scale)
    {
        // 查找所有Text组件
        Text[] allTexts = FindObjectsOfType<Text>();
        
        for (int i = 0; i < allTexts.Length; i++)
        {
            Text textComponent = allTexts[i];
            if (textComponent != null)
            {
                int originalSize = textComponent.fontSize;
                int newSize = (int)(originalSize * scale);
                
                // 限制字体大小范围
                if (newSize < 12) newSize = 12;
                if (newSize > 60) newSize = 60;
                
                textComponent.fontSize = newSize;
            }
        }
    }
    
    public void SetFontScale(float scale)
    {
        fontScale = scale;
        AdaptFontSizes();
    }
}