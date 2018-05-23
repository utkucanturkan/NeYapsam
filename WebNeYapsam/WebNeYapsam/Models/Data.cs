using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using WebNeYapsam.Classes;

namespace WebNeYapsam.Models
{
    public abstract class Data
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Gerekli Alan")]
        public string Descripton { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Point { get; set; }
        public bool State { get; set; }
        public string ApplicationUserId { get; set; }
        [NotMapped]
        public Result Result { get; set; }

        public Data()
        {
            Result = new Result();
        }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<Evaluation> Evaluations { get; set; }

        public void ChangeState(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Data data = db.Datas.Find(id);
                if (data == null)
                {
                    Result.Type = ResultType.NotFound;
                    return;
                }
                bool currentState = data.State;
                if (currentState)
                {
                    currentState = false;
                }
                else
                {
                    currentState = true;
                }
                data.State = currentState;
                db.SaveChanges();
                Result.Type = ResultType.Success;
            }
        }

        public void Delete(int id, string userId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Data data = db.Datas.Where(i => i.Id == id && i.ApplicationUserId == userId).FirstOrDefault();
                if (data == null)
                {
                    Result = new Result(ResultType.NotFound, "Başlık bulunamadı.");
                    return;
                }
                db.Datas.Remove(data);
                db.SaveChanges();
                Result = new Result(ResultType.Success, "Başlık silindi.");
            }
        }

        public void SetEvaluation(int id, string userName, string currentuserId, string dataType, EvaluationType evaluationType)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Evaluation evaluation = db.Evaluations.Where(e => e.EvaluationType != EvaluationType.Complaint && e.DataId == id && e.ApplicationUserId == currentuserId).FirstOrDefault();
                    Data data = db.Datas.Where(d => d.Id == id).FirstOrDefault();
                    ApplicationUser user = db.Users.Where(u => u.UserName == userName).FirstOrDefault();


                    if (evaluation == null)
                    {
                        if (data == null || user == null)
                        {
                            Result = new Result(ResultType.UnSuccessful, "Beklenmedik hata oluştu.");
                            return;
                        }
                        data.Point += 5;
                        if (dataType == "advise" && evaluationType == EvaluationType.Dislike)
                        {

                        }
                        else
                        {
                            user.Point += 10;
                            db.Entry(user).State = EntityState.Modified;
                            if (user.Point > user.Rank.ParentRankPoint)
                            {
                                user.Rank = user.Rank.ParentRank;
                            }
                        }
                        db.Evaluations.Add(new Evaluation { EvaluationType = evaluationType, DataId = data.Id, ApplicationUserId = currentuserId });
                    }
                    else
                    {
                        if (evaluation.EvaluationType == evaluationType)
                        {
                            db.Evaluations.Remove(evaluation);
                            data.Point -= 5;
                            if (dataType == "advise" && evaluationType == EvaluationType.Dislike)
                            {

                            }
                            else
                            {
                                user.Point -= 10;
                                db.Entry(user).State = EntityState.Modified;
                                Rank newrank = db.Ranks.Where(r => r.ParentRankId == user.Rank.Id).FirstOrDefault();
                                if (newrank != null)
                                {
                                    if (user.Point < newrank.ParentRankPoint)
                                    {
                                        user.Rank = newrank;
                                    }
                                }
                            }
                        }
                        else
                        {
                            evaluation.EvaluationType = evaluationType;
                        }

                    }
                    db.SaveChanges();
                    Result = new Result(ResultType.Success, "İşlem başarılı.");
                }
            }
            catch (Exception ex)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
                return;
            }
        }

        public void SetComplaint(SetComplaintViewModel model)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Data data = db.Datas.Find(model.DataId);
                    ApplicationUser user = db.Users.Find(model.ApplicationUserId);
                    if (data != null && user != null)
                    {
                        Evaluation _evaluation = db.Evaluations.Where(e => e.DataId == model.DataId && e.ApplicationUserId == model.ApplicationUserId && e.EvaluationType == EvaluationType.Complaint).FirstOrDefault();
                        if (_evaluation != null)
                        {
                            Result = new Result(ResultType.AlreadyTaken, "Şikayetiniz zaten bulunmaktadır.");
                            return;
                        }
                        Evaluation evaluation = new Evaluation { EvaluationType = model.EvaluationType, DataId = model.DataId, ApplicationUserId = model.ApplicationUserId };
                        db.Evaluations.Add(evaluation);
                        Complaint complaint = new Complaint { ComplaintType = model.ComplaintType, Evaluation = evaluation };
                        db.Complaints.Add(complaint);
                        db.SaveChanges();
                        Result = new Result(ResultType.Success, "Şikayetiniz Tarafımıza iletilmiştir.");
                    }
                    else
                    {
                        Result = new Result(ResultType.NotFound, "Veri bulunamadı.");
                    }
                }
            }
            catch (Exception)
            {
                Result = new Result(ResultType.Error, "Veritabanı ile bağlantı sağlanamadı.");
                return;
            }
        }
    }

    public class SetComplaintViewModel
    {
        public int DataId { get; set; }
        public EvaluationType EvaluationType = EvaluationType.Complaint;
        public string ApplicationUserId { get; set; }
        public ComplaintType ComplaintType { get; set; }
    }
}