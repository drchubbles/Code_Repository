using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Capstone_Project_441101_2223;
using static Capstone_Project_441101_2223.DisplayPurchase;
using static Capstone_Project_441101_2223.ProjectManager;
using static Capstone_Project_441101_2223.ProjectManager.NewBuild;


namespace Capstone_Project_441101_2223
{
    //This class is teh first class created within the Capstone project. This code is responcble for displayingt teh first menu to the user.
    //It allows them pick 2 choices, they can either create their own project or load a project from a file. Once the user had done either of these and at least one project,
    //Exists within the project manager, then it allows the user to edit their existing project or view all of the projects that have been created in a table.
    //This code makes use of the reusable menu system which was provided by simon.
    internal class ProjectManagerMenu : ConsoleMenu
    {
        private ProjectManager _manager;

        public ProjectManagerMenu(ProjectManager manager)
        {
            _manager = manager;
        }

        public override void CreateMenu()
        {
            _menuItems.Clear();
            _menuItems.Add(new AddNewProjectMenu(_manager));
            _menuItems.Add(new ReadProjectMenu(_manager));
            if (_manager.Projects.Count > 0)
            {
                _menuItems.Add(new SelectExistingProjectMenu(_manager));
                _menuItems.Add(new ViewProjectsMenuItem(_manager));
            }
            _menuItems.Add(new ExitMenuItem(this));
        }

        public override string MenuText()
        {
            return "Project Manager Menu";
        }
    }
    
    // This code inherits the manager from ProjectManagerMenu and enables the user to either add a new land(newbuild) or a renovation project.

    internal class AddNewProjectMenu : ConsoleMenu
    {
        ProjectManager _manager;

        public AddNewProjectMenu(ProjectManager manager)
        {
            _manager = manager;
        }

        public override void CreateMenu()
        {
            _menuItems.Clear();
            _menuItems.Add(new AddLandMenuItem(_manager));
            _menuItems.Add(new AddRenovationMenuItem(_manager));
        }

        public override string MenuText()
        {
            return "Add New Project";
        }
    }
    //This code inherits teh manager from the addnewmenu class. The code asks the user for the amount that the land cost for the newbuild.
    //This code then takes this information and creates a newbuild project. This project is then added to the project managers list of projects. This cod ethen resets the console back at teh new screen
    // if the user did not have any project made before creating this and didnt load from a file, then this will unlck the edit existing file section of the origonal table
    internal class AddLandMenuItem : MenuItem
    {
        private ProjectManager _manager;

        public AddLandMenuItem(ProjectManager manager)
        {
            _manager = manager;
        }

        public override string MenuText()
        {
            return "New Build";
        }

        public override void Select()
        {
            decimal amount = ConsoleHelpers.GetDecimal(NewBuild.MIN_Amount, "How much did this land project cost?");
            var newBuildProject = new NewBuild(amount);
            _manager.AddProject(newBuildProject);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
            new ProjectManagerMenu(_manager).Select();

        }
    }
  
    //This code is virtually the same as newbuild appart from the fact it created a renovation class not a newbuild.
    //please see newbuild comments for explination of how the code works.
    internal class AddRenovationMenuItem : MenuItem
    {
        private ProjectManager _manager;

        public AddRenovationMenuItem(ProjectManager manager)
        {
            _manager = manager;
        }

        public override string MenuText()
        {
            return "Renovation";
        }

        public override void Select()
        {
            decimal amount = ConsoleHelpers.GetDecimal(Renovation.MIN_Amount, "How much did this Renovation project cost?");

            var RenovationProject = new Renovation(amount);
            _manager.AddProject(RenovationProject);
            new ProjectManagerMenu(_manager).Select();
        }
    }

    //This code creates a class called selct existing project. This code is only accessible after a project has been creted,. this code inherits a list of projects
    // and the manager from the project manager. It then uses the list of projects to create a list of projects for the user to be able to select.
    internal class SelectExistingProjectMenu : ConsoleMenu
    {
        private ProjectManager _manager;
        private List<Project> _projects;

        public SelectExistingProjectMenu(ProjectManager manager)
        {
            _manager = manager;
            _projects = _manager.Projects;
        }

        public override void CreateMenu()
        {
            _menuItems.Clear();
            foreach (var project in _projects)
            {
                _menuItems.Add(new ProjectMenuItem(project, _manager));
            }
            _menuItems.Add(new ExitMenuItem(this));
        }

        public override string MenuText()
        {
            return "Edit Existing Projects";
        }

        // This code is the menu item wgich displays when a user picks an existing project. Thsi code is responcsible for linking all of the code together and allowing
        // the user to pick from all of the other menu items. this code takes the project from the selectexistingprojectmenu and puts it into all of the other menus
        // This code allows the user to edit all of their projects.
        internal class ProjectMenuItem : ConsoleMenu
        {
            private ProjectManager.Project _project;
            ProjectManager _manager;

