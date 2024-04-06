using System;
using System.Collections.Generic;
using System.IO;
using Lotus.GameboyEmulator;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

[Serializable]
public class TestMemory
{
    public Dictionary<string, string> cpu;
    public string[][] ram;
}

[Serializable]
public class InstTest
{
    public string name;
    public TestMemory initial;
    public TestMemory final;
    public string[][] cycles;
}

public class TestInstructions
{
    private List<InstTest> currentTests;

    private GameBoy _gameboy;

    [Test]
    [TestCase("01"), TestCase("02"), TestCase("03"), TestCase("04"), TestCase("05"), TestCase("06")]
    [TestCase("07"), TestCase("08"), TestCase("09"), TestCase("0A"), TestCase("0B"), TestCase("0C")]
    [TestCase("0D"), TestCase("0E"), TestCase("0F")]
    public void Run0X(string code)
    {
        RunJsonTests(code);
    }
    
    // not to sure about 0x10.
    // if it doesn't increase pc then it fails, but no where in documentations or other emus 
    // the STOP operation increases the pc (besides the initial read)
    [Test]
    [TestCase("11"), TestCase("12"), TestCase("13"), TestCase("14"), TestCase("15")]
    [TestCase("16"), TestCase("17"), TestCase("18"), TestCase("19"), TestCase("1A"), TestCase("1B")]
    [TestCase("1C"), TestCase("1D"), TestCase("1E"), TestCase("1F")]
    public void Run1X(string code)
    {
        RunJsonTests(code);
    }
    
    [Test]
    [TestCase("20"), TestCase("21"), TestCase("22"), TestCase("23"), TestCase("24"), TestCase("25")]
    [TestCase("26"), TestCase("27"), TestCase("28"), TestCase("29"), TestCase("2A"), TestCase("2B")]
    [TestCase("2C"), TestCase("2D"), TestCase("2E"), TestCase("2F")]
    public void Run2X(string code)
    {
        RunJsonTests(code);
    }
    
    [Test]
    [TestCase("30"), TestCase("31"), TestCase("32"), TestCase("33"), TestCase("34"), TestCase("35")]
    [TestCase("36"), TestCase("37"), TestCase("38"), TestCase("39"), TestCase("3A"), TestCase("3B")]
    [TestCase("3C"), TestCase("3D"), TestCase("3E"), TestCase("3F")]
    public void Run3X(string code)
    {
        RunJsonTests(code);
    }
    
    [Test]
    [TestCase("40"), TestCase("41"), TestCase("42"), TestCase("43"), TestCase("44"), TestCase("45")]
    [TestCase("46"), TestCase("47"), TestCase("48"), TestCase("49"), TestCase("4A"), TestCase("4B")]
    [TestCase("4C"), TestCase("4D"), TestCase("4E"), TestCase("4F")]
    public void Run4X(string code)
    {
        RunJsonTests(code);
    }
    
    [Test]
    [TestCase("50"), TestCase("51"), TestCase("52"), TestCase("53"), TestCase("54"), TestCase("55")]
    [TestCase("56"), TestCase("57"), TestCase("58"), TestCase("59"), TestCase("5A"), TestCase("5B")]
    [TestCase("5C"), TestCase("5D"), TestCase("5E"), TestCase("5F")]
    public void Run5X(string code)
    {
        RunJsonTests(code);
    }
    
    [Test]
    [TestCase("60"), TestCase("61"), TestCase("62"), TestCase("63"), TestCase("64"), TestCase("65")]
    [TestCase("66"), TestCase("67"), TestCase("68"), TestCase("69"), TestCase("6A"), TestCase("6B")]
    [TestCase("6C"), TestCase("6D"), TestCase("6E"), TestCase("6F")]
    public void Run6X(string code)
    {
        RunJsonTests(code);
    }
    
    [Test]
    [TestCase("70"), TestCase("71"), TestCase("72"), TestCase("73"), TestCase("74"), TestCase("75")]
    [TestCase("76"), TestCase("77"), TestCase("78"), TestCase("79"), TestCase("7A"), TestCase("7B")]
    [TestCase("7C"), TestCase("7D"), TestCase("7E"), TestCase("7F")]
    public void Run7X(string code)
    {
        RunJsonTests(code);
    }
    [Test]
    [TestCase("80"), TestCase("81"), TestCase("82"), TestCase("83"), TestCase("84"), TestCase("85")]
    [TestCase("86"), TestCase("87"), TestCase("88"), TestCase("89"), TestCase("8A"), TestCase("8B")]
    [TestCase("8C"), TestCase("8D"), TestCase("8E"), TestCase("8F")]
    public void Run8X(string code)
    {
        RunJsonTests(code);
    }
    
