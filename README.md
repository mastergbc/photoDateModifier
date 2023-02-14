# photoDateModifier
edit datetime infomation of exif &amp; fileinfo 
A .NET program that modifies the creation and modification dates of images based on the filename.

## Getting Started
These instructions will get you a copy of the program up and running on your local machine for development and testing purposes.

## Prerequisites
The following software is required to run the program:

- .NET Framework 4.8
- Visual Studio 2019 or later

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
The program is licensed under the MIT license. See the [LICENSE](LICENSE) file for more details.

## Acknowledgments
Base design used exifEditor: https://1drv.ms/u/s!AmsqTf3EgmJAoZdvU1tuW0xXWl-LTA by https://happybono.wordpress.com/
The code of ExifPhotoReader was modified at my own discretion.
  - v1.0.4 clone by 2023.02.14: https://github.com/andersonpereiradossantos/dotnet-exif-photo-reader
