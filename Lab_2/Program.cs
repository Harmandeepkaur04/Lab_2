using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Employee
{
    public int EmployeeID { get; set; }
    public string Name { get; set; }
    public string SIN { get; set; }

    public virtual decimal CalculateWeeklyPay()
    {
        return 0;
    }
}

public class Salaried : Employee
{
    public decimal Salary { get; set; }

    public override decimal CalculateWeeklyPay()
    {
        return Salary;
    }
}

public class Wage : Employee
{
    public decimal HourlyRate { get; set; }
    public decimal WorkHours { get; set; }

    public override decimal CalculateWeeklyPay()
    {
        decimal overtimeHours = Math.Max(0, WorkHours - 40);
        decimal regularHours = WorkHours - overtimeHours;
        return (regularHours * HourlyRate) + (overtimeHours * HourlyRate * 1.5m);
    }
}

public class PartTime : Employee
{
    public decimal HourlyRate { get; set; }
    public decimal WorkHours { get; set; }

    public override decimal CalculateWeeklyPay()
    {
        return HourlyRate * WorkHours;
    }
    class Program
    {
        static void Main()
        {
    
            List<Employee> employees = LoadEmployeesFromFile("res/employees.txt");

            decimal averageWeeklyPay = CalculateAverageWeeklyPay(employees);
            Console.WriteLine($"Average Weekly Pay: ${averageWeeklyPay:F2}");



        }

        static List<Employee> LoadEmployeesFromFile(string filePath)
        {
            List<Employee> employees = new List<Employee>();
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] parts = line.Split(',');

                if (parts.Length < 4)
                {
                    continue;
                }

               
                if (!int.TryParse(parts[0], out int employeeID))
                {
                    continue;
                }

                string name = parts[1];
                string sin = parts[2];

                if (employeeID >= 0 && employeeID <= 4)
                {
                    
                    if (!decimal.TryParse(parts[3], out decimal salary))
                    {
                        
                        continue; 
                    }

                    employees.Add(new Salaried { EmployeeID = employeeID, Name = name, SIN = sin, Salary = salary });
                }
                else if (employeeID >= 5 && employeeID <= 7)
                {
                    if (!decimal.TryParse(parts[3], out decimal hourlyRate))
                    {
                        continue; 
                    }

                    if (!decimal.TryParse(parts[4], out decimal workHours))
                    {
                        
                        continue;
                    }

                    employees.Add(new Wage { EmployeeID = employeeID, Name = name, SIN = sin, HourlyRate = hourlyRate, WorkHours = workHours });
                }
                else if (employeeID >= 8 && employeeID <= 9)
                {
                   
                    if (!decimal.TryParse(parts[3], out decimal hourlyRate))
                    {
                        
                        continue; 
                    }

                    
                    if (!decimal.TryParse(parts[4], out decimal workHours))
                    {
                        continue;
                    }

                    employees.Add(new PartTime { EmployeeID = employeeID, Name = name, SIN = sin, HourlyRate = hourlyRate, WorkHours = workHours });
                }
            }

            return employees;
        }


        static decimal CalculateAverageWeeklyPay(List<Employee> employees)
        {
            if (employees.Count == 0)
            {
                return 0;
            }

            decimal totalWeeklyPay = employees.Sum(e => e.CalculateWeeklyPay());
            return totalWeeklyPay / employees.Count;
        }

        static Employee GetHighestWageEmployee(List<Employee> employees)
        {
            return employees.OfType<Wage>().OrderByDescending(e => e.CalculateWeeklyPay()).FirstOrDefault();
        }

        static Employee GetLowestSalariedEmployee(List<Employee> employees)
        {
            return employees.OfType<Salaried>().OrderBy(e => e.CalculateWeeklyPay()).FirstOrDefault();
        }

        static Dictionary<string, decimal> GetEmployeeCategoryPercentages(List<Employee> employees)
        {
            int totalEmployees = employees.Count;
            var categoryCounts = employees.GroupBy(e => e.GetType().Name).ToDictionary(g => g.Key, g => (decimal)g.Count());
            var categoryPercentages = categoryCounts.ToDictionary(kvp => kvp.Key, kvp => kvp.Value / totalEmployees);
            return categoryPercentages;
        }
    }
}