            public ProjectMenuItem(ProjectManager.Project project, ProjectManager manager)
            {
                _project = project;
                _manager = manager;
            }

            public override void CreateMenu()
            {
                _menuItems.Clear();
                _menuItems.Add(new AddSaleMenuItem(_project, _project.ID));
                _menuItems.Add(new AddPurchaseMenuItem(_project, _project.ID));
                _menuItems.Add(new DisplaySales(_project));
                _menuItems.Add(new DisplayPurchase(_project));
                _menuItems.Add(new GenorateProjectSummery(_project));
                _menuItems.Add(new RemoveProjectMenuItem(_project, _manager));
                _menuItems.Add(new ExitMenuItem(this));

            }
            public override string MenuText()
            {
                return $"Project {_project.ID}";
            }
        }
        //This code is responciible for adding a sale to an existing project. this code takes a project from editExistingProjectMenu as well as the projects ID.
        //It then asks the user to add an amount of a sale, once a user had done this it assigns the type of sale to the transaction and adds it to the projects list of transactions
        //it then displays a message to teh user letting them know the sale has been added to the project number.
        internal class AddSaleMenuItem : MenuItem
        {
            private ProjectManager.Project _project;
            private int _projectId;

            public AddSaleMenuItem(ProjectManager.Project project, int projectId)
            {
                _project = project;
                _projectId = projectId;
            }

            public override string MenuText()
            {
                return "Add Sale";
            }

            public override void Select()
            {
                decimal amount = ConsoleHelpers.GetDecimal(0, "Enter the sale amount:");
                var sale = new ProjectManager.Sale(amount);
                sale.ExecuteTransaction(_project);
                Console.WriteLine($"Sale of {amount:C} added to project {_projectId}");
            }
        }
    }

    //This code is responciible for adding a purchase to an existing project. similar to the addsales menu item it takes a proejct from editExistingProjectMenu
    //It then asks the user to add an amount of a purchase, once a user had done this it assigns the type of purchase to the transaction and adds it to the projects list of transactions
    //it then displays a message to teh user letting them know the purchase has been added.
    internal class AddPurchaseMenuItem : MenuItem
    {
        private ProjectManager.Project _project;
        private int _projectId;

        public AddPurchaseMenuItem(ProjectManager.Project project, int projectId)
        {
            _project = project;
            _projectId = projectId;
        }

        public override string MenuText()
        {
            return "Add Purchase";
        }

        public override void Select()
        {
            decimal amount = ConsoleHelpers.GetDecimal(0, "Enter the purchase amount:");
            var purchase = new ProjectManager.Purchase(amount, "P");
            purchase.ExecuteTransaction(_project);
            Console.WriteLine($"Purchase  of {amount:C} added to project {_projectId}");
        }
    }
    //This code is responcible for displaying all of the sales which are within a project. This is important as the user will need to be able to see whre they have made sales
    //This code inherits the project from the EditExistingProjectMenu. It then gets all of the transactions that have the sales type, and displays them to the user.
    // DISCLAIMER: I used https://stackoverflow.com/questions/52501135/format-table-to-text-output-in-c-sharp for reference on how to create a display table, please note
    //no code was directly copied or stolen, I only used this to learn how to display the code in a nice table. I am putting this here as a reference for the sake of transparency.
    internal class DisplaySales : MenuItem
    {
        private ProjectManager.Project _project;

        public DisplaySales(ProjectManager.Project project)
        {
            _project = project;
        }

        public override string MenuText()
        {
            return "Display Sales";
        }

        public override void Select()
        {
            Console.WriteLine($"Sales for Project {_project.ID}:");
            Console.WriteLine("| Type | Amount |");
            Console.WriteLine("|------|--------|");

            foreach (var transaction in _project.Transactions)
            {
                if (transaction is Sale sale && sale.Type == "S")
                {
                    Console.WriteLine($"|  S   |{sale.Amount:C}|");
                }
            }
        }
    }
    //This code is reponsible for displaying all of the sales that are stored within a transaction list within each project class that is stored in the project manager.
    //It inherits the project from editExistingProjectMenu and is capable of displaying all of the purchases made in a project.
    // DISCLAIMER: I used https://stackoverflow.com/questions/52501135/format-table-to-text-output-in-c-sharp for reference on how to create a display table, please note
    //no code was directly copied or stolen, I only used this to learn how to display the code in a nice table. I am putting this here as a reference for the sake of transparency.
    internal class DisplayPurchase : MenuItem
    {
        private ProjectManager.Project _project;

        public DisplayPurchase(ProjectManager.Project project)
        {
            _project = project;
        }

        public override string MenuText()
        {
            return "Display Purchases";
        }

