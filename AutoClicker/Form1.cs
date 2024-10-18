using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoClicker
{
	// Token: 0x02000002 RID: 2
	public partial class Form1 : Form
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		public Form1()
		{
			this.InitializeComponent();
			base.KeyPreview = true;
			base.KeyDown += this.Form1_KeyDown;
			this.SetHook();
		}

		// Token: 0x06000002 RID: 2
		[DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		// Token: 0x06000003 RID: 3
		[DllImport("user32.dll")]
		private static extern bool ReleaseCapture();

		// Token: 0x06000004 RID: 4 RVA: 0x00002075 File Offset: 0x00000275
		private void Form1_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				Form1.ReleaseCapture();
				Form1.SendMessage(base.Handle, 161, 2, 0);
			}
		}

		// Token: 0x06000005 RID: 5
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr SetWindowsHookEx(int idHook, Form1.LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

		// Token: 0x06000006 RID: 6
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool UnhookWindowsHookEx(IntPtr hhk);

		// Token: 0x06000007 RID: 7
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		// Token: 0x06000008 RID: 8
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetModuleHandle(string lpModuleName);

		// Token: 0x06000009 RID: 9
		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
		public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

		// Token: 0x0600000A RID: 10 RVA: 0x000020A0 File Offset: 0x000002A0
		private void Form1_KeyDown(object sender, KeyEventArgs e)
		{
			if (this.button3Clicked)
			{
				MessageBox.Show("New hotkey: " + e.KeyCode.ToString());
				this.button1.Text = e.KeyCode.ToString();
				this.button3.Text = "Set Hotkey";
				this.button3Clicked = false;
				Form1.hotkey = e.KeyCode;
				this.button1.Text = "Start(" + Form1.hotkey.ToString() + ")";
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002147 File Offset: 0x00000347
		private static void ContinuousClickLeft(int delay)
		{
			Form1.isClicking = true;
			Task.Run(delegate()
			{
				while (Form1.isClicking)
				{
					Form1.mouse_event(2U, 0U, 0U, 0U, 0U);
					Thread.Sleep(10);
					Form1.mouse_event(4U, 0U, 0U, 0U, 0U);
					Thread.Sleep(delay);
				}
			});
		}

		// Token: 0x0600000C RID: 12 RVA: 0x0000216C File Offset: 0x0000036C
		private static void ContinuousClickRight(int delay)
		{
			Form1.isClicking = true;
			Task.Run(delegate()
			{
				while (Form1.isClicking)
				{
					Form1.mouse_event(8U, 0U, 0U, 0U, 0U);
					Thread.Sleep(10);
					Form1.mouse_event(16U, 0U, 0U, 0U, 0U);
					Thread.Sleep(delay);
				}
			});
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002191 File Offset: 0x00000391
		private static void StopContinuousClick()
		{
			Form1.isClicking = false;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x0000219C File Offset: 0x0000039C
		private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0 && wParam == (IntPtr)256 && Marshal.ReadInt32(lParam) == (int)Form1.hotkey)
			{
				Form1 form = Application.OpenForms.OfType<Form1>().FirstOrDefault<Form1>();
				if (form != null)
				{
					form.Invoke(new MethodInvoker(delegate()
					{
						if (Form1.hotkey_clicked)
						{
							form.button1.Text = "Start (" + Form1.hotkey.ToString() + ")";
							Form1.hotkey_clicked = false;
							Form1.StopContinuousClick();
							return;
						}
						form.button1.Text = "Stop (" + Form1.hotkey.ToString() + ")";
						Form1.hotkey_clicked = true;
						if (form.comboBox1.Text == "Left")
						{
							Form1.ContinuousClickLeft(int.Parse(form.textBox1.Text));
						}
						if (form.comboBox1.Text == "Right")
						{
							Form1.ContinuousClickLeft(int.Parse(form.textBox1.Text));
						}
					}));
				}
			}
			return Form1.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002214 File Offset: 0x00000414
		public void SetHook()
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				using (ProcessModule mainModule = currentProcess.MainModule)
				{
					Form1.SetWindowsHookEx(13, new Form1.LowLevelKeyboardProc(Form1.HookCallback), Form1.GetModuleHandle(mainModule.ModuleName), 0U);
				}
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002280 File Offset: 0x00000480
		private void button1_Click(object sender, EventArgs e)
		{
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002282 File Offset: 0x00000482
		private void button3_Click_1(object sender, EventArgs e)
		{
			this.button3.Text = "Press a key...";
			this.button3Clicked = true;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000229B File Offset: 0x0000049B
		private void button2_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000022A2 File Offset: 0x000004A2
		private void button4_Click(object sender, EventArgs e)
		{
			base.WindowState = FormWindowState.Minimized;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000022AB File Offset: 0x000004AB
		private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://discord.gg/N55YYbDZ7Z");
		}

		// Token: 0x04000001 RID: 1
		private static Keys hotkey;

		// Token: 0x04000002 RID: 2
		private bool button3Clicked;

		// Token: 0x04000003 RID: 3
		private const int WM_NCLBUTTONDOWN = 161;

		// Token: 0x04000004 RID: 4
		private const int HT_CAPTION = 2;

		// Token: 0x04000005 RID: 5
		public const uint MOUSEEVENTF_LEFTDOWN = 2U;

		// Token: 0x04000006 RID: 6
		public const uint MOUSEEVENTF_LEFTUP = 4U;

		// Token: 0x04000007 RID: 7
		public const uint MOUSEEVENTF_RIGHTDOWN = 8U;

		// Token: 0x04000008 RID: 8
		public const uint MOUSEEVENTF_RIGHTUP = 16U;

		// Token: 0x04000009 RID: 9
		public const int WH_KEYBOARD_LL = 13;

		// Token: 0x0400000A RID: 10
		public const int WM_KEYDOWN = 256;

		// Token: 0x0400000B RID: 11
		public static bool button1_clicked;

		// Token: 0x0400000C RID: 12
		public static bool hotkey_clicked;

		// Token: 0x0400000D RID: 13
		private static bool mouseButtonDown;

		// Token: 0x0400000E RID: 14
		private static bool isClicking;

		// Token: 0x02000006 RID: 6
		// (Invoke) Token: 0x06000020 RID: 32
		public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
	}
}
