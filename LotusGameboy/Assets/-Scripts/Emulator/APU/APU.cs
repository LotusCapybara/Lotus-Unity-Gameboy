using UnityGB;

namespace Lotus.GameboyEmulator.APU
{
	/// <summary>
	/// The audio was the part that I knew I was going to struggle a bit, so I ended up checking code from different
	/// github repositories..
	/// It's a bit of a mix from everywhere
	/// </summary>
    public class APU
    {
	    private int[] _soundRegisters = new int[0x30];

	    private SquareWaveGenerator _channel1;
	    private SquareWaveGenerator _channel2;
	    private VoluntaryWaveGenerator _channel3;
	    private NoiseGenerator _channel4;
	    private bool _soundEnabled = true;

	    private bool _channel1Enable = true;
	    private bool _channel2Enable = true;
	    private bool _channel3Enable = true;
	    private bool _channel4Enable = true;

	    private int _sampleRate = 44100;

	    public APU()
	    {
		    _channel1 = new SquareWaveGenerator(_sampleRate);
		    _channel2 = new SquareWaveGenerator(_sampleRate);
		    _channel3 = new VoluntaryWaveGenerator(_sampleRate);
		    _channel4 = new NoiseGenerator(_sampleRate);
	    }

	    public void Update()
	    {
		    if (_soundEnabled)
			    return;

		    int numChannels = 2; // Always stereo for Game Boy
		    int numSamples = AudioComponent.instance.GetSamplesAvailable();

		    byte[] b = new byte[numChannels * numSamples];

		    if (_channel1Enable)
			    _channel1.Play(b, numSamples, numChannels);
		    if (_channel2Enable)
			    _channel2.Play(b, numSamples, numChannels);
		    if (_channel3Enable)
			    _channel3.Play(b, numSamples, numChannels);
		    if (_channel4Enable)
			    _channel4.Play(b, numSamples, numChannels);

		    AudioComponent.instance.Play(b);
	    }
        

        public byte Read(ushort address)
        {
	        return (byte) _soundRegisters [address - 0xFF10];
        }
        
