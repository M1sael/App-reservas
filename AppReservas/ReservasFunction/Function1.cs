using AppReservas.ReservasDataAccess;
using AppReservas.ReservasModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppReservas.ReservasFunction
{
    public static class Function1
    {
        private static readonly ReservaDataAccess reservaDataAccess = new ReservaDataAccess("Server=DESKTOP-EGL2J80;Database=ReservasDB; Integrated Security=true;");

        [FunctionName("ObtenerReservas")]
        public static async Task<IActionResult> ObtenerReservas(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Recibida solicitud para obtener todas las reservas.");

            try
            {
                var reservas = reservaDataAccess.ObtenerReservas();
                return new OkObjectResult(reservas);
            }
            catch (Exception ex)
            {
                log.LogError($"Error al obtener las reservas: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("CrearReserva")]
        public static async Task<IActionResult> CrearReserva(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Recibida solicitud para crear una reserva.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Reserva>(requestBody);

            // Validar la solicitud
            //if (data == null || reservaDataAccess.ExisteReservaEnFecha(data.FechaReserva))
            //{
            //    return new BadRequestObjectResult("Fecha ya reservada o datos de reserva no válidos.");
            //}

            if (reservaDataAccess.ExisteReservaEnIntervalo(data.FechaInicio, data.FechaFin))
            {
                return new BadRequestObjectResult("El salón ya está reservado en ese intervalo de fechas.");
            }


            try
            {
                reservaDataAccess.CrearReserva(data);
                return new OkObjectResult("Reserva creada con éxito.");
            }
            catch (Exception ex)
            {
                log.LogError($"Error al crear la reserva: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("EditarReserva")]
        public static async Task<IActionResult> EditarReserva(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "EditarReserva/{id}")] HttpRequest req,
            int id,
            ILogger log)
        {
            log.LogInformation($"Recibida solicitud para editar la reserva con Id: {id}.");

            
            var reservaExistente = reservaDataAccess.ObtenerReservaPorId(id);
            if (reservaExistente == null)
            {
                return new NotFoundObjectResult($"Reserva con Id {id} no encontrada.");
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Reserva>(requestBody);

           
            //if (data == null || reservaDataAccess.ExisteReservaEnFecha(data.FechaReserva))
            //{
            //    return new BadRequestObjectResult("Fecha ya reservada o datos de reserva no válidos.");
            //}

            if (reservaDataAccess.ExisteReservaEnIntervalo(data.FechaInicio, data.FechaFin))
            {
                return new BadRequestObjectResult("El salón ya está reservado en ese intervalo de fechas.");
            }


            try
            {
                data.Id = id; 
                reservaDataAccess.EditarReserva(data);
                return new OkObjectResult("Reserva editada con éxito.");
            }
            catch (Exception ex)
            {
                log.LogError($"Error al editar la reserva: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("BorrarReserva")]
        public static async Task<IActionResult> BorrarReserva(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "BorrarReserva/{id}")] HttpRequest req,
            int id,
            ILogger log)
        {
            log.LogInformation($"Recibida solicitud para borrar la reserva con Id: {id}.");

            
            var reservaExistente = reservaDataAccess.ObtenerReservaPorId(id);
            if (reservaExistente == null)
            {
                return new NotFoundObjectResult($"Reserva con Id {id} no encontrada.");
            }

            try
            {
                reservaDataAccess.BorrarReserva(id);
                return new OkObjectResult("Reserva borrada con éxito.");
            }
            catch (Exception ex)
            {
                log.LogError($"Error al borrar la reserva: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }
    }
}
