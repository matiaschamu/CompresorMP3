using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BibliotecaMaf.Clases.Audio
{
    public class RawFormat
    {
        int mMuestrasPorSeg;
        int mBits;
        int mBytesPorSeg;
        int mBytesPorMuestra;
        int mCanales;

        public RawFormat(int MuestrasPorSeg, int Bits, int Canales)
        {
            mMuestrasPorSeg = MuestrasPorSeg;
            mCanales = Canales;
            mBits = Bits;
            mBytesPorMuestra = mBits / 8;
            mBytesPorSeg = mMuestrasPorSeg * mBytesPorMuestra * mCanales;
        }
        public int Bits
        {
            get
            {
                return mBits;
            }
        }
        public int MuestrasPorSeg
        {
            get
            {
                return mMuestrasPorSeg;
            }
            set
            {
                mMuestrasPorSeg = value;
            }
        }
        public int Canales
        {
            get
            {
                return mCanales;
            }
            set
            {
                mCanales = value;
            }
        }
        public int BytesPorSeg
        {
            get
            {
                return mBytesPorSeg;
            }

        }
        public int BytesPorMuestra
        {
            get
            {
                return mBytesPorMuestra;
            }

        }
    }
}
