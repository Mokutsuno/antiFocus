using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public static class WindowUtil
{
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int SW_RESTORE = 9;

    /// <summary>
    /// 実行中プロセスのウィンドウを最前面に持ってくる
    /// </summary>
    public static void BringOwnWindowToFront()
    {
        IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;

        if (hWnd == IntPtr.Zero)
        {
            // Unityなどでは MainWindowHandle が取れない場合があるので補助的に GetActiveWindow を使用
            hWnd = GetActiveWindow();
        }

        if (hWnd != IntPtr.Zero)
        {
            ShowWindow(hWnd, SW_RESTORE); // 最小化されていたら復元
            SetForegroundWindow(hWnd);    // 最前面に
        }
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();
}
