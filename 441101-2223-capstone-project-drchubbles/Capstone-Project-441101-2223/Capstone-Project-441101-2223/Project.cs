using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Capstone_Project_441101_2223;
using static Capstone_Project_441101_2223.ProjectManager;

Console.WriteLine("Capstone Project Menu");

ProjectManager manager = new ProjectManager();

new ProjectManagerMenu(manager).Select();