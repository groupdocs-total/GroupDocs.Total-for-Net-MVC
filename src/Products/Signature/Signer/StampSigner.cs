﻿using GroupDocs.Signature.Domain;
using GroupDocs.Signature.Options;
using GroupDocs.Total.MVC.Products.Signature.Entity.Web;
using GroupDocs.Total.MVC.Products.Signature.Entity.Xml;

namespace GroupDocs.Total.MVC.Products.Signature.Signer
{
    /// <summary>
    /// StampSigner
    /// </summary>
    public class StampSigner : BaseSigner
    {
        private StampXmlEntity[] stampData;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stampData">StampXmlEntity[]</param>
        /// <param name="signatureData">SignatureDataEntity</param>
        public StampSigner(StampXmlEntity[] stampData, SignatureDataEntity signatureData)
            : base(signatureData)
        {
            this.stampData = stampData;
        }

        /// <summary>
        /// Add pdf signature data
        /// </summary>
        /// <returns>SignOptions</returns>
        public override SignOptions SignPdf()
        {
            // setup options
            PdfStampSignOptions pdfSignOptions = new PdfStampSignOptions();
            pdfSignOptions.Height = signatureData.ImageHeight - 20;
            pdfSignOptions.Width = signatureData.ImageWidth - 20;
            pdfSignOptions.Top = signatureData.Top;
            pdfSignOptions.Left = signatureData.Left;
            pdfSignOptions.DocumentPageNumber = signatureData.PageNumber;
            pdfSignOptions.RotationAngle = signatureData.Angle;
            pdfSignOptions.BackgroundColor = getColor(stampData[stampData.Length - 1].backgroundColor);
            pdfSignOptions.BackgroundColorCropType = StampBackgroundCropType.OuterArea;
            // draw stamp lines
            for (int n = 0; n < stampData.Length; n++)
            {
                string text = "";
                // prepare line text
                for (int m = 0; m < stampData[n].textRepeat; m++)
                {
                    text = text + stampData[n].text;
                }
                // set reduction size - required to recalculate each stamp line height and font size after stamp resizing in the UI
                int reductionSize = 0;
                // check if reduction size is between 1 and 2. for example: 1.25
                if ((double)stampData[n].height / signatureData.ImageHeight > 1 && (double)stampData[n].height / signatureData.ImageHeight < 2)
                {
                    reductionSize = 2;
                }
                else if (stampData[n].height / signatureData.ImageHeight == 0)
                {
                    reductionSize = 1;
                }
                else
                {
                    reductionSize = stampData[n].height / signatureData.ImageHeight;
                }
                // draw most inner line - horizontal text
                if ((n + 1) == stampData.Length)
                {
                    StampLine squareLine = new StampLine();
                    squareLine.Text = text;
                    squareLine.Font.FontSize = stampData[n].fontSize / reductionSize;
                    squareLine.Font.Bold = stampData[n].bold;
                    squareLine.Font.Italic = stampData[n].italic;
                    squareLine.Font.Underline = stampData[n].underline;
                    squareLine.TextColor = getColor(stampData[n].textColor);
                    pdfSignOptions.InnerLines.Add(squareLine);
                    // check if stamp contains from only one line
                    if (stampData.Length == 1)
                    {
                        // if stamp contains only one line draw it as outer and inner line
                        StampLine line = new StampLine();
                        line.BackgroundColor = getColor(stampData[n].backgroundColor);
                        line.OuterBorder.Color = getColor(stampData[n].strokeColor);
                        line.OuterBorder.Weight = stampData[n].strokeWidth;
                        line.InnerBorder.Color = getColor(stampData[n].backgroundColor);
                        line.InnerBorder.Weight = 0.5;
                        line.Height = 1;
                        pdfSignOptions.OuterLines.Add(line);
                    }
                }
                else
                {
                    // draw outer stamp lines - rounded
                    int height = (stampData[n].radius - stampData[n + 1].radius) / reductionSize;
                    StampLine line = new StampLine();
                    line.BackgroundColor = getColor(stampData[n].backgroundColor);
                    line.OuterBorder.Color = getColor(stampData[n].strokeColor);
                    line.OuterBorder.Weight = stampData[n].strokeWidth;
                    line.InnerBorder.Color = getColor(stampData[n + 1].strokeColor);
                    line.InnerBorder.Weight = 0.5;
                    line.Text = text;
                    line.Height = height;
                    line.Font.FontSize = stampData[n].fontSize / reductionSize;
                    line.Font.Bold = stampData[n].bold;
                    line.Font.Italic = stampData[n].italic;
                    line.Font.Underline = stampData[n].underline;
                    line.TextColor = getColor(stampData[n].textColor);
                    line.TextBottomIntent = (height / 2) - (stampData[n].fontSize / 2);
                    line.TextRepeatType = StampTextRepeatType.RepeatWithTruncation;                    
                    pdfSignOptions.OuterLines.Add(line);
                }
            }
            return pdfSignOptions;
        }