        public void Write(ushort address, byte value)
        {
	        switch (address)
	        {
		        case 0xFF10:           // Sound channel 1, sweep
						_channel1.SetSweep(
							(value & 0x70) >> 4,
							(value & 0x07),
							(value & 0x08) == 1);
						_soundRegisters [0x10 - 0x10] = value;
						break;

					case 0xFF11:           // Sound channel 1, length and wave duty
						_channel1.SetDutyCycle((value & 0xC0) >> 6);
						_channel1.SetLength(value & 0x3F);
						_soundRegisters [0x11 - 0x10] = value;
						break;

					case 0xFF12:           // Sound channel 1, volume envelope
						_channel1.SetEnvelope(
							(value & 0xF0) >> 4,
							(value & 0x07),
							(value & 0x08) == 8);
						_soundRegisters [0x12 - 0x10] = value;
						break;

					case 0xFF13:           // Sound channel 1, frequency low
						_soundRegisters [0x13 - 0x10] = value;
						_channel1.SetFrequency(
						((_soundRegisters [0x14 - 0x10] & 0x07) << 8) + _soundRegisters [0x13 - 0x10]);
						break;

					case 0xFF14:           // Sound channel 1, frequency high
						_soundRegisters [0x14 - 0x10] = value;

						if ((_soundRegisters [0x14 - 0x10] & 0x80) != 0)
						{
							_channel1.SetLength(_soundRegisters [0x11 - 0x10] & 0x3F);
							_channel1.SetEnvelope(
								(_soundRegisters [0x12 - 0x10] & 0xF0) >> 4,
								(_soundRegisters [0x12 - 0x10] & 0x07),
								(_soundRegisters [0x12 - 0x10] & 0x08) == 8);
						}
						if ((_soundRegisters [0x14 - 0x10] & 0x40) == 0)
						{
							_channel1.SetLength(-1);
						}

						_channel1.SetFrequency(
							((_soundRegisters [0x14 - 0x10] & 0x07) << 8) + _soundRegisters [0x13 - 0x10]);

						break;

					case 0xFF17:           // Sound channel 2, volume envelope
						_channel2.SetEnvelope(
							(value & 0xF0) >> 4,
							value & 0x07,
							(value & 0x08) == 8);
						_soundRegisters [0x17 - 0x10] = value;
						break;

					case 0xFF18:           // Sound channel 2, frequency low
						_soundRegisters [0x18 - 0x10] = value;
						_channel2.SetFrequency(
							((_soundRegisters [0x19 - 0x10] & 0x07) << 8) + _soundRegisters [0x18 - 0x10]);
						break;

					case 0xFF19:           // Sound channel 2, frequency high
						_soundRegisters [0x19 - 0x10] = value;

						if ((value & 0x80) != 0)
						{
							_channel2.SetLength(_soundRegisters [0x21 - 0x10] & 0x3F);
							_channel2.SetEnvelope(
								(_soundRegisters [0x17 - 0x10] & 0xF0) >> 4,
								(_soundRegisters [0x17 - 0x10] & 0x07),
								(_soundRegisters [0x17 - 0x10] & 0x08) == 8);
						}
						if ((_soundRegisters [0x19 - 0x10] & 0x40) == 0)
						{
							_channel2.SetLength(-1);
						}
						_channel2.SetFrequency(
							((_soundRegisters [0x19 - 0x10] & 0x07) << 8) + _soundRegisters [0x18 - 0x10]);
						break;

					case 0xFF16:           // Sound channel 2, length and wave duty
						_channel2.SetDutyCycle((value & 0xC0) >> 6);
						_channel2.SetLength(value & 0x3F);
						_soundRegisters [0x16 - 0x10] = value;
						break;

					case 0xFF1A:           // Sound channel 3, on/off
						if ((value & 0x80) != 0)
						{
							_channel3.SetVolume((_soundRegisters [0x1C - 0x10] & 0x60) >> 5);
						} else
						{
							_channel3.SetVolume(0);
						}
						_soundRegisters [0x1A - 0x10] = value;
						break;

					case 0xFF1B:           // Sound channel 3, length
						_soundRegisters [0x1B - 0x10] = value;
						_channel3.SetLength(value);
						break;

					case 0xFF1C:           // Sound channel 3, volume
						_soundRegisters [0x1C - 0x10] = value;
						_channel3.SetVolume((value & 0x60) >> 5);
						break;

					case 0xFF1D:           // Sound channel 3, frequency lower 8-bit
						_soundRegisters [0x1D - 0x10] = value;
						_channel3.SetFrequency(
						((_soundRegisters [0x1E - 0x10] & 0x07) << 8) + _soundRegisters [0x1D - 0x10]);
						break;

					case 0xFF1E:           // Sound channel 3, frequency higher 3-bit
						_soundRegisters [0x1E - 0x10] = value;
						if ((_soundRegisters [0x19 - 0x10] & 0x80) != 0)
						{
							_channel3.SetLength(_soundRegisters [0x1B - 0x10]);
						}
						_channel3.SetFrequency(
							((_soundRegisters [0x1E - 0x10] & 0x07) << 8) + _soundRegisters [0x1D - 0x10]);
						break;

					case 0xFF20:           // Sound channel 4, length
						_channel4.SetLength(value & 0x3F);
						_soundRegisters [0x20 - 0x10] = value;
						break;


					case 0xFF21:           // Sound channel 4, volume envelope
						_channel4.SetEnvelope(
						(value & 0xF0) >> 4,
						(value & 0x07),
						(value & 0x08) == 8);
						_soundRegisters [0x21 - 0x10] = value;
						break;

					case 0xFF22:           // Sound channel 4, polynomial parameters
						_channel4.SetParameters(
						(value & 0x07),
						(value & 0x08) == 8,
						(value & 0xF0) >> 4);
						_soundRegisters [0x22 - 0x10] = value;
						break;

					case 0xFF23:          // Sound channel 4, initial/consecutive
						_soundRegisters [0x23 - 0x10] = value;
						if ((value & 0x80) != 0)
						{
							_channel4.SetLength(_soundRegisters [0x20 - 0x10] & 0x3F);
						} else if (((value & 0x80) & 0x40) == 0)
						{
							_channel4.SetLength(-1);
						}
						break;
					case 0xFF24:
						// TODO volume
						break;
					case 0xFF25:           // Stereo select
						int chanData;
						_soundRegisters [0x25 - 0x10] = value;

						chanData = 0;
						if ((value & 0x01) != 0)
						{
							chanData |= SquareWaveGenerator.CHAN_LEFT;
						}
						if ((value & 0x10) != 0)
						{
							chanData |= SquareWaveGenerator.CHAN_RIGHT;
						}
						_channel1.SetChannel(chanData);

						chanData = 0;
						if ((value & 0x02) != 0)
						{
							chanData |= SquareWaveGenerator.CHAN_LEFT;
						}
						if ((value & 0x20) != 0)
						{
							chanData |= SquareWaveGenerator.CHAN_RIGHT;
						}
						_channel2.SetChannel(chanData);

						chanData = 0;
						if ((value & 0x04) != 0)
						{
							chanData |= VoluntaryWaveGenerator.CHAN_LEFT;
						}
						if ((value & 0x40) != 0)
						{
							chanData |= VoluntaryWaveGenerator.CHAN_RIGHT;
						}
						_channel3.SetChannel(chanData);

						chanData = 0;
						if ((value & 0x08) != 0)
						{
							chanData |= NoiseGenerator.CHAN_LEFT;
						}
						if ((value & 0x80) != 0)
						{
							chanData |= NoiseGenerator.CHAN_RIGHT;
						}
						_channel4.SetChannel(chanData);

						break;
					case 0xFF26:
						_soundEnabled = (value & 0x80) == 1;
						break;
					case 0xFF30:
					case 0xFF31:
					case 0xFF32:
					case 0xFF33:
					case 0xFF34:
					case 0xFF35:
					case 0xFF36:
					case 0xFF37:
					case 0xFF38:
					case 0xFF39:
					case 0xFF3A:
					case 0xFF3B:
					case 0xFF3C:
					case 0xFF3D:
					case 0xFF3E:
					case 0xFF3F:
						_channel3.SetSamplePair(address - 0xFF30, value);
						_soundRegisters [address - 0xFF10] = value;
						break;
	        }
        }
    }
}