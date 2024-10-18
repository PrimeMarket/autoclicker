using System;
using System.Windows.Forms;

namespace AutoClicker
{
	// Token: 0x02000003 RID: 3
	internal static class Program
	{
		// Token: 0x06000017 RID: 23 RVA: 0x00002AE3 File Offset: 0x00000CE3
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}
