using System;

namespace SECRON.Models
{
    public class Mdl_AuditLog
    {
        public string Tabla { get; set; }
        public string Campo { get; set; }
        public string ValorAnterior { get; set; }
        public string ValorNuevo { get; set; }
        public string Action { get; set; }
        public DateTime ActionDate { get; set; }
        public string HostName { get; set; }
        public string IPAddress { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Rol { get; set; }
        public int TotalRegistros { get; set; }
    }
}