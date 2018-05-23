using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using WebNeYapsam.Classes;

namespace WebNeYapsam.Models
{
    public class Complaint
    {
        public int Id { get; set; }
        public int EvaluationId { get; set; }
        public virtual ComplaintType ComplaintType { get; set; }
        public virtual Evaluation Evaluation { get; set; }


        [NotMapped]
        public Result Result { get; set; }

        public Complaint()
        {
            Result = new Result();
        }

        // Panel
        public List<ListComplaintViewModelPanel> List()
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var complaints = from e in db.Evaluations join c in db.Complaints on e.Id equals c.EvaluationId where e.EvaluationType == EvaluationType.Complaint select new ListComplaintViewModelPanel { Id = e.Id, ComplaintType = c.ComplaintType, UserName = e.ApplicationUser.UserName, DataType = (e.Data is Title) ? "Başlık" : "Tavsiye" };
                    return complaints.ToList();
                }
            }
            catch (Exception)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
                return null;
            }
        }

        public GetComplaintViewModelPanel Get(int id)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    GetComplaintViewModelPanel complaint = (from e in db.Evaluations join c in db.Complaints on e.Id equals c.EvaluationId where e.EvaluationType == EvaluationType.Complaint where e.Id == id select new GetComplaintViewModelPanel { Id = e.Id, ComplaintType = c.ComplaintType, UserName = e.ApplicationUser.UserName, Data = e.Data, ComplaintedUserName = e.Data.ApplicationUser.UserName }).FirstOrDefault();
                    if (complaint == null)
                    {
                        Result = new Result(ResultType.NotFound, "Şikayet bulunamadı.");
                        return null;
                    }
                    return complaint;
                }
            }
            catch (Exception)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
                return null;
            }
        }

        public void Confirm(int id)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var complaint = (from e in db.Evaluations join c in db.Complaints on e.Id equals c.EvaluationId where e.EvaluationType == EvaluationType.Complaint where e.Id == id select new { evaluation = c.Evaluation, complaintType = c.ComplaintType, user = e.ApplicationUser, data = e.Data }).FirstOrDefault();
                    if (complaint == null)
                    {
                        Result = new Result(ResultType.NotFound, "Şikayet bulunamadı.");
                    }
                    switch (complaint.complaintType)
                    {
                        case ComplaintType.Hakaret:
                        case ComplaintType.Siyasi:
                        case ComplaintType.Pornografik:
                            complaint.user.Point -= (complaint.user.Point >= 10) ? 10 : complaint.user.Point;
                            break;
                        case ComplaintType.Irkcılık:
                            complaint.user.Point = 0;
                            complaint.user.State = false;
                            break;
                        default:
                            break;
                    }

                    if (complaint.complaintType != ComplaintType.YanlısKategori)
                    {
                        if (complaint.data is Title)
                        {
                            foreach (Data item in db.Advices.Where(a => a.TitleId == complaint.data.Id).ToList())
                            {
                                db.Datas.Remove(item);
                            }
                        }
                        db.Datas.Remove(complaint.data);
                    }

                    Rank newrank = db.Ranks.Where(r => r.ParentRankId == complaint.user.Rank.Id).FirstOrDefault();
                    if (newrank != null)
                    {
                        if (complaint.user.Point < newrank.ParentRankPoint)
                        {
                            complaint.user.Rank = newrank;
                        }
                    }
                    foreach (Evaluation item in complaint.user.Evaluations.Where(e => e.Id == complaint.data.Id && e.EvaluationType != EvaluationType.Complaint).ToList())
                    {
                        db.Evaluations.Remove(item);
                    }
                    db.Evaluations.Remove(complaint.evaluation);
                    db.SaveChanges();
                    Result = new Result(ResultType.Success, "İşlem başarılı.");
                }
            }
            catch (Exception ex)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
            }
        }

        public void Delete(int id)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var evaluation = db.Evaluations.Find(id);
                    if (evaluation == null)
                    {
                        Result = new Result(ResultType.NotFound, "Şikayet bulunamadı.");
                    }
                    db.Evaluations.Remove(evaluation);
                    db.SaveChanges();
                    Result = new Result(ResultType.Success, "İşlem başarılı.");
                }
            }
            catch (Exception)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
            }
        }
    }

    public enum ComplaintType
    {
        Hakaret,
        Siyasi,
        Pornografik,
        Irkcılık,
        YanlısKategori,
        Diger
    }


    public class ListComplaintViewModelPanel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string DataType { get; set; }
        public ComplaintType ComplaintType { get; set; }
    }

    public class GetComplaintViewModelPanel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public ComplaintType ComplaintType { get; set; }
        public Data Data { get; set; }
        public string ComplaintedUserName { get; set; }
    }


}