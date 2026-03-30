using SECRON.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SECRON.Views
{
    public partial class Frm_Accounts_Managment : Form
    {
        #region PropiedadesIniciales
        // Variables de entorno
        public Mdl_Security_UserInfo UserData { get; set; }
        public Frm_Accounts_Managment()
        {
            InitializeComponent();
        }
        #endregion PropiedadesIniciales
    }
}
