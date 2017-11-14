using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BibliotecaMaf.Clases.Audio
{
    public class RawDatos
    {
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

        /// <summary>
        /// Obtiene el valor de la muestra del canal Izquierdo
        /// </summary>
        /// <param name="NumeroDeMuestra">Numero de la muestra necesaria con base 0</param>
        /// <returns>Retorna el valor de la muestra</returns>
        public long GetValorMuestraIzquierda(long NumeroDeMuestra)
        {
            long Valor = 0;
            int Paso = mWaveFormat.BytesPorMuestra * 2;
            switch (mWaveFormat.BytesPorMuestra)
            {
                case 1:
                    Valor = mDatosRaw[NumeroDeMuestra * Paso];
                    break;
                case 2:
                    Valor = (mDatosRaw[NumeroDeMuestra * Paso] * 256) + mDatosRaw[(NumeroDeMuestra * Paso) + 1];
                    break;
            }
            return Valor;
        }

        /// <summary>
        /// Obtiene el valor de la muestra del canal Derecho
        /// </summary>
        /// <param name="NumeroDeMuestra">Numero de la muestra necesaria</param>
        /// <returns>Retorna el valor de la muestra</returns>
        public long GetValorMuestraDerecha(long NumeroDeMuestra)
        {
            long Valor = 0;
            int Paso = mWaveFormat.BytesPorMuestra * 2;
            switch (mWaveFormat.BytesPorMuestra)
            {
                case 1:
                    Valor = mDatosRaw[(NumeroDeMuestra * Paso) + 2];
                    break;
                case 2:
                    Valor = (mDatosRaw[(NumeroDeMuestra * Paso) + 2] * 256) + mDatosRaw[(NumeroDeMuestra * Paso) + 3];
                    break;
            }
            return Valor;
        }

        /// <summary>
        /// Obtiene el valor de la muestra del canal Mono
        /// </summary>
        /// <param name="NumeroDeMuestra">Numero de la muestra necesaria</param>
        /// <returns>Retorna el valor de la muestra</returns>
        public long GetValorMuestraMono(long NumeroDeMuestra)
        {
            long Valor = 0;
            int Paso = mWaveFormat.BytesPorMuestra;
            switch (mWaveFormat.BytesPorMuestra)
            {
                case 1:
                    Valor = mDatosRaw[(NumeroDeMuestra * Paso)];
                    break;
                case 2:
                    Valor = (mDatosRaw[(NumeroDeMuestra * Paso) + 1] * 256) + mDatosRaw[(NumeroDeMuestra * Paso)];
                    break;
            }
            return Valor;
        }
    }
}
