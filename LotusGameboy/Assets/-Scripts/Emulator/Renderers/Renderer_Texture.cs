using System.Collections;
using UnityEngine;

namespace Lotus.GameboyEmulator.Renderers
{
    public class Renderer_Texture : MonoBehaviour
    {
        [SerializeField]
        private Color[] _palette;

        public GameBoy gb;

        public RenderTexture targetRenderTexture;
        
        private PPU _ppu;

        private Texture2D _texture;
        
        private IEnumerator Start()
        {
            _texture = new Texture2D(PPU.SCREEN_WIDTH, PPU.SCREEN_HEIGHT, TextureFormat.RGB24, false);
            
            yield return null;
          
            _ppu = gb.ppu;
        }

        private void Update()
        {
            if(_ppu == null)
                return;
            
            
            var pixels = _texture.GetPixels();

            for (int x = 0; x < PPU.SCREEN_WIDTH; x++)
            {
                for (int y = 0; y < PPU.SCREEN_HEIGHT; y++)
                {
                    pixels[PPU.SCREEN_WIDTH * y + x] = _palette[_ppu.pixels[x, y]];
                }
            }
            
            _texture.SetPixels(pixels);
            _texture.Apply();
            
            Graphics.Blit(_texture, targetRenderTexture);
        }
    }
}