        public override void Select()
        {
            Console.WriteLine($"Purchases for Project {_project.ID}:");
            Console.WriteLine("+------+--------+");
            Console.WriteLine("| Type | Amount |");
            Console.WriteLine("+------+--------+");

            foreach (var transaction in _project.Transactions)
            {
                if (transaction is Purchase purchase)
                {
                    string type = "";
                    switch (purchase.Type)
                    {
                        case "L":
                            type = "L";
                            break;
                        case "R":
                            type = "R";
                            break;
                        case "P":
                            type = "P";
                            break;
                    }
                    Console.WriteLine($"|  {type}   |{purchase.Amount,8:C}|");
                }
            }

            Console.WriteLine("+------+--------+");
        }

        // This code inherits a project from the edit existing project class. It then displays a table summery of all of the project information for the user to see.
        // DISCLAIMER: I used https://stackoverflow.com/questions/52501135/format-table-to-text-output-in-c-sharp for reference on how to create a display table, please note
        //no code was directly copied or stolen, I only used this to learn how to display the code in a nice table. I am putting this here as a reference for the sake of transparency.
        internal class GenorateProjectSummery : MenuItem
        {
            private ProjectManager.Project _project;

            public GenorateProjectSummery(ProjectManager.Project project)
            {
                _project = project;

            }

            public override string MenuText()
            {
                return "Display Project summery";
            }

            public override void Select()
            {
                Console.WriteLine($"Purchases for Project {_project.ID}:");
                Console.WriteLine("+----+---------+-----------+--------+--------+");
                Console.WriteLine("| Id |  Sales  | Purchases | Refund | Profit |");
                Console.WriteLine("+----+---------+-----------+--------+--------+");
                Console.WriteLine($"| {_project.ID,-3} |{_project.Sales,9:N2}|{_project.Purchases,11:N2}|{_project.Refund,7:N2}|{_project.Total,8:N2} |");
                Console.WriteLine("+----+---------+-----------+--------+--------+");
            }
        }
    }

    //This code inherits the project manager, and prints out all of the ifnromation for all of teh project sas well as giveng the combined total of all of the projects.
    //This allows the user ot see all of the projects that they have as well as all of the information that these project hold.
    // I used https://stackoverflow.com/questions/52501135/format-table-to-text-output-in-c-sharp for reference on how to create a display table, please note
    //no code was directly copied or stolen, I only used this to learn how to display the code in a nice table. I am putting this here as a reference for for the sake of transparency.
    internal class ViewProjectsMenuItem : MenuItem
    {
        private ProjectManager _manager;

        public ViewProjectsMenuItem(ProjectManager manager)
        {
            _manager = manager;
        }
        decimal Totalsales;
        decimal Totalpurchases;
        decimal Totalrefund;
        decimal Overalltotal;

        public override string MenuText()
        {
            return "View Projects";
        }

        public override void Select()
        {
            string tableFormat = "|{0,-4}|{1,14:N2}|{2,16:N2}|{3,8:N2}|{4,16:N2}|";
            string header = string.Format(tableFormat, "Id", "Sales", "Purchases", "Refund", "Profit");
            Console.WriteLine("+" + new string('-', header.Length - 2) + "+");
            Console.WriteLine(header);
            Console.WriteLine("+" + new string('-', header.Length - 2) + "+");

            foreach (var project in _manager.Projects)
            {
                Console.WriteLine(string.Format(tableFormat, project.ID, project.Sales, project.Purchases, project.Refund, project.Total));
                Totalsales += project.Sales;
                Totalpurchases += project.Purchases;
                Totalrefund += project.Refund;
                Overalltotal += project.Total;
            }
            Console.WriteLine(string.Format(tableFormat, "All", Totalsales, Totalpurchases, Totalrefund, Overalltotal));
            Console.WriteLine("+" + new string('-', header.Length - 2) + "+");
        }
    }

    //This code uses the member method of the project manager which removes the project that it is given from the list of projects.
    //This is the menuItem which links the menu classes and the projects together to make them work, following object oriented principles.
    internal class RemoveProjectMenuItem : MenuItem
    {
        private Project _project;
        private ProjectManager _manager;
        public RemoveProjectMenuItem(Project project, ProjectManager manager)
        {
            _project = project;
            _manager = manager;
        }


        public override string MenuText()
        {
            return $"Remove project {_project.ID} and exit.";
        }

        public void RemoveProject(Project _project, ProjectManager _manager)
        {
            _manager.RemoveProject(_project);
        }
        public override void Select()
        {
            RemoveProject(_project, _manager);
            Console.WriteLine("Project removal successful.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
            new ProjectManagerMenu(_manager).Select();
        }
    }

