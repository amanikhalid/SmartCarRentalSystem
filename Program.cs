using System;
using System.Collections.Generic;
using System.IO;

namespace SmartCarRentalSystem
{
    internal class Program
    {
        static List<Vehicle> vehicles = new List<Vehicle>(); // List to hold all vehicles
        static readonly string dataFile = "vehicles.txt"; // File to store vehicle data

        static void Main(string[] args)
        {
            LoadData();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("SmartCar Rentals Main Menu");
                Console.WriteLine("1. View All Vehicles");
                Console.WriteLine("2. Filter by Vehicle Type");
                Console.WriteLine("3. Rent a Vehicle");
                Console.WriteLine("4. Save & Exit");
                Console.WriteLine("5. View Luxury Cars Only");
                Console.WriteLine("6. View Rental Rules");

                int option = GetIntInput("\nChoose an option: ", 1, 6);

                if (option == 1) // View all vehicles
                {
                    DisplayVehicles(vehicles);
                }
                else if (option == 2) // Filter vehicles by type
                {
                    Console.WriteLine("\nFilter Options:");
                    Console.WriteLine("1. Cars\n2. Trucks\n3. Motorbikes");
                    int filter = GetIntInput("Choose type: ", 1, 3);
                    List<Vehicle> filtered = vehicles.FindAll(v =>
                        (filter == 1 && v is Car) ||
                        (filter == 2 && v is Truck) ||
                        (filter == 3 && v is Motorbike));
                    DisplayVehicles(filtered);
                }
                else if (option == 3) // Rent a vehicle
                {
                    Console.WriteLine("\nWhich type of vehicle would you like to rent?");
                    Console.WriteLine("1. Car\n2. Truck\n3. Motorbike");
                    int type = GetIntInput("Choose type: ", 1, 3);

                    List<Vehicle> list = vehicles.FindAll(v =>
                        (type == 1 && v is Car) ||
                        (type == 2 && v is Truck) ||
                        (type == 3 && v is Motorbike));

                    if (list.Count == 0) // No vehicles available of selected type
                    {
                        Console.WriteLine("\nNo vehicles available of this type.");
                        Console.ReadLine();
                        continue;
                    }

                    DisplayVehicles(list); // Display available vehicles of selected type
                    int select = GetIntInput("\nSelect a vehicle to rent: ", 1, list.Count);
                    Vehicle selected = list[select - 1];

                    if (DateTime.Now.Year - selected.Year > 10) // Vehicle is too old
                    {
                        Console.WriteLine("This vehicle is too old to rent.");
                        Console.ReadLine();
                        continue;
                    }

                    int days = GetIntInput("Enter number of rental days: ", 1, 365); // Validate rental days
                    double total = 0;

                    if (selected is Car car)
                    {
                        bool withDriver = GetYesNo("Need a driver? (y/n): ");
                        total = car.CalculateRentalCost(days, withDriver);
                    }
                    else if (selected is Truck truck) // Calculate rental cost for truck
                    {
                        while (true)
                        {
                            double weight = GetDoubleInput("Enter cargo weight (kg): ");
                            if (weight > truck.MaxLoadKg)
                            {
                                Console.WriteLine($"Max load is {truck.MaxLoadKg}kg.");
                                Console.ReadLine();
                            }
                            else // Valid weight
                            {
                                total = truck.CalculateRentalCost(days, weight);
                                break;
                            }
                        }
                    }
                    else if (selected is Motorbike bike) 
                    {
                        total = bike.CalculateRentalCost(days);
                    }

                    if (days > 7)
                        total *= 0.9;

                    Console.WriteLine($"\nRental successful. Total Cost: ${total}"); // Display total cost
                    Console.ReadLine();
                }
                else if (option == 4)
                {
                    SaveData();
                    Console.WriteLine("\nData saved successfully. Goodbye!"); 
                    Console.ReadLine();
                    break;
                }
                else if (option == 5)
                {
                    List<Vehicle> luxuryCars = vehicles.FindAll(v => v is Car c && c.IsLuxury);
                    DisplayVehicles(luxuryCars);
                }
                else if (option == 6) // View rental rules
                {
                    Console.Clear();
                    Console.WriteLine("Rental Rules:");
                    Console.WriteLine("- Car: $60/day or $80/day if luxury (+$20/day with driver)");
                    Console.WriteLine("- Truck: $100/day + $0.01 per kg of cargo (Max Load applies)");
                    Console.WriteLine("- Motorbike: $40/day");
                    Console.WriteLine("- 10% discount for rentals longer than 7 days");
                    Console.WriteLine("- Vehicles older than 10 years cannot be rented");
                    Console.ReadLine();
                }
            }
        }

