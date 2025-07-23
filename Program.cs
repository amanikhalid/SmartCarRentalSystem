using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ConstrainedExecution;

namespace SmartCarRentalSystem
{
    internal class Program
    {


        static void Main(string[] args)
        {

            Console.WriteLine(" SmartCar Rentals System ");
            while (true)
            {
                Console.WriteLine("\nAvailable Vehicles:"); // Display available vehicles
                for (int i = 0; i < vehicles.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {vehicles[i].GetInfo()}");
                }

                // Prompt user to choose a vehicle to rent
                int choice = GetIntInput("\nChoose vehicle to rent (0 to exit): ", 0, vehicles.Count);
                if (choice == 0)
                {
                    SaveData();
                    Console.WriteLine("Thank you for using SmartCar Rentals!");
                    break;
                }

                Vehicle selected = vehicles[choice - 1]; // Get selected vehicle

                int days = GetIntInput("Enter number of rental days: ", 1, 365);

                if (DateTime.Now.Year - selected.Year > 10) // Check if vehicle is older than 10 years
                {
                    Console.WriteLine("❌ Cannot rent this vehicle. It's older than 10 years.");
                    continue;
                }
                double totalCost = 0;

                // Calculate rental cost based on vehicle type
                if (selected is Car car) 
                {
                    bool withDriver = GetYesNo("Do you want a driver? (y/n): ");
                    totalCost = car.CalculateRentalCost(days, withDriver);
                }
            }
        }
    }
}
