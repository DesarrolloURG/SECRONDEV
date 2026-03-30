using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    public class Mdl_Checks
    {
        // Campos principales
        public int CheckId { get; set; }
        public string CheckNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssuePlace { get; set; }

        // Montos
        public decimal Amount { get; set; }
        public decimal PrintedAmount { get; set; }

        // Beneficiario
        public string BeneficiaryName { get; set; }
        public int? EmployeeId { get; set; }

        // Información bancaria
        public int BankId { get; set; }
        public string BankAccountNumber { get; set; }

        // Estado y concepto
        public int StatusId { get; set; }
        public string Concept { get; set; }
        public string DetailDescription { get; set; }

        // Período y organización
        public string Period { get; set; }
        public int? LocationId { get; set; }
        public int? DepartmentId { get; set; }

        // Desgloses financieros
        public decimal Exemption { get; set; }
        public decimal TaxFreeAmount { get; set; }
        public decimal FoodAllowance { get; set; }
        public decimal IGSS { get; set; }
        public decimal WithholdingTax { get; set; }
        public decimal Retention { get; set; }
        public decimal Bonus { get; set; }
        public decimal Discounts { get; set; }
        public decimal Advances { get; set; }
        public decimal Viaticos { get; set; }

        // Referencias
        public string PurchaseOrderNumber { get; set; }
        public string Complement { get; set; }

        // Auditoría
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? AuthorizedDate { get; set; }
        public int? AuthorizedBy { get; set; }
        public DateTime? CashedDate { get; set; }

        // Propiedades agregadas
        public decimal Stamps { get; set; }
        public bool Predeclared { get; set; }

        // CAMPOS NUEVOS (en orden de creación en BD)
        public decimal Compensation { get; set; }   // Indemnización (índice 38)
        public decimal Vacation { get; set; }       // Vacaciones (índice 39)
        public string Bill { get; set; }            // Factura (índice 40)
        public decimal Aguinaldo { get; set; }      // Aguinaldo - SOLO LIQUIDACIONES (índice 41)
        public bool LastComplement { get; set; }    // Último complemento (índice 42)
        public string FileControl { get; set; }     // Control de archivo (índice 43)


        // Constructor vacío
        public Mdl_Checks()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
            IssuePlace = "GUATEMALA";
            Predeclared = false;
            LastComplement = false;

            // Inicializar valores por defecto
            Exemption = 0;
            TaxFreeAmount = 0;
            FoodAllowance = 0;
            IGSS = 0;
            WithholdingTax = 0;
            Retention = 0;
            Bonus = 0;
            Discounts = 0;
            Advances = 0;
            Viaticos = 0;
            Stamps = 0;
            Compensation = 0;
            Vacation = 0;
            Aguinaldo = 0;
            FileControl = "PENDIENTE";
        }

        // Constructor con parámetros principales
        public Mdl_Checks(string checkNumber, DateTime issueDate, decimal amount, string beneficiaryName,
            int bankId, int statusId, string concept, string period, string fileControl)
        {
            this.CheckNumber = checkNumber;
            this.IssueDate = issueDate;
            this.Amount = amount;
            this.PrintedAmount = amount;
            this.BeneficiaryName = beneficiaryName;
            this.BankId = bankId;
            this.StatusId = statusId;
            this.Concept = concept;
            this.Period = period;
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
            this.IssuePlace = "GUATEMALA";
            this.Predeclared = false;
            this.LastComplement = false;
            this.FileControl = fileControl;

            // Inicializar valores financieros
            this.Exemption = 0;
            this.TaxFreeAmount = 0;
            this.FoodAllowance = 0;
            this.IGSS = 0;
            this.WithholdingTax = 0;
            this.Retention = 0;
            this.Bonus = 0;
            this.Discounts = 0;
            this.Advances = 0;
            this.Viaticos = 0;
            this.Stamps = 0;
            this.Compensation = 0;
            this.Vacation = 0;
            this.Aguinaldo = 0;
        }
    }
}