    [Test]
    [TestCase("90"), TestCase("91"), TestCase("92"), TestCase("93"), TestCase("94"), TestCase("95")]
    [TestCase("96"), TestCase("97"), TestCase("98"), TestCase("99"), TestCase("9A"), TestCase("9B")]
    [TestCase("9C"), TestCase("9D"), TestCase("9E"), TestCase("9F")]
    public void Run9X(string code)
    {
        RunJsonTests(code);
    }
    
    [Test]
    [TestCase("A0"), TestCase("A1"), TestCase("A2"), TestCase("A3"), TestCase("A4"), TestCase("A5")]
    [TestCase("A6"), TestCase("A7"), TestCase("A8"), TestCase("A9"), TestCase("AA"), TestCase("AB")]
    [TestCase("AC"), TestCase("AD"), TestCase("AE"), TestCase("AF")]
    public void RunAX(string code)
    {
        RunJsonTests(code);
    }
    
    [Test]
    [TestCase("B0"), TestCase("B1"), TestCase("B2"), TestCase("B3"), TestCase("B4"), TestCase("B5")]
    [TestCase("B6"), TestCase("B7"), TestCase("B8"), TestCase("B9"), TestCase("BA"), TestCase("BB")]
    [TestCase("BC"), TestCase("BD"), TestCase("BE"), TestCase("BF")]
    public void RunBX(string code)
    {
        RunJsonTests(code);
    }
    
    [Test]
    [TestCase("C0"), TestCase("C1"), TestCase("C2"), TestCase("C3"), TestCase("C4"), TestCase("C5")]
    [TestCase("C6"), TestCase("C7"), TestCase("C8"), TestCase("C9"), TestCase("CA"), TestCase("CB")]
    [TestCase("CC"), TestCase("CD"), TestCase("CE"), TestCase("CF")]
    public void RunCX(string code)
    {
        RunJsonTests(code);
    }
    
    [Test]
    [TestCase("D0"), TestCase("D1"), TestCase("D2"), TestCase("D4"), TestCase("D5")]
    [TestCase("D6"), TestCase("D7"), TestCase("D8"), TestCase("D9"), TestCase("DA")]
    [TestCase("DC"), TestCase("DE"), TestCase("DF")]
    public void RunDX(string code)
    {
        RunJsonTests(code);
    }
    
    [Test]
    [TestCase("E0"), TestCase("E1"), TestCase("E2"), TestCase("E5")]
    [TestCase("E6"), TestCase("E7"), TestCase("E8"), TestCase("E9"), TestCase("EA")]
    [TestCase("EE"), TestCase("EF")]
    public void RunEX(string code)
    {
        RunJsonTests(code);
    }
    
    [Test]
    [TestCase("F0"), TestCase("F1"), TestCase("F2"), TestCase("F3"), TestCase("F5")]
    [TestCase("F6"), TestCase("F7"), TestCase("F8"), TestCase("F9"), TestCase("FA"), TestCase("FB")]
    [TestCase("FE")]
    public void RunFX(string code)
    {
        RunJsonTests(code);
    }
    
    

    private void RunJsonTests(string code)
    {
        _gameboy = GameObject.FindObjectOfType<GameBoy>();
        _gameboy.Initialize("TEST_RUNNER");
        
        string jsonsPath = "/-Tests/EditorTests/JsonTests/xx.json";

        string currentJsonPath = Application.dataPath + jsonsPath;
        currentJsonPath = currentJsonPath.Replace("xx", code);

        string jsonText = File.ReadAllText(currentJsonPath);
        
        currentTests = JsonConvert.DeserializeObject<List<InstTest>>(jsonText);
        
        RunInstruction((byte) Convert.ToUInt16(code, 16) );
    }

    private void RunInstruction(byte code)
    {
        foreach (var test in currentTests)
        {
            RunFor(code, test);
        }
    }

