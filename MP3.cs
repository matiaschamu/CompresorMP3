using System;
using System.IO;
using BibliotecaMaf.Clases.Audio.Lame;

namespace BibliotecaMaf.Clases.Audio
{
	[Obsolete("por favor use en su lugar NAudio.Lame.LameMP3FileWriter. ")]
	public class ConversorMP3 : BinaryWriter
	{
		private bool mDisposed = false;
		private bool mClosed = false;
		private uint m_HandleStream;
		private uint m_Samples;
		private byte[] m_BufferDeSalida;
		private uint m_TamBufferDeSalidaCodec;
		private uint m_TamBufferDeEntradaCodec;
		private BE_CONFIG m_pbeConfig;
		private byte[] m_BufferDeEntrada = new byte[0];

		private bool mFlushEjecutado = false;

		//Calidad predefinida en LAME
		public ConversorMP3(Stream output, Audio.Lame.Calidad calidadPredefinida)
			: this(output, calidadPredefinida, 0, LHV1.TipoDeCodificacion.Predefinida, new RawFormat(44100, 16, 1))
		{
		}

		//Calidad personalizada (VBR)
		public ConversorMP3(Stream output, Audio.Lame.CalidadVBR calidad)
			: this(output, Audio.Lame.Calidad.CalidadNormal, (int)calidad, LHV1.TipoDeCodificacion.VBR, new RawFormat(44100, 16, 1))
		{
		}

		[Obsolete("por favor use en su lugar public ConversorMP3(Stream output, Audio.Lame.BitRates calidad, RawFormat formato).")]
		//Calidad personalizada (CBR)
		public ConversorMP3(Stream output, Audio.Lame.BitRates calidad, int Muestras)
			: this(output, Audio.Lame.Calidad.CalidadNormal, (int)calidad, LHV1.TipoDeCodificacion.CBR, new RawFormat(Muestras, 16, 1))
		{
		}

		public ConversorMP3(Stream output, Audio.Lame.BitRates calidad, RawFormat formato)
			: this(output, Audio.Lame.Calidad.CalidadNormal, (int)calidad, LHV1.TipoDeCodificacion.CBR, formato)
		{

		}

		private ConversorMP3(Stream output, Audio.Lame.Calidad calidadPredefinida, int nivel, LHV1.TipoDeCodificacion tipo, RawFormat formato)
			: base(output)
		{
			uint Ret;

			//Creo la estrucura BE_CONFIG, según los parametros
			m_pbeConfig = new BE_CONFIG(calidadPredefinida, nivel, tipo, formato);

			//Inicializo
			int TamanoArquitectura = IntPtr.Size;
			if (TamanoArquitectura == 4)
			{
				Ret = Audio.Lame.NativeMethods.beInitStream(m_pbeConfig, ref m_Samples, ref m_TamBufferDeSalidaCodec, ref m_HandleStream);
			}
			else
			{
				Ret = Audio.Lame.NativeMethods.beInitStream_64(m_pbeConfig, ref m_Samples, ref m_TamBufferDeSalidaCodec, ref m_HandleStream);
			}

			if (Ret != Audio.Lame.NativeMethods.SUCCESSFUL)
			{
				if (TamanoArquitectura == 4)
				{
					Audio.Lame.NativeMethods.beCloseStream(m_HandleStream);
				}
				else
				{
					Audio.Lame.NativeMethods.beCloseStream_64(m_HandleStream);
				}
				throw new ApplicationException("Error", new Exception());
			}

			//Dimensiono los buffers de entrada y salida, segun la informacion retornada por beInitStream
			m_BufferDeSalida = new byte[m_TamBufferDeSalidaCodec];
			m_TamBufferDeEntradaCodec = 2 * m_Samples;
		}

		public int TamañoDelBuffer()
		{
			return (int)m_TamBufferDeEntradaCodec;
		}

		protected override void Dispose(bool Disposing)
		{
			if (!this.mDisposed)
			{
				try
				{
					if (Disposing)
					{
						// Release the managed resources you added in
						// this derived class here.
						m_BufferDeSalida = null;
						m_pbeConfig = null;
						m_BufferDeEntrada = null;
					}
					// Release the native unmanaged resources you added
					CerrarHandler();
				}
				catch
				{
					//int a = 8;
				}
				finally
				{
					this.mDisposed = true;
					base.Dispose(Disposing);
				}
			}
		}

