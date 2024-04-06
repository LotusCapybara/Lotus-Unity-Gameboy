using UnityEngine;

[CreateAssetMenu(menuName = "Lotus/Screen Palette")]
public class ScreenPalette : ScriptableObject
{
    public bool isBackLit;

    public Material matScreen;
    
    public Color[] palette;
}