    //Used website https://learn.microsoft.com/en-us/troubleshoot/developer/visualstudio/csharp/language-compilers/read-write-text-file and https://learn.microsoft.com/en-us/dotnet/csharp/how-to/parse-strings-using-split and https://learn.microsoft.com/en-us/dotnet/standard/base-types/divide-up-strings
    //in order to help develop this code. I also used infromation and reviewed code from the summative labs order to create this program
    //The code below is the read menu menu class which is the first piece of code i developed, Then decided to start again and eventually came back to reuse code.
    //This code inherits from the manager, takes a filename from a user and then reads the file and is capable of splitting strings in the both ways we were given them.
    //It does thsi by splitting the line into its base parts and assigning the projectID, Type of transaction and amount. It then feeds this information into the relevant sections, either to create a newbuild
    //renovation, sale or Purchase. For each successful line read it feeds that information back to the user so they can see which lines have been read.
    internal class ReadProjectMenu : MenuItem
    {
        private ProjectManager _manager;
        public ReadProjectMenu(ProjectManager manager)
        {
            _manager = manager;
        }


        public override string MenuText()
        {
            return "Read Projects from a file.";
        }

        public void ReadFile(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    ReadTransactionLine(line);
                }
            }
        }

        public void ReadTransactionLine(string line)
        {
            if (line.Contains("(") && line.Contains(")"))
            {
                string[] parts = line.Split('=', '(', ')');
                if (parts.Length != 4)
                {
                    Console.WriteLine($"Skipping invalid line: {line}");
                    return;
                }

                int projectId;
                if (!int.TryParse(parts[1], out projectId))
                {
                    Console.WriteLine($"Skipping invalid project id: {parts[1]}");
                    return;
                }

                string transactionType = parts[0].Trim();

                decimal amount;
                if (!decimal.TryParse(parts[3].Replace(";", "").Trim(), out amount))
                {
                    Console.WriteLine($"Skipping invalid amount: {parts[3]}");
                    return;
                }

                if (transactionType == "Land")
                {
                    var newBuildProject = new NewBuild(amount, projectId);
                    _manager.AddProject(newBuildProject);
                }
                else if (transactionType == "Renovation")
                {
                    var RenovationProject = new Renovation(amount, projectId);
                    _manager.AddProject(RenovationProject);
                }


                foreach (Project project in _manager.Projects)
                {
                    int projectID = project.ID;
                    Project foundproject = _manager.SearchProjectByID(projectID);
                    if (projectID == projectId)
                    {
                        if (transactionType == "Sale")
                        {
                            var sale = new ProjectManager.Sale(amount);
                            sale.ExecuteTransaction(foundproject);
                            Console.WriteLine($"Sale of {amount:C} added to project {projectID}");
                        }
                        else if (transactionType == "Purchase")
                        {
                            var purchase = new ProjectManager.Purchase(amount, "P");
                            purchase.ExecuteTransaction(foundproject);
                            Console.WriteLine($"Purchase of {amount:C} added to project {projectID}");
                        }
                    }
                }
            }

            else
            {
                string[] values = line.Split(',');
                if (values.Length != 3)
                {
                    Console.WriteLine($"Skipping invalid line: {line}");
                    return;
                }

                int projectId;
                if (!int.TryParse(values[0], out projectId))
                {
                    Console.WriteLine($"Skipping invalid project id: {values[0]}");
                    return;
                }

                string transactionType = values[1].Trim();

                decimal amount;
                if (!decimal.TryParse(values[2], out amount))
                {
                    Console.WriteLine($"Skipping invalid amount: {values[2]}");
                    return;
                }

                if (transactionType == "L")
                {
                    var newBuildProject = new NewBuild(amount, projectId);
                    _manager.AddProject(newBuildProject);
                }
                else if (transactionType == "R")
                {
                    var RenovationProject = new Renovation(amount, projectId);
                    _manager.AddProject(RenovationProject);
                }

                foreach (Project project in _manager.Projects)
                {
                    int projectID = project.ID;
                    Project foundproject = _manager.SearchProjectByID(projectID);
                    if (projectID == projectId)
                    {
                        if (transactionType == "S")
                        {
                            var sale = new ProjectManager.Sale(amount);
                            sale.ExecuteTransaction(foundproject);
                            Console.WriteLine($"Sale of {amount:C} added to project {projectID}");
                        }
                        else if (transactionType == "P")
                        {
                            var purchase = new ProjectManager.Purchase(amount, "P");
                            purchase.ExecuteTransaction(foundproject);
                            Console.WriteLine($"Purchase of {amount:C} added to project {projectID}");
                        }
                    }
                }
            }
        }
        public override void Select()
        {
            Console.Write("Enter file name: ");
            string fileName = Console.ReadLine();

            try
            {
                ReadFile(fileName);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                return;
            }
        }
    }
}

