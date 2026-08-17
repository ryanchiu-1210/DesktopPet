using desktopPet.UC;
using Gma.System.MouseKeyHook;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
namespace desktopPet
{
    public partial class mainForm : Form
    {
        public static mainForm instance;

        private IKeyboardMouseEvents _globalHook;

        // 定時自動儲存用 Timer
        private Timer _saveTimer;

        // 預先建立 UserControl 實例與狀態旗標
        private readonly BongoDown _ucDown = new BongoDown();
        private readonly BongoUp _ucUp = new BongoUp();
        private bool _isDown = true;

        #region Win32 API
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, bool wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        private const int WM_SETREDRAW = 0x000B;
        #endregion

        private readonly string _filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data.json");

        public mainForm()
        {
            InitializeComponent();
            instance = this;
            this.TopMost = true;

            // 1. 開啟 Form 與 mainpanel 雙重緩衝（抗閃爍）
            EnableDoubleBuffering();

            EnableDrag(this);
            SubscribeGlobalEvents();

            // 2. 預設顯示 BongoDown
            ChangeUC(_ucDown);

            // 3. 讀取 JSON 計數資料
            LoadCounterData();

            // 4. 初始化並啟動定時自動儲存（例如每 5000 毫秒 / 5 秒存一次）
            InitAutoSaveTimer(5000);
        }

        private void InitAutoSaveTimer(int intervalMs)
        {
            _saveTimer = new Timer();
            _saveTimer.Interval = intervalMs;
            _saveTimer.Tick += (s, e) => SaveCounterData();
            _saveTimer.Start();
        }

        private void EnableDoubleBuffering()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                           ControlStyles.UserPaint |
                           ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            typeof(Control).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance)
                           ?.SetValue(mainpanel, true, null);
        }

        private void LoadCounterData()
        {
            try
            {
                if (File.Exists(_filepath))
                {
                    string jsonText = File.ReadAllText(_filepath);
                    long count = JsonSerializer.Deserialize<long>(jsonText);
                    textBox1.Text = count.ToString();
                }
                else
                {
                    textBox1.Text = "0";
                }
            }
            catch
            {
                textBox1.Text = "0";
            }
        }

        private void SaveCounterData()
        {
            try
            {
                if (long.TryParse(textBox1.Text, out long count))
                {
                    string jsonString = JsonSerializer.Serialize(count);
                    File.WriteAllText(_filepath, jsonString);
                }
            }
            catch
            {
                // 忽略發生的檔案寫入例外
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE：防止桌寵強行搶走焦點
                return cp;
            }
        }

        private void SubscribeGlobalEvents()
        {
            _globalHook = Hook.GlobalEvents();
            _globalHook.KeyDown += GlobalHook_KeyDown;
            _globalHook.MouseDown += GlobalHook_MouseDown;
        }

        private void GlobalHook_KeyDown(object sender, KeyEventArgs e)
        {
            TriggerIncrement();
        }

        private void GlobalHook_MouseDown(object sender, MouseEventArgs e)
        {
            TriggerIncrement();
        }

        private void TriggerIncrement()
        {
            if (this.IsHandleCreated)
            {
                this.BeginInvoke(new Action(IncrementCount));
            }
        }

        private void IncrementCount()
        {
            // 1. 數字加 1
            string str = textBox1.Text.Trim();
            if (!long.TryParse(str, out long last))
            {
                last = 0;
            }
            last++;
            textBox1.Text = last.ToString();

            // 2. 切換 UserControl (Down / Up 輪流)
            _isDown = !_isDown;
            ChangeUC(_isDown ? _ucDown : _ucUp);
        }

        public void ChangeUC(UserControl uc)
        {
            SendMessage(mainpanel.Handle, WM_SETREDRAW, false, 0);

            try
            {
                mainpanel.Controls.Clear();
                EnableDrag(uc);
                mainpanel.Controls.Add(uc);
            }
            finally
            {
                SendMessage(mainpanel.Handle, WM_SETREDRAW, true, 0);
                mainpanel.Refresh();
            }
        }

        private void EnableDrag(Control control)
        {
            if (!(control is Button))
            {
                control.MouseDown -= Universal_MouseDown;
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 1. 停止並釋放 Timer
            if (_saveTimer != null)
            {
                _saveTimer.Stop();
                _saveTimer.Dispose();
            }

            // 2. 視窗關閉前最後執行一次存檔
            SaveCounterData();

            // 3. 解除鉤子綁定與釋放資源
            if (_globalHook != null)
            {
                _globalHook.KeyDown -= GlobalHook_KeyDown;
                _globalHook.MouseDown -= GlobalHook_MouseDown;
                _globalHook.Dispose();
            }

            base.OnFormClosing(e);
        }
    }
}