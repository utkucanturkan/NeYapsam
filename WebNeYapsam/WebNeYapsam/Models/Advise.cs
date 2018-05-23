using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using WebNeYapsam.Classes;

namespace WebNeYapsam.Models
{
    [Table("Advise")]
    public class Advise : Data
    {
        public int TitleId { get; set; }
        public virtual Title Title { get; set; }

        //PANEL
        public List<ListAdviseViewModelPanel> List(string userId)
        {
            List<ListAdviseViewModelPanel> list = new List<ListAdviseViewModelPanel>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<Data> advises = db.Datas.Where(d => d.ApplicationUserId == userId).ToList();
                foreach (Data data in advises)
                {
                    if (data is Advise)
                    {
                        Advise a = (Advise)data;
                        FillAdviseList(a, list);
                    }
                }
            }
            return list;
        }

        public void FillAdviseList(Advise advices, List<ListAdviseViewModelPanel> list)
        {
            var counts = advices.Evaluations.ToList();
            int likecount = counts.Where(i => i.EvaluationType == EvaluationType.Like).Count();
            int dislikecount = counts.Where(i => i.EvaluationType == EvaluationType.Dislike).Count();
            list.Add(new ListAdviseViewModelPanel { Id = advices.Id, Description = advices.Descripton, Point = advices.Point, LikeCount = likecount, DisLikeCount = dislikecount, State = advices.State, TitleId = advices.TitleId, CreatedDate = advices.CreatedDate });
        }

        public List<ListAdviseViewModelPanel> List(int titleId)
        {
            List<ListAdviseViewModelPanel> list = new List<ListAdviseViewModelPanel>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Data data = db.Datas.Where(d => d.Id == titleId).FirstOrDefault();
                if (data != null)
                {
                    Title title = (Title)data;
                    foreach (Advise a in title.Advises)
                    {
                        FillAdviseList(a, list);
                    }
                }
            }
            return list;
        }



        //WEB
        public IPagedList<ListAdviseViewModelWeb> List(int? page, int titleId, string currentUserId)
        {
            List<ListAdviseViewModelWeb> list = new List<ListAdviseViewModelWeb>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<Data> advises = db.Datas.Where(i => i.State == true).OrderBy(a => a.CreatedDate).ToList();
                foreach (Data data in advises)
                {
                    if (data is Advise && (data as Advise).TitleId == titleId)
                    {
                        Advise advices = (Advise)data;
                        var evaluations = advices.Evaluations.ToList();
                        int likecount = evaluations.Where(i => i.EvaluationType == EvaluationType.Like).Count();
                        int dislikecount = evaluations.Where(i => i.EvaluationType == EvaluationType.Dislike).Count();

                        EvaluationType currentUserEvaluationForAdvise = EvaluationType.NoEvaluation;
                        bool isComplainted = false;
                        if (!string.IsNullOrEmpty(currentUserId))
                        {
                            var evaluation = evaluations.Where(e => e.ApplicationUserId == currentUserId && e.EvaluationType != EvaluationType.Complaint).FirstOrDefault();
                            if (evaluation != null)
                            {
                                currentUserEvaluationForAdvise = evaluation.EvaluationType;
                            }
                            var complaint = evaluations.Where(e => e.ApplicationUserId == currentUserId && e.EvaluationType == EvaluationType.Complaint).FirstOrDefault();
                            if (complaint != null)
                            {
                                isComplainted = true;
                            }
                        }

                        list.Add(new ListAdviseViewModelWeb { Id = advices.Id, Description = advices.Descripton, CreatedDate = advices.CreatedDate, LikeCount = likecount, DisLikeCount = dislikecount, UserName = advices.ApplicationUser.UserName, UserImage = advices.ApplicationUser.Image, UserId = advices.ApplicationUserId, isEvaluated = currentUserEvaluationForAdvise, isComplainted = isComplainted });
                    }
                }
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return list.ToPagedList(pageNumber, pageSize);
        }

        public void Add(AddAdviseViewModelWeb model)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    //  advise exist kontrolü
                    //List<Advise> advises = new List<Advise>();
                    //foreach (var item in db.Datas)
                    //{
                    //    if (item is Advise)
                    //    {
                    //        advises.Add(item as Advise);
                    //    }
                    //}

                    //var advise = advises.Where(x => x.Descripton == model.Description).FirstOrDefault();
                    var title = db.Datas.Find(model.TitleId) as Title;
                    var user = db.Users.Find(model.UserId);
                    if (user == null || title == null)
                    {
                        Result = new Result(ResultType.Error, "Beklenmedik hata oluştu.");
                        return;
                    }
                    //if (advise != null)
                    //{
                    //    model.Result = new Result(ResultType.AlreadyTaken, " tavsiyesi zaten bulunmaktadır.");

                    //}

                    //  title ekleme

                    db.Datas.Add(new Advise { ApplicationUser = user, CreatedDate = DateTime.Now, Descripton = model.Description, Evaluations = null, Point = 0, State = true, Title = title });

                    //  user puan arttırımı

                    user.Point += 10;
                    db.Entry(user).State = EntityState.Modified;

                    //  user rank kontrolü

                    if (user.Point > user.Rank.ParentRankPoint)
                    {
                        user.Rank = user.Rank.ParentRank;
                    }

                    // tavsiye yapılan başlığın puan arttırımı

                    title.Point += 10;
                    db.Entry(title).State = EntityState.Modified;

                    // db ye kayıt işlemi

                    db.SaveChanges();
                    Result = new Result(ResultType.Success, "Tavsiye Eklendi.");
                }
            }
            catch (Exception)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
                return;
            }
        }

    }
    //WEB
    public class AddAdviseViewModelWeb
    {
        public string Description { get; set; }
        public string UserId { get; set; }
        public int TitleId { get; set; }
    }

    public class ListAdviseViewModelWeb
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int LikeCount { get; set; }
        public int DisLikeCount { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public string UserId { get; set; }
        public EvaluationType isEvaluated { get; set; }
        public bool isComplainted { get; set; }

    }

    //PANEL
    public class ListAdviseViewModelPanel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Point { get; set; }
        public int LikeCount { get; set; }
        public int DisLikeCount { get; set; }
        public bool State { get; set; }
        public int TitleId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
