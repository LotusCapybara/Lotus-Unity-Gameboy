using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Lotus.GameboyEmulator.Renderers
{
    public class Renderer_ComputeLCD : MonoBehaviour
    {
        public ScreenPalette palette;

        public GameBoy gb;

        public RenderTexture targetRenderTexture;

        public MeshRenderer screenMesh;

        public GameObject containerBackLights;

        public ComputeShader computeShader;
        
        private PPU _ppu;
        
        private ComputeBuffer _bufferPalette;
        private ComputeBuffer _bufferPixels;
        private ComputeBuffer[] _bufferPrevScreens;

        private int[] _pixels;

        private int _kInitialize;
        private int _kSetPixelsColor;
        private int _kGhostingPass;
        
        private IEnumerator Start()
        {
            yield return null;
          
            _ppu = gb.ppu;
            _pixels = new int[PPU.TOTAL_PIXELS];

            screenMesh.material = palette.matScreen;
            containerBackLights.SetActive(palette.isBackLit);
            
            // all compute stuff
            _kInitialize = computeShader.FindKernel("Initialize");
            _kSetPixelsColor = computeShader.FindKernel("SetPixelsColor");
            _kGhostingPass = computeShader.FindKernel("GhostingPass");

            // create and set compute buffers 
            _bufferPalette = new ComputeBuffer(4, Marshal.SizeOf<Color>());
            _bufferPalette.SetData(palette.palette);     
            
            _bufferPixels = new ComputeBuffer(PPU.TOTAL_PIXELS, sizeof(int));

            _bufferPrevScreens = new ComputeBuffer[7];

            for (int i = 0; i < 7; i++)
            {
                _bufferPrevScreens[i] = new ComputeBuffer(PPU.TOTAL_PIXELS, sizeof(float) * 4);
            }
            
            
            // init kernel "Initialize" 
            for (int i = 0; i < 7; i++)
            {
                computeShader.SetBuffer(_kInitialize, $"PrevScreen{i}", _bufferPrevScreens[i]);
            }
            
            
            // init kernel "SetPixelColor"
            computeShader.SetBuffer(_kSetPixelsColor, "Palette", _bufferPalette);
            computeShader.SetBuffer(_kSetPixelsColor, "PPUColors", _bufferPixels);
            computeShader.SetTexture(_kSetPixelsColor, "FinalTexture", targetRenderTexture);
            
            
            // init kernel "GhostingPass"
            computeShader.SetTexture(_kGhostingPass, "FinalTexture", targetRenderTexture);
            
            for (int i = 0; i < 7; i++)
            {
                computeShader.SetBuffer(_kGhostingPass, $"PrevScreen{i}", _bufferPrevScreens[i]);
            }
            
            // Initializing buffers in GPU
            computeShader.Dispatch(_kInitialize, PPU.TOTAL_PIXELS, 1, 1);
        }

        private void OnDestroy()
        {
            _bufferPalette.Dispose();
            _bufferPixels.Dispose();
            
            foreach (var bufferPrevScreen in _bufferPrevScreens)
            {
                bufferPrevScreen.Dispose();
            }
        }

        private void Update()
        {
            if(_ppu == null)
                return;
            
            for (int x = 0; x < PPU.SCREEN_WIDTH; x++)
            {
                for (int y = PPU.SCREEN_HEIGHT - 1; y >= 0 ; y--)
                {
                    // the ctr tvs shot scanlines from bottom up
                    // so I invert it here
                    _pixels[PPU.SCREEN_WIDTH * y + x] = _ppu.pixels[x, PPU.SCREEN_HEIGHT - y - 1];
                }
            }
            
            _bufferPixels.SetData(_pixels);

            int workX = PPU.SCREEN_WIDTH / 8;
            int workY = PPU.SCREEN_HEIGHT / 8;
            
            computeShader.Dispatch(_kSetPixelsColor, workX, workY, 1);
            computeShader.Dispatch(_kGhostingPass, workX, workY, 1);
        }
    }
}