        static void DisplayVehicles(List<Vehicle> list) // Display available vehicles
        {
            Console.Clear();
            Console.WriteLine("\nAvailable Vehicles"); // Display header
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {list[i].GetInfo()}");
            }
            Console.ReadLine();
        }

        static int GetIntInput(string prompt, int min, int max) // Get integer input with validation
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int val) && val >= min && val <= max)
                    return val;
                Console.WriteLine("Invalid input. Try again.");
            }
        }

        static double GetDoubleInput(string prompt) // Get double input with validation
        {
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), out double val) && val >= 0)
                    return val;
                Console.WriteLine("Invalid input. Try again.");
            }
        }

        static bool GetYesNo(string prompt) // Get yes/no input with validation
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine().ToLower();
                if (input == "y" || input == "yes") return true;
                if (input == "n" || input == "no") return false;
                Console.WriteLine("Please enter y or n.");
            }
        }

        static void LoadData() // Load vehicle data from file or initialize with default values
        {
            if (!File.Exists(dataFile))
            {
                vehicles = new List<Vehicle>
                {
                    new Car { Brand = "BMW", Model = "740Li", Year = 2023, LicensePlate = "BMW001", IsLuxury = true },
                    new Car { Brand = "Porsche", Model = "Panamera", Year = 2024, LicensePlate = "POR001", IsLuxury = true },
                    new Car { Brand = "Kia", Model = "K5", Year = 2023, LicensePlate = "KIA123", IsLuxury = false },
                    new Truck { Brand = "Volvo", Model = "FMX", Year = 2021, LicensePlate = "TRK001", MaxLoadKg = 20000 },
                    new Motorbike { Brand = "Suzuki", Model = "GSX", Year = 2020, LicensePlate = "BIK001", RequiresHelmet = true },
                    new Motorbike { Brand = "Yamaha", Model = "R15", Year = 2021, LicensePlate = "BIK002", RequiresHelmet = true }
                };
                SaveData();
            }
            else // Load existing data from file
            {
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

        static void SaveData() // Save vehicle data to file
        {
            using StreamWriter writer = new StreamWriter(dataFile);
            foreach (var v in vehicles)
            {
                if (v is Car car)
                    writer.WriteLine($"Car|{car.Brand}|{car.Model}|{car.Year}|{car.LicensePlate}|{car.IsLuxury}");
                else if (v is Truck truck)
                    writer.WriteLine($"Truck|{truck.Brand}|{truck.Model}|{truck.Year}|{truck.LicensePlate}|{truck.MaxLoadKg}");
                else if (v is Motorbike bike)
                    writer.WriteLine($"Motorbike|{bike.Brand}|{bike.Model}|{bike.Year}|{bike.LicensePlate}|{bike.RequiresHelmet}");
            }
        }
    }

    abstract class Vehicle // Base class for all vehicles
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string LicensePlate { get; set; }
        public virtual double CalculateRentalCost(int days) => 0;
        public virtual string GetInfo() => $"{Brand} {Model} {Year}";
    }

    class Car : Vehicle // Represents a car
    {
        public bool IsLuxury { get; set; }
        public override double CalculateRentalCost(int days) => days * (IsLuxury ? 80 : 60);
        public double CalculateRentalCost(int days, bool withDriver) => CalculateRentalCost(days) + (withDriver ? days * 20 : 0);
        public override string GetInfo() => base.GetInfo() + $" | Car | Luxury: {(IsLuxury ? "Yes" : "No")}";
    }

    class Truck : Vehicle // Represents a truck
    {
        public double MaxLoadKg { get; set; }
        public override double CalculateRentalCost(int days) => days * 100;
        public double CalculateRentalCost(int days, double weight) => CalculateRentalCost(days) + (weight * 0.01);
        public override string GetInfo() => base.GetInfo() + $" | Truck | Max Load: {MaxLoadKg}kg";
    }

    class Motorbike : Vehicle // Represents a motorbike
    {
        public bool RequiresHelmet { get; set; }
        public override double CalculateRentalCost(int days) => days * 40;
        public override string GetInfo() => base.GetInfo() + $" | Motorbike | Helmet Required: {(RequiresHelmet ? "Yes" : "No")}";
    }
}
