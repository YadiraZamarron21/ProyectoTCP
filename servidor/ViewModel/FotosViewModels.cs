﻿using GalaSoft.MvvmLight.Command;
using System.Text;
using servidor.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using cliente.Models;
using System;
using System.Net;

using servidor.Models.DTOS;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
namespace servidor.ViewModel
{
    internal class FotosViewModels : INotifyPropertyChanged
    {
        public FotosService Server { get; set; } = new FotosService();
        public ObservableCollection<string> Usuarios { get; set; } = new ObservableCollection<string>();
        public ICommand IniciarServerCommand { get; set; }
        public ICommand DetenerServerCommand { get; set; }
        public string IP { get; set; } = "0.0.0.0";
        public ObservableCollection<FotoModel> Fotos { get; set; } = new();
        public int NumMensaje { get; set; }

        public FotosViewModels()
        {
            var direcciones = Dns.GetHostAddresses(Dns.GetHostName());
            if (direcciones != null)
            {
                IP = string.Join(",", direcciones.Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).Select(x => x.ToString()));
            }
            IniciarServerCommand = new RelayCommand(IniciarServer);
            DetenerServerCommand = new RelayCommand(DetenerCommand);
            Server.FotoRecibida += Server_MensajeRecibido;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));

            IniciarServer();
        }

        private void Server_MensajeRecibido(object? sender, FotoDto e)
        {
            if (Usuarios.Contains(e.usuario))
            {
                var foto = Fotos.FirstOrDefault(x => x.base64 == e.foto);

                if (foto != null)
                {
                    Fotos.Remove(foto);
                }
                else
                {
                    byte[] binaryData = Convert.FromBase64String(e.foto);

                    BitmapImage bi = new BitmapImage();

                    bi.BeginInit();
                    bi.StreamSource = new MemoryStream(binaryData);
                    bi.EndInit();

                    Fotos.Add(new FotoModel()
                    {
                        fotobitmap = bi,
                        base64 = e.foto,
                    });
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
            else
            {
                Usuarios.Add(e.usuario);

                var foto = Fotos.FirstOrDefault(x => x.base64 == e.foto);

                if (foto != null)
                {
                    Fotos.Remove(foto);
                }
                else
                {
                    byte[] binaryData = Convert.FromBase64String(e.foto);

                    BitmapImage bi = new BitmapImage();

                    bi.BeginInit();
                    bi.StreamSource = new MemoryStream(binaryData);
                    bi.EndInit();

                    Fotos.Add(new FotoModel()
                    {
                        fotobitmap = bi,
                        base64 = e.foto,
                    });
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }


            }
        }

        public void DetenerCommand()
        {
            Fotos.Clear();
            Usuarios.Clear();
            Server.Detener();
        }
        public void IniciarServer()
        {
            Server.Iniciar();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
