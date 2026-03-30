using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_MovementTypes
    {
        public int MovementTypeId { get; set; }
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public string AffectsStock { get; set; }
        public bool RequiresSupplier { get; set; }
        public bool RequiresDestination { get; set; }
        public bool IsActive { get; set; }

        public Mdl_MovementTypes()
        {
            IsActive = true;
        }
    }
}