using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using MySql.Data.Entity;
using WebNeYapsam.Classes;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebNeYapsam.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool Gender { get; set; }    // 0-Erkek-False  1-Kadın-True
        public DateTime BirthDate { get; set; }
        public string Image { get; set; }
        public int Point { get; set; }
        public bool State { get; set; }
        public int RankId { get; set; }

        public virtual Rank Rank { get; set; }
        public virtual ICollection<Data> Datas { get; set; }
        public virtual ICollection<Evaluation> Evaluations { get; set; }

        [NotMapped]
        public Result Result;

        public ApplicationUser()
        {
            Result = new Result();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public List<ListUserViewModelPanel> List(/*string excludeUserId*/)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    return db.Users./*Where(u => u.UserName != excludeUserId).*/Select(u => new ListUserViewModelPanel { Id = u.Id, UserName = u.UserName, Rank = u.Rank.Name, State = u.State }).ToList();
                }
            }
            catch (Exception)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
                return null;
            }
        }

        public EditUserViewModelPanel Get(string username)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    ApplicationUser user = db.Users.Where(u => u.UserName == username).FirstOrDefault();
                    if (user == null)
                    {
                        Result = new Result(ResultType.NotFound, "Kullanıcı bulunamadı.");
                        return null;
                    }
                    List<ListTitleViewModelPanel> titles = new Title().List(user.Id);
                    List<ListAdviseViewModelPanel> advices = new Advise().List(user.Id);
                    return new EditUserViewModelPanel { Id = user.Id, UserName = user.UserName, BirthDate = user.BirthDate, Email = user.Email, Gender = user.Gender, Image = user.Image, State = user.State, RankName = user.Rank.Name, Titles = titles, Advices = advices, Point = user.Point };
                }
            }
            catch (Exception)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
                return null;
            }
        }

        public void Edit(EditUserViewModelPanel model)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    ApplicationUser user = db.Users.Find(model.Id);
                    if (user == null)
                    {
                        Result = new Result(ResultType.NotFound, "Kullanıcı bulunamadı.");
                        return;
                    }
                    else
                    {
                        if (user.UserName != model.UserName)
                        {
                            // Kullanıcı Ad Kontrolü
                            int userNameCount = db.Users.Where(u => u.UserName == model.UserName && u.Id != model.Id).Count();
                            if (userNameCount != 0)
                            {
                                Result = new Result(ResultType.AlreadyTaken, model.UserName + " isminde kullanıcı bulunmaktadır.");
                                return;
                            }
                        }
                        if (user.Email != model.Email)
                        {
                            // Email kontrolü
                            int emailCount = db.Users.Where(u => u.Email == model.Email && u.Id != model.Id).Count();
                            if (emailCount != 0)
                            {
                                Result = new Result(ResultType.AlreadyTaken, model.Email + " ile kullanıcı bulunmaktadır.");
                                return;
                            }
                        }
                    }
                    user.UserName = model.UserName;
                    user.Image = model.Image;
                    user.Email = model.Email;
                    user.BirthDate = model.BirthDate;
                    user.Gender = model.Gender;
                    user.State = model.State;
                    db.SaveChanges();
                    Result = new Result(ResultType.Success, "Bilgiler guncellendi");
                }
            }
            catch (Exception)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
            }
        }

        public void ChangeState(string id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ApplicationUser user = db.Users.Find(id);
                if (user == null)
                {
                    Result.Type = ResultType.NotFound;
                }
                else
                {
                    bool currentState = user.State;
                    if (currentState)
                    {
                        currentState = false;
                    }
                    else
                    {
                        currentState = true;
                    }
                    user.State = currentState;
                    db.SaveChanges();
                    Result.Type = ResultType.Success;
                }
            }
        }

    }

    //.....ViewModels
    //.....Panel
    public class ListUserViewModelPanel
    {
        public string Id { get; set; }
        [Display(Name = "Kullanıcı Ad")]
        public string UserName { get; set; }
        [Display(Name = "Rütbe")]
        public string Rank { get; set; }
        public bool State { get; set; }
    }

    public class EditUserViewModelPanel
    {
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = "Gerekli Alan")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Gerekli Alan"), DataType(DataType.EmailAddress, ErrorMessage = "E-mail adresi girilmedi")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Gerekli Alan")]
        public bool Gender { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), Required(ErrorMessage = "Gerekli Alan")]
        public DateTime BirthDate { get; set; }
        public string Image { get; set; }
        public int Point { get; set; }
        public bool State { get; set; }
        public string RankName { get; set; }
        public List<ListTitleViewModelPanel> Titles { get; set; }
        public List<ListAdviseViewModelPanel> Advices { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("NeYapsamDB")
        {
            Database.SetInitializer(new ApplicationDbInitializer());
        }

        public DbSet<Rank> Ranks { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Data> Datas { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Advise> Advices { get; set; }
        public DbSet<Title> Titles { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }

    public class ApplicationDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            string[,] ranks = { { "Bilgin", "10000" }, { "Tecrübeli", "1000" }, { "Normal", "100" } };
            for (int i = 0; i < ranks.GetLength(0); i++)
            {
                context.Ranks.Add(new Rank
                {
                    Name = ranks[i, 0],
                    ParentRankPoint = int.Parse(ranks[i, 1]),
                    ParentRankId = (i != 0) ? i : (int?)null
                });
            }

            IdentityRole RoleAdmin = new IdentityRole() { Name = "Admin" };
            IdentityRole RoleUser = new IdentityRole() { Name = "User" };
            RoleManager<IdentityRole> RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            RoleManager.Create(RoleAdmin);
            RoleManager.Create(RoleUser);

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            var adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@mail.com",
                BirthDate = DateTime.Now,
                RankId = 1,
                State = true,
                Image = "userphoto.jpg",
                Point = 0
            };

            var result = userManager.Create(adminUser, "Sifre*123");
            if (result.Succeeded)
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }

            base.Seed(context);
        }
    }
}