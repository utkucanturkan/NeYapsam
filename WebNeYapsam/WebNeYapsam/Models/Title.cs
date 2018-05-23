using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using WebNeYapsam.Classes;
using static WebNeYapsam.Models.Advise;

namespace WebNeYapsam.Models
{
    [Table("Title")]
    public class Title : Data
    {
        [Required, StringLength(100, MinimumLength = 10)]
        public string Name { get; set; }
        public string MediaUrl { get; set; }
        public int ViewCount { get; set; }
        public int CategoryId { get; set; }

        public virtual ICollection<Advise> Advises { get; set; }
        public virtual Category Category { get; set; }

        public Title()
        {
            Result = new Result();
        }

        //PANEL

        public List<ListTitleViewModelPanel> List()
        {
            try
            {
                List<ListTitleViewModelPanel> list = new List<ListTitleViewModelPanel>();
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    List<Data> titles = db.Datas.OrderBy(t => t.CreatedDate).ToList();
                    FillTitleList(titles, list);
                }
                return list;

            }
            catch (Exception)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
                return null;
            }
        }

        public List<ListTitleViewModelPanel> List(string userId)
        {
            try
            {
                List<ListTitleViewModelPanel> list = new List<ListTitleViewModelPanel>();
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    List<Data> titles = db.Datas.Where(d => d.ApplicationUserId == userId).OrderBy(t => t.CreatedDate).ToList();
                    FillTitleList(titles, list);
                }
                return list;
            }
            catch (Exception)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
                return null;
            }
        }

        public void FillTitleList(List<Data> titles, List<ListTitleViewModelPanel> list)
        {
            foreach (Data item in titles)
            {
                if (item is Title)
                {
                    Title t = (Title)item;
                    var counts = t.Evaluations.ToList();
                    int likecount = counts.Where(c => c.EvaluationType == EvaluationType.Like).Count();
                    int dislikecount = counts.Where(c => c.EvaluationType == EvaluationType.Dislike).Count();
                    list.Add(new ListTitleViewModelPanel { Id = t.Id, Name = t.Name, CategoryName = t.Category.Name, LikeCount = likecount, DisLikeCount = dislikecount, Point = t.Point, ViewCount = t.ViewCount, State = t.State });
                }
            }
        }

        public EditTitleViewModelPanel Get(int id)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Data data = db.Datas.Find(id);
                    if (data == null)
                    {
                        Result = new Result(ResultType.NotFound, "Başlık bulunamadı.");
                        return null;
                    }
                    Title title = (Title)data;
                    var evaluations = title.Evaluations.ToList();
                    int dislikeCount = evaluations.Where(e => e.EvaluationType == EvaluationType.Dislike).Count();
                    int likeCount = evaluations.Where(e => e.EvaluationType == EvaluationType.Like).Count();
                    List<ListAdviseViewModelPanel> advices = new Advise().List(title.Id);
                    return new EditTitleViewModelPanel { Id = title.Id, Name = title.Name, Description = title.Descripton, Point = title.Point, DisLikeCount = dislikeCount, LikeCount = likeCount, ViewCount = title.ViewCount, State = title.State, Advices = advices, MediaURL = title.MediaUrl, UserName = title.ApplicationUser.UserName };
                }
            }
            catch (Exception)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
                return null;
            }

        }

        public void Edit(EditTitleViewModelPanel model, string uId)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Data data = db.Datas.Where(d => d.ApplicationUserId == uId && d.Id == model.Id).FirstOrDefault();
                    if (data == null)
                    {
                        Result = new Result(ResultType.NotFound, "Başlık bulunamadı.");
                        return;
                    }
                    Title t = (Title)data;
                    t.State = model.State;
                    t.Descripton = model.Description;
                    t.MediaUrl = model.MediaURL;
                    db.SaveChanges();
                    Result = new Result(ResultType.Success, t.Name + " guncellendi.");
                }
            }
            catch (Exception)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
            }
        }





        //WEB
        public IPagedList<ListTitleViewModelWeb> List(int? page, int? CategoryId, string SearchString)
        {
            List<ListTitleViewModelWeb> list = new List<ListTitleViewModelWeb>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<Data> titles = db.Datas.Where(x => x.State == true).OrderBy(t => t.CreatedDate).ToList();
                FillTitleList(titles, list);
            }

            if (CategoryId != 0 && CategoryId != null)
            {
                list = list.Where(x => x.Category.Id == CategoryId).ToList();
            }

            if (!string.IsNullOrEmpty(SearchString))
            {
                list = list.Where(x => x.Name.ToLower().Contains(SearchString.ToLower())).ToList();
            }

            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return list.ToPagedList(pageNumber, pageSize);
        }

        public void FillTitleList(List<Data> titles, List<ListTitleViewModelWeb> list)
        {
            foreach (Data item in titles)
            {
                if (item is Title)
                {
                    Title t = (Title)item;
                    int advisesCount = t.Advises.Where(i => i.State == true).Count();
                    var image = t.ApplicationUser.Image;
                    list.Add(new ListTitleViewModelWeb { Id = t.Id, Name = t.Name, AdviseCount = advisesCount, CreatedDate = t.CreatedDate, Description = t.Descripton, ViewCount = t.ViewCount, User = t.ApplicationUser, Category = t.Category });
                }
            }
        }

        public GetTitleViewModelWeb Get(int TitleId, string currentUserId)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Title title = db.Titles.Where(t => t.Id == TitleId && t.State == true).FirstOrDefault();
                    if (title == null)
                    {
                        Result = new Result(ResultType.NotFound, "Başlık bulunamadı.");
                        return null;
                    }

                    var evaluations = title.Evaluations.ToList();
                    int likecount = evaluations.Where(c => c.EvaluationType == EvaluationType.Like).Count();
                    int dislikecount = evaluations.Where(c => c.EvaluationType == EvaluationType.Dislike).Count();

                    EvaluationType currentUserEvaluationForTitle = EvaluationType.NoEvaluation;
                    bool isComplainted = false;
                    if (!string.IsNullOrEmpty(currentUserId))
                    {
                        var evaluation = evaluations.Where(e => e.ApplicationUserId == currentUserId && e.EvaluationType != EvaluationType.Complaint).FirstOrDefault();
                        if (evaluation != null)
                        {
                            currentUserEvaluationForTitle = evaluation.EvaluationType;
                        }
                        var complaint = evaluations.Where(e => e.ApplicationUserId == currentUserId && e.EvaluationType == EvaluationType.Complaint).FirstOrDefault();
                        if (complaint != null)
                        {
                            isComplainted = true;
                        }
                    }

                    title.ViewCount++;
                    db.Entry(title).State = EntityState.Modified;
                    db.SaveChanges();

                    return new GetTitleViewModelWeb
                    {
                        Id = title.Id,
                        Name = title.Name,
                        Description = title.Descripton,
                        AdviseCount = title.Advises.Count,
                        Category = title.Category,
                        CreatedDate = title.CreatedDate,
                        DisLikeCount = dislikecount,
                        LikeCount = likecount,
                        User = title.ApplicationUser,
                        ViewCount = title.ViewCount,
                        Image = title.MediaUrl,
                        isEvaluated = currentUserEvaluationForTitle,
                        isComplainted = isComplainted
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı saglanamadı.");
                return null;
            }
        }

        public List<ListTitleViewModelWeb> ListTopOfDay(int? CategoryId)
        {
            List<ListTitleViewModelWeb> list = new List<ListTitleViewModelWeb>();
            List<ListTitleViewModelWeb> currentList = new List<ListTitleViewModelWeb>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<Data> titles = db.Datas.Where(x => x.State == true).OrderBy(t => t.CreatedDate).ToList();
                FillTitleList(titles, list);
                foreach (var item in list)
                {
                    if ((DateTime.Now - item.CreatedDate).TotalDays < 1)
                    {
                        currentList.Add(item);
                    }
                }
            }
            if (CategoryId != 0 && CategoryId != null)
            {
                return currentList.Where(x => x.Category.Id == CategoryId).Take(Math.Min(currentList.Count, 7)).ToList();
            }

            return currentList.Take(Math.Min(currentList.Count, 7)).ToList();
        }

        public List<ListTitleViewModelWeb> ListTopOfWeek(int? CategoryId)
        {
            List<ListTitleViewModelWeb> list = new List<ListTitleViewModelWeb>();
            List<ListTitleViewModelWeb> currentList = new List<ListTitleViewModelWeb>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<Data> titles = db.Datas.Where(x => x.State == true).OrderBy(t => t.CreatedDate).ToList();
                FillTitleList(titles, list);
                foreach (var item in list)
                {
                    if ((DateTime.Now - item.CreatedDate).TotalDays < 8)
                    {
                        currentList.Add(item);
                    }
                }
            }
            if (CategoryId != 0 && CategoryId != null)
            {
                return currentList.Where(x => x.Category.Id == CategoryId).Take(Math.Min(currentList.Count, 7)).ToList();
            }

            return currentList.Take(Math.Min(currentList.Count, 7)).ToList();
        }

        public Title Add(AddTitleViewModel model)
        {
            Title title = new Title();
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    title = db.Titles.Where(t => (t.Name == model.Name) && (t.CategoryId == model.CategoryId)).FirstOrDefault();
                    if (title != null)
                    {
                        title.Result = new Result(ResultType.AlreadyTaken, "Kategoride" + model.Name + " isminde başlık bulunmaktadır.");
                        return title;
                    }
                    title = new Title();

                    db.Datas.Add(new Title { Name = model.Name, CategoryId = model.CategoryId, ApplicationUserId = model.UserId, CreatedDate = DateTime.Now, Descripton = model.Description, MediaUrl = model.Media, State = true, Point = 0, ViewCount = 0 });

                    //  user puan arttırımı
                    ApplicationUser user = db.Users.Find(model.UserId);
                    user.Point += 10;
                    db.Entry(user).State = EntityState.Modified;

                    //  user rank kontrolü
                    if (user.Point > user.Rank.ParentRankPoint)
                    {
                        user.Rank = user.Rank.ParentRank;
                    }

                    // db ye kayıt işlemi
                    db.SaveChanges();
                    title.Result = new Result(ResultType.Success, model.Name + " başlık eklenmiştir.");
                    return db.Titles.Where(t => t.Name == model.Name).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                title.Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
                return title;
            }
        }
    }

    // Web
    public class ListTitleViewModelWeb
    {
        public int Id { get; set; }
        [Display(Name = "Başlık Ad")]
        public string Name { get; set; }
        [Display(Name = "Açıklama")]
        public string Description { get; set; }
        [Display(Name = "Görüntülenme Sayısı")]
        public int ViewCount { get; set; }
        [Display(Name = "Tarih")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Advise Sayısı")]
        public int AdviseCount { get; set; }
        [Display(Name = "User")]
        public ApplicationUser User { get; set; }
        [Display(Name = "Kategori")]
        public Category Category { get; set; }
    }

    public class GetTitleViewModelWeb
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ViewCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public int AdviseCount { get; set; }
        public ApplicationUser User { get; set; }
        public Category Category { get; set; }
        public int LikeCount { get; set; }
        public int DisLikeCount { get; set; }
        public string Image { get; set; }
        public EvaluationType isEvaluated { get; set; }
        public bool isComplainted { get; set; }
    }

    public class AddTitleViewModel
    {
        [Required, StringLength(maximumLength: 200, MinimumLength = 10)]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public string Media { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public string UserId { get; set; }
    }

    // SHARED OLMALI
    public class ListTitleViewModelPanel
    {
        public int Id { get; set; }
        [Display(Name = "Başlık Ad")]
        public string Name { get; set; }
        [Display(Name = "Kategori Ad")]
        public string CategoryName { get; set; }
        [Display(Name = "Puan")]
        public int Point { get; set; }
        [Display(Name = "Like")]
        public int LikeCount { get; set; }
        [Display(Name = "Dislike")]
        public int DisLikeCount { get; set; }
        [Display(Name = "Görüntülenme")]
        public int ViewCount { get; set; }
        public bool State { get; set; }

    }

    public class EditTitleViewModelPanel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MediaURL { get; set; }
        public int Point { get; set; }
        public int LikeCount { get; set; }
        public int DisLikeCount { get; set; }
        public int ViewCount { get; set; }
        [DataType(DataType.MultilineText), Required(ErrorMessage = "Gerekli Alan")]
        public string Description { get; set; }
        public bool State { get; set; }
        public string UserName { get; set; }
        public List<ListAdviseViewModelPanel> Advices { get; set; }

    }

}