        /// <summary>
        /// Add image signature data
        /// </summary>
        /// <returns>SignOptions</returns>
        public override SignOptions SignImage()
        {
            // setup options
            ImagesStampSignOptions imageSignOptions = new ImagesStampSignOptions();
            imageSignOptions.Height = signatureData.ImageHeight - 20;
            imageSignOptions.Width = signatureData.ImageWidth - 20;
            imageSignOptions.Top = signatureData.Top;
            imageSignOptions.Left = signatureData.Left;
            imageSignOptions.DocumentPageNumber = signatureData.PageNumber;
            imageSignOptions.RotationAngle = signatureData.Angle;
            imageSignOptions.BackgroundColor = getColor(stampData[stampData.Length - 1].backgroundColor);
            imageSignOptions.BackgroundColorCropType = StampBackgroundCropType.OuterArea;
            // draw stamp lines
            for (int n = 0; n < stampData.Length; n++)
            {
                string text = "";
                // prepare line text
                for (int m = 0; m < stampData[n].textRepeat; m++)
                {
                    text = text + stampData[n].text;
                }
                // set reduction size - required to recalculate each stamp line height and font size after stamp resizing in the UI
                int reductionSize = 0;
                // check if reduction size is between 1 and 2. for example: 1.25
                if ((double)stampData[n].height / signatureData.ImageHeight > 1 && (double)stampData[n].height / signatureData.ImageHeight < 2)
                {
                    reductionSize = 2;
                }
                else if (stampData[n].height / signatureData.ImageHeight == 0)
                {
                    reductionSize = 1;
                }
                else
                {
                    reductionSize = stampData[n].height / signatureData.ImageHeight;
                }
                // draw most inner line - horizontal text
                if ((n + 1) == stampData.Length)
                {
                    StampLine squareLine = new StampLine();
                    squareLine.Text = text;
                    squareLine.Font.FontSize = stampData[n].fontSize / reductionSize;
                    squareLine.Font.Bold = stampData[n].bold;
                    squareLine.Font.Italic = stampData[n].italic;
                    squareLine.Font.Underline = stampData[n].underline;
                    squareLine.TextColor = getColor(stampData[n].textColor);
                    imageSignOptions.InnerLines.Add(squareLine);
                    // check if stamp contains from only one line
                    if (stampData.Length == 1)
                    {
                        // if stamp contains only one line draw it as outer and inner line
                        StampLine line = new StampLine();
                        line.BackgroundColor = getColor(stampData[n].backgroundColor);
                        line.OuterBorder.Color = getColor(stampData[n].strokeColor);
                        line.OuterBorder.Weight = stampData[n].strokeWidth;
                        line.InnerBorder.Color = getColor(stampData[n].backgroundColor);
                        line.InnerBorder.Weight = 0.5;
                        line.Height = 1;
                        imageSignOptions.OuterLines.Add(line);
                    }
                }
                else
                {
                    // draw outer stamp lines - rounded
                    int height = (stampData[n].radius - stampData[n + 1].radius) / reductionSize;
                    StampLine line = new StampLine();
                    line.BackgroundColor = getColor(stampData[n].backgroundColor);
                    line.OuterBorder.Color = getColor(stampData[n].strokeColor);
                    line.OuterBorder.Weight = stampData[n].strokeWidth;
                    line.InnerBorder.Color = getColor(stampData[n + 1].strokeColor);
                    line.InnerBorder.Weight = 0.5;
                    line.Text = text;
                    line.Font.Bold = stampData[n].bold;
                    line.Font.Italic = stampData[n].italic;
                    line.Font.Underline = stampData[n].underline;
                    line.Height = height;
                    line.Font.FontSize = stampData[n].fontSize / reductionSize;
                    line.TextColor = getColor(stampData[n].textColor);
                    line.TextBottomIntent = (height / 2) - (stampData[n].fontSize / 2);
                    line.TextRepeatType = StampTextRepeatType.RepeatWithTruncation;
                    imageSignOptions.OuterLines.Add(line);
                }
            }
            return imageSignOptions;
        }

