using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using servidor.Models.DTOS;
using System.Windows;

namespace servidor.Services
{
    internal class FotosService
    {
        TcpListener server = null!;

        public event EventHandler<FotoDto>? FotoRecibida;
        
        public void Iniciar()
        {
            server = new(new IPEndPoint(IPAddress.Any, 9000));
            server.Start();
            new Thread(Escuchar) { IsBackground = true }.Start();

        }

        void Escuchar()
        {
            while (server.Server.IsBound)
            {
                var tcpClient = server.AcceptTcpClient();

                Thread t = new(() =>
                {
                    RecibirMensajes(tcpClient);
                });
                t.IsBackground = true;
                t.Start();
            }
        }
        void RecibirMensajes(TcpClient cliente)
        {
            var ns = cliente.GetStream();
            byte[] lengthBuffer = new byte[4];
            while (cliente.Connected)
            {
                while (cliente.Available == 0)
                {
                    Thread.Sleep(500);
                }

                ns.ReadAsync(lengthBuffer, 0, 4);
                int messageLength = BitConverter.ToInt32(lengthBuffer);

                byte[] messageBuffer = new byte[messageLength];
                int bytesRead = 0;
                while (bytesRead < messageLength)
                {
                    bytesRead += ns.Read(messageBuffer, bytesRead, messageLength - bytesRead);
                }

                string json = Encoding.UTF8.GetString(messageBuffer);

                var mensaje = JsonSerializer.Deserialize<FotoDto>(json);

                if (mensaje != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        FotoRecibida?.Invoke(this, mensaje);

                    });
                }
            }
        }

        public void Detener()
        {
            try
            {
                if (server != null)
                {
                    server.Stop();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