		[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
		private void CerrarHandler()
		{
			if (this.mClosed == false)
			{
				try
				{
					int TamanoArquitectura = IntPtr.Size;
					if (TamanoArquitectura == 4)
					{
						Audio.Lame.NativeMethods.beCloseStream(m_HandleStream);
					}
					else
					{
						Audio.Lame.NativeMethods.beCloseStream_64(m_HandleStream);
					}
				}
				catch
				{
				}
				finally
				{
					this.mClosed = true;
				}
			}
		}

		public override void Close()
		{
			int TamanoArquitectura = IntPtr.Size;
			uint Ret = 1;
			uint CantidadCodificada = 0;
			try
			{
				//Termino la conversion
				//Write(m_BufferDeEntrada, 0,  m_BufferDeEntrada.Length);
				if (mFlushEjecutado == false)
				{

					Ret = Audio.Lame.NativeMethods.Convertir(m_HandleStream, m_BufferDeEntrada, 0, (uint)m_BufferDeEntrada.Length, m_BufferDeSalida, ref CantidadCodificada);
					if (Ret == Audio.Lame.NativeMethods.SUCCESSFUL)
					{
						//Grabo la informacion convertida
						if (CantidadCodificada > 0)
						{
							try
							{
								base.Write(m_BufferDeSalida, 0, (int)CantidadCodificada);
							}
							catch
							{
								CerrarHandler();
								throw;
							}
						}
					}

					CantidadCodificada = 0;

					Ret = 1;

					if (TamanoArquitectura == 4)
					{
						Ret = Audio.Lame.NativeMethods.beDeinitStream(m_HandleStream, m_BufferDeSalida, ref CantidadCodificada);
					}
					else
					{
						Ret = Audio.Lame.NativeMethods.beDeinitStream_64(m_HandleStream, m_BufferDeSalida, ref CantidadCodificada);
					}

					if (Ret == Audio.Lame.NativeMethods.SUCCESSFUL)
					{
						if (CantidadCodificada > 0)
						{
							try
							{
								base.Write(m_BufferDeSalida, 0, (int)CantidadCodificada);
							}
							catch
							{
								CerrarHandler();
								throw;
							}
						}
					}
				}
				CerrarHandler();
			}
			finally
			{
				base.Close();
			}
		}

		public override void Flush()
		{
			int TamanoArquitectura = IntPtr.Size;
			uint Ret = 1;
			uint CantidadCodificada = 0;
			try
			{
				//Termino la conversion
				//Write(m_BufferDeEntrada, 0,  m_BufferDeEntrada.Length);
				Ret = Audio.Lame.NativeMethods.Convertir(m_HandleStream, m_BufferDeEntrada, 0, (uint)m_BufferDeEntrada.Length, m_BufferDeSalida, ref CantidadCodificada);
				if (Ret == Audio.Lame.NativeMethods.SUCCESSFUL)
				{
					//Grabo la informacion convertida
					if (CantidadCodificada > 0)
					{
						try
						{
							base.Write(m_BufferDeSalida, 0, (int)CantidadCodificada);
						}
						catch
						{
							CerrarHandler();
							throw;
						}
					}
				}

				CantidadCodificada = 0;

				Ret = 1;

				if (TamanoArquitectura == 4)
				{
					Ret = Audio.Lame.NativeMethods.beDeinitStream(m_HandleStream, m_BufferDeSalida, ref CantidadCodificada);
				}
				else
				{
					Ret = Audio.Lame.NativeMethods.beDeinitStream_64(m_HandleStream, m_BufferDeSalida, ref CantidadCodificada);
				}

				if (Ret == Audio.Lame.NativeMethods.SUCCESSFUL)
				{
					if (CantidadCodificada > 0)
					{
						try
						{
							base.Write(m_BufferDeSalida, 0, (int)CantidadCodificada);
						}
						catch
						{
							CerrarHandler();
							throw;
						}
					}
				}
				CerrarHandler();
			}
			finally
			{
				base.Flush();
				mFlushEjecutado = true;
			}
		}

		public override void Write(byte[] InfoAConvertir, int Indice, int Cantidad)
		{

			if (Cantidad == 0)
			{
				return;
			}

			uint Ret = 0;
			uint CantidadCodificada = 0;
			int TamañoBufferEntrada = m_BufferDeEntrada.Length;
			Array.Resize(ref m_BufferDeEntrada, (m_BufferDeEntrada.Length + Cantidad));
			Array.Copy(InfoAConvertir, Indice, m_BufferDeEntrada, TamañoBufferEntrada, Cantidad);

			while (m_BufferDeEntrada.Length > m_TamBufferDeEntradaCodec)
			{
				//Convierto la porcion del archivo, enviada en el array InfoAConvertir
				Ret = Audio.Lame.NativeMethods.Convertir(m_HandleStream, m_BufferDeEntrada, 0, (uint)m_TamBufferDeEntradaCodec, m_BufferDeSalida, ref CantidadCodificada);

				byte[] Temp = new byte[m_BufferDeEntrada.Length - m_TamBufferDeEntradaCodec];
				Array.Copy(m_BufferDeEntrada, m_TamBufferDeEntradaCodec, Temp, 0, Temp.Length);
				m_BufferDeEntrada = Temp;

				if (Ret == Audio.Lame.NativeMethods.SUCCESSFUL)
				{
					//Grabo la informacion convertida
					if (CantidadCodificada > 0)
					{
						try
						{
							base.Write(m_BufferDeSalida, 0, (int)CantidadCodificada);
						}
						catch
						{
							Array.Resize(ref m_BufferDeEntrada, 0);
							throw;
						}
					}
				}
			}
		}
	}
}