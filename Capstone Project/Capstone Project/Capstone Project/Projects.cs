namespace Capstone_Project_441101_2223
{
    //This code is most of the business logic, This class is the roject manager class. as the name implies it is responsible for managing the projects.
    //it contains a lisst of the projects and taken ids so a user cannnot create anothe project with the same ID.  it also contains the methods for adding and removing projects from the manager
    internal class ProjectManager
    {
        public List<Project> Projects { get; private set; }
        public List<int> TakenIDs { get; private set; }
        public ProjectManager()
        {
            Projects = new List<Project>();
            TakenIDs = new List<int>();
        }

        public void AddProject(Project pProject) // adds a project to the project list and taken IDs list
        {
            if (!TakenIDs.Contains(pProject.ID))
            {
                Projects.Add(pProject);
                TakenIDs.Add(pProject.ID);
            }
            else
            {
                Console.WriteLine($"Project with ID {pProject.ID} already exists. Cannot add project.");
            }
        }

        public void RemoveProject(Project pProject) // removes a project from the project list and the TakenIDs list
        {
            Projects.Remove(pProject);
            TakenIDs.Remove(pProject.ID);
        }
        public Project SearchProjectByID(int projectID)
        {
            return Projects.Find(project => project.ID == projectID);
        }

        //This is the project class, it defines all of the things that a project is required to have, as awell as having an ID counter which incrents with each new project.
        //This code is inherited from for both the NewBuild and Renovaion classes and has all of the key aspects a project needs. it also allows for projects to add new transactions
        //to the list of transactions. This is important as it allows the user to have projects with multiple transactions.
        internal class Project
        {
            public static int IDCounter = 100;
            public int ID = IDCounter;
            public decimal Total = 0;
            public decimal Sales = 0;
            public decimal Purchases = 0;
            public decimal Refund = 0;
            public string Type;
            public List<Transaction> Transactions { get; set; }

            public Project()
            {
                Transactions = new List<Transaction>();
            }

            public void AddTransaction(Transaction transaction)
            {
                Transactions.Add(transaction);
            }


        }

        //This code defines everything that a transacton needs as well as establising the method for adding the transactions to the projects stored in project manager.
        internal abstract class Transaction
        {
            public decimal Amount { get; protected set; }
            public string Type { get; protected set; }

            public Transaction(decimal amount, string type)
            {
                Amount = amount;
                Type = type;
            }

            public abstract void ExecuteTransaction(Project project);
        }

        //This code inherits from transaction and assigns the project a new sale.
        internal class Sale : Transaction
        {
            public Sale(decimal amount) : base(amount, "S") { }
            public override void ExecuteTransaction(Project project)
            {
                project.Sales = project.Sales + Amount;
                project.Total += Amount;
                project.AddTransaction(this);
            }
        }

        //This code inherits from transaction and assigns the project a new purchase.
        internal class Purchase : Transaction
        {
            public Purchase(decimal amount, string type) : base(amount, type) { }
            decimal refundValue;
            public override void ExecuteTransaction(Project project)
            {
                project.Purchases = project.Purchases + Amount;
                project.AddTransaction(this);
                if (project.Type == "L")
                {
                    refundValue = Amount * 0.2m;
                    project.Refund += refundValue;
                    project.Total = (project.Total - Amount) + refundValue;
                }
                else
                {
                    project.Total = project.Total - Amount;
                }
            }

        }

        //This code established teh newbuild class which is a type of project. It keeps track of the refund and adds the cost of teh land to transactions as well as a refund as it is a new build.
        internal class NewBuild : Project
        {
            public const int MIN_Amount = 0;

            public NewBuild(decimal amount, int id)
            {
                ID = id;


                Purchases = Purchases + amount;

                decimal refundamount = amount * 0.2m;

                Refund += refundamount;
                Type = "L";

                Total -= (amount - Refund);

                AddTransaction(new Purchase(amount, "L"));

                Console.WriteLine($"NewBuild project {ID} successfully created");
            }
            public NewBuild(decimal amount)
            {
                IDCounter++;
                Purchases = Purchases + amount;

                decimal refundamount = amount * 0.2m;

                Refund += refundamount;
                Type = "L";

                Total -= (amount - Refund);

                AddTransaction(new Purchase(amount, "L"));

                Console.WriteLine($"NewBuild project {ID} successfully created");
            }
        }

        //This code does similar to the NewBuild class.
        internal class Renovation : Project
        {
            public const int MIN_Amount = 0;
            public Renovation(decimal amount, int id)
            {
                ID = id;
                Purchases = Purchases + amount;

                Type = "R";

                Total -= (amount - Refund);

                AddTransaction(new Purchase(amount, "R"));

                Console.WriteLine($"Renevation project {ID} successfully created");
            }
            public Renovation(decimal amount)
            {
                IDCounter++;
                Purchases = Purchases + amount;

                Type = "R";

                Total -= amount;

                AddTransaction(new Purchase(amount, "R"));

                Console.WriteLine($"Renevation project {ID} successfully created");
            }
        }
    }
}
