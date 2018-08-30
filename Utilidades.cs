using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NAudio.FileFormats.Mp3;
using NAudio.Lame;
using NAudio.Wave;
using System.Windows.Forms;

namespace BibliotecaMaf.Clases.Audio
{
    /// <summary>
    /// Utilidades de ayuda para trabajar con audio
    /// </summary>
    public static class Utilidades
    {
        /// <summary>
        /// Genera una onda con el sonido BEEP cada 1 Segundo del largo especificado, el primer Beep es triple
        /// </summary>
        /// <param name="Segundos">Cantidad de segundos generada</param>
        /// <returns>RawDatos con la onda generada</returns>
        public static RawDatosA GenerarBeepInicial(int Segundos)
        {
            if (Segundos > 0)
            {
                byte[] B = Resources.beep3;
                byte[] B2 = Resources.beep;
                using (WaveStream mPcmStream = new WaveFileReader(new MemoryStream(B)))
                {
                    using (WaveStream mPcmStream2 = new WaveFileReader(new MemoryStream(B2)))
                    {
                        byte[] Buf = new byte[mPcmStream.Length];
                        mPcmStream.Read(Buf, 0, Buf.Length);
                        for (int r = 0; r < Segundos - 1; r++)
                        {
                            int tamañoAnterior = Buf.Length;
                            Array.Resize(ref Buf, (int)(Buf.Length + mPcmStream2.Length));
                            mPcmStream2.Position = 0;
                            mPcmStream2.Read(Buf, tamañoAnterior, (int)mPcmStream2.Length);
                        }
                        return new RawDatosA(Buf, new RawFormat(48000, 16, 1));
                    }
                }
            }
            else
            {
                return new RawDatosA(new byte[0], new RawFormat(48000, 16, 1));
            }
        }

        /// <summary>
        /// Genera una onda con el sonido BEEP cada 1 Segundo del largo especificado
        /// </summary>
        /// <param name="Segundos">Cantidad de segundos generada</param>
        /// <returns>RawDatos con la onda generada</returns>
        public static RawDatosA GenerarBeep(int Segundos)
        {
            if (Segundos > 0)
            {
                byte[] B = Resources.beep;
                using (WaveStream mPcmStream = new WaveFileReader(new MemoryStream(B)))
                {
                    byte[] Buf = new byte[0];
                    for (int r = 0; r < Segundos; r++)
                    {
                        int tamañoAnterior = Buf.Length;
                        Array.Resize(ref Buf, (int)(Buf.Length + mPcmStream.Length));
                        mPcmStream.Position = 0;
                        mPcmStream.Read(Buf, tamañoAnterior, (int)mPcmStream.Length);
                    }
                    return new RawDatosA(Buf, new RawFormat(48000, 16, 1));
                }
            }
            else
            {
                return new RawDatosA(new byte[0], new RawFormat(48000, 16, 1));
            }
        }

        public static RawDatosA GenerarSilencio(int mSeg)
        {
            if (mSeg > 0)
            {
                return new RawDatosA(new byte[96 * mSeg], new RawFormat(48000, 16, 1));
            }
            else
            {
                return new RawDatosA(new RawFormat(48000, 16, 1));
            }
        }

