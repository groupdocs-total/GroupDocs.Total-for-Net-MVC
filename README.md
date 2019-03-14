# GroupDocs.Total for .NET MVC Example
###### version 0.4.20

[![Build status](https://ci.appveyor.com/api/projects/status/kvkv1oswg5xfd6uq/branch/master?svg=true)](https://ci.appveyor.com/project/egorovpavel/groupdocs-total-for-net-mvc/branch/master)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/b5cf669acf9043888c8fc4a8d46abdc3)](https://www.codacy.com/app/GroupDocs/GroupDocs.Total-for-NET-MVC?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=groupdocs-total/GroupDocs.Total-for-NET-MVC&amp;utm_campaign=Badge_Grade)

## System Requirements
- .NET Framework 4.5
- Visual Studio 2015


## Description
GroupDocs.Total UI suite is a native, simple, fully configurable and optimized application which allows you to manipulate documents within your desktop solutions and web apps without requiring any other commercial application through GroupDocs APIs.

**Note** Without a license application will run in trial mode, purchase [GroupDocs.Total for .NET license](https://purchase.groupdocs.com/order-online-step-1-of-8.aspx) or request [GroupDocs.Total for .NET temporary license](https://purchase.groupdocs.com/temporary-license).


## Demo Video
Coming soon


## Features
#### GroupDocs.Viewer
- Clean, modern and intuitive design
- Easily switchable colour theme (create your own colour theme in 5 minutes)
- Responsive design
- Mobile support (open application on any mobile device)
- Support over 50 documents and image formats
- HTML and image modes
- Fully customizable navigation panel
- Open password protected documents
- Text searching & highlighting
- Download documents
- Upload documents
- Print document
- Rotate pages
- Zoom in/out documents without quality loss in HTML mode
- Thumbnails
- Smooth page navigation
- Smooth document scrolling
- Preload pages for faster document rendering
- Multi-language support for displaying errors
- Display two or more pages side by side (when zooming out)
- Cross-browser support (Safari, Chrome, Opera, Firefox)
- Cross-platform support (Windows, Linux, MacOS)
#### GroupDocs.Signature
- Clean, modern and intuitive design
- Easily switchable colour theme (create your own colour theme in 5 minutes)
- Responsive design
- Mobile support (open application on any mobile device)
- Support over 50 documents and image formats
- Image mode
- Fully customizable navigation panel
- Sign password protected documents
- Download original documents
- Download signed documents
- Upload documents
- Upload signatures
- Sign document with such signature types: digital certificate, image, stamp, qrCode, barCode.
- Draw signature image
- Draw stamp signature
- Generate bar code signature
- Generate qr code signature
- Print document
- Smooth page navigation
- Smooth document scrolling
- Preload pages for faster document rendering
- Multi-language support for displaying errors
- Cross-browser support (Safari, Chrome, Opera, Firefox)
- Cross-platform support (Windows, Linux, MacOS)
#### GroupDocs.Annotation
- Clean, modern and intuitive design
- Easily switchable colour theme (create your own colour theme in 5 minutes)
- Responsive design
- Mobile support (open application on any mobile device)
- Support over 50 documents and image formats
- Image mode
- Fully customizable navigation panel
- Annotate password protected documents
- Download original documents
- Download annotated documents
- Upload documents
- Annotate document with such annotation types: 
   * Text
   * Area
   * Point
   * TextStrikeout
   * Polyline
   * TextField
   * Watermark
   * TextReplacement
   * Arrow
   * TextRedaction
   * ResourcesRedaction
   * TextUnderline
   * Distance
- Draw annotation over the document page
- Add comment or reply
- Print document
- Smooth page navigation
- Smooth document scrolling
- Preload pages for faster document rendering
- Multi-language support for displaying errors
- Cross-browser support (Safari, Chrome, Opera, Firefox)
- Cross-platform support (Windows, Linux, MacOS)
#### GroupDocs.Comparison
- Clean, modern and intuitive design
- Easily switchable colour theme (create your own colour theme in 5 minutes)
- Responsive design
- Mobile support (open application on any mobile device)
- HTML and image modes
- Fully customizable navigation panel
- Compare documents
- Multi-compare several documents
- Compare password protected documents
- Upload documents
- Display clearly visible differences
- Download comparison results
- Print comparison results
- Smooth document scrolling
- Preload pages for faster document rendering
- Multi-language support for displaying errors
- Cross-browser support (Safari, Chrome, Opera, Firefox)
- Cross-platform support (Windows, Linux, MacOS)

## How to run

You can run this sample by one of following methods

#### Build from source

Download [source code](https://github.com/groupdocs-total/GroupDocs.Total-for-NET-MVC/archive/master.zip) from github or clone this repository.

```bash
git clone https://github.com/groupdocs-total/GroupDocs.Total-for-NET-MVC
```

Open solution in the VisualStudio.
Update common parameters in `web.config` and example related properties in the `configuration.yml` to meet your requirements.

Open http://localhost:8080/ in your favorite browser

#### Docker image
Use [docker](https://www.docker.com/) image.

```bash
mkdir DocumentSamples
mkdir Licenses
docker run -p 8080:8080 --env application.hostAddress=localhost -v `pwd`/DocumentSamples:/home/groupdocs/app/DocumentSamples -v `pwd`/Licenses:/home/groupdocs/app/Licenses groupdocs/total
## Open http://localhost:8080/ in your favorite browser.
```



## Resources
- **Website:** [www.groupdocs.com](http://www.groupdocs.com)
- **Product Home:** [GroupDocs.Total for .NET](https://products.groupdocs.com/total/net)
- **Product API References:** [GroupDocs.Total for .NET API](https://apireference.groupdocs.com)
- **Download:** [Download GroupDocs.Total for .NET](https://downloads.groupdocs.com/total/net)
- **Documentation:** [GroupDocs.Total for .NET Documentation](https://docs.groupdocs.com/dashboard.action)
- **Free Support Forum:** [GroupDocs.Total for .NET Free Support Forum](https://forum.groupdocs.com/c/total)
- **Paid Support Helpdesk:** [GroupDocs.Total for .NET Paid Support Helpdesk](https://helpdesk.groupdocs.com)
- **Blog:** [GroupDocs.Total for .NET Blog](https://blog.groupdocs.com/category/groupdocs-total-product-family)
