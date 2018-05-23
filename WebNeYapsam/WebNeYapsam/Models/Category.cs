using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WebNeYapsam.Classes;

namespace WebNeYapsam.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public bool State { get; set; }

        public virtual ICollection<Title> Titles { get; set; }

        [NotMapped]
        public Result Result;

        public Category()
        {
            Result = new Result();
        }


        //PANEL

        public void Add(CategoryViewModel model)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Category category = db.Categories.Where(c => c.Name == model.Name).FirstOrDefault();
                    if (category != null)
                    {
                        Result = new Result(ResultType.AlreadyTaken, model.Name + " isminde kategori bulunmaktadır.");
                        return;
                    }
                    db.Categories.Add(new Category { Name = model.Name, State = model.State });
                    db.SaveChanges();
                    Result = new Result(ResultType.Success, model.Name + " kategorisi eklenmiştir.");
                }
            }
            catch (Exception)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
                return;
            }
        }

        public void Edit(EditCategoryViewModel model)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Category category = db.Categories.Where(c => c.Name == model.Name && c.Id != model.Id).FirstOrDefault();
                    if (category != null)
                    {
                        Result = new Result(ResultType.AlreadyTaken, model.Name + " isminde kategori bulunmaktadır.");
                        return;
                    }
                    category = db.Categories.Find(model.Id);
                    if (category == null)
                    {
                        Result = new Result(ResultType.NotFound, "Kategori bulunamadı.");
                        return;
                    }
                    category.Name = model.Name;
                    category.State = model.State;
                    db.SaveChanges();
                    Result = new Result(ResultType.Success, model.Name + " guncellenmiştir.");
                }
            }
            catch (Exception)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
                return;
            }

        }

        public List<ListCategoryViewModelShared> List()
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    return db.Categories.Select(c => new ListCategoryViewModelShared { Id = c.Id, Name = c.Name, TitleCount = c.Titles.Count }).ToList();
                }
            }
            catch (Exception)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
                return null;
            }
        }

        public EditCategoryViewModel Get(string nameCategory)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Category category = db.Categories.Where(c => c.Name == nameCategory).FirstOrDefault();
                    if (category == null)
                    {
                        Result = new Result(ResultType.NotFound, "Kategori bulunamadı.");
                        return null;
                    }
                    return new EditCategoryViewModel { Id = category.Id, Name = category.Name, State = category.State };
                }
            }
            catch (Exception)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
                return null;
            }
        }

        public void Delete(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    Result.Type = ResultType.NotFound;
                }
                else
                {
                    int categoryTitleCount = category.Titles.Count;
                    if (categoryTitleCount > 0)
                    {
                        Result.Type = ResultType.UnSuccessful;
                    }
                    else
                    {
                        db.Categories.Remove(category);
                        db.SaveChanges();
                        Result.Type = ResultType.Success;
                    }
                }
            }
        }




        //WEB

        public List<ListCategoryViewModelShared> ListTop7()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return db.Categories.Select(c => new ListCategoryViewModelShared { Id = c.Id, Name = c.Name, TitleCount = c.Titles.Count }).OrderByDescending(x=>x.TitleCount).Take(Math.Min(db.Categories.Count(), 7)).ToList();
            }
        }

    }




    public class CategoryViewModel
    {
        [Required(ErrorMessage = "Gerekli Alan"), StringLength(50, ErrorMessage = "{0} - {2} karakter arası girilmelidir.", MinimumLength = 3), Display(Name = "Kategori Ad")]
        public string Name { get; set; }
        [Required, Display(Name = "Durum")]
        public bool State { get; set; }
    }

    //Shared olarak değiştirilmeli 
    //Shared olarak değiştirildi
    public class ListCategoryViewModelShared
    {
        public int Id { get; set; }
        [Display(Name = "Kategori Ad")]
        public string Name { get; set; }
        public int TitleCount { get; set; }
    }

    public class EditCategoryViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Gerekli Alan"), StringLength(50, ErrorMessage = "{0} - {2} karakter arası girilmelidir.", MinimumLength = 3), Display(Name = "Kategori Ad")]
        public string Name { get; set; }
        [Required, Display(Name = "Durum")]
        public bool State { get; set; }
    }

}