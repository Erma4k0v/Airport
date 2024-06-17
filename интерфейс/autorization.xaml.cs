using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.SqlClient;

namespace интерфейс
{
    public partial class autorization : Window
    {
        private const string connectionString = @"Server=DESKTOP-1DTLLG9\SQLEXPRESS; Database=airport; Integrated Security=True; TrustServerCertificate=True";
        private Okno1 mainWindow;

        public autorization()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mainWindow = new Okno1();
            Loaded += autorization_Loaded;
        }

        private void autorization_Loaded(object sender, RoutedEventArgs e)
        {
            // Добавляем обработчик события PreviewKeyDown для текстовых полей
            LoginTextBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            PasswordBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            RegistrationLoginTextBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            RegistrationPasswordBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            ConfirmPasswordBox.PreviewKeyDown += TextBox_PreviewKeyDown;

            // Добавляем обработчик события PreviewKeyDown для кнопок
            LoginButton.PreviewKeyDown += Button_PreviewKeyDown;
            RegistrationButton.PreviewKeyDown += Button_PreviewKeyDown;
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Если была нажата клавиша Enter, переходим к следующему элементу управления
            if (e.Key == Key.Enter)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                (sender as FrameworkElement)?.MoveFocus(request);
            }
        }

        private void Button_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Если была нажата клавиша Enter, вызываем соответствующий обработчик
            if (e.Key == Key.Enter)
            {
                if (sender == LoginButton)
                {
                    Login_Click(sender, e);
                }
                else if (sender == RegistrationButton)
                {
                    Registration_Click(sender, e);
                }
            }
        }

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            string login = RegistrationLoginTextBox.Text;
            string password = RegistrationPasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
            }
            else if (password != confirmPassword)
            {
                MessageBox.Show("Пароль и подтверждение пароля не совпадают.");
            }
            else
            {
                // Добавьте код для регистрации пользователя в базе данных
                MessageBox.Show($"Регистрация успешно завершена.");
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
            }
            else
            {
                // Проверка авторизации пользователя в базе данных
                bool isAuthenticated = CheckUserAuthentication(login, password);
                if (isAuthenticated)
                {
                    MessageBox.Show($"Авторизация успешна. Добро пожаловать, {login}!");
                    // Создание экземпляра нового окна
                    Okno1 mainWindow = new Okno1();
                    // Показ нового окна
                    mainWindow.Show();
                    // Закрытие текущего окна авторизации
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неправильный логин или пароль.");
                }
            }
        }

        private bool CheckUserAuthentication(string login, string password)
        {
            bool isAuthenticated = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Login = @Login AND Password = @Password";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@Password", password);
                    int count = (int)command.ExecuteScalar();
                    if (count > 0)
                    {
                        isAuthenticated = true;
                    }
                }
            }
            return isAuthenticated;
        }

    }
}
