using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;

namespace BibliotecaMaf.Clases.Audio
{
    public class ReproductorEventArg : EventArgs
    {
        /// <summary>
        /// Matriz en la que se colocaran los datos a reproducir
        /// </summary>
        public byte[] DatosNuevos = new byte[0];

        /// <summary>
        /// Cantidad maxima de datos a colocar
        /// </summary>
        public int CantidadMax = 0;

        /// <summary>
        /// Si se establece en true la reproduccion finalizara inmediatamente
        /// </summary>
        public bool Stop = false;

        /// <summary>
        /// Inicia una nueva instancia de la clase
        /// </summary>
        /// <param name="cantidadmaxima">Cantidad maxima de datos que se deben devolver</param>
        public ReproductorEventArg(int cantidadmaxima)
        {
            CantidadMax = cantidadmaxima;
        }
    }

    public class PlayingEventArg : EventArgs
    {
        /// <summary>
        /// Cantidad de bytes que fueron enviados al hardware
        /// </summary>
        public readonly long CantidadBytesReproducidos = 0;

        public readonly int MuestrasPorSegundo = 0;
        public readonly int Canales = 0;
        public readonly int BitsPorMuestra = 0;
        public readonly TimeSpan TiempoReproducido = new TimeSpan();

        /// <summary>
        /// Si se establece en true la reproduccion finalizara inmediatamente
        /// </summary>
        public bool Stop = false;

        /// <summary>
        /// Inicia una nueva instancia de la clase
        /// </summary>
        /// <param name="bytesreproducidos"></param>
        /// <param name="muestrasporsegundo"></param>
        /// <param name="canales"></param>
        /// <param name="bitspormuestra"></param>
        public PlayingEventArg(long bytesreproducidos, int muestrasporsegundo, int canales, int bitspormuestra)
        {
            CantidadBytesReproducidos = bytesreproducidos;
            MuestrasPorSegundo = muestrasporsegundo;
            Canales = canales;
            BitsPorMuestra = bitspormuestra;
            double mTiempomseg = (double)bytesreproducidos / canales / (bitspormuestra / 8) / muestrasporsegundo * 10000000;
            TiempoReproducido = new TimeSpan((long)mTiempomseg);
        }
    }

    public class Reproductor : IDisposable
    {
        private IWavePlayer mWaveOutt;
        private WaveFormat mWaveFormat;
        private BufferedWaveProvider mWaveProvider;
        private System.Threading.Thread mThreadReproducir;
        private long mByteBufferLowEvent = 0;
        private bool mDisposed = false;
        private bool mCancelarReproduccion = false;
        private readonly System.Threading.Semaphore mBlokArranqueSubproceso = new System.Threading.Semaphore(1, 1);
        //private System.Threading.Semaphore mBlokEsperaCierreSubproceso = new System.Threading.Semaphore(1, 1);
        private readonly System.Threading.Semaphore mBlokCargarDatos = new System.Threading.Semaphore(1, 1);
        private long mCantidadReproducida = 0;

        private long mPlayingBytesCountOnPlaying = 0;
        public bool AutoStopBufferEmpty = true;


        /// <summary>
        /// Delegado para los eventos del reproductor
        /// </summary>
        /// <param name="sender">Objeto que inicia la llamada</param>
        /// <param name="e">Propiedades de la llamada</param>
        public delegate void DelegadoEventReproductor(object sender, ReproductorEventArg e);

        public delegate void DelegadoOnPlaying(object sender, PlayingEventArg e);

        public delegate void DelegadoEvento(object sender, EventArgs e);

        /// <summary>
        /// Evento que se produce cuando el stream de salida necesita mas datos
        /// </summary>
        public event DelegadoEventReproductor BufferReproducirBajo;

        /// <summary>
        /// Evento que se produce durante la reproduccion para informar el tiempo reproducido
        /// </summary>
        public event DelegadoOnPlaying OnPlaying;

        public event DelegadoEvento Stopped;


        /// <summary>
        /// Inicia una instancia de esta clase
        /// </summary>
        public Reproductor()
        {
        }

        /// <summary>
        /// Inicia una instancia de esta clase
        /// </summary>
        /// <param name="bufferlowbytescountlevel">Cantidad de bytes minimos para que se produzca el evento BufferReproducirBajo</param>
        /// <param name="mplayingbytescountonplaying">Cantidad de bytes reproducidos necesarios para que se produzca el evento OnPlaying</param>
        public Reproductor(long bufferlowbytescountlevel = 0, long mplayingbytescountonplaying = 0)
        {
            mByteBufferLowEvent = bufferlowbytescountlevel;
            mPlayingBytesCountOnPlaying = mplayingbytescountonplaying;
        }

