using cliente.Models.DTOS;
using cliente.Services;

using GalaSoft.MvvmLight.Command;

using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace cliente.ViewModel
{
    public class FotosViewModel : INotifyPropertyChanged
    {
        public string Ip { get; set; } = "0.0.0.0";
        public bool Conectado { get; set; }
        public FotosService fotosservice { get; set; } = new FotosService();
        public ObservableCollection<string> Fotos { get; set; } = new ObservableCollection<string>();

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand EnviarFotoCommand { get; set; }
        public ICommand ConectarseCommand { get; set; }
        public ICommand TomarFotoCommand { get; set; }
        public ICommand EliminarFotoCommand { get; set; }

        public FotoDto Foto { get; set; } = new FotoDto();

        string _direccion = "";
        public string Direccion
        {
            get => _direccion;
            set
            {
                _direccion = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }
         
        public FotosViewModel()
        {
            EnviarFotoCommand = new RelayCommand(EnviarFoto);
            ConectarseCommand = new RelayCommand(Conectarse);
            TomarFotoCommand = new RelayCommand(TomarFoto);
            EliminarFotoCommand = new RelayCommand(EliminarFoto);
        }

        private void EliminarFoto()
        {
            if(Fotos.Contains(Direccion))
            {
                byte[] image = File.ReadAllBytes(Direccion);

                FotoDto foto = new FotoDto()
                {
                    usuario = Environment.UserName,
                    foto = Convert.ToBase64String(image)
                };

                fotosservice.EnviarMensaje(foto);
                Fotos.Remove(Direccion);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        private void TomarFoto()
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.png) | *.jpg; *.jpeg; *.jpe; *.png";
            dialog.ShowDialog();

            if (dialog.FileName != null)
            {
                Direccion = dialog.FileName;
                byte[] image = File.ReadAllBytes(dialog.FileName);

                Foto = new FotoDto()
                {
                    usuario = Environment.UserName,
                    foto = Convert.ToBase64String(image)
                };
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        private void Conectarse()
        {
            IPAddress.TryParse(Ip, out var ip);

            if (ip != null) 
            { 
                fotosservice.Conectar(ip);
                Conectado = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        private void EnviarFoto()
        {

            fotosservice.EnviarMensaje(Foto);
            Fotos.Add(Direccion);
            Foto = new FotoDto();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
