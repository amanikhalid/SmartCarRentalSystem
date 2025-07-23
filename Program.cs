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
            }
        }
    }
}
