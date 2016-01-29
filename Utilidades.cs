using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		public static RawDatos GenerarBeepInicial(int Segundos)
		{
			if (Segundos > 0)
			{
				WaveStream pcmStream = new WaveFileReader(Application.StartupPath + "\\beep3.dat");
				WaveStream pcmStream2 = new WaveFileReader(Application.StartupPath + "\\beep.dat");
				byte[] Buf = new byte[pcmStream.Length];
				pcmStream.Read(Buf, 0, Buf.Length);
				for (int r = 0 ; r < Segundos - 1 ; r++)
				{
					int tamañoAnterior = Buf.Length;
					Array.Resize(ref Buf, (int)(Buf.Length + pcmStream2.Length));
					pcmStream2.Position = 0;
					pcmStream2.Read(Buf, tamañoAnterior, (int)pcmStream2.Length);
				}

				return new RawDatos(Buf, new RawFormat(48000, 16, 1));
			}
			else
			{
				return new RawDatos(new byte[0], new RawFormat(48000, 16, 1));
			}
		}
		/// <summary>
		/// Genera una onda con el sonido BEEP cada 1 Segundo del largo especificado
		/// </summary>
		/// <param name="Segundos">Cantidad de segundos generada</param>
		/// <returns>RawDatos con la onda generada</returns>
		public static RawDatos GenerarBeep(int Segundos)
		{
			if (Segundos > 0)
			{
				WaveStream pcmStream2 = new WaveFileReader(Application.StartupPath + "\\beep.dat");
				byte[] Buf = new byte[0];
				for (int r = 0 ; r < Segundos ; r++)
				{
					int tamañoAnterior = Buf.Length;
					Array.Resize(ref Buf, (int)(Buf.Length + pcmStream2.Length));
					pcmStream2.Position = 0;
					pcmStream2.Read(Buf, tamañoAnterior, (int)pcmStream2.Length);
				}
				return new RawDatos(Buf, new RawFormat(48000, 16, 1));

			}
			else
			{
				return new RawDatos(new byte[0], new RawFormat(48000, 16, 1));
			}
		}

		/// <summary>
		///	Transfoma un archivo MP3 a "RawBytes"
		/// </summary>
		/// <param name="PathMP3">Path donde esta ubicado el archivo MP3</param>
		/// <returns>RawDatos con la informacion del archivo MP3</returns>
		public static RawDatos Mp3ArchivoToRawBytes(string PathMP3)
		{


			try
			{
				Mp3FileReader mReader = new Mp3FileReader(PathMP3);
				WaveStream mPcmStream = WaveFormatConversionStream.CreatePcmStream(mReader);
				//using ()
				//{
				//	using ()
				//	{
				try
				{
					byte[] mRetorno = new byte[mPcmStream.Length];
					int mLeido = mPcmStream.Read(mRetorno, 0, (int) mPcmStream.Length);
					return new RawDatos(mRetorno, new RawFormat(mPcmStream.WaveFormat.SampleRate, mPcmStream.WaveFormat.BitsPerSample, mPcmStream.WaveFormat.Channels));
				}
				catch (Exception)
				{
					return null;
				}
				finally
				{
					mReader.Close();
					mPcmStream.Close();
				}



				//	}
				//}

				//WaveStream pcmStream;
				//Mp3FileReader reader = new Mp3FileReader(PathMP3);
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				
			}

		}

		public static RawDatos Mp3StreamToRawBytes(System.IO.Stream streamMp3)
		{
			try
			{
				Mp3FileReader mReader = new Mp3FileReader(streamMp3);
				WaveStream mPcmStream = WaveFormatConversionStream.CreatePcmStream(mReader);
				
				try
				{
					byte[] mRetorno = new byte[mPcmStream.Length];
					int mLeido = mPcmStream.Read(mRetorno, 0, (int)mPcmStream.Length);
					return new RawDatos(mRetorno, new RawFormat(mPcmStream.WaveFormat.SampleRate, mPcmStream.WaveFormat.BitsPerSample, mPcmStream.WaveFormat.Channels));
				}
				catch (Exception)
				{
					return null;
				}
				finally
				{
					mReader.Close();
					mPcmStream.Close();
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
		/// <param name="Audio">Datos de audio "RawDatos"</param>
		/// <param name="OffsetmSeg">Offset en mseg dentro del archivo donde se comienza el muestreo</param>
		/// <param name="CantMuestras">Cantidad de muestras dentro del calculo</param>
		/// <returns>Un valor de 0-100%</returns>
		public static byte ValorPorcentualDelVolumen(RawDatos Audio, long OffsetmSeg, int CantMuestras)
		{
			if (OffsetmSeg < 0 || CantMuestras < 1)
			{
				throw new Exception();
			}
			long Promedio = 0;
			long NumeroMuestra = (OffsetmSeg * Audio.Formato.MuestrasPorSeg / 1000);
			for (int i = 0 ; i < CantMuestras ; i++)
			{
				long ValorMuestra = Audio.GetValorMuestraMono(NumeroMuestra + i);
				if (ValorMuestra > 32767)
				{
					ValorMuestra = 65536 - ValorMuestra;
				}
				Promedio = Promedio + ValorMuestra;
			}
			Promedio = Promedio / CantMuestras;
			return (byte)(Promedio * 100 / 32768);
		}
		
		/// <summary>
		/// Devuelve el valor Maximo de Volumen de 0 - 100% en una ubicacion determinada hasta la duracion indicada distribuyendo las muestras de forma proporcional.
		/// </summary>
		/// <param name="audio">Datos de audio "RawDatos"</param>
		/// <param name="offsetmSeg">Offset en mseg dentro del archivo donde se comienza el muestreo</param>
		/// <param name="duracion">La duracion del muestreo en mSeg</param>
		/// <param name="cantMuestras">Cantidad de muestras dentro del calculo</param>
		/// <returns>Un valor de 0-100%</returns>
		public static byte ValorMaximoDelVolumen(RawDatos audio, long offsetmSeg, long duracionMseg , int cantMuestras)
		{
			if (offsetmSeg < 0 || cantMuestras < 1)
			{
				throw new Exception();
			}
			long mValorMax = 0;
			long NumeroMuestra = (offsetmSeg * audio.Formato.MuestrasPorSeg / 1000);
			double mCalcSaltos = ((double)(audio.Formato.MuestrasPorSeg)) / 1000 * duracionMseg / cantMuestras;
			long mSaltoMuestras = (long) mCalcSaltos;

			if (mSaltoMuestras==0)
			{
				mSaltoMuestras = 1;
				cantMuestras = (int) (((double) (audio.Formato.MuestrasPorSeg))/1000*duracionMseg);
			}
			
			for (long i = 0 ; i < cantMuestras ; i++)
			{
				long ValorMuestra = audio.GetValorMuestraMono(NumeroMuestra + i * mSaltoMuestras);
				if (ValorMuestra > 32767)
				{
					ValorMuestra = 65536 - ValorMuestra;
				}

				if (ValorMuestra>mValorMax)
				{
					mValorMax = ValorMuestra;
				}	
			}
			return (byte)(mValorMax * 100 / 32768);
		}














		/// <summary>
		/// Devuelve un valor Double que indica la duracion del archivo en mSeg
		/// </summary>
		/// <param name="PathArchivo">Path donde esta ubicado el archivo MP3</param>
		/// <returns>Duracion del archivo en mSeg</returns>
		public static double DuracionmSegMp3(string PathArchivo)
		{
			RawDatos Audio = Mp3ArchivoToRawBytes(PathArchivo);
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
