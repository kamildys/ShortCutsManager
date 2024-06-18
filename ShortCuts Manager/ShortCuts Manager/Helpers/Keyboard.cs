using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ShortCuts_Manager.Helpers
{
    public static class Keyboard
    {
        [DllImport("user32.dll")]
        static extern uint keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);


        const byte VK_UP = 0x26; // Arrow Up key
        const byte VK_DOWN = 0x28; // Arrow Down key

        const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag, the key is going to be pressed
        const int KEYEVENTF_KEYUP = 0x0002; //Key up flag, the key is going to be released

        public static void KeyDown()
        {
            keybd_event(VK_DOWN, 0, KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_DOWN, 0, KEYEVENTF_KEYUP, 0);
        }

        public static void KeyUp()
        {
            keybd_event(VK_UP, 0, KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_UP, 0, KEYEVENTF_KEYUP, 0);
        }
    }
}
