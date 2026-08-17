using desktopPet.UC;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace desktopPet
{
    public partial class mainForm : Form
    {
        public static mainForm instance;

        // Windows API 用於視窗拖曳
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public mainForm()
        {
            InitializeComponent();
            instance = this;

            // 為 mainForm 本身與 mainpanel 綁定拖曳
            EnableDrag(this);

            ChangeUC(new BongoDown());
        }

        private void mainForm_Load(object sender, EventArgs e)
        {

        }

        public void ChangeUC(UserControl uc)
        {
            mainpanel.Controls.Clear();

            // 為新加入的 UserControl 及其內部元件（如 PictureBox）自動綁定拖曳事件
            EnableDrag(uc);

            mainpanel.Controls.Add(uc);
            ClientSize = uc.Size;
        }

        /// <summary>
        /// 遞迴將控制項與其所有子控制項綁定 MouseDown 拖曳事件
        /// </summary>
        private void EnableDrag(Control control)
        {
            // 若有不希望觸發拖曳的元件（例如按鈕），可以在此處排除
            if (!(control is Button))
            {
                control.MouseDown -= Universal_MouseDown; // 避免重複訂閱
                control.MouseDown += Universal_MouseDown;
            }

            foreach (Control child in control.Controls)
            {
                EnableDrag(child);
            }
        }

        private void Universal_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }
    }
}