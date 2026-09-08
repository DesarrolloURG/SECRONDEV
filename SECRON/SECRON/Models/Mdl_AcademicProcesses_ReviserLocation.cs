using System;

namespace SECRON.Models
{
    internal class Mdl_AcademicProcesses_ReviserLocation
    {
        public int ReviserLocationId { get; set; }
        public int ReviserId { get; set; }

        public int LocationId { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }

        public bool IsActive { get; set; }

        public int? AssignedBy { get; set; }
        public string AssignedByName { get; set; }
        public DateTime AssignedDate { get; set; }

        public int? RemovedBy { get; set; }
        public string RemovedByName { get; set; }
        public DateTime? RemovedDate { get; set; }

        public Mdl_AcademicProcesses_ReviserLocation()
        {
            IsActive = true;
        }
    }
}