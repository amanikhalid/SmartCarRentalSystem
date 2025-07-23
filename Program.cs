using System;
using System.Collections.Generic;
using System.IO;

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

            }
        }
    }
}