        /// <summary>
        /// Add word signature data
        /// </summary>
        /// <returns>SignOptions</returns>
        public override SignOptions SignWord()
        {
            // setup options
            WordsStampSignOptions wordsSignOptions = new WordsStampSignOptions();
            wordsSignOptions.Height = signatureData.ImageHeight - 20;
            wordsSignOptions.Width = signatureData.ImageWidth - 20;
            wordsSignOptions.Top = signatureData.Top;
            wordsSignOptions.Left = signatureData.Left;
            wordsSignOptions.DocumentPageNumber = signatureData.PageNumber;
            wordsSignOptions.RotationAngle = signatureData.Angle;
            wordsSignOptions.BackgroundColor = getColor(stampData[stampData.Length - 1].backgroundColor);
            wordsSignOptions.BackgroundColorCropType = StampBackgroundCropType.OuterArea;
            // draw stamp lines
            for (int n = 0; n < stampData.Length; n++)
            {
                string text = "";
                // prepare line text
                for (int m = 0; m < stampData[n].textRepeat; m++)
                {
                    text = text + stampData[n].text;
                }
                // set reduction size - required to recalculate each stamp line height and font size after stamp resizing in the UI
                int reductionSize = 0;
                // check if reduction size is between 1 and 2. for example: 1.25
                if ((double)stampData[n].height / signatureData.ImageHeight > 1 && (double)stampData[n].height / signatureData.ImageHeight < 2)
                {
                    reductionSize = 2;
                }
                else if (stampData[n].height / signatureData.ImageHeight == 0)
                {
                    reductionSize = 1;
                }
                else
                {
                    reductionSize = stampData[n].height / signatureData.ImageHeight;
                }
                // draw most inner line - horizontal text
                if ((n + 1) == stampData.Length)
                {
                    StampLine squareLine = new StampLine();
                    squareLine.Text = text;
                    squareLine.Font.FontSize = stampData[n].fontSize / reductionSize;
                    squareLine.TextColor = getColor(stampData[n].textColor);
                    squareLine.Font.Bold = stampData[n].bold;
                    squareLine.Font.Italic = stampData[n].italic;
                    squareLine.Font.Underline = stampData[n].underline;
                    wordsSignOptions.InnerLines.Add(squareLine);
                    // check if stamp contains from only one line
                    if (stampData.Length == 1)
                    {
                        // if stamp contains only one line draw it as outer and inner line
                        StampLine line = new StampLine();
                        line.BackgroundColor = getColor(stampData[n].backgroundColor);
                        line.OuterBorder.Color = getColor(stampData[n].strokeColor);
                        line.OuterBorder.Weight = stampData[n].strokeWidth;
                        line.InnerBorder.Color = getColor(stampData[n].backgroundColor);
                        line.InnerBorder.Weight = 0.5;
                        line.Height = 1;
                        wordsSignOptions.OuterLines.Add(line);
                    }
                }
                else
                {
                    // draw outer stamp lines - rounded
                    int height = (stampData[n].radius - stampData[n + 1].radius) / reductionSize;
                    StampLine line = new StampLine();
                    line.BackgroundColor = getColor(stampData[n].backgroundColor);
                    line.OuterBorder.Color = getColor(stampData[n].strokeColor);
                    line.OuterBorder.Weight = stampData[n].strokeWidth;
                    line.InnerBorder.Color = getColor(stampData[n + 1].strokeColor);
                    line.InnerBorder.Weight = 0.5;
                    line.Font.Bold = stampData[n].bold;
                    line.Font.Italic = stampData[n].italic;
                    line.Font.Underline = stampData[n].underline;
                    line.Text = text;
                    line.Height = height;
                    line.Font.FontSize = stampData[n].fontSize / reductionSize;
                    line.TextColor = getColor(stampData[n].textColor);
                    line.TextBottomIntent = (height / 2) - (stampData[n].fontSize / 2);
                    line.TextRepeatType = StampTextRepeatType.RepeatWithTruncation;
                    wordsSignOptions.OuterLines.Add(line);
                }
            }
            return wordsSignOptions;
        }

