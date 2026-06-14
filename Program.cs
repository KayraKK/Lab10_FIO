using System;
using System.IO;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Lab10_Ivanov
{
    class Program
    {
        private static UserValidator _validator = new UserValidator();

        static void Main(string[] args)
        {
            // Настройка Serilog
            string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            Directory.CreateDirectory(logDir);
            string logFile = Path.Combine(logDir, "log.txt");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss}] {Message}{NewLine}{Exception}")
                .WriteTo.File(logFile,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} | {Message}{NewLine}{Exception}")
                .CreateLogger();

            Log.Information("=== Программа запущена ===");

            Console.WriteLine("=== Регистрация пользователя ===\n");

            // Демонстрация работы на разных данных
            TestRegistration("ivan123", "Пароль123!", "Пароль123!");
            TestRegistration("admin", "Пароль123!", "Пароль123!");
            TestRegistration("ab", "Пароль123!", "Пароль123!");
            TestRegistration("user@mail.ru", "Пароль123!", "Пароль123!");
            TestRegistration("+7-123-456-7890", "Пароль123!", "Пароль123!");
            TestRegistration("test_user", "слабый", "слабый");
            TestRegistration("valid_login", "КорректныйПароль1!", "КорректныйПароль1!");
            TestRegistration("valid_login", "КорректныйПароль1!", "ДругойПароль1!");

            Console.WriteLine("\n=== Ручной ввод ===");
            Console.Write("Введите логин: ");
            string login = Console.ReadLine();
            Console.Write("Введите пароль: ");
            string password = Console.ReadLine();
            Console.Write("Подтвердите пароль: ");
            string confirm = Console.ReadLine();

            TestRegistration(login, password, confirm);

            Log.Information("=== Программа завершена ===");
            Log.CloseAndFlush();

            Console.WriteLine("\nЛоги сохранены в папке logs/");
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static void TestRegistration(string login, string password, string confirm)
        {
            string maskedPass = UserValidator.MaskPassword(password);
            string maskedConfirm = UserValidator.MaskPassword(confirm);
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var (success, error) = _validator.Validate(login, password, confirm);

            if (success)
            {
                Console.WriteLine($"✓ УСПЕХ: {login}");
                Log.Information($"Регистрация | Логин: {login} | Пароль: {maskedPass} | Подтверждение: {maskedConfirm} | Результат: Успешная регистрация");
            }
            else
            {
                Console.WriteLine($"✗ ОШИБКА: {login} - {error}");
                Log.Error($"Регистрация | Логин: {login} | Пароль: {maskedPass} | Подтверждение: {maskedConfirm} | Ошибка: {error}");
            }
            Console.WriteLine();
        }
    }
}