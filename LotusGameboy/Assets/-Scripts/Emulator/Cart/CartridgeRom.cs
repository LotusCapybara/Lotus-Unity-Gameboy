namespace Lotus.GameboyEmulator
{
    
    /// <summary>
    /// CartridgeRom.
    /// Abstract class for the implementation of a Cart ROM
    /// there are different MBCs with different formats and specifications.
    /// I'm not supporting many and I don't know when I'll have some time to support the rest.
    /// If I check only for code implementing it alongside to the specifications and documentation, it might
    /// not take that much time.. but we'll see
    /// </summary>
    public abstract class CartridgeRom
    {
        protected byte[] _loadedRom;

        protected bool _testMode;

        public void Load(byte[] fullRom, bool testMode)
        {
            _testMode = testMode;
            _loadedRom = fullRom;
        }

        public abstract byte ReadLowRom(ushort address);
        public abstract byte ReadHighRom(ushort address);
        public abstract void WriteRom(ushort address, byte value);
        public abstract byte ReadERam(ushort address);
        public abstract void WriteERam(ushort address, byte value);
    }
}