    private void RunFor(byte code, InstTest test)
    {
        CPU_Registers reg = _gameboy.cpu.registers;

        _gameboy.cpu.isHalted = false;
        _gameboy.cpu.isMasterEnabled = true;
        
        // initial memory and register values
        reg.a = (byte) Convert.ToInt32(test.initial.cpu["a"], 16);
        reg.b = (byte) Convert.ToInt32(test.initial.cpu["b"], 16);
        reg.c = (byte) Convert.ToInt32(test.initial.cpu["c"], 16);
        reg.d = (byte) Convert.ToInt32(test.initial.cpu["d"], 16);
        reg.e = (byte) Convert.ToInt32(test.initial.cpu["e"], 16);
        // _gameboy.cpu.registers.f = byte.Parse(test.initial.cpu["a"]);
        reg.h = (byte) Convert.ToInt32(test.initial.cpu["h"], 16);
        reg.l = (byte) Convert.ToInt32(test.initial.cpu["l"], 16);

        reg.pc = (ushort) Convert.ToInt32(test.initial.cpu["pc"], 16);
        reg.sp = (ushort) Convert.ToInt32(test.initial.cpu["sp"], 16);
        
        byte f = (byte)Convert.ToInt32(test.initial.cpu["f"], 16);
        reg.flagZero = (f & (1 << 7)) != 0;
        reg.flagSubtraction = (f & (1 << 6)) != 0;
        reg.flagHalfCarry = (f & (1 << 5)) != 0;
        reg.flagCarry = (f & (1 << 4)) != 0;

        if(test.name == "c4 da 2a")
            Debug.Log("break");
        
        foreach (string[] memoryValues in test.initial.ram)
        {
            ushort address = Convert.ToUInt16(memoryValues[0], 16);
            byte value = (byte) Convert.ToUInt16(memoryValues[1], 16);
            _gameboy.bus.Write8(address, value);
        }

        int cycles = _gameboy.cpu.DoStep();
        
        // confirm final state is as desired

        if (reg.a != (byte)Convert.ToInt32(test.final.cpu["a"], 16))
            throw new Exception();
        
        if (reg.b != (byte)Convert.ToInt32(test.final.cpu["b"], 16))
            throw new Exception();
        
        if (reg.c != (byte)Convert.ToInt32(test.final.cpu["c"], 16))
            throw new Exception();
        
        if (reg.d != (byte)Convert.ToInt32(test.final.cpu["d"], 16))
            throw new Exception();
        
        if (reg.e != (byte)Convert.ToInt32(test.final.cpu["e"], 16))
            throw new Exception();
        
        if (reg.h != (byte)Convert.ToInt32(test.final.cpu["h"], 16))
            throw new Exception();
        
        if (reg.l != (byte)Convert.ToInt32(test.final.cpu["l"], 16))
            throw new Exception();
        
        if (reg.pc != (ushort)Convert.ToInt32(test.final.cpu["pc"], 16))
            throw new Exception();
        
        if (reg.sp != (ushort)Convert.ToInt32(test.final.cpu["sp"], 16))
            throw new Exception();

        Assert.AreEqual(reg.l, (byte) Convert.ToInt32(test.final.cpu["l"], 16));
        
        foreach (string[] memoryValues in test.final.ram)
        {
            ushort address = Convert.ToUInt16(memoryValues[0], 16);
            byte finalValue = (byte) Convert.ToUInt16(memoryValues[1], 16);
            byte memValue = _gameboy.bus.Read8(address);

            if (finalValue != memValue)
            {
                Assert.IsTrue(false);
            }
        }
        
        byte finalF = (byte)Convert.ToInt32(test.final.cpu["f"], 16);
        bool flagZero = (finalF & (1 << 7)) != 0;
        bool flagSubtraction = (finalF & (1 << 6)) != 0;
        bool flagHalfCarry = (finalF & (1 << 5)) != 0;
        bool flagCarry = (finalF & (1 << 4)) != 0;
        
        Assert.AreEqual(reg.flagZero, flagZero);
        Assert.AreEqual(reg.flagSubtraction, flagSubtraction);
        Assert.AreEqual(reg.flagHalfCarry, flagHalfCarry);
        Assert.AreEqual(reg.flagCarry, flagCarry);
    }
}
