using System;

namespace SECRON.Models
{
    public class Mdl_CourseLocationPricingMaster
    {
        public int CourseLocationPricingId { get; set; }
        public int CareerCourseId { get; set; }
        public int CareerPensumId { get; set; } // NUEVO: para poder seleccionar el pensum correcto al elegir una fila
        public int LocationId { get; set; }
        public int ModalityId { get; set; }

        // Datos denormalizados, solo para mostrar en grillas sin hacer JOIN en cada consulta
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string CareerCode { get; set; }
        public string CareerName { get; set; }
        public string LocationName { get; set; }
        public string ModalityName { get; set; }

        // Precio vigente, solo para mostrar en grilla (viene del JOIN con Detail)
        public decimal? CurrentPrice { get; set; }
        public DateTime? CurrentPriceEffectiveFrom { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}