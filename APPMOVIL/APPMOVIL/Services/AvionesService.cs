using APPMOVIL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace APPMOVIL.Services
{
    public class AvionesService
    {


        HttpClient cliente = new HttpClient
        {
            BaseAddress = new Uri("https://avionesaf.sistemas19.com/")
        };

        public event Action<List<string>> Error;
        public async Task<bool> Insert(Partidas p)
        {
            //Validar

            var json = JsonConvert.SerializeObject(p);
            var response = await cliente.PostAsync("api/Aviones", new StringContent(json, Encoding.UTF8,
                "application/json"));
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) //BadRequest
            {
                var errores = await response.Content.ReadAsStringAsync();
                LanzarErrorJson(errores);
                return false;
            }
            return true;
        }

        public async Task<List<Partidas>> GetVuelos()
        {
            List<Partidas> categorias = null;

            var response = await cliente.GetAsync("api/Aviones");

            if (response.IsSuccessStatusCode) //status= 200 ok
            {
                var json = await response.Content.ReadAsStringAsync();
                categorias = JsonConvert.DeserializeObject<List<Partidas>>(json);
            }

            if (categorias != null)
            {
                return categorias;
            }
            else
            {
                return new List<Partidas>();
            }
        }


        public async Task<bool> Update(Partidas p)
        {
            //Validar

            var json = JsonConvert.SerializeObject(p);
            var response = await cliente.PutAsync("api/Aviones", new StringContent(json, Encoding.UTF8,
                "application/json"));
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) //BadRequest
            {
                var errores = await response.Content.ReadAsStringAsync();
                LanzarErrorJson(errores);
                return false;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                LanzarError("No se encontro el producto");
            }
            return true;
        }
        void LanzarError(string mensaje)
        {
            Error?.Invoke(new List<string> { mensaje });
        }
        void LanzarErrorJson(string json)
        {
            List<string> obj = JsonConvert.DeserializeObject<List<string>>(json);
            if (obj != null)
            {
                Error?.Invoke(obj);
            }
        }




    }
}
