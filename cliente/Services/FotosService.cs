using cliente.Models.DTOS;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace cliente.Services
{
    public class FotosService
    {
        TcpClient cliente = null!;
        public string Equipo { get; set; } = null!;
        public void Conectar(IPAddress ip)
        {

            try
            {
                IPEndPoint ipe = new(ip, 9000);

                cliente = new TcpClient();
                cliente.Connect(ipe);

                Equipo = Environment.UserName;

                var msg = new FotoDto
                {
                    usuario = "HELLO"
                };

                EnviarMensaje(msg);

            }
            catch (Exception ex)
            {

            }
        }
        public void Desconectar()
        {
            try
            {
                var msg = new FotoDto
                {
                     usuario = "BYE"
                };

                EnviarMensaje(msg);

                cliente.Close();
            }
            catch (Exception ex)
            {
            }
        }

        public void EnviarMensaje(FotoDto foto)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(foto.foto))
                {
                    string json = JsonSerializer.Serialize(foto);

                    byte[] buffer = Encoding.UTF8.GetBytes(json);
                    byte[] lengthBuffer = BitConverter.GetBytes(buffer.Length);

                    NetworkStream ns = cliente.GetStream();
                    ns.Write(lengthBuffer);                 
                    ns.Write(buffer, 0, buffer.Length);
                    ns.Flush();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
