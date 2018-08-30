using System;
using BibliotecaMaf.Clases.Audio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SonidoTest
{
    [TestClass]
    public class UnitTestUtilidades
    {
        RawDatosA mRawDatos16bitStereoIzq = new RawDatosA(new byte[24] { 0, 0, 0, 0, 0, 1, 0, 0, 127, 255, 0, 0, 128, 0, 0, 0, 255, 254, 0, 0, 255, 255, 0, 0 }, new RawFormat(48000, 16, 2));
        RawDatosA mRawDatos16bitStereoDer = new RawDatosA(new byte[24] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 127, 255, 0, 0, 128, 0, 0, 0, 255, 254, 0, 0, 255, 255 }, new RawFormat(48000, 16, 2));
        RawDatosA mRawDatos16bitMono = new RawDatosA(new byte[16] { 0, 0, 1, 0, 255, 127, 0, 128, 254, 255, 255, 255,0,2 ,255,192}, new RawFormat(48000, 16, 1));
        RawDatosA mRawDatos16bitStereo = new RawDatosA(new byte[16] { 0, 0, 5, 0, 0, 126, 255, 128, 254, 255, 255, 255, 0, 2, 255, 192 }, new RawFormat(48000, 16, 2));
        RawDatosA mRawDatos8bitStereoIzq = new RawDatosA(new byte[8] { 0, 0, 1, 0, 254, 0, 255, 0 }, new RawFormat(48000, 8, 2));
        RawDatosA mRawDatos8bitStereoDer = new RawDatosA(new byte[8] { 0, 0, 0, 1, 0, 254, 0, 255 }, new RawFormat(48000, 8, 2));
        RawDatosA mRawDatos8bitMono = new RawDatosA(new byte[9] { 0, 1, 254, 255,127,192,253,252,2 }, new RawFormat(48000, 8, 1));
        RawDatosA mRawDatos8bitStereo = new RawDatosA(new byte[9] { 0, 10, 192, 85, 127, 192, 253, 252, 2 }, new RawFormat(48000, 8, 2));

        [TestMethod]
        public void TestMethodValorMaximoDelVolumen()
        {
            //16bit Mono
            byte mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos16bitMono, 0, 1,false, (long)0);
            Assert.AreEqual(0, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos16bitMono, 0, 3, false, (long)0);
            Assert.AreEqual(100, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos16bitMono, 3, 2, false, (long)0);
            Assert.AreEqual(100, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos16bitMono, 3, 1, false, (long)0);
            Assert.AreEqual(100, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos16bitMono, 4, 1, false, (long)0);
            Assert.AreEqual(0, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos16bitMono, 5, 1, false, (long)0);
            Assert.AreEqual(0, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos16bitMono, 6, 1, false, (long)0);
            Assert.AreEqual(1, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos16bitMono, 7, 1, false, (long)0);
            Assert.AreEqual(49, mVolumen);

            //16Bit Stereo
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos16bitStereo, 0, 1, false, (long)0);
            Assert.AreEqual(0, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos16bitStereo, 1, 1, false, (long)0);
            Assert.AreEqual(98, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos16bitStereo, 0, 2, false, (long)0);
            Assert.AreEqual(98, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos16bitStereo, 0, 3, false, (long)0);
            Assert.AreEqual(98, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos16bitStereo, 0, 4, false, (long)0);
            Assert.AreEqual(98, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos16bitStereo, 3, 1, false, (long)0);
            Assert.AreEqual(25, mVolumen);

            //8Bit Mono
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos8bitMono, 0, 1, false, (long)0);
            Assert.AreEqual(100, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos8bitMono, 1, 1, false, (long)0);
            Assert.AreEqual(99, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos8bitMono, 2, 1, false, (long)0);
            Assert.AreEqual(100, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos8bitMono, 3, 1, false, (long)0);
            Assert.AreEqual(100, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos8bitMono, 4, 1, false, (long)0);
            Assert.AreEqual(0, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos8bitMono, 5, 1, false, (long)0);
            Assert.AreEqual(51, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos8bitMono, 6, 1, false, (long)0);
            Assert.AreEqual(99, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos8bitMono, 7, 1, false, (long)0);
            Assert.AreEqual(98, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos8bitMono, 8, 1, false, (long)0);
            Assert.AreEqual(98, mVolumen);

            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos8bitMono, 0, 9, false, (long)2);
            Assert.AreEqual(100, mVolumen);

            //8Bit Stereo
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos8bitStereo, 0, 1, false, (long)0);
            Assert.AreEqual(96, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos8bitStereo, 1, 1, false, (long)0);
            Assert.AreEqual(41, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos8bitStereo, 2, 1, false, (long)0);
            Assert.AreEqual(25, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos8bitStereo, 3, 1, false, (long)0);
            Assert.AreEqual(98, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos8bitStereo, 1, 2, false, (long)0);
            Assert.AreEqual(41, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos8bitStereo, 1, 3, false, (long)0);
            Assert.AreEqual(98, mVolumen);
            mVolumen = Utilidades.ValorPromedioMaximoDelVolumen(mRawDatos8bitStereo, 0, 4, false, (long)0);
            Assert.AreEqual(98, mVolumen);
        }

        [TestMethod]
        public void TestMethodValorMaximoDelVolumen_mSeg()
        {
            //8bit Mono
            byte mVolumen = Utilidades.ValorMaximoDelVolumen(mRawDatos8bitMono, 0, 0, 2);
            Assert.AreEqual(0, mVolumen);
            mVolumen = Utilidades.ValorMaximoDelVolumen(mRawDatos8bitMono, 2,5, 2);
            Assert.AreEqual(0, mVolumen);
            mVolumen = Utilidades.ValorMaximoDelVolumen(mRawDatos8bitMono, 0, 5, 2);
            Assert.AreEqual(100, mVolumen);

        }

        [TestMethod]
        public void TestMethodValorPorcentualDelVolumen()
        {
            //8bit Mono
            byte mVolumen = Utilidades.ValorPorcentualDelVolumen(mRawDatos8bitMono, 0, 1);
            Assert.AreEqual(100, mVolumen);
            mVolumen = Utilidades.ValorPorcentualDelVolumen(mRawDatos8bitMono, 0, 2);
            Assert.AreEqual(99, mVolumen);
            mVolumen = Utilidades.ValorPorcentualDelVolumen(mRawDatos8bitMono, 0, 3);
            Assert.AreEqual(99, mVolumen);
            mVolumen = Utilidades.ValorPorcentualDelVolumen(mRawDatos8bitMono, 0, 4);
            Assert.AreEqual(99, mVolumen);
            mVolumen = Utilidades.ValorPorcentualDelVolumen(mRawDatos8bitMono, 0, 5);
            Assert.AreEqual(79, mVolumen);
            mVolumen = Utilidades.ValorPorcentualDelVolumen(mRawDatos8bitMono, 0, 6);
            Assert.AreEqual(74, mVolumen);
            mVolumen = Utilidades.ValorPorcentualDelVolumen(mRawDatos8bitMono, 0, 7);
            Assert.AreEqual(77, mVolumen);
            mVolumen = Utilidades.ValorPorcentualDelVolumen(mRawDatos8bitMono, 0, 8);
            Assert.AreEqual(80, mVolumen);
            mVolumen = Utilidades.ValorPorcentualDelVolumen(mRawDatos8bitMono, 0, 9);
            Assert.AreEqual(82, mVolumen);
        }

        [TestMethod]
        public void TestMethodAbsSample()
        {
            //8bit
            short valor = Utilidades.AbsSample((short)0, false);
            Assert.AreEqual(127, valor);
            valor = Utilidades.AbsSample((short)1, false);
            Assert.AreEqual(126, valor);
            valor = Utilidades.AbsSample((short)2, false);
            Assert.AreEqual(125, valor);
            valor = Utilidades.AbsSample((short)126, false);
            Assert.AreEqual(1, valor);
            valor = Utilidades.AbsSample((short)127, false);
            Assert.AreEqual(0, valor);
            valor = Utilidades.AbsSample((short)128, false);
            Assert.AreEqual(1, valor);
            valor = Utilidades.AbsSample((short)129, false);
            Assert.AreEqual(2, valor);
            valor = Utilidades.AbsSample((short)254, false);
            Assert.AreEqual(127, valor);
            valor = Utilidades.AbsSample((short)255, false);
            Assert.AreEqual(127, valor);

            //16bit
            valor = Utilidades.AbsSample((short)-32768, true);
            Assert.AreEqual(32767, valor);
            valor = Utilidades.AbsSample((short)-32767, true);
            Assert.AreEqual(32767, valor);
            valor = Utilidades.AbsSample((short)-30000, true);
            Assert.AreEqual(30000, valor);
            valor = Utilidades.AbsSample((short)-2, true);
            Assert.AreEqual(2, valor);
            valor = Utilidades.AbsSample((short)-1, true);
            Assert.AreEqual(1, valor);
            valor = Utilidades.AbsSample((short)0, true);
            Assert.AreEqual(0, valor);
            valor = Utilidades.AbsSample((short)1, true);
            Assert.AreEqual(1, valor);
            valor = Utilidades.AbsSample((short)2, true);
            Assert.AreEqual(2, valor);
            valor = Utilidades.AbsSample((short)30000, true);
            Assert.AreEqual(30000, valor);
            valor = Utilidades.AbsSample((short)32766, true);
            Assert.AreEqual(32766, valor);
            valor = Utilidades.AbsSample((short)32767, true);
            Assert.AreEqual(32767, valor);
        }
    }
}
