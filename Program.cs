using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace RHL16_Launcher
{
    class Program
    {
        const int PROCESS_ALL_ACCESS = 0x001F0FFF;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "RHL 16 Launcher by Exodren";
            Console.WriteLine("========================================");
            Console.WriteLine("     RHL 16 BYPASS v1.0 | BY EXODREN    ");
            Console.WriteLine("========================================");
            Console.WriteLine("[*] Запускаем оригинальный файл rhl16.exe...");

            try
            {
                Process.Start("rhl16.exe");

                Thread.Sleep(500);

                Process[] processes = Process.GetProcessesByName("rhl16");
                if (processes.Length == 0)
                {
                    Console.WriteLine("[-] Ошибка: Процесс игры не найден.");
                    Console.ReadLine();
                    return;
                }

                IntPtr processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, processes[0].Id);
                if (processHandle == IntPtr.Zero)
                {
                    Console.WriteLine("[-] Ошибка: Нет прав на запись. Запустите от имени Администратора.");
                    Console.ReadLine();
                    return;
                }

                IntPtr address1 = (IntPtr)0x00976EC8;
                IntPtr address2 = (IntPtr)0x00976F8B;
                byte[] patch = { 0xEB };
                IntPtr bytesWritten;

                Console.WriteLine("[*] Начинаем агрессивный перехват памяти...");

                for (int i = 0; i < 40; i++)
                {
                    WriteProcessMemory(processHandle, address1, patch, patch.Length, out bytesWritten);
                    WriteProcessMemory(processHandle, address2, patch, patch.Length, out bytesWritten);
                    Thread.Sleep(50);
                }

                Console.WriteLine("[+] Патч успешно вшит на лету!");

                CloseHandle(processHandle);
                Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[-] Системная ошибка: {ex.Message}");
                Console.ReadLine();
            }
        }
    }
}