        /// <summary>
        /// Add cells signature data
        /// </summary>
        /// <returns>SignOptions</returns>
        public override SignOptions SignCells()
        {
            // setup options
            CellsStampSignOptions cellsSignOptions = new CellsStampSignOptions();
            cellsSignOptions.Height = signatureData.ImageHeight - 20;
            cellsSignOptions.Width = signatureData.ImageWidth - 20;
            cellsSignOptions.Top = signatureData.Top;
            cellsSignOptions.Left = signatureData.Left;
            cellsSignOptions.DocumentPageNumber = signatureData.PageNumber;
            cellsSignOptions.RotationAngle = signatureData.Angle;
            cellsSignOptions.BackgroundColor = getColor(stampData[stampData.Length - 1].backgroundColor);
            cellsSignOptions.BackgroundColorCropType = StampBackgroundCropType.OuterArea;
            // draw stamp lines
            for (int n = 0; n < stampData.Length; n++)
            {
                string text = "";
                // prepare line text
                for (int m = 0; m < stampData[n].textRepeat; m++)
                {
                    text = text + stampData[n].text;
                }
                // set reduction size - required to recalculate each stamp line height and font size after stamp resizing in the UI
                int reductionSize = 0;
                // check if reduction size is between 1 and 2. for example: 1.25
                if ((double)stampData[n].height / signatureData.ImageHeight > 1 && (double)stampData[n].height / signatureData.ImageHeight < 2)
                {
                    reductionSize = 2;
                }
                else if (stampData[n].height / signatureData.ImageHeight == 0)
                {
                    reductionSize = 1;
                }
                else
                {
                    reductionSize = stampData[n].height / signatureData.ImageHeight;
                }
                // draw most inner line - horizontal text
                if ((n + 1) == stampData.Length)
                {
                    StampLine squareLine = new StampLine();
                    squareLine.Text = text;
                    squareLine.Font.FontSize = stampData[n].fontSize / reductionSize;
                    squareLine.TextColor = getColor(stampData[n].textColor);
                    squareLine.Font.Bold = stampData[n].bold;
                    squareLine.Font.Italic = stampData[n].italic;
                    squareLine.Font.Underline = stampData[n].underline;
                    cellsSignOptions.InnerLines.Add(squareLine);
                    // check if stamp contains from only one line
                    if (stampData.Length == 1)
                    {
                        // if stamp contains only one line draw it as outer and inner line
                        StampLine line = new StampLine();
                        line.BackgroundColor = getColor(stampData[n].backgroundColor);
                        line.OuterBorder.Color = getColor(stampData[n].strokeColor);
                        line.OuterBorder.Weight = stampData[n].strokeWidth;
                        line.InnerBorder.Color = getColor(stampData[n].backgroundColor);
                        line.InnerBorder.Weight = 0.5;
                        line.Height = 1;
                        cellsSignOptions.OuterLines.Add(line);
                    }
                }
                else
                {
                    // draw outer stamp lines - rounded
                    int height = (stampData[n].radius - stampData[n + 1].radius) / reductionSize;
                    StampLine line = new StampLine();
                    line.BackgroundColor = getColor(stampData[n].backgroundColor);
                    line.OuterBorder.Color = getColor(stampData[n].strokeColor);
                    line.OuterBorder.Weight = stampData[n].strokeWidth;
                    line.InnerBorder.Color = getColor(stampData[n + 1].strokeColor);
                    line.InnerBorder.Weight = 0.5;
                    line.Text = text;
                    line.Height = height;
                    line.Font.Bold = stampData[n].bold;
                    line.Font.Italic = stampData[n].italic;
                    line.Font.Underline = stampData[n].underline;
                    line.Font.FontSize = stampData[n].fontSize / reductionSize;
                    line.TextColor = getColor(stampData[n].textColor);
                    line.TextBottomIntent = (height / 2) - (stampData[n].fontSize / 2);
                    line.TextRepeatType = StampTextRepeatType.RepeatWithTruncation;
                    cellsSignOptions.OuterLines.Add(line);
                }
            }
            return cellsSignOptions;
        }

