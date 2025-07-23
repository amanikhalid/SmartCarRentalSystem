using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.ConstrainedExecution;

namespace SmartCarRentalSystem
{
    internal class Program
    {

        static List<Vehicle> vehicles = new List<Vehicle>(); // List to store available vehicles

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

            static double GetDoubleInput(string prompt) // Method to get double input from user
            {
                while (true) // Prompt user for double input
                {
                    Console.Write(prompt);
                    if (double.TryParse(Console.ReadLine(), out double value) && value >= 0)
                        return value;
                    Console.WriteLine("Invalid input. Try again.");
                }
            }

            static bool GetYesNo(string prompt) // Method to get yes/no input from user
            {
                while (true) // Prompt user for yes/no input
                {
                    Console.Write(prompt);
                    string input = Console.ReadLine().ToLower();
                    if (input == "y" || input == "yes") return true;
                    if (input == "n" || input == "no") return false;
                    Console.WriteLine("Invalid input. Please enter y or n.");
                }
            }

            static void LoadData() 
            {
                if (!File.Exists(dataFile))
                {
                    vehicles = new List<Vehicle>
                {
                    new Car { Brand = "Toyota", Model = "Corolla", Year = 2022, LicensePlate = "A123", IsLuxury = false },
                    new Car { Brand = "Mercedes", Model = "E300", Year = 2023, LicensePlate = "B456", IsLuxury = true },
                    new Truck { Brand = "Volvo", Model = "FH16", Year = 2021, LicensePlate = "C789", MaxLoadKg = 18000 },
                    new Motorbike { Brand = "Honda", Model = "CBR", Year = 2019, LicensePlate = "D000", RequiresHelmet = true }
                };
                    SaveData(); // Save initial data
                    return;
                }

                string[] lines = File.ReadAllLines(dataFile);
                foreach (var line in lines)
                {
                    string[] parts = line.Split('|');
                    string type = parts[0];
                    if (type == "Car")
                        vehicles.Add(new Car { Brand = parts[1], Model = parts[2], Year = int.Parse(parts[3]), LicensePlate = parts[4], IsLuxury = bool.Parse(parts[5]) });
                    else if (type == "Truck")
                        vehicles.Add(new Truck { Brand = parts[1], Model = parts[2], Year = int.Parse(parts[3]), LicensePlate = parts[4], MaxLoadKg = double.Parse(parts[5]) });
                    else if (type == "Motorbike")
                        vehicles.Add(new Motorbike { Brand = parts[1], Model = parts[2], Year = int.Parse(parts[3]), LicensePlate = parts[4], RequiresHelmet = bool.Parse(parts[5]) });
                }
            }

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

        
 }
         