        /// <summary>
        /// Devuelve o establece el nivel del buffer en el cual se va a producir el evento BufferReproducirBajo
        /// </summary>
        public long AvisoBufferLow
        {
            get
            {
                return mByteBufferLowEvent;
            }
            set
            {
                mByteBufferLowEvent = value;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.mDisposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    mWaveOutt.Dispose();
                    mBlokArranqueSubproceso.Dispose();
                    //mBlokEsperaCierreSubproceso.Dispose();
                    mBlokCargarDatos.Dispose();
                    //component.Dispose();
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here.
                // If disposing is false, 
                // only the following code is executed.
                //CloseHandle(handle);
                //handle = IntPtr.Zero;
            }
            mDisposed = true;
        }

        /// <summary>
        /// Dispara el evento "DelegadoEventReproductor"
        /// </summary>
        public void OnBufferReproducirBajo()
        {
            if (BufferReproducirBajo != null)
            {
                ReproductorEventArg e = new ReproductorEventArg(mWaveProvider.BufferLength - mWaveProvider.BufferedBytes);
                BufferReproducirBajo(this, e);
                if (e.Stop == true)
                {
                    mWaveOutt.Stop();
                    return;
                }
                if (e.DatosNuevos.Length > 0)
                {
                    try
                    {
                        mBlokCargarDatos.WaitOne();
                        mCantidadReproducida += e.DatosNuevos.Length;
                        mWaveProvider.AddSamples(e.DatosNuevos, 0, e.DatosNuevos.Length);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        mBlokCargarDatos.Release();
                    }
                }
            }
        }

        public void OnStop()
        {
            if (Stopped != null)
            {
                Stopped(this, new EventArgs());
            }
        }

        /// <summary>
        /// Dispara el evento OnPlaying
        /// </summary>
        public void OnOnPlaying()
        {
            if (OnPlaying != null)
            {
                PlayingEventArg e = new PlayingEventArg(PlayingBytesCount, mWaveFormat.SampleRate, mWaveFormat.Channels, mWaveFormat.BitsPerSample);
                OnPlaying(this, e);
                if (e.Stop == true)
                {
                    Stop();
                    return;
                }
            }
        }

        /// <summary>
        /// Devuelve la cantidad de bytes reproducidos hasta el momento
        /// </summary>
        public long PlayingBytesCount
        {
            get
            {
                try
                {
                    mBlokCargarDatos.WaitOne();
                    return mCantidadReproducida - mWaveProvider.BufferedBytes;
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    mBlokCargarDatos.Release();
                }
            }
        }

        /// <summary>
        /// Devuelve la cantidad de tiempo reproducida hasta el momento
        /// </summary>
        public TimeSpan PlayingTimesCount
        {
            get
            {
                return new TimeSpan((PlayingBytesCount / mWaveFormat.AverageBytesPerSecond * 10000000));
            }
        }

        /// <summary>
        /// Devuelve o establece cada cuantos bytes se va a producir el evento OnPlaying
        /// </summary>
        public long PlayingBytesCountOnPlayingEvent
        {
            get
            {
                return mPlayingBytesCountOnPlaying;
            }
            set
            {
                mPlayingBytesCountOnPlaying = value;
            }
        }

        /// <summary>
        /// Inicia una reproduccion por la salida de sonido predeterminada
        /// </summary>
        /// <param name="TamañoBuffer">Establece el tamaño del buffer para la reproduccion en curso</param>
        /// <param name="MuestrasPorSeg">Cantidad de muestras por segundo para la reproduccion</param>
        /// <param name="Bits">Cantidad de bits para la reproduccion</param>
        /// <param name="Canales">Cantidad de canales para la reproduccion</param>
        /// <param name="audio">Datos de audio que se cargaran al inicio, es Opcional</param>
        public void Reproducir(int TamañoBuffer, int MuestrasPorSeg, int Bits, int Canales, RawDatosA audio = null)
        {
            Console.Write("Iniciando Sonido \r\n");
            mCancelarReproduccion = true;

            try
            {
                mBlokArranqueSubproceso.WaitOne();

                if (mWaveOutt == null)
                {
                    mWaveOutt = new DirectSoundOut();
                    mWaveFormat = new WaveFormat(MuestrasPorSeg, Bits, Canales);
                }

                if (AvisoBufferLow == 0)
                {
                    AvisoBufferLow = (int)(TamañoBuffer * 0.1);
                }

                mWaveProvider = new BufferedWaveProvider(mWaveFormat);
                mWaveProvider.BufferLength = TamañoBuffer;
                mWaveOutt.Init(mWaveProvider);

                if (audio != null)
                {
                    try
                    {
                        mBlokCargarDatos.WaitOne();
                        mCantidadReproducida += audio.DatosRaw.Length;
                        mWaveProvider.AddSamples(audio.DatosRaw, 0, audio.DatosRaw.Length);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        mBlokCargarDatos.Release();
                    }
                }

                mWaveOutt.Play();

                Console.Write("Iniciando Sonido, " + mWaveOutt.PlaybackState.ToString() + "\r\n");

                mThreadReproducir = new System.Threading.Thread(SubProcesoReproducir);
                mThreadReproducir.IsBackground = true;
                mThreadReproducir.Name = "Reproducir";
                mThreadReproducir.Start();
                mCancelarReproduccion = false;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                mBlokArranqueSubproceso.Release();
            }
        }

