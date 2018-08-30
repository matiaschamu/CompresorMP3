using System;
using BibliotecaMaf.Clases.Audio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SonidoTest
{
    [TestClass]
    public class UnitTestRawDatos
    {
        RawDatosA mRawDatos16bitStereoIzq = new RawDatosA(new byte[24] { 0, 0, 0, 0, 1, 0, 0, 0, 255, 127, 0, 0, 0, 128, 0, 0, 254, 255, 0, 0, 255, 255, 0, 0 }, new RawFormat(48000, 16, 2));
        RawDatosA mRawDatos16bitStereoDer = new RawDatosA(new byte[24] { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 255, 127, 0, 0, 0, 128, 0, 0, 254, 255, 0, 0, 255, 255 }, new RawFormat(48000, 16, 2));
        RawDatosA mRawDatos16bitMono = new RawDatosA(new byte[12] { 0, 0, 1, 0, 255, 127, 0, 128, 254, 255, 255, 255 }, new RawFormat(48000, 16, 1));
        RawDatosA mRawDatos16bitStereo = new RawDatosA(new byte[24] { 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 254, 255, 0, 0, 255, 255, 253, 255, 255, 255, 255, 255, 255, 255 }, new RawFormat(48000, 16, 2));
        RawDatosA mRawDatos8bitStereoIzq = new RawDatosA(new byte[8] { 0, 0, 1, 0, 254, 0, 255, 0 }, new RawFormat(48000, 8, 2));
        RawDatosA mRawDatos8bitStereoDer = new RawDatosA(new byte[8] { 0, 0, 0, 1, 0, 254, 0, 255 }, new RawFormat(48000, 8, 2));
        RawDatosA mRawDatos8bitMono = new RawDatosA(new byte[4] { 0, 1, 254, 255 }, new RawFormat(48000, 8, 1));
        RawDatosA mRawDatos8bitStereo = new RawDatosA(new byte[8] { 0, 0, 0, 2, 253, 255, 255, 255 }, new RawFormat(48000, 8, 2));

        [TestMethod]
        public void TestMethodGetValorMuestraIzquierda()
        {
            //16bits
            short A = mRawDatos16bitStereoIzq.GetValorMuestraIzquierda(0);
            Assert.AreEqual(0, A);
            A = mRawDatos16bitStereoIzq.GetValorMuestraIzquierda(1);
            Assert.AreEqual(1, A);
            A = mRawDatos16bitStereoIzq.GetValorMuestraIzquierda(2);
            Assert.AreEqual(32767, A);
            A = mRawDatos16bitStereoIzq.GetValorMuestraIzquierda(3);
            Assert.AreEqual(-32768, A);
            A = mRawDatos16bitStereoIzq.GetValorMuestraIzquierda(4);
            Assert.AreEqual(-2, A);
            A = mRawDatos16bitStereoIzq.GetValorMuestraIzquierda(5);
            Assert.AreEqual(-1, A);
            //8bits
            A = mRawDatos8bitStereoIzq.GetValorMuestraIzquierda(0);
            Assert.AreEqual(0, A);
            A = mRawDatos8bitStereoIzq.GetValorMuestraIzquierda(1);
            Assert.AreEqual(1, A);
            A = mRawDatos8bitStereoIzq.GetValorMuestraIzquierda(2);
            Assert.AreEqual(254, A);
            A = mRawDatos8bitStereoIzq.GetValorMuestraIzquierda(3);
            Assert.AreEqual(255, A);
        }

        [TestMethod]
        public void TestMethodGetValorMuestraDerecha()
        {
            //16bits
            short A = mRawDatos16bitStereoDer.GetValorMuestraDerecha(0);
            Assert.AreEqual(0, A);
            A = mRawDatos16bitStereoDer.GetValorMuestraDerecha(1);
            Assert.AreEqual(1, A);
            A = mRawDatos16bitStereoDer.GetValorMuestraDerecha(2);
            Assert.AreEqual(32767, A);
            A = mRawDatos16bitStereoDer.GetValorMuestraDerecha(3);
            Assert.AreEqual(-32768, A);
            A = mRawDatos16bitStereoDer.GetValorMuestraDerecha(4);
            Assert.AreEqual(-2, A);
            A = mRawDatos16bitStereoDer.GetValorMuestraDerecha(5);
            Assert.AreEqual(-1, A);
            //8bits
            A = mRawDatos8bitStereoDer.GetValorMuestraDerecha(0);
            Assert.AreEqual(0, A);
            A = mRawDatos8bitStereoDer.GetValorMuestraDerecha(1);
            Assert.AreEqual(1, A);
            A = mRawDatos8bitStereoDer.GetValorMuestraDerecha(2);
            Assert.AreEqual(254, A);
            A = mRawDatos8bitStereoDer.GetValorMuestraDerecha(3);
            Assert.AreEqual(255, A);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestMethodGetValorMuestraMono()
        {
            //16bits mono
            short A = mRawDatos16bitMono.GetValorMuestraMono(0);
            Assert.AreEqual(0, A);
            A = mRawDatos16bitMono.GetValorMuestraMono(1);
            Assert.AreEqual(1, A);
            A = mRawDatos16bitMono.GetValorMuestraMono(2);
            Assert.AreEqual(32767, A);
            A = mRawDatos16bitMono.GetValorMuestraMono(3);
            Assert.AreEqual(-32768, A);
            A = mRawDatos16bitMono.GetValorMuestraMono(4);
            Assert.AreEqual(-2, A);
            A = mRawDatos16bitMono.GetValorMuestraMono(5);
            Assert.AreEqual(-1, A);

            //16bits stereo
            try
            {
                A = mRawDatos16bitStereo.GetValorMuestraMono(0);
            }
            catch (Exception)
            {
            }

            //8bits mono
            A = mRawDatos8bitMono.GetValorMuestraMono(0);
            Assert.AreEqual(0, A);
            A = mRawDatos8bitMono.GetValorMuestraMono(1);
            Assert.AreEqual(1, A);
            A = mRawDatos8bitMono.GetValorMuestraMono(2);
            Assert.AreEqual(254, A);
            A = mRawDatos8bitMono.GetValorMuestraMono(3);
            Assert.AreEqual(255, A);

            //8bits stereo
            try
            {
                A = mRawDatos8bitStereo.GetValorMuestraMono(0);
            }
            catch (Exception)
            {
                throw new Exception("El formato debe ser mono");
            }
        }
    }
}
