using ClassLib;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Server
{
    public static class dbWork
    {
        private static int GetComputerId(string name)
        {
            using (var db = new ApplicationContext())
            {
                var computerId = db.Computers.Where(cp => cp.Name == name).Select(cp => cp.Id).FirstOrDefault();
                return computerId;
            }
        }

        private static int GetProgramId(string name)
        {
            using (var db = new ApplicationContext())
            {
                return db.Programs.Where(pr => pr.Name == name).Select(pr => pr.Id).FirstOrDefault();
            }
        }

        private static void InsertNewComputer(AppsContainer container)
        {
          
            using (var db = new ApplicationContext())
            {
                var comp = new Computer { Name = container.hostname, UpdTime = container.date.ToString() };
                var programs = new List<Program>();
                db.Computers.Add(comp);
                Console.WriteLine($"Computer {comp.Name} added");
                db.SaveChangesAsync();
                var newComputerId = GetComputerId(comp.Name);
                foreach (var app in container.apps)
                    programs.Add(new Program {Name = app.name, Version = app.version, IdC = newComputerId});
                db.Programs.AddRange(programs.AsEnumerable());
                Console.WriteLine($"Programs {programs.Count} added to DB for {comp.Name}");
                db.SaveChangesAsync();

            }
        }

        public static void UpdateDB(AppsContainer container)
        {
            var computerId = GetComputerId(container.hostname);
            if (computerId == 0) InsertNewComputer(container);
            else
            {
                using (var db = new ApplicationContext())
                {
                    var comp = new Computer
                    {
                        Id = computerId, Name = container.hostname, UpdTime = container.date.ToString()
                    };
                    db.Computers.AddOrUpdate(comp);
                    foreach (var app in container.apps)
                    {
                        if (app.status == AppStatus.Installed && GetProgramId(app.name) == 0)
                        {
                            db.Programs.Add(new Program {Name = app.name, Version = app.version, IdC = computerId});
                            Console.WriteLine("1 program added for " + container.hostname);
                        }
                        else if (app.status == AppStatus.Updated)
                        {
                            var programId = GetProgramId(app.name);
                            var pr = db.Programs.FirstOrDefault(c => c.Id == programId);
                            pr.Version = app.version;
                            Console.WriteLine("1 program updated for " + container.hostname);
                        }
                        else if (app.status == AppStatus.Deleted)
                        {
                            var programId = GetProgramId(app.name);
                            var pr = db.Programs.FirstOrDefault(o => o.Id == programId);
                            db.Programs.Remove(pr);
                            Console.WriteLine("1 program deleted for " + container.hostname);
                        }
                    }
                    db.SaveChangesAsync();
                }
            }
        }
        public static List<AppsContainer> GetAllData()
        {
            var appsContainers = new List<AppsContainer>();
            using (var db = new ApplicationContext())
            {
                var computers = db.Computers.ToList();
                foreach(var comp in computers)
                {
                    var appsContainer = new AppsContainer {hostname = comp.Name, date = long.Parse(comp.UpdTime)};
                    var programs = db.Programs.Where(p=>p.IdC==comp.Id).ToList();
                    foreach (var pr in programs)
                    {
                        var app = new App(pr.Name, pr.Version);
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
                var computers = db.Computers.ToList();
                foreach (var comp in computers)
                {
                    var programs = db.Programs.Where(p => p.IdC == comp.Id).ToList();
                    db.Programs.RemoveRange(programs.AsQueryable());
                }
                db.Computers.RemoveRange(computers.AsQueryable());

                var deleteSqliteSequence = "DELETE FROM sqlite_sequence";
                db.Database.ExecuteSqlCommand(deleteSqliteSequence);

                Console.WriteLine("DB cleared");
                db.SaveChangesAsync();
            }
        }

        public static List<AppsContainer> GetOneComputerData(string hostname)
        {
            var appsContainers = new List<AppsContainer>();
            using (var db = new ApplicationContext())
            {
                var computerId = GetComputerId(hostname);
                if (computerId == 0)
                {
                    return appsContainers;
                }
                var computer = db.Computers.Where(cc=>cc.Id== computerId).ToList().FirstOrDefault();
                var appsContainer = new AppsContainer { hostname = computer.Name, date = long.Parse(computer.UpdTime) };
                var programs = db.Programs.Where(p => p.IdC == computer.Id).ToList();
                foreach (var pr in programs)
                {
                    var app = new App(pr.Name, pr.Version);
                    appsContainer.apps.Add(app);
                }
                appsContainers.Add(appsContainer); 
            }
            return appsContainers;
        }
    }
}
