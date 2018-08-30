using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;

namespace BibliotecaMaf.Clases.Audio
{
    public enum eModoDeMuestreo
    {
        eResamplerDmoStream = 1,
        eWaveFormatConversionStream = 2
    }
    public static class ReMuestreo
    {
        public static RawDatosA ResamplerStream(RawDatosA DatosOrigen, RawFormat FormatoDestino)
        {
            System.IO.MemoryStream Datos = null;
            try
            {
                Datos = new System.IO.MemoryStream(DatosOrigen.DatosRaw);
                using (WaveStream StreamOrigen = new NAudio.Wave.RawSourceWaveStream(Datos, new NAudio.Wave.WaveFormat(DatosOrigen.Formato.MuestrasPorSeg, DatosOrigen.Formato.Bits, DatosOrigen.Formato.Canales)))
                {
                    return ResamplerStream(StreamOrigen, new NAudio.Wave.WaveFormat(FormatoDestino.MuestrasPorSeg, FormatoDestino.Bits, FormatoDestino.Canales));
                }
            }
            finally
            {
                if (Datos != null)
                {
                    //Datos.Dispose();
                }
            }
        }
        private static RawDatosA ResamplerStream(WaveStream StreamOrigen, NAudio.Wave.WaveFormat FormatoDestino)
        {
            if (StreamOrigen.Length > 0)
            {
                ResamplerDmoStream Resampler = null;
                WaveFormatConversionStream Resampler2 = null;
                //byte[] BufferResult = new byte[0];
                List<byte> BufferResult = new List<byte>();
                eModoDeMuestreo mModoMuestreo = eModoDeMuestreo.eResamplerDmoStream;
                try
                {
                    StreamOrigen.Position = 0;
                    Resampler = new ResamplerDmoStream(StreamOrigen, FormatoDestino);
                    mModoMuestreo = eModoDeMuestreo.eResamplerDmoStream;
                }
                catch
                {
                    mModoMuestreo = eModoDeMuestreo.eWaveFormatConversionStream;
                }


                if (mModoMuestreo == eModoDeMuestreo.eWaveFormatConversionStream)
                {
                    try
                    {
                        Resampler2 = new WaveFormatConversionStream(FormatoDestino, StreamOrigen);
                        mModoMuestreo = eModoDeMuestreo.eWaveFormatConversionStream;
                    }
                    catch
                    {

                    }
                }

                int Leidos = 0;
                do
                {
                    byte[] Temp = new byte[1024];
                    if (mModoMuestreo == eModoDeMuestreo.eResamplerDmoStream)
                    {
                        Leidos = Resampler.Read(Temp, 0, 1024);
                    }
                    else if (mModoMuestreo == eModoDeMuestreo.eWaveFormatConversionStream)
                    {
                        Leidos = Resampler2.Read(Temp, 0, 1024);
                    }

                    if (Leidos > 0)
                    {
                        //int IndiceCopia = BufferResult.Length;
                        //Array.Resize(ref BufferResult, BufferResult.Length + Leidos);
                        //Array.Copy(Temp, 0, BufferResult, IndiceCopia, Leidos);

                        BufferResult.AddRange(Temp);
                    }
                } while (Leidos > 0);

                int AgregarBytes = 0;
                if (mModoMuestreo == eModoDeMuestreo.eResamplerDmoStream)
                {
                    AgregarBytes = (int)(Resampler.Length - Resampler.Position);
                }
                else if (mModoMuestreo == eModoDeMuestreo.eWaveFormatConversionStream)
                {
                    AgregarBytes = (int)(Resampler2.Length - Resampler2.Position);
                }
                if (AgregarBytes > 0)
                {
                    //Array.Resize(ref BufferResult, BufferResult.Length + AgregarBytes);
                    BufferResult.AddRange(new byte[AgregarBytes]);
                }

                return new RawDatosA(BufferResult.ToArray(), new RawFormat(FormatoDestino.SampleRate, FormatoDestino.BitsPerSample, FormatoDestino.Channels));
            }
            else
            {
                return new RawDatosA(new RawFormat(FormatoDestino.SampleRate, FormatoDestino.BitsPerSample, FormatoDestino.Channels));
            }
        }
    }
}
