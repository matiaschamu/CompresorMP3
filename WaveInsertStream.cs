using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BibliotecaMaf.Clases.Audio
{
	//class WaveInsertStream : NAudio.Wave.WaveStream
	//{
	//	//private System.IO.Stream sourceStream;
	//	private NAudio.Wave.WaveFormat waveFormat;
	//	private BibliotecaMaf.Clases.Audio.RawDatos mDatos;
	//	private long mPosicion = 0;
	//	private bool mFinalAlcanzado = false;

	//	public WaveInsertStream()
	//	{

	//	}

	//	public WaveInsertStream(RawDatos Datos)
	//	{
	//		mDatos = new RawDatos(Datos.DatosRaw, Datos.Formato);
	//		this.waveFormat = new NAudio.Wave.WaveFormat(Datos.Formato.MuestrasPorSeg, Datos.Formato.Bits, Datos.Formato.Canales);
	//	}

	//	public WaveInsertStream(System.IO.Stream sourceStream, NAudio.Wave.WaveFormat waveFormat)
	//	{
	//		byte[] Temp = new byte[sourceStream.Length];
	//		RawFormat Format = new RawFormat(waveFormat.SampleRate, waveFormat.BitsPerSample, waveFormat.Channels);
	//		mDatos = new RawDatos(Temp, Format);
	//		//this.sourceStream = sourceStream;
	//		this.waveFormat = waveFormat;
	//	}

	//	public void InsertarDatos (byte[] Datos)
	//	{
			

	//	}
	//	public void InsertarDatos(RawDatos Datos)
	//	{
	//		InsertarDatos(Datos.DatosRaw);
	//	}

	//	public override NAudio.Wave.WaveFormat WaveFormat
	//	{
	//		get
	//		{
	//			return this.waveFormat;
	//		}
	//	}

	//	public override long Length
	//	{
	//		get
	//		{
	//			return long.MaxValue;
	//		}
	//	}

	//	public bool FinalAlcanzado
	//	{
	//		get
	//		{
	//			return mFinalAlcanzado;
	//		}
	//	}

	//	public override long Position
	//	{
	//		get
	//		{
	//			return mPosicion;
	//		}
	//		set
	//		{
	//			mPosicion = value;
	//		}
	//	}

	//	public override int Read(byte[] buffer, int offset, int count)
	//	{
	//		//try
	//		//{

	//			//for (int i = 0 ; i < buffer.Length ; i=i+8)
	//			//{
	//			//	buffer[i] = 0;
	//			//	buffer[i+1] = 0;
	//			//	buffer[i + 2] = 255;
	//			//	buffer[i + 3] = 50;
	//			//	buffer[i+4] = 0;
	//			//	buffer[i + 5] = 0;
	//			//	buffer[i + 6] = 0;
	//			//	buffer[i + 7] = 0;

	//			//}
	//			//return count;


	//				if ((mDatos.DatosRaw.LongLength - mPosicion) > count)
	//				{
	//					Array.Copy(mDatos.DatosRaw, mPosicion, buffer, offset, count);
	//					mPosicion = mPosicion + count - 1;
	//					return count;
	//				}
	//				else
	//				{
	//					Array.Copy(mDatos.DatosRaw, mPosicion, buffer, offset, mDatos.DatosRaw.Length - mPosicion);
	//					mPosicion = mDatos.DatosRaw.Length;
	//					mFinalAlcanzado = true;
	//					return count;

	//				}
	//		//}
	//		//catch
	//		//{
	//		//	int a = 9;
	//		//	return 0;
	//		//}
	//	}
	//}
}
