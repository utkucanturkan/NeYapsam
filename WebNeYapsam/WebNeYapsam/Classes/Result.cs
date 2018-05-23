using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebNeYapsam.Classes
{
    public class Result
    {
        private ResultType _type;

        public ResultType Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (value == ResultType.Success)
                {
                    IsSuccess = true;
                }
                _type = value;
            }
        }

        public string Message
        {
            get
            {
                switch (Type)
                {
                    case ResultType.Success:
                        return "İşlem Başarılı.";
                    case ResultType.UnSuccessful:
                        return "İşlem Başarısız.";
                    case ResultType.NotFound:
                        return "Veri Bulunamadı!";
                    case ResultType.AlreadyTaken:
                        return "Veri Daha Once Kullanılmış!";
                    default:
                        return "Beklenmedik Hata Oluştu!";
                }
            }
        }

        public string InnerMessage { get; set; }

        //
        //  Get, List ve özellikle Delete işlemlerinde sonuçun boolean şeklinde tutar.
        //  Ajax servislerde kullanılabilir.
        //
        public bool IsSuccess { get; private set; }

        public Result()
        {
            this.IsSuccess = false;
        }

        public Result(ResultType type, string innerMessage)
        {
            Type = type;
            InnerMessage = innerMessage;
        }
    }

    public enum ResultType
    {        
        Success,
        Error,
        UnSuccessful,
        NotFound,
        AlreadyTaken
    }
}