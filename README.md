# photoDateModifier 
![](https://img.shields.io/badge/VisualStudio-5C2D91?style=flat-square&logo=VisualStudio&logoColor=white)
![](https://img.shields.io/badge/C%23-A8B9CC?style=flat-square&logo=C&logoColor=white)

Extract DateTime information from image files through fileinfo, exifData, xmpData. Then, the earliest date is determined by combining the dateTimes inferred from the folder name and file name of the file.
A .NET-based program that matches all time values with the earliest date computed in this way.

![image](https://user-images.githubusercontent.com/125341305/220281298-7b154585-6c9f-44f6-81e7-9de6cf01f3f8.png)

## Getting Started
These instructions will get you a copy of the program up and running on your local machine for development and testing purposes.

## Prerequisites
The following software is required to run the program:

- .NET Framework 4.8
- Visual Studio 2019 or later

## NuGet packages (recommended)
- MetadataExtractor
- Newtonsoft.Json
- ObjectListView.Official
- System.ComponentModel.Annotations
- System.Drawing.Common
- System.Threading.Tasks.Parallel
- WindowsAPICodePack-Core
- WindowsAPICodePack-Shell
- XmpCore

## How to use photoDateModifier

1. Clone or download the photoDateModifier repository from GitHub.
2. Open the photoDateModifier solution in Visual Studio or any other code editor.
3. Build and run the solution.
4. Follow the on-screen instructions to select the photos you want to modify and the new date you want to set.
5. photoDateModifier will modify the date of your photos and save the changes.

## Usage
The program can be run in two ways:

From the command line, by navigating to the output directory (e.g. bin/Debug) and running photoDateModifier.exe.
From Visual Studio, by setting the program as the startup project and running the debugger.
The program will prompt the user for the folder that contains the images to be modified. Once the folder is selected, the program will read the filenames of all images in the folder, extract the date from the filename using a regular expression, and use that date to update the creation and modification dates of the images.

## Technical Details
The program is written in C# and uses the .NET Framework. It makes use of the following libraries:

System.IO
System.Text.RegularExpressions

## Contributing
If you would like to contribute to the development of the program, please open a pull request on the [GitHub repository](httPs://https://github.com/mastergbc/photoDateModifier).

## License
The program is licensed under the MIT license. See the [LICENSE](https://github.com/mastergbc/photoDateModifier/blob/master/LICENCE) file for more details.

## Acknowledgments
Base design used exifEditor: https://1drv.ms/u/s!AmsqTf3EgmJAoZdvU1tuW0xXWl-LTA by https://happybono.wordpress.com/
The code of ExifPhotoReader was modified at my own discretion.
  - v1.0.4 clone by 2023.02.14: https://github.com/andersonpereiradossantos/dotnet-exif-photo-reader
