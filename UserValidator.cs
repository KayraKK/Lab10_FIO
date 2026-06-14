using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lab10_Ivanov
{
    public class UserValidator
    {
        // Запрещённые логины
        private readonly HashSet<string> _blockedLogins = new HashSet<string>
        {
            "admin", "user", "root", "test", "guest", "moderator"
        };

        /// <summary>
        /// Проверяет данные пользователя
        /// </summary>
        /// <returns>(bool success, string errorMessage)</returns>
        public (bool, string) Validate(string login, string password, string confirmPassword)
        {
            // 1. Проверка логина (пустой)
            if (string.IsNullOrWhiteSpace(login))
                return (false, "Логин не может быть пустым");

            // 2. Проверка логина (запрещённый)
            if (_blockedLogins.Contains(login.ToLower()))
                return (false, "Логин запрещён. Используйте другой логин");

            // 3. Определение типа логина и валидация
            if (IsPhoneNumber(login))
            {
                // Телефон уже прошёл валидацию в IsPhoneNumber
            }
            else if (IsEmail(login))
            {
                // Email уже прошёл валидацию в IsEmail
            }
            else
            {
                // Проверка как обычной строки
                if (login.Length < 5)
                    return (false, "Логин (строка) должен содержать минимум 5 символов");

                if (!Regex.IsMatch(login, @"^[a-zA-Z0-9_]+$"))
                    return (false, "Логин (строка) должен содержать только латиницу, цифры и знак подчеркивания");
            }

            // 4. Проверка пароля
            if (string.IsNullOrWhiteSpace(password))
                return (false, "Пароль не может быть пустым");

            if (password.Length < 7)
                return (false, "Пароль должен содержать минимум 7 символов");

            if (!Regex.IsMatch(password, @"^[а-яА-ЯёЁ0-9!@#$%^&*()_+\-=\[\]{};':\\|,.<>\/?]+$"))
                return (false, "Пароль должен содержать только кириллицу, цифры и спецсимволы");

            if (!Regex.IsMatch(password, @"[А-ЯЁ]"))
                return (false, "Пароль должен содержать хотя бы одну заглавную букву");

            if (!Regex.IsMatch(password, @"[а-яё]"))
                return (false, "Пароль должен содержать хотя бы одну строчную букву");

            if (!Regex.IsMatch(password, @"[0-9]"))
                return (false, "Пароль должен содержать хотя бы одну цифру");

            if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':\\|,.<>\/?]"))
                return (false, "Пароль должен содержать хотя бы один спецсимвол");

            // 5. Проверка совпадения паролей
            if (password != confirmPassword)
                return (false, "Пароль и подтверждение пароля не совпадают");

            // Всё хорошо
            return (true, "");
        }

        private bool IsPhoneNumber(string login)
        {
            // Формат +X-XXX-XXX-XXXX (например +7-123-456-7890)
            return Regex.IsMatch(login, @"^\+\d-\d{3}-\d{3}-\d{4}$");
        }

        private bool IsEmail(string login)
        {
            // Простая проверка email: xxx@xxx.xxx
            return Regex.IsMatch(login, @"^[^@]+@[^@]+\.[^@]+$");
        }

        /// <summary>
        /// Простое маскирование пароля (хеш) - одинаковый пароль = одинаковый результат
        /// </summary>
        public static string MaskPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return "empty";
            // Простейший "хеш" - длина + первые 2 символа + последние 2 символа + контрольная сумма
            int hash = 0;
            foreach (char c in password)
                hash = (hash * 31 + c) % 10000;
            return $"***{password.Length}_{hash:D4}***";
        }
    }
}