using AppReservas.ReservasModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace AppReservas.ReservasDataAccess
{
    public class ReservaDataAccess
    {
        private readonly string connectionString;

        public ReservaDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IEnumerable<Reserva> ObtenerReservas()
        {
            using IDbConnection dbConnection = new SqlConnection(connectionString);
            dbConnection.Open();
            return dbConnection.Query<Reserva>("SELECT * FROM Reservas");
        }

        public Reserva ObtenerReservaPorId(int reservaId)
        {
            using IDbConnection dbConnection = new SqlConnection(connectionString);
            dbConnection.Open();
            return dbConnection.QueryFirstOrDefault<Reserva>("SELECT * FROM Reservas WHERE Id = @Id", new { Id = reservaId });
        }

        public void CrearReserva(Reserva reserva)
        {
            using IDbConnection dbConnection = new SqlConnection(connectionString);
            dbConnection.Open();
            dbConnection.Execute("INSERT INTO Reservas (NombreCliente, FechaInicio, FechaFin) VALUES (@NombreCliente, @FechaInicio, @FechaFin)", reserva);
        }

        public void EditarReserva(Reserva reserva)
        {
            using IDbConnection dbConnection = new SqlConnection(connectionString);
            dbConnection.Open();
            dbConnection.Execute("UPDATE Reservas SET NombreCliente = @NombreCliente, FechaInicio = @FechaInicio, FechaFin = @FechaFin WHERE Id = @Id", reserva);
        }

        public void BorrarReserva(int reservaId)
        {
            using IDbConnection dbConnection = new SqlConnection(connectionString);
            dbConnection.Open();
            dbConnection.Execute("DELETE FROM Reservas WHERE Id = @Id", new { Id = reservaId });
        }

        //public bool ExisteReservaEnFecha(DateTime fechaReserva)
        //{
        //    // Lógica para verificar si ya hay una reserva en la fecha especificada
        //    using IDbConnection dbConnection = new SqlConnection(connectionString);
        //    dbConnection.Open();
        //    var existingReservation = dbConnection.QueryFirstOrDefault<Reserva>("SELECT * FROM Reservas WHERE FechaReserva = @FechaReserva", new { FechaReserva = fechaReserva });
        //    return existingReservation != null;
        //}

        public bool ExisteReservaEnIntervalo(DateTime fechaInicio, DateTime fechaFin)
        {
            using IDbConnection dbConnection = new SqlConnection(connectionString);
            dbConnection.Open();

            var existingReservation = dbConnection.QueryFirstOrDefault<Reserva>(
                "SELECT * FROM Reservas WHERE (FechaInicio <= @FechaFin AND FechaFin >= @FechaInicio) OR (FechaInicio <= @FechaInicio AND FechaFin >= @FechaFin)",
                new { FechaInicio = fechaInicio, FechaFin = fechaFin});

            return existingReservation != null;
        }
    }
}
