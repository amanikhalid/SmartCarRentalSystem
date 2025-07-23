using System;
using System.Collections.Generic;
using System.IO;

namespace SmartCarRentalSystem
{
    internal class Program
    {
        static List<Vehicle> vehicles = new List<Vehicle>();
        static readonly string dataFile = "vehicles.txt";

        static void Main(string[] args)
        {
            LoadData();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("\nSmartCar Rentals Main Menu");
                Console.WriteLine("1. View All Vehicles");
                Console.WriteLine("2. Filter by Vehicle Type");
                Console.WriteLine("3. Rent a Vehicle");
                Console.WriteLine("4. Exit");

                int option = GetIntInput("\nChoose an option: ", 1, 4);

                if (option == 1) // View all vehicles 
                {
                    DisplayVehicles(vehicles);
                }

                else if (option == 2) // Filter by Vehicle Type
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

                else if (option == 3) // Rent a Vehicle
                {
                    Console.WriteLine("\nWhich type of vehicle would you like to rent?");
                    Console.WriteLine("1. Car\n2. Truck\n3. Motorbike");
                    int type = GetIntInput("Choose type: ", 1, 3);

                    List<Vehicle> list = vehicles.FindAll(v =>
                        (type == 1 && v is Car) ||
                        (type == 2 && v is Truck) ||
                        (type == 3 && v is Motorbike));


                }

                if (list.Count == 0) // Save and Exit 
                {
                    Console.WriteLine("\nNo vehicles available of this type.");
                    Console.ReadLine();
                    continue;
                }

                DisplayVehicles(list); // Display filtered vehicles
                int select = GetIntInput("\nSelect a vehicle to rent: ", 1, list.Count);
                Vehicle selected = list[select - 1];

                if (DateTime.Now.Year - selected.Year > 10) // Check vehicle age
                {
                    Console.WriteLine("This vehicle is too old to rent.");
                    Console.ReadLine();
                    continue;
                }

                int days = GetIntInput("Enter number of rental days: ", 1, 365); // Rental days input
                double total = 0;

                if (selected is Car car) // Calculate rental cost for car
                {
                    bool withDriver = GetYesNo("Need a driver? (y/n): ");
                    total = car.CalculateRentalCost(days, withDriver);
                }

            }

        static void LoadData()
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
                SaveData(); // Save default data on first run
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

        static void SaveData()
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

        static int GetIntInput(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int value) && value >= min && value <= max)
                    return value;
                Console.WriteLine("Invalid input. Try again.");
            }
        }

        static double GetDoubleInput(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), out double value) && value >= 0)
                    return value;
                Console.WriteLine("Invalid input. Try again.");
            }
        }

        static bool GetYesNo(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine().ToLower();
                if (input == "y" || input == "yes") return true;
                if (input == "n" || input == "no") return false;
                Console.WriteLine("Invalid input. Please enter y or n.");
            }
        }
    }

    // Base class
    abstract class Vehicle
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string LicensePlate { get; set; }

        public virtual double CalculateRentalCost(int days) => 0;
        public virtual string GetInfo() => $"{Brand} {Model} {Year}";
    }

    class Car : Vehicle
    {
        public bool IsLuxury { get; set; }

        public override double CalculateRentalCost(int days)
        {
            return days * (IsLuxury ? 80 : 60);
        }

        public double CalculateRentalCost(int days, bool withDriver)
        {
            double baseCost = CalculateRentalCost(days);
            return withDriver ? baseCost + (days * 20) : baseCost;
        }

        public override string GetInfo() => $"{base.GetInfo()} | Car | Luxury: {(IsLuxury ? "Yes" : "No")}";
    }

    class Truck : Vehicle
    {
        public double MaxLoadKg { get; set; }

        public override double CalculateRentalCost(int days)
        {
            return days * 100;
        }

        public double CalculateRentalCost(int days, double cargoWeight)
        {
            return CalculateRentalCost(days) + (cargoWeight * 0.01);
        }

        public override string GetInfo() => $"{base.GetInfo()} | Truck | Max Load: {MaxLoadKg}kg";
    }

    class Motorbike : Vehicle
    {
        public bool RequiresHelmet { get; set; }

        public override double CalculateRentalCost(int days)
        {
            return days * 40;
        }

        public override string GetInfo() => $"{base.GetInfo()} | Motorbike | Helmet Required: {(RequiresHelmet ? "Yes" : "No")}";
    }
}
