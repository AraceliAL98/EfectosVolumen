using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Reproductor
{
    class Delay : ISampleProvider

    {
        private ISampleProvider fuente;
        public int offsetMilisegundos { get; set; }
        private int cantidadMuestrasOffset;

        private List<float> bufferDelay = new List<float>();


        private int duracionBufferSegundos;
        private int tamañoBuffer;
        private int cantidadMuestrasTranscurridas = 0;
        private int cantidadMuestrasBorradas = 0;


        public WaveFormat WaveFormat
        {
            get
            {
                return fuente.WaveFormat;

            }
        }

        public Delay(ISampleProvider fuente)
        {

            this.fuente = fuente;
            offsetMilisegundos = 500;

           cantidadMuestrasOffset = (int)
                (((float) offsetMilisegundos / 1000.0f) * (float)fuente.WaveFormat.SampleRate);
            
            duracionBufferSegundos = 10;
            tamañoBuffer = fuente.WaveFormat.SampleRate * duracionBufferSegundos;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            //Leemos las muestras de la señal fuente
            var read = fuente.Read(buffer, offset, count);
            
            //calcular tiempo
            float tiempoTranscurrido = 
                (float)cantidadMuestrasTranscurridas /
                (float)fuente.WaveFormat.SampleRate;

            float MilisegundosTranscurridos = tiempoTranscurrido * 1000.0f;



            //Llenando el buffer
            for (int i = 0; 1 < read; i++)
            {
                bufferDelay.Add(buffer[i + offset]);

            }

            //Elimiar excedentes del buffer
            if(bufferDelay.Count > tamañoBuffer)
            {
                int diferencia = bufferDelay.Count - tamañoBuffer;
                bufferDelay.RemoveRange(0, diferencia);
                cantidadMuestrasBorradas += diferencia;

            }

            //Aplicar el efecto

            if (MilisegundosTranscurridos > offsetMilisegundos)
            {
                for( int i = 0; i < read; i++)
                {
                    buffer[offset + i] +=
                        bufferDelay[cantidadMuestrasTranscurridas - cantidadMuestrasBorradas + i - cantidadMuestrasOffset];


                }


            }
            cantidadMuestrasTranscurridas +=
                read;

            return read;
        }
    }
}
