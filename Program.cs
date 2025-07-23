using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                    Console.WriteLine("Cannot rent this vehicle. It's older than 10 years.");
                    continue;
                }
                double totalCost = 0;

                // Calculate rental cost based on vehicle type
                if (selected is Car car)
                {
                    bool withDriver = GetYesNo("Do you want a driver? (y/n): ");
                    totalCost = car.CalculateRentalCost(days, withDriver);
                }

                else if (selected is Truck truck)
                {
                    double weight = GetDoubleInput("Enter cargo weight (kg): ");
                    if (weight > truck.MaxLoadKg)
                    {
                        Console.WriteLine($"Cannot load more than {truck.MaxLoadKg} kg.");
                        continue;
                    }
                    totalCost = truck.CalculateRentalCost(days, weight);
                }

                // Check if the vehicle is a motorbike and calculate cost accordingly
                else if (selected is Motorbike motorbike) 
                {
                    totalCost = motorbike.CalculateRentalCost(days);
                }

                if (days > 7)
                    totalCost *= 0.9; // 10% discount

                Console.WriteLine($"Total Rental Cost: ${totalCost}");
            }
        }

        abstract class Vehicle
        {
            // Properties common to all vehicles
            public string Brand { get; set; } 
            public string Model { get; set; }
            public int Year { get; set; }
            public string LicensePlate { get; set; }

            // Constructor to initialize vehicle properties
            public virtual double CalculateRentalCost(int days) => 0;
            public virtual string GetInfo() => $"{Brand} {Model} {Year}";
        }
        
        class Car : Vehicle // Inherits from Vehicle 
        {
            public bool IsLuxury { get; set; } // Indicates if the car is luxury

            public override double CalculateRentalCost(int days)
            {
                return days * (IsLuxury ? 80 : 60);
            }

            public double CalculateRentalCost(int days, bool withDriver) // Overloaded method to include driver option
            
            {
                double baseCost = CalculateRentalCost(days);
                return withDriver ? baseCost + (days * 20) : baseCost; // additional cost for driver
            }

            public override string GetInfo() => $"{base.GetInfo()} | Car | Luxury: {(IsLuxury ? "Yes" : "No")}"; // Display car info
        }

        class Truck : Vehicle // Inherits from Vehicle
        {
            public double MaxLoadKg { get; set; } // Maximum load capacity in kg

            public override double CalculateRentalCost(int days) // Base rental cost for truck
            
            {
                return days * 100;
            }

            public double CalculateRentalCost(int days, double cargoWeight) // Overloaded method to include cargo weight
            
            {
                return CalculateRentalCost(days) + (cargoWeight * 0.01); // optional extra fee per kg
            }

            public override string GetInfo() => $"{base.GetInfo()} | Truck | Max Load: {MaxLoadKg}kg"; // Display truck info
        }

        class Motorbike : Vehicle // Inherits from Vehicle
        
        {
            public bool RequiresHelmet { get; set; } // Indicates if a helmet is required

            public override double CalculateRentalCost(int days)
            {
                return days * 40;
            }

            public override string GetInfo() => $"{base.GetInfo()} | Motorbike | Helmet Required: {(RequiresHelmet ? "Yes" : "No")}"; // Display motorbike info
        }

        static int GetIntInput(string prompt, int min, int max) // Method to get integer input from user

        {
            while (true) // Prompt user for integer input
        
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int value) && value >= min && value <= max)
                    return value;
                Console.WriteLine("Invalid input. Try again.");
            }
        }

    }
         
        
        
}