        /// <summary>
        /// Add slides signature data
        /// </summary>
        /// <returns>SignOptions</returns>
        public override SignOptions SignSlides()
        {
            // setup options
            SlidesStampSignOptions slidesSignOptions = new SlidesStampSignOptions();
            slidesSignOptions.Height = signatureData.ImageHeight - 20;
            slidesSignOptions.Width = signatureData.ImageWidth - 20;
            slidesSignOptions.Top = signatureData.Top;
            slidesSignOptions.Left = signatureData.Left;
            slidesSignOptions.DocumentPageNumber = signatureData.PageNumber;
            slidesSignOptions.RotationAngle = signatureData.Angle;
            slidesSignOptions.BackgroundColor = getColor(stampData[stampData.Length - 1].backgroundColor);
            slidesSignOptions.BackgroundColorCropType = StampBackgroundCropType.OuterArea;
            // draw stamp lines
            for (int n = 0; n < stampData.Length; n++)
            {
                string text = "";
                // prepare line text
                for (int m = 0; m < stampData[n].textRepeat; m++)
                {
                    text = text + stampData[n].text;
                }
                // set reduction size - required to recalculate each stamp line height and font size after stamp resizing in the UI
                int reductionSize = 0;
                // check if reduction size is between 1 and 2. for example: 1.25
                if ((double)stampData[n].height / signatureData.ImageHeight > 1 && (double)stampData[n].height / signatureData.ImageHeight < 2)
                {
                    reductionSize = 2;
                }
                else if (stampData[n].height / signatureData.ImageHeight == 0)
                {
                    reductionSize = 1;
                }
                else
                {
                    reductionSize = stampData[n].height / signatureData.ImageHeight;
                }
                // draw most inner line - horizontal text
                if ((n + 1) == stampData.Length)
                {
                    StampLine squareLine = new StampLine();
                    squareLine.Text = text;
                    squareLine.Font.FontSize = stampData[n].fontSize / reductionSize;
                    squareLine.Font.Bold = stampData[n].bold;
                    squareLine.Font.Italic = stampData[n].italic;
                    squareLine.Font.Underline = stampData[n].underline;
                    squareLine.TextColor = getColor(stampData[n].textColor);
                    slidesSignOptions.InnerLines.Add(squareLine);
                    // check if stamp contains from only one line
                    if (stampData.Length == 1)
                    {
                        // if stamp contains only one line draw it as outer and inner line
                        StampLine line = new StampLine();
                        line.BackgroundColor = getColor(stampData[n].backgroundColor);
                        line.OuterBorder.Color = getColor(stampData[n].strokeColor);
                        line.OuterBorder.Weight = stampData[n].strokeWidth;
                        line.InnerBorder.Color = getColor(stampData[n].backgroundColor);
                        line.InnerBorder.Weight = 0.5;
                        line.Height = 1;
                        slidesSignOptions.OuterLines.Add(line);
                    }
                }
                else
                {
                    // draw outer stamp lines - rounded
                    int height = (stampData[n].radius - stampData[n + 1].radius) / reductionSize;
                    StampLine line = new StampLine();
                    line.BackgroundColor = getColor(stampData[n].backgroundColor);
                    line.OuterBorder.Color = getColor(stampData[n].strokeColor);
                    line.OuterBorder.Weight = stampData[n].strokeWidth;
                    line.InnerBorder.Color = getColor(stampData[n + 1].strokeColor);
                    line.InnerBorder.Weight = 0.5;
                    line.Text = text;
                    line.Font.Bold = stampData[n].bold;
                    line.Font.Italic = stampData[n].italic;
                    line.Font.Underline = stampData[n].underline;
                    line.Height = height;
                    line.Font.FontSize = stampData[n].fontSize / reductionSize;
                    line.TextColor = getColor(stampData[n].textColor);
                    line.TextBottomIntent = (height / 2) - (stampData[n].fontSize / 2);
                    line.TextRepeatType = StampTextRepeatType.RepeatWithTruncation;
                    slidesSignOptions.OuterLines.Add(line);
                }
            }
            return slidesSignOptions;
        }
    }
}