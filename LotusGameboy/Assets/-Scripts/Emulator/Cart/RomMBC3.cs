using System;
using Lotus.GameboyEmulator;

namespace Emulator.Cart
{
    public class RealTimeClock
    {
        public byte s;
        public byte m;
        public byte h;
        public byte dl;
        public byte dh;
        public Int64 zero;
        public string savePath;
    }
    
    public class RomMBC3 : CartridgeRom
    {
        private const int ROM_OFFSET = 0x4000;
        private const int ERAM_OFFSET = 0x2000;
        
        // external ram
        // When MBC3 is set to maximize the eram space,
        // the max value is 0x8000
        // using 4 banks
        private byte[] _eram = new byte[0x8000];  
        
        private int _romBank;

        private int _ramBank;

        private bool _isEramEnabled;

        // not a bool
        private RealTimeClock _realTimeClock;
        
        // TODO: mbc3 has battery to save games..
        
        
        public override byte ReadLowRom(ushort address)
        {
            return _loadedRom[address];
        }

        public override byte ReadHighRom(ushort address)
        {
            return _loadedRom[address];
        }

        public override void WriteRom(ushort address, byte value)
        {
            // MBC 0 doesn't support writing to ROM 
            // Debug.LogError("fail");
            if(_testMode)
                _loadedRom[address] = value;
        }

        public override byte ReadERam(ushort address)
        {
            if (_isEramEnabled)
            {
                if (_ramBank >= 0 && _ramBank <= 3)
                {
                    return _eram[ERAM_OFFSET * _ramBank + address & 0x1FFF];
                }
            }
            

            return 0XFF;
        }

        public override void WriteERam(ushort address, byte value)
        {
            // Debug.LogError("fail");
            // MBC 0 doesn't support ERam

            if (_testMode)
            {
                _loadedRom[address] = value;    
            }
        }
    }
}