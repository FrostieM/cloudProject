using ClassLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class dbWork
    {
   
        static public int GetComputerId(string name)
        {
            using (var db = new ApplicationContext())
            {
                var computerId = db.Computers.Where(cp => cp.Name == name).Select(cp => cp.Id).FirstOrDefault();
                return computerId;
            }
        }
        static public int GetProgramId(string name)
        {
            using (var db = new ApplicationContext())
            {
                var programId = db.Programs.Where(pr => pr.Name == name).Select(pr => pr.Id).FirstOrDefault();
                return programId;
            }
        }
        static public void InsertNewComputer(AppsContainer container)
        {
          
            using (var db = new ApplicationContext())
            {
                var comp = new Computer { Name = container.hostname, UpdTime = container.date.ToString() };
                List<Program> programs = new List<Program>();
                db.Computers.Add(comp);
                Console.WriteLine("Computer "+comp.Name+ " added");
                db.SaveChangesAsync();
                int newComputerId = GetComputerId(comp.Name);
                foreach (var app in container.apps)
                {
                    programs.Add(new Program { Name = app.name, Version = app.version, IdC = newComputerId});
                }

                db.Programs.AddRange(programs.AsEnumerable());
                Console.WriteLine("Programs " + programs.Count + " added for "+ comp.Name);
                db.SaveChangesAsync();

            }
        }
        static public void UpdateDB(AppsContainer container)
        {
                int computerId= GetComputerId(container.hostname);
                if (computerId == 0)
                    InsertNewComputer(container);
                else
                {
                    foreach (var app in container.apps)
                    {
                        if (app.status == AppStatus.Installed && GetProgramId(app.name)==0)
                        {
                            using (var db = new ApplicationContext())
                            {
                                db.Programs.Add(new Program { Name = app.name, Version = app.version, IdC = computerId });
                                Console.WriteLine("1 program added for " + container.hostname);
                                db.SaveChangesAsync();
                            }
                        }
                        else if (app.status == AppStatus.Updated)
                        {
                            using (var db = new ApplicationContext())
                            {
                                int programId = GetProgramId(app.name);
                                var pr = db.Programs.Where(c => c.Id == programId).FirstOrDefault();
                                pr.Version = app.version;
                            Console.WriteLine("1 program updated for " + container.hostname);
                            db.SaveChangesAsync();
                            }
                        }
                        else if(app.status == AppStatus.Deleted)
                        {
                            using (var db = new ApplicationContext())
                            {
                                int programId = GetProgramId(app.name);
                                var pr = db.Programs.Where(o => o.Id == programId).FirstOrDefault();
                                db.Programs.Remove(pr);
                                Console.WriteLine("1 program deleted for " + container.hostname);
                                db.SaveChangesAsync();
                            }
                           
                        }
                    }
                }
        }
        public static List<AppsContainer> GetAllData()
        {
            List<AppsContainer> appsContainers = new List<AppsContainer>();
            using (var db = new ApplicationContext())
            {
                List<Computer> computers = db.Computers.ToList();
                foreach(var comp in computers)
                {
                    AppsContainer appsContainer = new AppsContainer();
                    appsContainer.hostname = comp.Name;
                    appsContainer.date = long.Parse(comp.UpdTime);
                    List<Program> programs = db.Programs.Where(p=>p.IdC==comp.Id).ToList();
                    foreach (var pr in programs)
                    {
                        App app = new App(pr.Name, pr.Version);
                        appsContainer.apps.Add(app);
                    }
                    appsContainers.Add(appsContainer);
                }
            }
            return appsContainers;
        }
        public static void ClearDB()
        {
            using (var db = new ApplicationContext())
            {
                List<Computer> computers = db.Computers.ToList();
                foreach (var comp in computers)
                {
                    List<Program> programs = db.Programs.Where(p => p.IdC == comp.Id).ToList();
                    db.Programs.RemoveRange(programs.AsQueryable());
                }
                db.Computers.RemoveRange(computers.AsQueryable());

                string deleteSqliteSequence = "DELETE FROM sqlite_sequence";
                db.Database.ExecuteSqlCommand(deleteSqliteSequence);

                Console.WriteLine("DB cleared");
                db.SaveChangesAsync();
            }
        }
    }
}
