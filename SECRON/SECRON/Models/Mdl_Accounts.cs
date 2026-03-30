using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECRON.Models
{
    internal class Mdl_Accounts
    {
        public int AccountId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string ParentAccountCode { get; set; }
        public int Level { get; set; }
        public string Sign { get; set; }
        public decimal Balance { get; set; }
        public int BankCode { get; set; }
        public string BankName { get; set; }
        public string BankAccountType { get; set; }
        public int CheckNumber { get; set; }
        public string Currency { get; set; }
        public string CurrencyName { get; set; }

        public Mdl_Accounts() { }

        public Mdl_Accounts(string code, string name, string type, string parentAccountCode,
            int level, string sign, decimal balance, int bankCode, string bankName,
            string bankAccountType, int checkNumber, string currency, string currencyName)
        {
            this.Code = code;
            this.Name = name;
            this.Type = type;
            this.ParentAccountCode = parentAccountCode;
            this.Level = level;
            this.Sign = sign;
            this.Balance = balance;
            this.BankCode = bankCode;
            this.BankName = bankName;
            this.BankAccountType = bankAccountType;
            this.CheckNumber = checkNumber;
            this.Currency = currency;
            this.CurrencyName = currencyName;
        }
    }
}