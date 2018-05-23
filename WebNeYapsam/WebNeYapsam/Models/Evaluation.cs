using System.Collections.Generic;

namespace WebNeYapsam.Models
{
    public class Evaluation
    {
        public int Id { get; set; }
        public virtual EvaluationType EvaluationType { get; set; }
        public int DataId { get; set; }
        public string ApplicationUserId { get; set; }

        public virtual Data Data { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<Complaint> Complaints { get; set; }

    }

    public enum EvaluationType
    {
        NoEvaluation,
        Like,
        Dislike,
        Complaint
    }
}