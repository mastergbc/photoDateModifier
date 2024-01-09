#pragma once

#include <exiv2/exiv2.hpp>
#include <exiv2/image.hpp>
#include <exiv2/xmp_exiv2.hpp>
#include <exiv2/iptc.hpp>
#include <exiv2/exif.hpp>
#include <msclr/marshal_cppstd.h>
#include <msclr/marshal_atl.h>
#include <msclr/marshal.h>
#include <iostream>

using namespace System;
using namespace System::Runtime::InteropServices;

namespace Exiv2Wrapper
{
    /**
     * Inside the Exiv2Metadata class,
     * add a private member variable of type `Exiv2: :Image`:.
     * @param imagePath: get image full path from C# class
     */
    public ref class Exiv2Metadata
    {
    private:
        Exiv2::Image* image;
        Exiv2::ExifData* exifMetadata;
        Exiv2::IptcData* iptcMetadata;
        Exiv2::XmpData* xmpMetadata;

    private:
        std::string StringToUtf8(System::String^ input)
        {
            array<Byte>^ bytes = System::Text::Encoding::UTF8->GetBytes(input);
            pin_ptr<Byte> pinnedBytes = &bytes[0];
            return std::string(reinterpret_cast<char*>(pinnedBytes), bytes->Length);
        }

        System::String^ Utf8ToString(const std::string& input)
        {
            array<Byte>^ bytes = gcnew array<Byte>(static_cast<int>(input.size()));
            System::Runtime::InteropServices::Marshal::Copy(System::IntPtr((void*)input.data()), bytes, 0, static_cast<int>(input.size()));
            return System::Text::Encoding::UTF8->GetString(bytes);
        }

    public:
        Exiv2Metadata(System::String^ imagePath)
            : image(nullptr), exifMetadata(nullptr), iptcMetadata(nullptr), xmpMetadata(nullptr)
        {
            //IntPtr ip = Marshal::StringToHGlobalAnsi(imagePath);
            //std::string stdImagePath = static_cast<const char*>(ip.ToPointer());
            std::string stdImagePath = StringToUtf8(imagePath);
            try
            {
                image = Exiv2::ImageFactory::open(stdImagePath).release();

                image->readMetadata();
                exifMetadata = new Exiv2::ExifData(image->exifData());
                iptcMetadata = new Exiv2::IptcData(image->iptcData());
                xmpMetadata = new Exiv2::XmpData(image->xmpData());
            }
            catch (Exiv2::AnyError& e)
            {
                std::cout<< "exception: " << e;
            }
            //Marshal::FreeHGlobal(ip);
        }
        ~Exiv2Metadata()
        {
            if (image != nullptr)
            {
                delete image;
                image = nullptr;
            }
        }

    public:
        System::Collections::Generic::Dictionary<System::String^, System::String^>^ GetAllMetadata()
        {
            System::Collections::Generic::Dictionary<System::String^, System::String^>^ metadataDictionary = gcnew System::Collections::Generic::Dictionary<System::String^, System::String^>();

            // Add Exif metadata
            if (exifMetadata)
            {
                for (Exiv2::ExifData::const_iterator it = exifMetadata->begin(); it != exifMetadata->end(); ++it)
                {
                    //System::String^ key = gcnew System::String(it->key().c_str());
                    //System::String^ value = gcnew System::String(it->toString().c_str());
                    System::String^ key = Utf8ToString(it->key().c_str());
                    System::String^ value = Utf8ToString(it->toString().c_str());

                    if (!metadataDictionary->ContainsKey(key))
                    {
                        metadataDictionary->Add(key, value);
                    }
                }
            }

            // Add IPTC metadata
            if (iptcMetadata)
            {
                for (Exiv2::IptcData::const_iterator it = iptcMetadata->begin(); it != iptcMetadata->end(); ++it)
                {
                    //System::String^ key = gcnew System::String(it->key().c_str());
                    //System::String^ value = gcnew System::String(it->toString().c_str());
                    System::String^ key = Utf8ToString(it->key().c_str());
                    System::String^ value = Utf8ToString(it->toString().c_str());

                    if (!metadataDictionary->ContainsKey(key))
                    {
                        metadataDictionary->Add(key, value);
                    }
                }
            }

            // Add XMP metadata
            if (xmpMetadata)
            {
                for (Exiv2::XmpData::const_iterator it = xmpMetadata->begin(); it != xmpMetadata->end(); ++it)
                {
                    //System::String^ key = gcnew System::String(it->key().c_str());
                    //System::String^ value = gcnew System::String(it->toString().c_str());
                    System::String^ key = Utf8ToString(it->key().c_str());
                    System::String^ value = Utf8ToString(it->toString().c_str());

                    if (!metadataDictionary->ContainsKey(key))
                    {
                        metadataDictionary->Add(key, value);
                    }
                }
            }

            return metadataDictionary;
        }

    public:
        void WriteMetadata(System::Collections::Generic::Dictionary<System::String^, System::String^>^ updatedMetadata)
        {
            try
            {
                if (image != nullptr)
                {
                    for each (System::Collections::Generic::KeyValuePair<System::String^, System::String^> metadata in updatedMetadata)
                    {
                        IntPtr ipKey = Marshal::StringToHGlobalAnsi(metadata.Key);
                        std::string key = static_cast<const char*>(ipKey.ToPointer());
                        IntPtr ipValue = Marshal::StringToHGlobalAnsi(metadata.Value);
                        std::string value = static_cast<const char*>(ipValue.ToPointer());

                        if (key.compare(0, 5, "Exif.") == 0) {
                            System::String^ formattedMessage = String::Format("Exif key pare[{0}]: {1}", metadata.Key, metadata.Value);
                            Console::WriteLine(formattedMessage);
                            Exiv2::ExifKey exifKey(key);
                            if (exifMetadata->findKey(exifKey) != exifMetadata->end())
                            {
                                exifMetadata->operator[](key).setValue(StringToUtf8(metadata.Value));
                            }
                        }
                        else if (key.compare(0, 5, "Iptc.") == 0) {
                            System::String^ formattedMessage = String::Format("Iptc key pare[{0}]: {1}", metadata.Key, metadata.Value);
                            Console::WriteLine(formattedMessage);
                            Exiv2::IptcKey iptcKey(key);
                            if (iptcMetadata->findKey(iptcKey) != iptcMetadata->end())
                            {
                                iptcMetadata->operator[](key).setValue(StringToUtf8(metadata.Value));
                            }
                        }
                        else if (key.compare(0, 4, "Xmp.") == 0) {
                            System::String^ formattedMessage = String::Format("Xmp key pare[{0}]: {1}", metadata.Key, metadata.Value);
                            Console::WriteLine(formattedMessage);
                            Exiv2::XmpKey xmpKey(key);
                            if (xmpMetadata->findKey(xmpKey) != xmpMetadata->end())
                            {
                                xmpMetadata->operator[](key).setValue(StringToUtf8(metadata.Value));
                            }
                        }
                        else {
                            System::String^ formattedMessage = String::Format("Invalid KeyValuePair[{0}]: {1}", metadata.Key, metadata.Value);
                            Console::WriteLine(formattedMessage);
                        }
                        Marshal::FreeHGlobal(ipKey);
                        Marshal::FreeHGlobal(ipValue);
                    }
                    image->setExifData(*exifMetadata);
                    image->setIptcData(*iptcMetadata);
                    image->setXmpData(*xmpMetadata);

                    image->writeMetadata();
                }
            }
            catch (Exiv2::AnyError& e)
            {
                System::String^ exceptionMessage = Utf8ToString(e.what());
                System::String^ formattedMessage = String::Format("WriteMetadata exception[{0}]: {1}", e.code(), exceptionMessage);
                Console::WriteLine(formattedMessage);
            }
        }

    public:
        System::String^ ReadExif(System::String^ key)
        {
            IntPtr ipKey = Marshal::StringToHGlobalAnsi(key);
            std::string stdKey = static_cast<const char*>(ipKey.ToPointer());
            Exiv2::ExifData& exifData = image->exifData();
            auto it = exifData.findKey(Exiv2::ExifKey(stdKey));

            if (it != exifData.end())
            {
                System::String^ value = Marshal::PtrToStringAnsi((IntPtr)(void*)it->toString().c_str());
                Marshal::FreeHGlobal(ipKey);
                return value;
            }
            Marshal::FreeHGlobal(ipKey);
            return nullptr;
        }

        void WriteExif(System::String^ key, System::String^ value)
        {
            IntPtr ipKey = Marshal::StringToHGlobalAnsi(key);
            std::string stdKey = static_cast<const char*>(ipKey.ToPointer());
            IntPtr ipValue = Marshal::StringToHGlobalAnsi(value);
            std::string stdValue = static_cast<const char*>(ipValue.ToPointer());
            image->exifData()[stdKey] = stdValue;
            Marshal::FreeHGlobal(ipKey);
            Marshal::FreeHGlobal(ipValue);
        }

        System::String^ ReadIptc(System::String^ key)
        {
            IntPtr ipKey = Marshal::StringToHGlobalAnsi(key);
            std::string stdKey = static_cast<const char*>(ipKey.ToPointer());
            Exiv2::IptcData& iptcData = image->iptcData();
            auto it = iptcData.findKey(Exiv2::IptcKey(stdKey));

            if (it != iptcData.end())
            {
                System::String^ value = Marshal::PtrToStringAnsi((IntPtr)(void*)it->toString().c_str());
                Marshal::FreeHGlobal(ipKey);
                return value;
            }
            Marshal::FreeHGlobal(ipKey);
            return nullptr;
        }

        void WriteIptc(System::String^ key, System::String^ value)
        {
            IntPtr ipKey = Marshal::StringToHGlobalAnsi(key);
            std::string stdKey = static_cast<const char*>(ipKey.ToPointer());
            IntPtr ipValue = Marshal::StringToHGlobalAnsi(value);
            std::string stdValue = static_cast<const char*>(ipValue.ToPointer());
            image->iptcData()[stdKey] = stdValue;
            Marshal::FreeHGlobal(ipKey);
            Marshal::FreeHGlobal(ipValue);
        }

        System::String^ ReadXmp(System::String^ key)
        {
            IntPtr ipKey = Marshal::StringToHGlobalAnsi(key);
            std::string stdKey = static_cast<const char*>(ipKey.ToPointer());
            Exiv2::XmpData& xmpData = image->xmpData();
            auto it = xmpData.findKey(Exiv2::XmpKey(stdKey));

            if (it != xmpData.end())
            {
                System::String^ value = Marshal::PtrToStringAnsi((IntPtr)(void*)it->toString().c_str());
                Marshal::FreeHGlobal(ipKey);
                return value;
            }
            Marshal::FreeHGlobal(ipKey);
            return nullptr;
        }

        void WriteXmp(System::String^ key, System::String^ value)
        {
            IntPtr ipKey = Marshal::StringToHGlobalAnsi(key);
            std::string stdKey = static_cast<const char*>(ipKey.ToPointer());
            IntPtr ipValue = Marshal::StringToHGlobalAnsi(value);
            std::string stdValue = static_cast<const char*>(ipValue.ToPointer());
            image->xmpData()[stdKey] = stdValue;
            Marshal::FreeHGlobal(ipKey);
            Marshal::FreeHGlobal(ipValue);
        }

    public:
        void SaveChanges()
        {
            image->writeMetadata();
        }
    };
}
