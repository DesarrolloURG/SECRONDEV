using System;
using System.Windows.Forms;

namespace SECRON.Utils
{
    // Detecta actividad de mouse/teclado en toda la aplicación (incluye formularios hijos del MDI)
    internal class Cls_ActivityMessageFilter : IMessageFilter
    {
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_MOUSEWHEEL = 0x020A;

        public Action OnActivity { get; set; }

        public bool PreFilterMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_MOUSEMOVE:
                case WM_LBUTTONDOWN:
                case WM_RBUTTONDOWN:
                case WM_KEYDOWN:
                case WM_MOUSEWHEEL:
                    OnActivity?.Invoke();
                    break;
            }
            return false; // No consumir el mensaje, dejar que siga su flujo normal
        }
    }
}