using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using GroupDocs.Total.MVC.Products.Comparison.Model.Request;
using GroupDocs.Total.MVC.Products.Comparison.Model.Response;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace GroupDocs.Total.MVC.Products.Comparison.Service
{
    public interface IComparisonService
    {

        List<FileDescriptionEntity> LoadFiles(PostedDataEntity fileTreeRequest);

        /// <summary>
        /// Compare two documents, save results in files
        /// </summary>
        /// <param name="compareRequest">PostedDataEntity</param>
        /// <returns>CompareResultResponse</returns>
        CompareResultResponse Compare(CompareRequest compareRequest);        

        /// <summary>
        ///  Load document pages as images
        /// </summary>
        /// <param name="loadResultPageRequest">PostedDataEntity</param>
        /// <returns>LoadedPageEntity</returns>
        LoadDocumentEntity LoadDocumentPages(string path);

        /// <summary>
        ///  Produce file names for results
        /// </summary>
        /// <param name="documentGuid">string</param>
        /// <param name="index">int</param>
        /// <param name="ext">string</param>
        /// <returns>string</returns>
       // string CalculateResultFileName(string documentGuid, string index, string ext);



        /// <summary>
        /// Check format files for comparing
        /// </summary>
        /// <param name="file">CompareRequest</param>
        /// <returns>bool</returns>
        bool CheckFiles(CompareRequest files);

        /// <summary>
        /// Compare more than two files at once
        /// </summary>
        /// <param name="files"></param>
        /// <param name="passwords"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        CompareResultResponse MultiCompareFiles(List<Stream> files, List<string> passwords, string ext);
    }
}