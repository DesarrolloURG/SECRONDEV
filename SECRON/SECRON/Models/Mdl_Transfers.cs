using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_Transfers
    {
        // Campos principales
        public int TransferId { get; set; }
        public string TransferNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssuePlace { get; set; }

        // Montos principales
        public decimal Amount { get; set; }
        public decimal PrintedAmount { get; set; }

        // Beneficiario
        public string BeneficiaryName { get; set; }
        public int? EmployeeId { get; set; }

        // Información bancaria
        public int BankId { get; set; }
        public string BankAccountNumber { get; set; }
        public int BanksAccountTypeId { get; set; }   // NUEVO: FK a BanksAccountTypes

        // Estado
        public int StatusId { get; set; }

        // Concepto
        public string Concept { get; set; }
        public string DetailDescription { get; set; }

        // Otros datos administrativos
        public string Period { get; set; }
        public int? LocationId { get; set; }
        public int? DepartmentId { get; set; }

        // Campos de montos adicionales (igual que cheques)
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
        public decimal Stamps { get; set; }
        public decimal Compensation { get; set; }
        public decimal Vacation { get; set; }
        public decimal Aguinaldo { get; set; }

        // Documento asociado / referencia
        public string PurchaseOrderNumber { get; set; }
        public string Bill { get; set; }

        // Complementos
        public string Complement { get; set; }
        public bool LastComplement { get; set; }

        // FileControl
        public string FileControl { get; set; }

        // Auditoría
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? AuthorizedDate { get; set; }
        public int? AuthorizedBy { get; set; }
        public DateTime? CashedDate { get; set; }

        // Constructor vacío con valores por defecto
        public Mdl_Transfers()
        {
            IssueDate = DateTime.Today;
            IssuePlace = "GUATEMALA";

            Amount = 0;
            PrintedAmount = 0;

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

            IsActive = true;
            FileControl = "PENDIENTE";

            LastComplement = false;
        }

        // Constructor corto (los campos que casi siempre necesitas)
        public Mdl_Transfers(string transferNumber, DateTime issueDate, decimal amount,
            string beneficiaryName, int bankId, int banksAccountTypeId,
            int statusId, string concept, string period, string fileControl)
            : this()
        {
            TransferNumber = transferNumber;
            IssueDate = issueDate;
            Amount = amount;
            PrintedAmount = amount;
            BeneficiaryName = beneficiaryName;
            BankId = bankId;
            BanksAccountTypeId = banksAccountTypeId;
            StatusId = statusId;
            Concept = concept;
            Period = period;
            FileControl = fileControl;
        }
    }
}
