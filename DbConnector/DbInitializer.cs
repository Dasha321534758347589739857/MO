using System.Windows;
using MO_kursasch_25.DbConnector.Models;

namespace MO_kursasch_25.DbConnector
{
    public static class DbInitializer
    {
        public static void Initialize(UsersContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User
                    {
                        Id = 1,
                        Username = "admin",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
                        Role = Role.Admin
                    },
                    new User
                    {
                        Id = 2,
                        Username = "user",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("user"),
                        Role = Role.User
                    }
                );

                context.SaveChanges();

                string message = "По умолчанию были созданы следующие пользователи:\n\n" +
                                 "1. Логин: admin, Пароль: admin, Роль: Администратор\n" +
                                 "2. Логин: user, Пароль: user, Роль: Пользователь";

                MessageBox.Show(message, "Первый запуск", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
