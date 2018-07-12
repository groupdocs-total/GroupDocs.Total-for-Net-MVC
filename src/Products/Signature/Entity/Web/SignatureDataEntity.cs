﻿
namespace GroupDocs.Total.MVC.Products.Signature.Entity.Web
{
    /// <summary>
    /// SignatureDataEntity
    /// </summary>
    public class SignatureDataEntity
    {
        public string Reason{ get; set; }
        public string Contact{ get; set; }
        public string Address{ get; set; }
        public string Date{ get; set; }
        public string SignaturePassword{ get; set; }
        public string SignatureComment{ get; set; }
        public string DocumentType{ get; set; }
        public string SignatureGuid{ get; set; }
        public string SignatureType{ get; set; }
        public int PageNumber{ get; set; }
        public int Left{ get; set; }
        public int Top{ get; set; }
        public int ImageWidth{ get; set; }
        public int ImageHeight{ get; set; }
        public int Angle{ get; set; }
        public bool isDeleted{ get; set; }
    }
}