using System;
using System.Runtime.InteropServices;

namespace BibliotecaMaf.Clases.Audio.Lame
{
    public enum BitRates
    {
        _8 = 8,
        _16 = 16,
        _24 = 24,
        _32 = 32,
        _40 = 40,
        _48 = 48,
        _56 = 56,
        _64 = 64,
        _80 = 80,
        _96 = 96,
        _112 = 112,
        _128 = 128,
        _144 = 144,
        _160 = 160,
        _192 = 192,
        _224 = 224,
        _256 = 256,
        _320 = 320
    }

    public enum CalidadVBR
    {
        _0,
        _1,
        _2,
        _3,
        _4,
        _5,
        _6,
        _7,
        _8,
        _9,
    }

    public enum Calidad
    {
        CalidadMuyBuena = 0, //LQP_CD
        CalidadNormal, //LQP_TAPE
        CalidadRegular, //LQP_RADIO
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LHV1
    {
        private const uint MPEG1 = 1;
        private const uint MPEG2 = 0;

        public enum TipoDeCodificacion
        {
            VBR, //Bitrate variable
            CBR, //Bitrate constante
            Predefinida //Usar una configuracion predefinida de LAME
        }

        private enum Modos : uint
        {
            STEREO = 0,
            JOINT_STEREO = 1,
            MONO = 3
        }

        //Metodos VBR
        private enum MetodosVBR : int
        {
            VBR_METHOD_NONE = -1,
            VBR_METHOD_DEFAULT,
            VBR_METHOD_OLD,
            VBR_METHOD_NEW,
            VBR_METHOD_MTRH,
            VBR_METHOD_ABR
        }

        //Algunas de las configuraciones (Calidades) predefinidas de LAME
        private enum CalidadesPredefinidasDeLAME : int
        {
            LQP_NORMAL_QUALITY = 0,

            LQP_RADIO = 6000,
            LQP_TAPE = 7000,
            LQP_CD = 9000,
        }

        //Miembros de la estructura LHV1
        private uint dwStructVersion;
        private uint dwStructSize;
        private uint dwSampleRate;
        private uint dwReSampleRate;
        private Modos nMode;
        private uint dwBitrate;
        private uint dwMaxBitrate;
        private CalidadesPredefinidasDeLAME nPreset;
        private uint dwMpegVersion;
        private uint dwPsyModel;
        private uint dwEmphasis;
        private int bPrivate;
        private int bCRC;
        private int bCopyright;
        private int bOriginal;
        private int bWriteVBRHeader;
        private int bEnableVBR;
        private int nVBRQuality;
        private uint dwVbrAbr_bps;
        private MetodosVBR nVbrMethod;
        private int bNoRes;

        public LHV1(Calidad CalidadPredefinida, int Nivel, TipoDeCodificacion Tipo, RawFormat formato)
        {
            dwStructVersion = 1;
            dwStructSize = (uint)Marshal.SizeOf(typeof(BE_CONFIG));
            dwMpegVersion = MPEG1;
            dwSampleRate = (uint)formato.MuestrasPorSeg;
            //if (Muestras != 0)
            //{
            //	dwSampleRate = (uint)Muestras;
            //}
            dwReSampleRate = 0;

            if (formato.Canales == 1)
            {
                nMode = Modos.MONO;
            }
            else
            {
                nMode = Modos.STEREO;
            }

            dwBitrate = 0;
            bWriteVBRHeader = 0;
            bEnableVBR = 0;
            nVBRQuality = 0;
            nVbrMethod = MetodosVBR.VBR_METHOD_NONE;

            //Determino cual es el tipo de calidad en que se quiere grabar
            if (Tipo == TipoDeCodificacion.Predefinida) //Usar una calidad predefinida en LAME
            {
                switch (CalidadPredefinida)
                {
                    case Calidad.CalidadMuyBuena:
                        nPreset = CalidadesPredefinidasDeLAME.LQP_CD;
                        break;

                    case Calidad.CalidadNormal:
                        nPreset = CalidadesPredefinidasDeLAME.LQP_TAPE;
                        break;

                    case Calidad.CalidadRegular:
                        nPreset = CalidadesPredefinidasDeLAME.LQP_RADIO;
                        break;

                    default:
                        throw new ArgumentException("La calidad no es valida");
                }
            }
            else
            {
                nPreset = CalidadesPredefinidasDeLAME.LQP_NORMAL_QUALITY;

                if (Tipo == TipoDeCodificacion.CBR) //Usar una calidad personalizada (CBR)
                    dwBitrate = (uint)Nivel;
                else if (Tipo == TipoDeCodificacion.VBR) //Usar una calidad personalizada (VBR)
                {
                    bEnableVBR = 1;
                    nVBRQuality = Nivel;
                    bWriteVBRHeader = 1;
                    nVbrMethod = MetodosVBR.VBR_METHOD_NEW;
                }
            }

            dwPsyModel = 0;
            dwEmphasis = 0;
            bOriginal = 0;
            bNoRes = 0;
            bCopyright = 0;
            bCRC = 0;
            bPrivate = 0;
            dwMaxBitrate = 0;
            dwVbrAbr_bps = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class BE_CONFIG
    {
        public const uint BE_CONFIG_LAME = 256;

        public uint dwConfig;
        public LHV1 Info;

        public BE_CONFIG(Calidad CalidadPredefinida, int Nivel, LHV1.TipoDeCodificacion Tipo, RawFormat formato)
        {
            dwConfig = BE_CONFIG_LAME;
            Info = new LHV1(CalidadPredefinida, Nivel, Tipo, formato);
        }
    }

    internal class NativeMethods
    {
        //Valores de retorno de las funciones
        public const uint SUCCESSFUL = 0;
        public const uint INVALID_FORMAT = 1;
        public const uint INVALID_FORMAT_PARAMETERS = 2;
        public const uint NO_MORE_HANDLES = 3;
        public const uint INVALID_HANDLE = 4;

        public enum eCodigoError
        {
            Exito = 0,
            Formato_Invalido = 1,
            Formato_Parametro_Invalido = 2,
            No_Mas_Handles = 3,
            Handle_Invalido = 4,
        }

        /// <summary>
        /// Esta funcion es la primera a llamarse antes de comensar un flujo codificado
        /// </summary>
        /// <param name="pbeConfig">Puntero a estructura que contiene la configuracion del codificador.</param>
        /// <param name="dwSamples">Puntero a DWord (unsig long 32bits) donde el numero de samples to enviar en cada BeEncodeChunk es retornado.</param>
        /// <param name="dwBufferSize">Puntero a DWord (unsig long 32bits) donde el minimo tamaño en bytes del buffer de salida es retornado.</param>
        /// <param name="phbeStream">Puntero a un entero sin signo (unsig int 32bit) donde el handle del stream es retornado.</param>
        /// <returns>Codigo de error 0=exito</returns>
        [DllImport("Lame_enc.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint beInitStream(BE_CONFIG pbeConfig, ref uint dwSamples, ref uint dwBufferSize, ref uint phbeStream);

        /// <summary>
        /// Última función que se llamará cuando termine de codificar una secuencia. En caso de diferencia beDeinitStream () también se puede llamar si la codificación se cancela.
        /// </summary>
        /// <param name="hbeStream">Handle del stream, un entero (sig 32bit).</param>
        /// <returns>Codigo de error 0=exito</returns>
        [DllImport("Lame_enc.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint beCloseStream(uint hbeStream);

        /// <summary>
        /// Esta función debe ser llamada después de que codifica la ultima porción pasada con el fin de limpiar el codificador.
        /// Se escribe todos los datos codificados que aún podrían quedar en el interior del codificador al búfer de salida.
        /// Esta función no debe llamarse a menos que haya codificado todos los trozos en su stream.
        /// </summary>
        /// <param name="hbeStream">Handle del stream, un entero (sig 32bit).</param>
        /// <param name="pOutput">Puntero a BYTE (unsig char 8 Bits) Dónde escribir los datos codificados. Este Buffer debe ser de al menos el tamaño mínimo devuelto por beInitStream ().</param>
        /// <param name="pdwOutput">Puntero a DWORD (unsig 32 bits)Donde se retorna el numero de bytes escritos</param>
        /// <returns>Codigo de error 0=exito</returns>
        [DllImport("Lame_enc.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint beDeinitStream(uint hbeStream, [In, Out] byte[] pOutput, ref uint pdwOutput);

        /// <summary>
        /// Codifica un trozo de muestras. Tenga en cuenta que si ha configurado la salida mono para generar archivos MP3 se debe alimentar beEncodeChunk () con muestras mono!
        /// </summary>
        /// <param name="hbeStream">Handle del stream, un entero (sig 32bit).</param>
        /// <param name="nSamples">Número de muestras (DWORD) que se van a codificar para esta llamada.
        /// Esta debe ser idéntica a lo que se devuelve por beInitStream (), 
        /// a menos que se codifica el último fragmento, que podría ser más pequeño.</param>
        /// <param name="pSamples">Puntero a las muestras de 16-bits con signo (PSHORT signed 16 bit) para ser codificados.
        /// Estos deben estar en estéreo cuando se codifica un equipo de música MP3 y mono cuando se codifica un MP3 mono.</param>
        /// <param name="pOutput">Puntero a BYTE (unsig char 8 Bits) Dónde escribir los datos codificados. 
        /// Este Buffer debe ser de al menos el tamaño mínimo devuelto por beInitStream ().</param>
        /// <param name="pdwOutput">Puntero a DWORD (unsig 32 bits)Donde se retorna el numero de bytes escritos</param>
        /// <returns>Codigo de error 0=exito</returns>
        [DllImport("Lame_enc.dll", CallingConvention = CallingConvention.Cdecl)]
        //protected static extern uint beEncodeChunk(uint hbeStream, uint nSamples, IntPtr pSamples, [In, Out] byte[] pOutput, ref uint pdwOutput);
        protected static extern uint beEncodeChunk(UIntPtr hbeStream, uint nSamples, UIntPtr pSamples, UIntPtr pOutput, ref uint pdwOutput);





        /// <summary>
        /// Esta funcion es la primera a llamarse antes de comensar un flujo codificado
        /// </summary>
        /// <param name="pbeConfig">Puntero a estructura que contiene la configuracion del codificador.</param>
        /// <param name="dwSamples">Puntero a DWord (unsig long 32bits) donde el numero de samples to enviar en cada BeEncodeChunk es retornado.</param>
        /// <param name="dwBufferSize">Puntero a DWord (unsig long 32bits) donde el minimo tamaño en bytes del buffer de salida es retornado.</param>
        /// <param name="phbeStream">Puntero a un entero sin signo (unsig int 32bit) donde el handle del stream es retornado.</param>
        /// <returns>Codigo de error 0=exito</returns>
        [DllImport("lame_enc x64.dll", EntryPoint = "beInitStream", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint beInitStream_64(BE_CONFIG pbeConfig, ref uint dwSamples, ref uint dwBufferSize, ref uint phbeStream);

        /// <summary>
        /// Última función que se llamará cuando termine de codificar una secuencia. En caso de diferencia beDeinitStream () también se puede llamar si la codificación se cancela.
        /// </summary>
        /// <param name="hbeStream">Handle del stream, un entero (sig 32bit).</param>
        /// <returns>Codigo de error 0=exito</returns>
        [DllImport("Lame_enc x64.dll", EntryPoint = "beCloseStream", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint beCloseStream_64(uint hbeStream);

        /// <summary>
        /// Esta función debe ser llamada después de que codifica la ultima porción pasada con el fin de limpiar el codificador.
        /// Se escribe todos los datos codificados que aún podrían quedar en el interior del codificador al búfer de salida.
        /// Esta función no debe llamarse a menos que haya codificado todos los trozos en su stream.
        /// </summary>
        /// <param name="hbeStream">Handle del stream, un entero (sig 32bit).</param>
        /// <param name="pOutput">Puntero a BYTE (unsig char 8 Bits) Dónde escribir los datos codificados. Este Buffer debe ser de al menos el tamaño mínimo devuelto por beInitStream ().</param>
        /// <param name="pdwOutput">Puntero a DWORD (unsig 32 bits)Donde se retorna el numero de bytes escritos</param>
        /// <returns>Codigo de error 0=exito</returns>
        [DllImport("Lame_enc x64.dll", EntryPoint = "beDeinitStream", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint beDeinitStream_64(uint hbeStream, [In, Out] byte[] pOutput, ref uint pdwOutput);

        /// <summary>
        /// Codifica un trozo de muestras. Tenga en cuenta que si ha configurado la salida mono para generar archivos MP3 se debe alimentar beEncodeChunk () con muestras mono!
        /// </summary>
        /// <param name="hbeStream">Handle del stream, un entero (sig 32bit).</param>
        /// <param name="nSamples">Número de muestras (DWORD) que se van a codificar para esta llamada.
        /// Esta debe ser idéntica a lo que se devuelve por beInitStream (), 
        /// a menos que se codifica el último fragmento, que podría ser más pequeño.</param>
        /// <param name="pSamples">Puntero a las muestras de 16-bits con signo (PSHORT signed 16 bit) para ser codificados.
        /// Estos deben estar en estéreo cuando se codifica un equipo de música MP3 y mono cuando se codifica un MP3 mono.</param>
        /// <param name="pOutput">Puntero a BYTE (unsig char 8 Bits) Dónde escribir los datos codificados. 
        /// Este Buffer debe ser de al menos el tamaño mínimo devuelto por beInitStream ().</param>
        /// <param name="pdwOutput">Puntero a DWORD (unsig 32 bits)Donde se retorna el numero de bytes escritos</param>
        /// <returns>Codigo de error 0=exito</returns>
        [DllImport("Lame_enc x64.dll", EntryPoint = "beEncodeChunk", CallingConvention = CallingConvention.Cdecl)]
        protected static extern uint beEncodeChunk_64(UIntPtr hbeStream, uint nSamples, UIntPtr pSamples, UIntPtr pOutput, ref uint pdwOutput);








        /// <summary>
        /// Codifica un trozo de muestras. Tenga en cuenta que si ha configurado la salida mono para generar archivos MP3 se debe alimentar beEncodeChunk () con muestras mono!
        /// </summary>
        /// <param name="hbeStream">Handle del stream, un entero sin signo (unsig 32bit).</param>
        /// <param name="buffer">Buffer que contiene las muestras codificadas en 16 Bits</param>
        /// <param name="pOutput">Buffer de salida</param>
        /// <param name="pdwOutput">Numero de bytes escritos en la salida</param>
        /// <returns>Codigo de error 0=exito</returns>
        public static uint Convertir(uint hbeStream, byte[] buffer, byte[] pOutput, ref uint pdwOutput)
        {
            return Convertir(hbeStream, buffer, 0, (uint)buffer.Length, pOutput, ref pdwOutput);
        }

        /// <summary>
        /// Codifica un trozo de muestras. Tenga en cuenta que si ha configurado la salida mono para generar archivos MP3 se debe alimentar beEncodeChunk () con muestras mono!
        /// </summary>
        /// <param name="hbeStream">Handle del stream, un entero sin signo (unsig 32bit).</param>
        /// <param name="buffer">Buffer que contiene las muestras codificadas en 16 Bits</param>
        /// <param name="index">Offset en el buffer de las muestras a partir del cual se empieza a leer</param>
        /// <param name="nBytes">Numero de bytes de las muestras</param>
        /// <param name="pOutput">Buffer de salida</param>
        /// <param name="pdwOutput">Numero de bytes escritos en la salida</param>
        /// <returns>Codigo de error 0=exito</returns>
        public static uint Convertir(uint hbeStream, byte[] buffer, int index, uint nBytes, byte[] pOutput, ref uint pdwOutput)
        {
            uint res = 0;
            GCHandle bufferGC = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            GCHandle pOutputGC = GCHandle.Alloc(pOutput, GCHandleType.Pinned);
            try
            {
                int TamanoArquitectura = IntPtr.Size;
                if (TamanoArquitectura == 4)
                {
                    UIntPtr PBuffer = (UIntPtr)(bufferGC.AddrOfPinnedObject().ToInt64() + index);
                    UIntPtr PpOutput = (UIntPtr)(pOutputGC.AddrOfPinnedObject().ToInt64());
                    UIntPtr PhbeStream = new UIntPtr(hbeStream);
                    //res = beEncodeChunk(hbeStream, nBytes / 2, Puntero, pOutput, ref pdwOutput);
                    res = beEncodeChunk(PhbeStream, nBytes / 2, PBuffer, PpOutput, ref pdwOutput);
                }
                else
                {
                    UIntPtr PBuffer = (UIntPtr)(bufferGC.AddrOfPinnedObject().ToInt64() + index);
                    UIntPtr PpOutput = (UIntPtr)(pOutputGC.AddrOfPinnedObject().ToInt64());
                    UIntPtr PhbeStream = new UIntPtr(hbeStream);
                    //res = beEncodeChunk(hbeStream, nBytes / 2, Puntero, pOutput, ref pdwOutput);
                    res = beEncodeChunk_64(PhbeStream, nBytes / 2, PBuffer, PpOutput, ref pdwOutput);
                }
            }
            catch
            {
                res = 1;
            }
            finally
            {
                bufferGC.Free();
                pOutputGC.Free();
            }
            return res;
        }
    }
}