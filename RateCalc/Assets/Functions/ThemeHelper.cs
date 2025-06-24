using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThemeHelper
{
    public static class ThemeHelper
    {
        /// <summary>
        /// Kullanıcının sistem teması koyu mu?
        /// </summary>
        public static bool IsWindowsInDarkMode()
        {
            const string registryKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            object? value = Registry.GetValue(registryKey, "AppsUseLightTheme", null);

            if (value is int lightTheme)
            {
                return lightTheme == 0; // 0 = Dark Mode
            }

            // Varsayılan olarak açık tema varsayalım (hata veya bulunamadıysa)
            return false;
        }

    }
}
