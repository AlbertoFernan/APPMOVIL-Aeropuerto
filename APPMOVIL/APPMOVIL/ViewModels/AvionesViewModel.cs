using APPMOVIL.Models;
using APPMOVIL.Services;
using APPMOVIL.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Xamarin.Forms;

namespace APPMOVIL.ViewModels
{
    public enum door { A01, B01, C01, D01 };
    public class AvionesViewModel:INotifyPropertyChanged
    {

        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public Array Puertas{ get; set; }
        public Partidas Partida { get; set; }

        public List<Partidas> Partidas { get; set; }

       AvionesService AvionesService { get; set; }
     
        AgregarVuelo VistaAgregar;
        ListaVuelosView VistaVuelos;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Errores { get; set; }

        public ICommand VerAgregarCommand { get; set; }
        public ICommand VerListaCommand { get; set; }

        public ICommand AgregarCommand { get; set; }

        private static System.Timers.Timer aTimer;


        public AvionesViewModel()
        {
            Puertas = Enum.GetValues(typeof(door));
            AvionesService = new AvionesService();
            AvionesService.Error += AvionesService_Error;
            VerListaCommand = new Command(VerLista);
           VerAgregarCommand = new Command(VerAgregar);
            AgregarCommand = new Command(Agregar);

           
        }
        private  void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(10000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += ATimer_ElapsedAsync;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private  async void ATimer_ElapsedAsync(object sender, ElapsedEventArgs e)
        {
           await ActualizarLista();
     
        }

        private async void VerLista()
        {
           await ActualizarLista();
           SetTimer();

            VistaVuelos = new ListaVuelosView() { BindingContext = this };

            await Application.Current.MainPage.Navigation.PushAsync(VistaVuelos);

        }

        private async Task ActualizarLista()
        {
          
                Partidas = await AvionesService.GetVuelos();
            
          
            DateTime fechaactual = DateTime.Now;
           // TimeSpan horaactual = DateTime.Now.TimeOfDay;

            //foreach (var item in Partidas)
            //{


            //    if ((item.Fecha.Date <= fechaactual) && ((item.Hora - horaactual).TotalMinutes < 10))
            //    {
            //        item.Status = "On Boarding";
            //        await AvionesService.Update(item);
            //    }

            //}


            foreach (var item in Partidas)
            {


                if (item.Tiempo.Date <= fechaactual && item.Status!="On Boarding" )
                {
                    if (((item.Tiempo.TimeOfDay - fechaactual.TimeOfDay).TotalMinutes) < 10)
                    {
                        item.Status = "On Boarding";
                        await AvionesService.Update(item);
                    }
           


                }
              

            }
            Actualizar(nameof(Partidas));
           

          

        }


        private void AvionesService_Error(List<string> obj)
        {

            Errores = string.Join("\n", obj); 
            Actualizar(nameof(Errores)); Actualizar(nameof(Partidas));
        }

        private async void Agregar()
        {
           
            Partida.Status = "on Time";
            DateTime tiempo = new DateTime(Fecha.Year, Fecha.Month, Fecha.Day, Hora.Hours, Hora.Minutes, Hora.Seconds);
            Partida.Tiempo= tiempo;
            if (await AvionesService.Insert(Partida))
            {
                Actualizar(nameof(Partidas));
                VerLista();
            }
           
          
           
        }

        private void VerAgregar()
        {

            Partida = new Partidas();
            Fecha= DateTime.Now.Date;
           Hora = DateTime.Now.TimeOfDay;
          VistaAgregar = new AgregarVuelo() { BindingContext = this };

            Application.Current.MainPage.Navigation.PushAsync(VistaAgregar);
             

        }

        public void Actualizar(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
