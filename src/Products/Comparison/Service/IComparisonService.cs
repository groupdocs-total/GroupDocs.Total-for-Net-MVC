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
        /// <returns>LoadDocumentEntity</returns>
        LoadDocumentEntity LoadDocumentPages(string path);

        /// <summary>
        ///  Load document page as images
        /// </summary>
        /// <param name="postedData">PostedDataEntity</param>
        /// <returns>LoadDocumentEntity</returns>
        LoadDocumentEntity LoadDocumentPage(PostedDataEntity postedData);

        /// <summary>
        ///  Load document info
        /// </summary>
        /// <param name="postedData">PostedDataEntity</param>
        /// <returns>LoadDocumentEntity</returns>
        LoadDocumentEntity LoadDocumentInfo(PostedDataEntity postedData);

        /// <summary>
        /// Check format files for comparing
        /// </summary>
        /// <param name="file">CompareRequest</param>
        /// <returns>bool</returns>
        bool CheckFiles(CompareRequest files);       
    }
}