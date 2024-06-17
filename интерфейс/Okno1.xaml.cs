using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace интерфейс
{
    /// <summary>
    /// Логика взаимодействия для Okno1.xaml
    /// </summary>
    /// 
    public interface IListContainer
    {
        ObservableCollection<ListItem> Model { get; set; }
        ObservableCollection<ListItem> LastName { get; set; }
        ObservableCollection<ListItem> FirstName { get; set; }
        ObservableCollection<ListItem> PassportNumber { get; set; }
        ObservableCollection<ListItem> Number { get; set; }
        ObservableCollection<ListItem> Count { get; set; }
    }

    public partial class Okno1 : Window
    {
        public ObservableCollection<ListItem> Airplanes { get; set; } = new ObservableCollection<ListItem>();
        public ObservableCollection<ListItem> CargoAirplanes { get; set; } = new ObservableCollection<ListItem>();
        public ObservableCollection<ListItem> Passengers { get; set; } = new ObservableCollection<ListItem>();

        const string connectionString = @"Server=DESKTOP-1DTLLG9\SQLEXPRESS;Database=airport; Integrated Security=True; TrustServerCertificate=True";

        public ObservableCollection<ListItem> Model { get; private set; }

        public Okno1()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;
        }

        private void AddAirplane_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                int count = countId(connection);
                string query = "INSERT INTO airplanes(Id, Model, FlightNumber, Capacity) " +
                    "VALUES (@id, @model, @flightNumber, @capacity)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", count + 1);
                    command.Parameters.AddWithValue("@model", model.Text);
                    command.Parameters.AddWithValue("@flightNumber", flightNumber.Text);
                    command.Parameters.AddWithValue("@capacity", int.Parse(capacity.Text));
                    command.ExecuteNonQuery();
                    MessageBox.Show("Самолет успешно добавлен");
                }
            }
        }

        private static int countId(SqlConnection connection)
        {
            var sqlQuery1 = "SELECT COUNT(ID) FROM airplanes";
            var countCommand = new SqlCommand(sqlQuery1, connection);
            int count = (int)countCommand.ExecuteScalar();
            return count;
        }

        private void AddCargoAirplane_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                int count = countId(connection);
                string query = "INSERT INTO cargoplanes(Id, Model, FlightNumber, Capacity,CargoCapacity) VALUES (@Id, @Model, @FlightNumber, @Capacity,@CargoCapacity)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", count + 1);
                    command.Parameters.AddWithValue("@Model", model1.Text.Trim());
                    command.Parameters.AddWithValue("@FlightNumber", flightNumber1.Text.Trim());
                    command.Parameters.AddWithValue("@Capacity", int.Parse(capacity1.Text.Trim()));
                    command.Parameters.AddWithValue("@CargoCapacity", int.Parse(cargoCapacity.Text.Trim()));
                    command.ExecuteNonQuery();
                    MessageBox.Show("Грузовой самолет успешно добавлен");
                }
            }
        }

        private void AddPassenger_Click(object sender, RoutedEventArgs e)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            string query = "INSERT INTO passengers(lastName, firstName, passportNumber) " +
                "VALUES (@lastName, @firstName, @passportNumber)";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@lastName", lastName.Text);
            command.Parameters.AddWithValue("@firstName", firstName.Text);
            command.Parameters.AddWithValue("@passportNumber", passportNumber.Text);
            command.ExecuteNonQuery();
            MessageBox.Show("пассажир успешно добавлен");

        }

        private void RemoveAirplane_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM airplanes WHERE FlightNumber = @number";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@number", RemovePlane.Text.Trim());
                    command.ExecuteNonQuery();
                    MessageBox.Show("Самолет успешно удален");
                }
            }
        }

        private void RemoveCargoAirplane_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RemoveCargoPlane.Text.Trim()))
            {
                MessageBox.Show("Пожалуйста, введите номер рейса грузового самолета.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM cargoplanes WHERE FlightNumber = @number";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@number", RemoveCargoPlane.Text.Trim());
                    command.ExecuteNonQuery();
                    MessageBox.Show("Грузовой самолет успешно удален");
                }
            }
        }
    

        private void RemovePassenger_Click(object sender, RoutedEventArgs e)
        {
            RemoveEntity("passengers", "passportNumber", RemovePassenger.Text.Trim(), "пассажир");
        }

        private void RemoveEntity(string tableName, string paramName, string paramValue, string entityName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"DELETE FROM {tableName} WHERE {paramName} = @{paramName}";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue($"@{paramName}", paramValue);
                    command.ExecuteNonQuery();
                    MessageBox.Show($"{entityName} успешно удален");
                }
            }
        }



        private void UpdateAirplane_Click(object sender, RoutedEventArgs e)
        {
            UpdateEntity("airplanes", newReiseNum2.Text.Trim(), reiseNum2.Text.Trim(), modelUpdate2.Text.Trim(), int.Parse(countCapcity2.Text.Trim()));
        }

        private void UpdateEntity(string tableName, string newFlightNumber, string oldFlightNumber, string model, int capacity)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"UPDATE {tableName} SET Model = @model, FlightNumber = @newFlightNumber, Capacity = @capacity WHERE FlightNumber = @oldFlightNumber";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@newFlightNumber", newFlightNumber);
                    command.Parameters.AddWithValue("@oldFlightNumber", oldFlightNumber);
                    command.Parameters.AddWithValue("@model", model);
                    command.Parameters.AddWithValue("@capacity", capacity);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Самолет успешно обновлен");
                }
            }
        }

        private void UpdatePassenger_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE passengers SET passportNumber = @newPassportNumber, firstName = @firstName, lastName = @lastName WHERE passportNumber = @oldPassportNumber";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@newPassportNumber", int.Parse(newNumPass1.Text.Trim()));
                    command.Parameters.AddWithValue("@oldPassportNumber", int.Parse(passNumber1.Text.Trim()));
                    command.Parameters.AddWithValue("@firstName", name1.Text.Trim());
                    command.Parameters.AddWithValue("@lastName", surename1.Text.Trim());
                    command.ExecuteNonQuery();
                    MessageBox.Show("Пассажир успешно обновлен");
                }
            }
        }

        private void UpdateCargoAirplane_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE cargoplanes SET Model = @model, FlightNumber = @newFlightNumber, Capacity = @capacity, CargoCapacity = @cargoCapacity WHERE FlightNumber = @oldFlightNumber";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@newFlightNumber", newReiseNum1.Text.Trim());
                    command.Parameters.AddWithValue("@oldFlightNumber", reiseNum1.Text.Trim());
                    command.Parameters.AddWithValue("@model", modelUpdate1.Text.Trim());
                    command.Parameters.AddWithValue("@capacity", int.Parse(Capcity1.Text.Trim()));
                    command.Parameters.AddWithValue("@cargoCapacity", int.Parse(countCapcity1.Text.Trim()));
                    command.ExecuteNonQuery();
                    MessageBox.Show("Самолет успешно обновлен");
                }
            }
        }


        // Реализация метода ListAirplanes_Click для вывода списка самолетов
        private void ListAirplanes_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                Airplanes.Clear();
                connection.Open();
                string query = "SELECT Model, FlightNumber, Capacity FROM airplanes";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Airplanes.Add(new ListItem()
                            {
                                Model = reader.GetString(reader.GetOrdinal("Model")),
                                FlightNumber = reader.GetString(reader.GetOrdinal("FlightNumber")),
                                Capacity = reader.GetInt32(reader.GetOrdinal("Capacity")).ToString()
                            });
                        }
                    }
                }
            }

            viewPlane.ItemsSource = Airplanes; // устанавливаем источник данных для ListView
        }

        private void ListCargoAirplanes_Click(object sender, RoutedEventArgs e)
        {
            CargoAirplanes.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Model, FlightNumber, Capacity, CargoCapacity FROM cargoplanes";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CargoAirplanes.Add(new ListItem()
                            {
                                Model = reader.GetString(reader.GetOrdinal("Model")),
                                FlightNumber = reader.GetString(reader.GetOrdinal("FlightNumber")),
                                Capacity = reader.GetInt32(reader.GetOrdinal("Capacity")).ToString(),
                                CargoCapacity = reader.GetInt32(reader.GetOrdinal("CargoCapacity")).ToString()
                            });
                        }
                    }
                }
            }

            viewCargo.ItemsSource = CargoAirplanes; // устанавливаем источник данных для ListView
        }

        private void ListPassengers_Click(object sender, RoutedEventArgs e)
        {
            Passengers.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT lastName, firstName, passportNumber FROM passengers";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Passengers.Add(new ListItem()
                            {
                                LastNameModel = reader.GetString(reader.GetOrdinal("lastName")),
                                FirstNameModel = reader.GetString(reader.GetOrdinal("firstName")),
                                PassportNameModel = reader.GetInt32(reader.GetOrdinal("passportNumber")).ToString()
                            });
                        }
                    }
                }
            }

            viewPassenger.ItemsSource = Passengers; // устанавливаем источник данных для ListView
        }

        public class Airplane<T>
        {
            public T Id { get; set; }
            public string Model { get; set; }
            public string FlightNumber { get; set; }
            public int Capacity { get; set; }
            public T CargoCapacity { get; set; }
        }

        private void viewCargo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void viewPlane_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
    public class ListItem
    {
        public string Model { get; set; }
        public string Number { get; set; }
        public string Count { get; set; }
        public string LastNameModel { get; set; }
        public string FirstNameModel { get; set; }
        public string PassportNameModel { get; set; }
        public string FlightNumber { get; internal set; }
        public string Capacity { get; internal set; }
        public string CargoCapacity { get; internal set; }
    }

    public class Passenger
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string PassportNumber { get; set; }
    }

}
