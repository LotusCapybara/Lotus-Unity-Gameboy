using System;
using Lotus.GameboyEmulator;
using UnityEngine;
using UnityEditor;
using Tools = Lotus.GameboyEmulator.Utils.Tools;

/// <summary>
/// Editor Window used to debug the emulator in runtime.
/// It displays information like the state of registers, program stack and counter,
/// program lines, etc.
/// It's a bit clunky, I might spend some time to improve it (maybe? lel)
/// </summary>
public class EmuDebugWindow : EditorWindow
{
    private GameBoy _gb;

    [MenuItem("Emulator/Debug Window")]
    private static void Init()
    {
        // Get existing open window or if none, make a new one:
        EmuDebugWindow window = (EmuDebugWindow)EditorWindow.GetWindow(typeof(EmuDebugWindow));
        window.Show();
    }

    private void Update()
    {
        Repaint();
    }

    private void OnGUI()
    {
        if (!Application.isPlaying)
        {
            GUILayout.Label("Runtime Online");
            return;
        }

        if (_gb == null)
            _gb = FindObjectOfType<GameBoy>();
        
        EditorGUILayout.BeginHorizontal();
        {
            DrawRegisters();

            DrawPPURegisters();
            
            DrawInstructions();
            
            DrawMemory();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawRegisters()
    {
        GUILayout.BeginVertical(GUILayout.Width(120));
        {
            GUILayout.Label("---------");
            
            GUILayout.Label("[af]" + Tools.HexString(_gb.cpu.registers.af), GUILayout.Width(120));
            GUILayout.Label("[bc]" + Tools.HexString(_gb.cpu.registers.bc), GUILayout.Width(120));
            GUILayout.Label("[de]" + Tools.HexString(_gb.cpu.registers.de), GUILayout.Width(120));
            GUILayout.Label("[hl]" + Tools.HexString(_gb.cpu.registers.hl), GUILayout.Width(120));
            
            GUILayout.Label("---------");
            
            GUILayout.Label("SP: " + Tools.HexString( _gb.cpu.registers.sp), GUILayout.Width(80));
            GUILayout.Label("PC: " + Tools.HexString( _gb.cpu.registers.pc), GUILayout.Width(80));

            GUILayout.Label("---------");
            
            string flags = "";
            flags += "Z: "  + (_gb.cpu.registers.flagZero ? "1" : "0"); 
            flags += " S: "  + (_gb.cpu.registers.flagSubtraction ? "1" : "0");
            flags += " H:"  + (_gb.cpu.registers.flagHalfCarry ? "1" : "0");
            flags += " C:"  + (_gb.cpu.registers.flagCarry ? "1" : "0");
            GUILayout.Label(flags, GUILayout.Width(120));
            
            GUILayout.Label("---------");
            
            string ime_ima = $"IME: {_gb.cpu.isMasterEnabled.ToString()} IMA: {_gb.cpu.stopCounter.ToString()}";
            GUILayout.Label(ime_ima, GUILayout.Width(120));
            
            GUILayout.Label("-----------", GUILayout.Width(120));
        }
        GUILayout.EndVertical(); 
    }

    private void DrawPPURegisters()
    {
        GUILayout.BeginVertical(GUILayout.Width(120));
        {
            GUILayout.Label("---------");

            GUILayout.Label("[lcdControl]" + Tools.HexString(_gb.bus.lcdControl, 2), GUILayout.Width(120));
            GUILayout.Label("[STAT]" + Tools.HexString(_gb.bus.lcdStat, 2), GUILayout.Width(120));
            GUILayout.Label("[LY]" + Tools.HexString(_gb.bus.ly, 2), GUILayout.Width(120));
            GUILayout.Label("[IE]" + Tools.HexString(_gb.bus.IE, 2), GUILayout.Width(120));
            GUILayout.Label("[IF]" + Tools.HexString(_gb.bus.IF, 2), GUILayout.Width(120));
            
        }
        GUILayout.EndVertical(); 
    }
    
    private void DrawInstructions()
    {
        if(_gb.cpu.currentOp == null)
            return;
        
        EditorGUILayout.BeginVertical(GUILayout.Width(500));
        {
            for(int i = 0; i < _gb.cpu.debugOperationsFuture.Length; i++)
            {
                DrawSingleInstruction( _gb.cpu.debugOperationsFuture [i], 0 );
            }

            Color contentColor = GUI.contentColor;
            
            for(int i = 0; i < _gb.cpu.debugOperations.Length; i++)
            {
                if(i == 0)
                    GUI.color = Color.cyan;
                
                DrawSingleInstruction( _gb.cpu.debugOperations [i], _gb.cpu.qtyInstructions - i );

                GUI.color = contentColor;
            }
        }
        EditorGUILayout.EndVertical();
    }
    
    private void DrawSingleInstruction(InstructionDebug instructionDebug, long qtyInst)
    {
        EditorGUILayout.BeginHorizontal();
        {
             GUILayout.Label($"{Tools.HexString(instructionDebug.address)}", GUILayout.Width(60));
             GUILayout.Label($" | [{Tools.HexString(instructionDebug.code, 2)}]", GUILayout.Width(55));
             
             GUILayout.Label($"{ OperationsMap.map[instructionDebug.code].fullInstructionCode }", GUILayout.Width(70));
             GUILayout.Label($"| {instructionDebug.bytesString}", GUILayout.Width(100));
             GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();
    }


    private string _from = "0x0000";
    
    private void DrawMemory()
    {
        
        
        EditorGUILayout.BeginVertical(GUILayout.Width(500));
        {
            _from = GUILayout.TextField(_from, GUILayout.Width(100));

            try
            {
                int from = Convert.ToInt32(_from, 16);
                int to = Mathf.Min(from + 60, 0xFFFF);
            
                for (int i = to - 1; i>= from; i --)
                {
                    ushort val = _gb.bus.Read8((ushort)i);
                    
                    GUILayout.Label($"[{Tools.HexString(i, 2)}]  {Tools.HexString(val, 2)}", GUILayout.Width(150));
                }
            }
            catch (Exception e)
            {
            }
        }
        
        
        EditorGUILayout.EndVertical();

        
    }
}