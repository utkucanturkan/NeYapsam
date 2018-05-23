using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using WebNeYapsam.Classes;

namespace WebNeYapsam.Models
{
    public class Rank
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int? ParentRankPoint { get; set; }
        public int? ParentRankId { get; set; }

        public virtual Rank ParentRank { get; set; }
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }

        [NotMapped]
        public Result Result;

        public Rank()
        {
            Result = new Result();
        }

        public void Add(RankViewModel model)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Rank rank = db.Ranks.Where(r => r.Name == model.Name).FirstOrDefault();
                    if (rank != null)
                    {
                        Result = new Result(ResultType.AlreadyTaken, model.Name + " isminde rutbe kullanışmıştır.");
                        return;
                    }
                    db.Ranks.Add(new Rank { Name = model.Name, ParentRankId = model.ParentRankId, ParentRankPoint = model.ParentRankPoint });
                    db.SaveChanges();
                    Result = new Result(ResultType.Success, model.Name + " rutbesi eklenmiştir.");
                }
            }
            catch (Exception)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
                return;
            }

        }

        public EditRankViewModel Get(string name)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Rank rank = db.Ranks.Where(r => r.Name == name).FirstOrDefault();
                    if (rank == null)
                    {
                        Result = new Result(ResultType.NotFound, "Rutbe bulunamadı.");
                        return null;
                    }
                    return new EditRankViewModel { Id = rank.Id, Name = rank.Name, ParentRankId = rank.ParentRankId, ParentRankPoint = rank.ParentRankPoint };
                }
            }
            catch (Exception)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
                return null;
            }
        }

        public List<ListRankViewModel> List()
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    return db.Ranks.Select(r => new ListRankViewModel { Id = r.Id, Name = r.Name, UserCount = r.ApplicationUsers.Count }).ToList();
                }

            }
            catch (Exception)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
                return null;
            }
        }

        public void Edit(EditRankViewModel model)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Rank rank = db.Ranks.Where(r => r.Name == model.Name && r.Id != model.Id).FirstOrDefault();
                    if (rank != null)
                    {
                        Result = new Result(ResultType.AlreadyTaken, model.Name + " isminde rutbe kullanışmıştır.");
                        return;
                    }
                    rank = db.Ranks.Find(model.Id);
                    if (rank == null)
                    {
                        Result = new Result(ResultType.NotFound, "Rutbe bulunamadı.");
                        return;
                    }
                    rank.Name = model.Name;
                    rank.ParentRankPoint = model.ParentRankPoint;
                    rank.ParentRankId = model.ParentRankId;
                    db.SaveChanges();
                    Result = new Result(ResultType.Success, model.Name + " rutbesi guncellenmiştir.");
                }
            }
            catch (Exception)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
                return;
            }
        }

        public void Delete(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Rank rank = db.Ranks.Find(id);
                if (rank == null)
                {
                    Result.Type = ResultType.NotFound;
                }
                else
                {
                    int rankUserCount = rank.ApplicationUsers.Count;
                    if (rankUserCount > 0)
                    {
                        Result.Type = ResultType.UnSuccessful;
                    }
                    else
                    {
                        db.Ranks.Remove(rank);
                        db.SaveChanges();
                        Result.Type = ResultType.Success;
                    }
                }
            }
        }
    }

    public class ListRankViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Rütbe Ad")]
        public string Name { get; set; }
        public int UserCount { get; set; }
    }

    public class RankViewModel
    {
        [Required(ErrorMessage = "Gerekli Alan"), Display(Name = "Ad")]
        public string Name { get; set; }
        public int? ParentRankId { get; set; }
        [Display(Name = "Üst Rütbe Puan")]
        public int? ParentRankPoint { get; set; }
    }

    public class EditRankViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Gerekli Alan"), Display(Name = "Ad")]
        public string Name { get; set; }
        public int? ParentRankId { get; set; }
        [Display(Name = "Üst Rütbe Puan")]
        public int? ParentRankPoint { get; set; }
    }
}