        /// <summary>
        /// Inicia una reproduccion por la salida de sonido predeterminada
        /// </summary>
        /// <param name="Audio">Datos de audio para reproducir</param>
        /// <param name="TamañoBuffer">Establece el tamaño del buffer para la reproduccion en curso</param>
        public void Reproducir(RawDatosA Audio, int TamañoBuffer)
        {
            Reproducir(TamañoBuffer, Audio.Formato.MuestrasPorSeg, Audio.Formato.Bits, Audio.Formato.Canales, Audio);
        }

        /// <summary>
        /// Detiene la reproduccion en curso
        /// </summary>
        public void Stop()
        {
            if (mWaveOutt != null)
            {
                mCancelarReproduccion = true;
                try
                {
                    //if (System.Threading.Thread.CurrentThread.Name != "Reproducir")
                    //{
                    //	mBlokArranqueSubproceso.WaitOne();
                    ////}
                    //mWaveOutt.Stop();
                    //mWaveOutt.Dispose();
                    //mWaveOutt = null;
                    //mCantidadReproducida = 0;
                    //Console.Write("Stop General \r\n");
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    //if (System.Threading.Thread.CurrentThread.Name != "Reproducir")
                    //{
                    //mBlokArranqueSubproceso.Release();
                    //}
                }
            }
        }

        /// <summary>
        /// Detiene la reproduccion en curso para ser usado desde el subproceso REPRODUCIR
        /// </summary>
        private void StopInterno()
        {
            if (mWaveOutt != null)
            {
                mCancelarReproduccion = true;
                try
                {
                    mWaveOutt.Stop();
                    mWaveOutt.Dispose();
                    mWaveOutt = null;
                    mCantidadReproducida = 0;
                    Console.Write("Stop Interno \r\n");
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Subproceso que se encarga de ir llenando el buffer con mas datos
        /// </summary>
        private void SubProcesoReproducir()
        {
            Console.Write("Iniciando Subproceso \r\n");
            try
            {
                mBlokArranqueSubproceso.WaitOne();

                //mBlokEsperaCierreSubproceso.WaitOne();
                long mPlayingCountTemp = 0;
                while ((mWaveOutt.PlaybackState == PlaybackState.Playing) && (mCancelarReproduccion == false))
                {
                    if (mWaveProvider.BufferedBytes < mByteBufferLowEvent)
                    {
                        OnBufferReproducirBajo();

                        if (mWaveProvider.BufferedBytes == 0)
                        {
                            if (mWaveOutt != null)
                            {
                                if (AutoStopBufferEmpty == true)
                                {
                                    mWaveOutt.Stop();
                                }
                            }
                        }
                    }
                    if (mPlayingBytesCountOnPlaying > 0)
                    {
                        if (PlayingBytesCount - mPlayingCountTemp > mPlayingBytesCountOnPlaying)
                        {
                            mPlayingCountTemp = PlayingBytesCount;
                            OnOnPlaying();
                        }
                    }
                }
                Console.Write("Saliendo Subproceso, " + mWaveOutt.PlaybackState.ToString() + ", " + mCancelarReproduccion + "\r\n");

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //mBlokEsperaCierreSubproceso.Release();
                try
                {
                    mWaveOutt.Stop();
                    mWaveOutt.Dispose();
                    mWaveOutt = null;
                    mCantidadReproducida = 0;
                    OnStop();
                }
                catch (Exception)
                {
                    throw;
                }

                mBlokArranqueSubproceso.Release();
            }
        }
    }
}
