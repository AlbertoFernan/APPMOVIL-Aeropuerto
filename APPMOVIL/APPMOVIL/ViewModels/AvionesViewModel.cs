using APPMOVIL.Models;
using APPMOVIL.Services;
using APPMOVIL.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Xamarin.Forms;

namespace APPMOVIL.ViewModels
{
    public enum door { A01, B01, C01, D01 };
    public class AvionesViewModel : INotifyPropertyChanged
    {

        public DateTime Fecha { get; set; }

        public DateTime FechaFiltro { get; set; }
        public TimeSpan Hora { get; set; }
        public List<string> Puertas{ get; set; }

   
        public Partidas Partida { get; set; }

        public List<Partidas> Partidas { get; set; }
        public List<Partidas> PartidasFiltradas { get; set; }

        AvionesService AvionesService { get; set; }
     
        AgregarVuelo VistaAgregar;
        EditarVueloView VistaEditar;
        ListaVuelosView VistaVuelos;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Errores { get; set; }

        public ICommand VerAgregarCommand { get; set; }
        public ICommand FiltrarCommand { get; set; }
        public ICommand VerEditarCommand { get; set; }
        public ICommand VerListaCommand { get; set; }

        public ICommand AgregarCommand { get; set; }
        public ICommand GuardarCommand { get; set; }
        public ICommand CancelarCommand { get; set; }

        public ICommand CancelarAccionCommand { get; set; }
        private static System.Timers.Timer aTimer;


        public AvionesViewModel()
        {
            Puertas = new List<string>();
            foreach (var item in Enum.GetValues(typeof(door)))
            {
                Puertas.Add(item.ToString()) ;
            }

            AvionesService = new AvionesService();
            AvionesService.Error += AvionesService_Error;
            VerListaCommand = new Command(VerLista);
            VerEditarCommand = new Command<Partidas>(VerEditar);
            FiltrarCommand = new Command(Filtrar);
            GuardarCommand = new Command(Guardar);
            VerAgregarCommand = new Command(VerAgregar);
            AgregarCommand = new Command(Agregar);
            CancelarCommand = new Command<Partidas>(Cancelar);
            CancelarAccionCommand= new Command(RegresarAsync);

            FechaFiltro =DateTime.Now.Date;
        }

        private async void RegresarAsync()
        {
            ActualizarLista();
            await Application.Current.MainPage.Navigation.PopAsync();

        }

        private void Filtrar()
        {
            PartidasFiltradas = Partidas.Select(x => x).Where(x => x.Tiempo.Date == FechaFiltro).ToList();
            Actualizar(nameof(PartidasFiltradas));
        }

        private async void Guardar()
        {
            Partida.Vuelo = Partida.Vuelo.ToUpper();
            if (await AvionesService.Update(Partida))
            {

                
                RegresarAsync();
            }
        }

        private async void Cancelar(Partidas p)
        {
    
            if(p.Status!= "Cancelado")
            {
                p.Status = "Cancelado";
                await AvionesService.Update(p);
                Filtrar();
            }
          
        }

        private  void SetTimer()
        {
          
            aTimer = new System.Timers.Timer(20000);
       
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


            // DateTime fechaactual = DateTime.Now;
            //// TimeSpan horaactual = DateTime.Now.TimeOfDay;

            // //foreach (var item in Partidas)
            // //{


            // //    if ((item.Fecha.Date <= fechaactual) && ((item.Hora - horaactual).TotalMinutes < 10))
            // //    {
            // //        item.Status = "On Boarding";
            // //        await AvionesService.Update(item);
            // //    }

            // //}


            // foreach (var item in Partidas)
            // {


            //     if (item.Tiempo.Date <= fechaactual && item.Status!="On Boarding" )
            //     {
            //         if (((item.Tiempo.TimeOfDay - fechaactual.TimeOfDay).TotalMinutes) < 10)
            //         {
            //             item.Status = "On Boarding";
            //             await AvionesService.Update(item);
            //         }



            //     }


            // }
            PartidasFiltradas = Partidas.Select(x => x).Where(x => x.Tiempo.Date == FechaFiltro).ToList();
            Actualizar(nameof(PartidasFiltradas));
           

          

        }


        private void AvionesService_Error(List<string> obj)
        {

            Errores = string.Join("\n", obj); 
            Actualizar(nameof(Errores)); Actualizar(nameof(Partidas));
        }

        private async void Agregar()
        {
            Partida.Vuelo = Partida.Vuelo.ToUpper();
            Partida.Status = "Programado";
            DateTime tiempo = new DateTime(Fecha.Year, Fecha.Month, Fecha.Day, Hora.Hours, Hora.Minutes, Hora.Seconds);
            Partida.Tiempo= tiempo;
            if ((Partida.Tiempo>DateTime.Now))

            {
                if (await AvionesService.Insert(Partida))
                {

                    RegresarAsync();
                }
            }
            else
            {
                Errores = "Ingrese una fecha futura";
            }

            Actualizar(nameof(Errores));



        }

        private void VerAgregar()
        {
            Errores = null;
            Partida = new Partidas();
            Fecha= DateTime.Now.Date;
           Hora = DateTime.Now.TimeOfDay;
          VistaAgregar = new AgregarVuelo() { BindingContext = this };

            Application.Current.MainPage.Navigation.PushAsync(VistaAgregar);
             

        }

        private void VerEditar(Partidas p)
        {
            Errores = null;
            Partida = p;

           
            
            VistaEditar = new EditarVueloView() { BindingContext = this };

            Application.Current.MainPage.Navigation.PushAsync(VistaEditar);


        }


        public void Actualizar(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
