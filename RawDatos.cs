using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BibliotecaMaf.Clases.Audio
{
    [Obsolete("En su lugar utilice RawDatosA")]
    public class RawDatos
    {
        public struct Conversacion
        {
            public readonly TimeSpan Fecha;
            public readonly RawDatos Audio;
            public Conversacion(TimeSpan fecha, RawDatos audio)
            {
                Fecha = fecha;
                Audio = audio;
            }
        }
        RawFormat mWaveFormat;
        byte[] mDatosRaw;
        TimeSpan mDuracion;
        long mNumeroDeMuestras;
        public RawDatos(RawFormat WaveFormat)
        {
            mWaveFormat = WaveFormat;
            mDatosRaw = new byte[0];
            RecalcularVariables();


            //mDuracion = new TimeSpan((long)((((double)mDatosRaw.Length) / (mWaveFormat.MuestrasPorSeg * mWaveFormat.BytesPorMuestra * mWaveFormat.Canales)) * 10000000));
            //mNumeroDeMuestras = 0;
        }
        public RawDatos(byte[] DatosAudio, RawFormat WaveFormat)
        {
            mWaveFormat = WaveFormat;
            mDatosRaw = DatosAudio;
            RecalcularVariables();


            //mDuracion = new TimeSpan((long)((((double)mDatosRaw.Length) / (mWaveFormat.MuestrasPorSeg * mWaveFormat.BytesPorMuestra * mWaveFormat.Canales)) * 10000000));
            //mNumeroDeMuestras = DatosAudio.Length / mWaveFormat.Canales / mWaveFormat.BytesPorMuestra;
        }

        public void InsertarRaw(RawDatos Datos)
        {
            InsertarRaw(Datos.DatosRaw);
        }
        public void InsertarRaw(byte[] Datos)
        {
            //byte[] Temp = new byte[mDatosRaw.Length + Datos.Length];
            Array.Resize(ref mDatosRaw, mDatosRaw.Length + Datos.Length);
            Array.Copy(Datos, 0, mDatosRaw, mDatosRaw.Length - Datos.Length, Datos.Length);
            RecalcularVariables();
        }

        private void RecalcularVariables()
        {
            mDuracion = new TimeSpan((long)((((double)mDatosRaw.Length) / (mWaveFormat.MuestrasPorSeg * mWaveFormat.BytesPorMuestra * mWaveFormat.Canales)) * 10000000));
            mNumeroDeMuestras = mDatosRaw.Length / mWaveFormat.Canales / mWaveFormat.BytesPorMuestra;
        }
        public long NumeroDeMuestras
        {
            get
            {
                return mNumeroDeMuestras;
            }
        }
        public byte[] DatosRaw
        {
            get
            {
                return mDatosRaw;
            }
        }
        public RawFormat Formato
        {
            get
            {
                return mWaveFormat;
            }
        }
        public TimeSpan Duracion
        {
            get
            {
                return mDuracion;
            }
        }
        public RawDatos GetRawDatos(int mSegOffset, int mSegDuracion = 0)
        {
            return new RawDatos(GetRawBytes(mSegOffset, mSegDuracion), mWaveFormat);
        }

        public byte[] GetRawBytes(int mSegOffset, int mSegDuracion = 0)
        {
            int index = (int)((mSegOffset / 1000d) * mWaveFormat.MuestrasPorSeg * mWaveFormat.Canales * mWaveFormat.BytesPorMuestra);
            int Durac = (int)((mSegDuracion / 1000d) * mWaveFormat.MuestrasPorSeg * mWaveFormat.Canales * mWaveFormat.BytesPorMuestra);

            index = (int)(Math.Truncate(((double)index) / (mWaveFormat.Canales * mWaveFormat.BytesPorMuestra)) * (mWaveFormat.Canales * mWaveFormat.BytesPorMuestra));

            if (index > mDatosRaw.Length - 1)
            {
                return new byte[0];
            }
            else
            {
                byte[] Resul = new byte[0];
                if ((index + Durac) > mDatosRaw.Length)
                {
                    Resul = new byte[mDatosRaw.Length - index - 1];
                    Array.Copy(mDatosRaw, index, Resul, 0, mDatosRaw.Length - index);
                }
                else
                {
                    Resul = new byte[0];
                    if (Durac == 0)
                    {
                        Resul = new byte[mDatosRaw.Length - index];
                        Array.Copy(mDatosRaw, index, Resul, 0, mDatosRaw.Length - index);
                    }
                    else
                    {
                        Resul = new byte[Durac];
                        Array.Copy(mDatosRaw, index, Resul, 0, Durac);
                    }
                }
                return Resul;
            }
        }

        public byte[] GetRawBytes(long start, long lenght)
        { 
            if (start > mDatosRaw.Length - 1)
            {
                return new byte[0];
            }
            else
            {
                byte[] Resul = new byte[0];
                if ((start + lenght) > mDatosRaw.Length)
                {
                    Resul = new byte[mDatosRaw.Length - start];
                    Array.Copy(mDatosRaw, start, Resul, 0, mDatosRaw.Length - start);
                }
                else
                {
                    Resul = new byte[0];
                    if (lenght == 0)
                    {
                        Resul = new byte[mDatosRaw.Length - start];
                        Array.Copy(mDatosRaw, start, Resul, 0, mDatosRaw.Length - start);
                    }
                    else
                    {
                        Resul = new byte[lenght];
                        Array.Copy(mDatosRaw, start, Resul, 0, lenght);
                    }
                }
                return Resul;
            }
        }

            /// <summary>
            /// Devuelve fragmentos de audio separados en conversaciones en base a un nivel umbral
            /// </summary>
            /// <param name="levelTrigger">Nivel a partir del cual se considera el umbral 0 - 100%</param>
            /// <returns>Devuelve fragmentos de audio separados en conversaciones en base a un nivel umbral</returns>
            public List<Conversacion> GetConversacionesMono(short levelTrigger)
        {
            List<Conversacion> mResult = new List<Conversacion>();
            int mStart = 0;
            short mValorMuestra = 0;
            int Paso = mWaveFormat.BytesPorMuestra;
            short mTrigger = 0;

            if (Formato.Bits == 8)
            {
                mTrigger = (short)(levelTrigger * 127 / 100);
            }
            else
            {
                mTrigger = (short)(levelTrigger * 32767 / 100);
            }

            switch (mWaveFormat.BytesPorMuestra)
            {
                case 1:
                    for (int i = mStart; i < mNumeroDeMuestras; i++)
                    {
                        mValorMuestra = mDatosRaw[(i * Paso)];
                    }

                    break;
                case 2:
                    for (int i = mStart; i < mNumeroDeMuestras; i++)
                    {
                        mValorMuestra = Utilidades.AbsSample((short)((mDatosRaw[(i * Paso) + 1] * 256) + mDatosRaw[(i * Paso)]), true);
                        if (mValorMuestra > mTrigger)
                        {
                            for (int k = i; k < mNumeroDeMuestras; k++)
                            {
                                mValorMuestra = Utilidades.AbsSample((short)((mDatosRaw[(k * Paso) + 1] * 256) + mDatosRaw[(k * Paso)]), true);
                                if (mValorMuestra < mTrigger)
                                {
                                    bool mResul = false;
                                    for (int m = k; m < mNumeroDeMuestras; m++)
                                    {
                                        mValorMuestra = Utilidades.AbsSample((short)((mDatosRaw[(m * Paso) + 1] * 256) + mDatosRaw[(m * Paso)]), true);
                                        if (mValorMuestra < mTrigger)
                                        {
                                            if ((m - (3 * mWaveFormat.MuestrasPorSeg)) > k)
                                            {
                                                mResult.Add(new Conversacion(new TimeSpan(0), new RawDatos(GetRawBytes((long)i * 2, (k + (1 * mWaveFormat.BytesPorSeg))), mWaveFormat)));
                                                i = m;
                                                mResul = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            k = m;
                                            break;
                                        }
                                    }
                                    if (mResul)
                                    {
                                        break;
                                    }
                                }
                                else
                                {

                                }
                            }
                        }
                        else
                        {

                        }
                    }
                    break;
            }
            return mResult;
        }


        /// <summary>
        /// Obtiene el valor de la muestra del canal Izquierdo
        /// </summary>
        /// <param name="NumeroDeMuestra">Numero de la muestra necesaria con base 0</param>
        /// <returns>Retorna el valor de la muestra de -32768 a 32767 en 16bit y de 0 a 255 en 8bit</returns>
        public short GetValorMuestraIzquierda(long NumeroDeMuestra)
        {
            short Valor = 0;
            int Paso = mWaveFormat.BytesPorMuestra * 2;
            switch (mWaveFormat.BytesPorMuestra)
            {
                case 1:
                    Valor = mDatosRaw[NumeroDeMuestra * Paso];
                    break;
                case 2:
                    Valor = (short)(((mDatosRaw[(NumeroDeMuestra * Paso) + 1]) * 256) + mDatosRaw[(NumeroDeMuestra * Paso)]);
                    break;
            }
            return Valor;
        }

        /// <summary>
        /// Obtiene el valor de la muestra del canal Derecho
        /// </summary>
        /// <param name="NumeroDeMuestra">Numero de la muestra necesaria, con base 0</param>
        /// <returns>Retorna el valor de la muestra de -32768 a 32767 en 16bit y de 0 a 255 en 8bit</returns>
        public short GetValorMuestraDerecha(long NumeroDeMuestra)
        {
            short Valor = 0;
            int Paso = mWaveFormat.BytesPorMuestra * 2;
            switch (mWaveFormat.BytesPorMuestra)
            {
                case 1:
                    Valor = mDatosRaw[(NumeroDeMuestra * Paso) + 1];
                    break;
                case 2:
                    Valor = (short)((mDatosRaw[(NumeroDeMuestra * Paso) + 3] * 256) + mDatosRaw[(NumeroDeMuestra * Paso) + 2]);
                    break;
            }
            return Valor;
        }

        /// <summary>
        /// Obtiene el valor de la muestra del canal en representacion de complemento de a dos
        /// </summary>
        /// <param name="NumeroDeMuestra">Numero de la muestra necesaria, con base 0</param>
        /// <returns>Retorna el valor de la muestra de -32768 a 32767 en 16bit y de 0 a 255 en 8bit</returns>
        public short GetValorMuestraMono(long NumeroDeMuestra)
        {
            if (mWaveFormat.Canales > 1)
            {
                throw new Exception("El formato debe ser mono");
            }
            short Valor = 0;
            int Paso = mWaveFormat.BytesPorMuestra;
            switch (mWaveFormat.BytesPorMuestra)
            {
                case 1:
                    Valor = mDatosRaw[(NumeroDeMuestra * Paso)];
                    break;
                case 2:
                    Valor = (short)((mDatosRaw[(NumeroDeMuestra * Paso) + 1] * 256) + mDatosRaw[(NumeroDeMuestra * Paso)]);
                    break;
            }
            return Valor;
        }
    }
}
