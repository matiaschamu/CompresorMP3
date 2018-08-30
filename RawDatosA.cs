using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BibliotecaMaf.Clases.Audio
{
    public class RawDatosA
    {
        public struct Conversacion
        {
            public readonly TimeSpan Posicion;
            public readonly RawDatosA Audio;
            public readonly long Start;
            public readonly long Stop;

            public Conversacion(TimeSpan posicion, RawDatosA audio, long start, long stop)
            {
                Posicion = posicion;
                Audio = audio;
                Start = start;
                Stop = stop;
            }
        }
        RawFormat mWaveFormat;
        List<byte> mListDatosRaw;
        TimeSpan mDuracion;
        long mNumeroDeMuestras;
        public RawDatosA(RawFormat WaveFormat)
        {
            mWaveFormat = WaveFormat;
            mListDatosRaw = new List<byte>();
            RecalcularVariables();
            //mDuracion = new TimeSpan((long)((((double)mDatosRaw.Length) / (mWaveFormat.MuestrasPorSeg * mWaveFormat.BytesPorMuestra * mWaveFormat.Canales)) * 10000000));
            //mNumeroDeMuestras = 0;
        }
        public RawDatosA(byte[] DatosAudio, RawFormat WaveFormat)
        {
            mWaveFormat = WaveFormat;
            mListDatosRaw = new List<byte>(DatosAudio);
            RecalcularVariables();


            //mDuracion = new TimeSpan((long)((((double)mDatosRaw.Length) / (mWaveFormat.MuestrasPorSeg * mWaveFormat.BytesPorMuestra * mWaveFormat.Canales)) * 10000000));
            //mNumeroDeMuestras = DatosAudio.Length / mWaveFormat.Canales / mWaveFormat.BytesPorMuestra;
        }

        public void InsertarRaw(RawDatosA Datos)
        {
            InsertarRaw(Datos.DatosRaw);
        }
        public void InsertarRaw(byte[] Datos)
        {
            //byte[] Temp = new byte[mDatosRaw.Length + Datos.Length];
            //Array.Resize(ref mDatosRaw, mDatosRaw.Length + Datos.Length);
            //Array.Copy(Datos, 0, mDatosRaw, mDatosRaw.Length - Datos.Length, Datos.Length);
            mListDatosRaw.AddRange(Datos);
            RecalcularVariables();
        }

        private void RecalcularVariables()
        {
            mDuracion = new TimeSpan((long)((((double)mListDatosRaw.Count) / (mWaveFormat.MuestrasPorSeg * mWaveFormat.BytesPorMuestra * mWaveFormat.Canales)) * 10000000));
            mNumeroDeMuestras = mListDatosRaw.Count / mWaveFormat.Canales / mWaveFormat.BytesPorMuestra;
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
                return mListDatosRaw.ToArray();
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
        public RawDatosA GetRawDatos(int mSegOffset, int mSegDuracion = 0)
        {
            return new RawDatosA(GetRawBytes(mSegOffset, mSegDuracion), mWaveFormat);
        }

        public byte[] GetRawBytes(int mSegOffset, int mSegDuracion = 0)
        {
            int index = (int)((mSegOffset / 1000d) * mWaveFormat.MuestrasPorSeg * mWaveFormat.Canales * mWaveFormat.BytesPorMuestra);
            int Durac = (int)((mSegDuracion / 1000d) * mWaveFormat.MuestrasPorSeg * mWaveFormat.Canales * mWaveFormat.BytesPorMuestra);

            index = (int)(Math.Truncate(((double)index) / (mWaveFormat.Canales * mWaveFormat.BytesPorMuestra)) * (mWaveFormat.Canales * mWaveFormat.BytesPorMuestra));

            if (index > mListDatosRaw.Count - 1)
            {
                return new byte[0];
            }
            else
            {
                byte[] Resul = new byte[0];
                if ((index + Durac) > mListDatosRaw.Count)
                {
                    //Resul = new byte[mListDatosRaw.Count - index - 1];
                    //Array.Copy(mDatosRaw, index, Resul, 0, mDatosRaw.Length - index);


                    Resul = mListDatosRaw.GetRange(index, mListDatosRaw.Count - index).ToArray();

                }
                else
                {
                    Resul = new byte[0];
                    if (Durac == 0)
                    {
                        //Resul = new byte[mDatosRaw.Length - index];
                        //Array.Copy(mDatosRaw, index, Resul, 0, mDatosRaw.Length - index);

                        Resul = mListDatosRaw.GetRange(index, mListDatosRaw.Count - index).ToArray();
                    }
                    else
                    {
                        //Resul = new byte[Durac];
                        //Array.Copy(mDatosRaw, index, Resul, 0, Durac);

                        Resul = mListDatosRaw.GetRange(index, Durac).ToArray();
                    }
                }
                return Resul;
            }
        }

        public byte[] GetRawBytes(long start, long lenght)
        {
            if (start > mListDatosRaw.Count - 1)
            {
                return new byte[0];
            }
            else
            {
                byte[] Resul = new byte[0];
                if ((start + lenght) > mListDatosRaw.Count)
                {
                    //Resul = new byte[mDatosRaw.Length - start];
                    //Array.Copy(mDatosRaw, start, Resul, 0, mDatosRaw.Length - start);
                    Resul = mListDatosRaw.GetRange((int)start, (int)(mListDatosRaw.Count - start)).ToArray();
                }
                else
                {
                    //Resul = new byte[0];
                    if (lenght == 0)
                    {
                        //Resul = new byte[mDatosRaw.Length - start];
                        //Array.Copy(mDatosRaw, start, Resul, 0, mDatosRaw.Length - start);
                        Resul = mListDatosRaw.GetRange((int)start, (int)(mListDatosRaw.Count - start)).ToArray();
                    }
                    else
                    {
                        //Resul = new byte[lenght];
                        //Array.Copy(mDatosRaw, start, Resul, 0, lenght);
                        Resul = mListDatosRaw.GetRange((int)start, (int)(lenght)).ToArray();
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
            int mStart = -1;
            int mStop = -1;
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
                    //TODO Para audios de 8 bit falta el codigo
                    break;
                case 2:
                    int i = -1;
                    bool mSiguienteConv;
                    do
                    {
                        i++;
                        if (i >= mNumeroDeMuestras)
                        {
                            break;
                        }
                        mStart = -1;
                        mStop = -1;
                        mSiguienteConv = false;

                        mValorMuestra = Utilidades.AbsSample((short)((mListDatosRaw[(i * Paso) + 1] * 256) + mListDatosRaw[(i * Paso)]), true);
                        if (mValorMuestra > mTrigger)
                        {
                            mStart = i;
                            mStart = (mStart - (1 * mWaveFormat.BytesPorSeg));
                            if (mStart < 0)
                            {
                                mStart = 0;
                            }

                            do
                            {
                                i++;
                                if (i >= mNumeroDeMuestras)
                                {
                                    break;
                                }

                                mValorMuestra = Utilidades.AbsSample((short)((mListDatosRaw[(i * Paso) + 1] * 256) + mListDatosRaw[(i * Paso)]), true);
                                if (mValorMuestra < mTrigger)
                                {
                                    mStop = i;
                                    mStop = (mStop + (1 * mWaveFormat.BytesPorSeg));
                                    if (mStop > mNumeroDeMuestras)
                                    {
                                        mStop = (int)mNumeroDeMuestras;
                                    }

                                    do
                                    {
                                        i++;
                                        if (i >= mNumeroDeMuestras)
                                        {
                                            mStop = -1;
                                            break;
                                        }

                                        mValorMuestra = Utilidades.AbsSample((short)((mListDatosRaw[(i * Paso) + 1] * 256) + mListDatosRaw[(i * Paso)]), true);
                                        if (mValorMuestra < mTrigger)
                                        {
                                            if ((i - (3 * mWaveFormat.MuestrasPorSeg)) > mStop)
                                            {
                                                Double Seg = ((double)i) / mWaveFormat.MuestrasPorSeg * 10000000;
                                                mResult.Add(new Conversacion(new TimeSpan((long)(Seg)), new RawDatosA(GetRawBytes((long)(mStart * 2), (long)(((mStop - mStart) * 2) + (1 * mWaveFormat.BytesPorSeg))), mWaveFormat), mStart, mStop));
                                                mSiguienteConv = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            mStop = -1;
                                            i--;
                                            break;
                                        }
                                    } while (true);
                                    if (mSiguienteConv == true)
                                    {
                                        break;
                                    }
                                }
                            } while (true);
                        }
                    } while (true);
                    if ((mStart > -1) & (mStop == -1))
                    {
                        Double Seg = ((double)i) / mWaveFormat.MuestrasPorSeg * 10000000;
                        mResult.Add(new Conversacion(new TimeSpan((long)(Seg)), new RawDatosA(GetRawBytes((long)(mStart * 2), (long)(mNumeroDeMuestras * 2)), mWaveFormat), mStart, mStop));
                    }

                    break;





                    //for (int i = mStart; i < mNumeroDeMuestras; i++)
                    //{
                    //    mValorMuestra = Utilidades.AbsSample((short)((mListDatosRaw[(i * Paso) + 1] * 256) + mListDatosRaw[(i * Paso)]), true);
                    //    if (mValorMuestra > mTrigger)
                    //    {
                    //        bool mResulk = false;
                    //        for (int k = i; k < mNumeroDeMuestras; k++)
                    //        {
                    //            mValorMuestra = Utilidades.AbsSample((short)((mListDatosRaw[(k * Paso) + 1] * 256) + mListDatosRaw[(k * Paso)]), true);
                    //            if (mValorMuestra < mTrigger)
                    //            {
                    //                bool mResulm = false;
                    //                for (int m = k; m < mNumeroDeMuestras; m++)
                    //                {
                    //                    mValorMuestra = Utilidades.AbsSample((short)((mListDatosRaw[(m * Paso) + 1] * 256) + mListDatosRaw[(m * Paso)]), true);
                    //                    if (mValorMuestra < mTrigger)
                    //                    {
                    //                        if ((m - (3 * mWaveFormat.MuestrasPorSeg)) > k)
                    //                        {
                    //                            mResult.Add(new Conversacion(new TimeSpan(0), new RawDatosA(GetRawBytes((long)i * 2, (k + (1 * mWaveFormat.BytesPorSeg))), mWaveFormat)));
                    //                            i = m;
                    //                            mResulm = true;
                    //                            break;
                    //                        }

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
                    Valor = mListDatosRaw[(int)(NumeroDeMuestra * Paso)];
                    break;
                case 2:
                    Valor = (short)(((mListDatosRaw[(int)((NumeroDeMuestra * Paso) + 1)]) * 256) + mListDatosRaw[(int)((NumeroDeMuestra * Paso))]);
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
                    Valor = mListDatosRaw[(int)((NumeroDeMuestra * Paso) + 1)];
                    break;
                case 2:
                    Valor = (short)((mListDatosRaw[(int)((NumeroDeMuestra * Paso) + 3)] * 256) + mListDatosRaw[(int)((NumeroDeMuestra * Paso) + 2)]);
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
                    Valor = mListDatosRaw[(int)((NumeroDeMuestra * Paso))];
                    break;
                case 2:
                    Valor = (short)((mListDatosRaw[(int)((NumeroDeMuestra * Paso) + 1)] * 256) + mListDatosRaw[(int)((NumeroDeMuestra * Paso))]);
                    break;
            }
            return Valor;
        }
    }
}