        /// <summary>
        ///	Transfoma un archivo MP3 a "RawBytes"
        /// </summary>
        /// <param name="PathMP3">Path donde esta ubicado el archivo MP3</param>
        /// <returns>RawDatos con la informacion del archivo MP3</returns>
        public static RawDatosA Mp3ArchivoToRawBytes(string PathMP3)
        {
            try
            {
                using (Mp3FileReader mReader = new Mp3FileReader(PathMP3))
                using (WaveStream mPcmStream = WaveFormatConversionStream.CreatePcmStream(mReader))
                {
                    try
                    {
                        byte[] mRetorno = new byte[mPcmStream.Length];
                        int mLeido = mPcmStream.Read(mRetorno, 0, mRetorno.Length);
                        return new RawDatosA(mRetorno, new RawFormat(mPcmStream.WaveFormat.SampleRate, mPcmStream.WaveFormat.BitsPerSample, mPcmStream.WaveFormat.Channels));
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {

            }

        }

        /// <summary>
        /// Lee un Stream en MP3 y devuelve el resultado en RawDatos
        /// Hay que tener en cuenta que esta funcion lee el stream completo hasta el fin.
        /// </summary>
        /// <param name="streamMp3">Stream donde se encuentran los Frames MP3</param>
        /// <returns>Devuelve una objeto RawDatos con la informacion PCM convertida</returns>
        public static RawDatosA Mp3StreamToRawBytes(System.IO.Stream streamMp3)
        {
            try
            {
                using (Mp3FileReader mReader = new Mp3FileReader(streamMp3))
                using (WaveStream mPcmStream = WaveFormatConversionStream.CreatePcmStream(mReader))
                {
                    try
                    {
                        byte[] mRetorno = new byte[mPcmStream.Length];
                        int mLeido = mPcmStream.Read(mRetorno, 0, mRetorno.Length);
                        return new RawDatosA(mRetorno, new RawFormat(mPcmStream.WaveFormat.SampleRate, mPcmStream.WaveFormat.BitsPerSample, mPcmStream.WaveFormat.Channels));
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static int Mp3BytesStreamingToRawBytes(byte[] mp3, ref RawDatosA datos, ref IMp3FrameDecompressor decompressor)
        {

            List<byte> mDatosConvertidos = new List<byte>();
            MemoryStream M = new MemoryStream(mp3);
            WaveFormat waveFormat = null;
            int mBytesProcesed = 0;
            do
            {
                NAudio.Wave.Mp3Frame frame = null;
                try
                {
                    frame = Mp3Frame.LoadFromStream(M);
                }
                catch (System.IO.EndOfStreamException)
                {

                }
                catch (Exception)
                {
                    throw;
                }

                if (frame != null)
                {
                    var buffer = new byte[65536]; // needs to be big enough to hold a decompressed frame

                    if (decompressor == null)
                    {
                        waveFormat = new Mp3WaveFormat(frame.SampleRate, frame.ChannelMode == ChannelMode.Mono ? 1 : 2, frame.FrameLength, frame.BitRate);
                        decompressor = new DmoMp3FrameDecompressor(waveFormat);
                    }
                    int decompressed = decompressor.DecompressFrame(frame, buffer, 0);
                    mBytesProcesed = mBytesProcesed + frame.FrameLength;
                    Array.Resize(ref buffer, decompressed);
                    mDatosConvertidos.AddRange(buffer);
                }
            } while (M.Position < M.Length);

            datos = new RawDatosA(new RawFormat(decompressor.OutputFormat.SampleRate, decompressor.OutputFormat.BitsPerSample, decompressor.OutputFormat.Channels));
            datos.InsertarRaw(mDatosConvertidos.ToArray());
            return mBytesProcesed;
        }


        /// <summary>
        /// Devuelve el valor promedio de volumen de 0 - 100% en una ubicacion dada.
        /// </summary>
        /// <param name="PathArchivo">Path donde esta ubicado el archivo MP3</param>
        /// <param name="OffsetmSeg">Offset en mseg dentro del archivo donde se comienza el muestreo</param>
        /// <param name="CantMuestras">Cantidad de muestras dentro del calculo</param>
        /// <returns>Un valor de 0-100%</returns>
        public static byte ValorPorcentualDelVolumen(string PathArchivo, long OffsetmSeg, int CantMuestras)
        {
            return ValorPorcentualDelVolumen(Mp3ArchivoToRawBytes(PathArchivo), OffsetmSeg, CantMuestras);
        }

        /// <summary>
        /// Devuelve el valor promedio de volumen de 0 - 100% en una ubicacion dada.
        /// </summary>
        /// <param name="audio">Datos de audio "RawDatos"</param>
        /// <param name="offsetmSeg">Offset en mseg dentro del archivo donde se comienza el muestreo</param>
        /// <param name="cantMuestras">Cantidad de muestras dentro del calculo</param>
        /// <returns>Un valor de 0-100%</returns>
        public static byte ValorPorcentualDelVolumen(RawDatosA audio, long offsetmSeg, int cantMuestras)
        {
            if (offsetmSeg < 0 || cantMuestras < 0)
            {
                throw new Exception();
            }
            long NumeroMuestraInicio = (offsetmSeg * audio.Formato.MuestrasPorSeg / 1000);

            return ValorPromedioMaximoDelVolumen(audio, NumeroMuestraInicio, cantMuestras, true, (long)0);
        }

        /// <summary>
        /// Devuelve el valor Maximo de Volumen de 0 - 100% en una ubicacion determinada hasta la duracion indicada distribuyendo las muestras de forma proporcional.
        /// </summary>
        /// <param name="audio">Datos de audio "RawDatos"</param>
        /// <param name="offsetmSeg">Offset en mseg dentro del archivo donde se comienza el muestreo</param>
        /// <param name="duracion">La duracion del muestreo en mSeg</param>
        /// <param name="cantMuestras">Cantidad de muestras dentro del calculo, si es 0 se calculan todas las muestras</param>
        /// <returns>Un valor de 0-100%</returns>
        public static byte ValorMaximoDelVolumen(RawDatosA audio, long offsetmSeg, long duracionMseg, int cantMuestras)
        {
            if (offsetmSeg < 0 || cantMuestras < 0 || duracionMseg < 0)
            {
                throw new Exception();
            }

            long NumeroMuestraInicio = (offsetmSeg * audio.Formato.MuestrasPorSeg / 1000);
            long Duracion = (duracionMseg * audio.Formato.MuestrasPorSeg / 1000);

            return ValorPromedioMaximoDelVolumen(audio, NumeroMuestraInicio, Duracion, false, (long)cantMuestras);
        }

        /// <summary>
        /// Devuelve el valor Maximo o Promedio de Volumen de 0 - 100% en una ubicacion determinada hasta la duracion indicada distribuyendo las muestras de forma proporcional.
        /// Si el audio es stereo se devuelve el valor Maximo o Promedio del promedio de los dos canales.
        /// </summary>
        /// <param name="audio">Datos de audio "RawDatos"</param>
        /// <param name="sampleStart">Offset dentro del archivo donde se comienza el muestreo,
        /// el paso es de a muestras completas</param>
        /// <param name="sampleLenght">La cantidad de muestras</param>
        /// <param name="returnPromedio">Si es false devuelve el valor Maximo(pico) de volumen, si es true devuelve el promedio de volumen</param>
        /// <param name="sampleCount">Opcional, la cantidad de muestras que se quieren en vez de comprobar el audio completo, si es 0 se comprueba todo el audio</param>
        /// <returns>Un valor de 0-100% que corresponde al valor Maximo o Promedio del audio</returns>
        public static byte ValorPromedioMaximoDelVolumen(RawDatosA audio, long sampleStart, long sampleLenght, bool returnPromedio, long sampleCount = 0)
        {
            if (sampleStart < 0 || sampleLenght < 0 || sampleCount < 0)
            {
                throw new Exception();
            }

            if (audio.DatosRaw.Length == 0 || sampleStart > audio.NumeroDeMuestras - 1)
            {
                return 0;
            }

            if ((sampleLenght + sampleStart) > (audio.NumeroDeMuestras))
            {
                sampleLenght = audio.NumeroDeMuestras - sampleStart;
            }

            bool is16BitSample = false;
            if (audio.Formato.Bits == 16)
            {
                is16BitSample = true;
            }
            int mStep = 0;
            if (sampleCount > 0)
            {
                mStep = (int)((sampleLenght - sampleStart) / sampleCount);
            }

            short mValorMax = 0;
            long mValorPromedio = 0;
            int mIteracciones = 0;
            for (long i = sampleStart; i < (sampleStart + sampleLenght); i++)
            {
                short ValorMuestra = 0;
                if (audio.Formato.Canales == 1)
                {
                    ValorMuestra = AbsSample(audio.GetValorMuestraMono(i), is16BitSample);
                }
                else
                {
                    ValorMuestra = AbsSample(audio.GetValorMuestraIzquierda(i), is16BitSample);
                    short ValorMuestra2 = AbsSample(audio.GetValorMuestraDerecha(i), is16BitSample);
                    ValorMuestra = (short)((ValorMuestra + ValorMuestra2) / 2);
                }

                if (ValorMuestra > mValorMax)
                {
                    mValorMax = ValorMuestra;
                }
                mValorPromedio = mValorPromedio + ValorMuestra;
                mIteracciones++;
                if (mStep > 0)
                {
                    i = i + mStep - 1;
                }
            }

            if (is16BitSample)
            {
                if (returnPromedio)
                {
                    return (byte)((mValorPromedio / mIteracciones) * 100 / 32767);
                }
                else
                {
                    return (byte)(mValorMax * 100 / 32767);
                }
            }
            else
            {
                if (returnPromedio)
                {
                    return (byte)((mValorPromedio / mIteracciones) * 100 / 127);
                }
                else
                {
                    return (byte)(mValorMax * 100 / 127);
                }
            }
        }
        /// <summary>
        /// Devuelve el valor pico absoluto de una muestra, si es de 8bit devuelve de 0-127 y si es de 16bit devuelve 0-32767
        /// </summary>
        /// <param name="valorMuestra">Valor al que se le quiere sacar el valor absoluto</param>
        /// <param name="is16BitSample">es true si la muestra es de 16bits</param>
        /// <returns> Si es de 8bit devuelve de 0-127 y si es de 16bit devuelve 0-32767</returns>
        public static short AbsSample(short valorMuestra, bool is16BitSample)
        {
            short ret = 0;
            ret = valorMuestra;
            if (is16BitSample)
            {
                if (valorMuestra < 0)
                {
                    if (valorMuestra > -32768)
                    {
                        ret = (short)(valorMuestra * -1);
                    }
                    else
                    {
                        ret = 32767;
                    }
                }
            }
            else
            {
                if (valorMuestra > 127)
                {
                    if (valorMuestra == 255)
                    {
                        valorMuestra = 254;
                    }
                    ret = (short)(valorMuestra - 127);
                }
                else
                {
                    ret = (short)(127 - valorMuestra);
                }
            }
            return ret;
        }


        /// <summary>
        /// Devuelve un valor Double que indica la duracion del archivo en mSeg
        /// </summary>
        /// <param name="PathArchivo">Path donde esta ubicado el archivo MP3</param>
        /// <returns>Duracion del archivo en mSeg</returns>
        public static double DuracionmSegMp3(string PathArchivo)
        {
            RawDatosA Audio = Mp3ArchivoToRawBytes(PathArchivo);
            return Audio.Duracion.TotalMilliseconds;
        }

        /// <summary>
        /// Genera el Riff Header de una secuencia "RawData"
        /// </summary>
        /// <param name="RawDataLength">Tamaño de los datos en bytes dela secuencia "RawData" de la cual se genera el encabezado</param>
        /// <param name="RawFormat">Formato de la secuencia (bits, canales, muestrasPorSeg)</param>
        /// <returns></returns>
        public static byte[] WaveHeader(long RawDataLength, RawFormat RawFormat)
        {
            byte[] Temp = new byte[0];
            System.IO.MemoryStream S = null;
            System.IO.BinaryWriter Bw = null;
            try
            {
                S = new System.IO.MemoryStream();
                Bw = new System.IO.BinaryWriter(S);

                Bw.Write(new char[4] { 'R', 'I', 'F', 'F' });
                Bw.Write((int)(RawDataLength + 36));
                Bw.Write(new char[8] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
                Bw.Write((int)16);
                Bw.Write((short)1);
                Bw.Write((short)RawFormat.Canales);
                Bw.Write((int)RawFormat.MuestrasPorSeg);
                Bw.Write((int)(RawFormat.MuestrasPorSeg * ((RawFormat.BytesPorMuestra * RawFormat.Canales))));
                Bw.Write((short)((RawFormat.BytesPorMuestra * RawFormat.Canales)));
                Bw.Write((short)RawFormat.Bits);
                Bw.Write(new char[4] { 'd', 'a', 't', 'a' });
                Bw.Write((int)RawDataLength);

                Temp = new byte[S.Length];
                S.Position = 0;
                S.Read(Temp, 0, (int)S.Length);
            }
            finally
            {
                if (Bw != null)
                {
                    Bw.Close();
                }
            }
            return Temp;
        }
    }
}
