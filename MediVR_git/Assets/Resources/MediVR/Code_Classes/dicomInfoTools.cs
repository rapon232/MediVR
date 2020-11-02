using System;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Codec;
using Dicom.Imaging.Render;
using Dicom.Log;
using Dicom.Network;
using Dicom.Media;

using TMPro;

public class dicomInfoTools : ISerializable
{
    private string dateFormat;
    private string timeFormat;

    private int imageWidth;
    private int imageHeight;
    private string imageTransferSyntax;
    private string defaultDicomTransferSyntax = DicomTransferSyntax.ImplicitVRLittleEndian.UID.ToString();
    private int imageRescaleSlope;
    private int imageRescaleIntercept;
    private int imageWindowWidth;
    private int imageWindowCenter;
    private double[] imageOrientationPatient;
    private string orientationPatient;

    private int patientId;
    private int patientAge;
    private DateTime patientBd;
    private string patientSex;
    private string patientName;

    private int studyId;
    private DateTime studyTime;
    private DateTime studyDate;
    private string studyDescription;
    private string studySeriesDescription;
    private string studyProtocolName;
    private float  studySliceThickness;
    private string studyDoctorName;

    private string modality;
    private string modalityManufacturer;

    private dicomInfoString strings;

    public struct dicomInfoString
    {
        public string patientInfo { get;}
        public string studyInfo { get;}
        public string modalityInfo { get;}

        public dicomInfoString(string patientInfo, string studyInfo, string modalityInfo)
        {
            this.patientInfo = patientInfo;
            this.studyInfo = studyInfo;
            this.modalityInfo = modalityInfo;
        }
    }

    public string DateFormat
    {
        get { return dateFormat; }
        set { dateFormat = value; }
    }
    public string TimeFormat
    {
        get { return timeFormat; }
        set { timeFormat = value; }
    }
    
    public int ImageWidth
    {
        get { return imageWidth; }
        set { imageWidth = value; }
    }
    public int ImageHeight
    {
        get { return imageHeight; }
        set { imageHeight = value; }
    }
    public string ImageTransferSyntax
    {
        get { return imageTransferSyntax; }
        set { imageTransferSyntax = value; }
    }
    public string DefaultDicomTransferSyntax
    {
        get { return defaultDicomTransferSyntax; }
        set { defaultDicomTransferSyntax = value; }
    }
    public int ImageRescaleSlope
    {
        get { return imageRescaleSlope; }
        set { imageRescaleSlope = value; }
    }
    public int ImageRescaleIntercept
    {
        get { return imageRescaleIntercept; }
        set { imageRescaleIntercept = value; }
    }
    public int ImageWindowWidth
    {
        get { return imageWindowWidth; }
        set { imageWindowWidth = value; }
    }
    public int ImageWindowCenter
    {
        get { return imageWindowCenter; }
        set { imageWindowCenter = value; }
    }
    public double[] ImageOrientationPatient
    {
        get { return imageOrientationPatient; }
        set { imageOrientationPatient = value; }
    }
    public string OrientationPatient
    {
        get { return orientationPatient; }
        set { orientationPatient = value; }
    }

    public int PatientId
    {
        get { return patientId; }
        set { patientId = value; }
    }
    public int PatientAge
    {
        get { return patientAge; }
        set { patientAge = value; }
    }
    public DateTime PatientBd
    {
        get { return patientBd; }
        set { patientBd = value; }
    }
    public string PatientSex
    {
        get { return patientSex; }
        set { patientSex = value; }
    }
    public string PatientName
    {
        get { return patientName; }
        set { patientName = value; }
    }

    public int StudyId
    {
        get { return studyId; }
        set { studyId = value; }
    }
    public DateTime StudyTime
    {
        get { return studyTime; }
        set { studyTime = value; }
    }
    public DateTime StudyDate
    {
        get { return studyDate; }
        set { studyDate = value; }
    }
    public string StudyDescription
    {
        get { return studyDescription; }
        set { studyDescription = value; }
    }
    public string StudySeriesDescription
    {
        get { return studySeriesDescription; }
        set { studySeriesDescription = value; }
    }
    public string StudyProtocolName
    {
        get { return studyProtocolName; }
        set { studyProtocolName = value; }
    }
    public float StudySliceThickness
    {
        get { return studySliceThickness; }
        set { studySliceThickness = value; }
    }
    public string StudyDoctorName
    {
        get { return studyDoctorName; }
        set { studyDoctorName = value; }
    }

    public string Modality
    {
        get { return modality; }
        set { modality = value; }
    }
    public string ModalityManufacturer
    {
        get { return modalityManufacturer; }
        set { modalityManufacturer = value; }
    } 

    public dicomInfoString Strings
    {
        get { return strings; }
    }

    

    public dicomInfoTools()
    {
        dateFormat = "N/A";
        timeFormat = "N/A";

        imageWidth = 0;
        imageHeight = 0;
        imageTransferSyntax = null;
        imageRescaleSlope = 0;
        imageRescaleIntercept = 0;
        imageWindowWidth = 0;
        imageWindowCenter = 0;
        imageOrientationPatient = new double[6];
        orientationPatient = null;

        patientId = 0;
        patientAge = 0;
        patientBd = DateTime.MinValue;
        patientSex = "N/A";
        patientName = "N/A";

        studyId = 0;
        studyTime = DateTime.MinValue;
        studyDate = DateTime.MinValue;
        studyDescription = "N/A";
        studySeriesDescription = "N/A";
        studyProtocolName = "N/A";
        studySliceThickness = 0f;
        studyDoctorName = "N/A";

        modality = "N/A";
        modalityManufacturer = "N/A";

        GetDicomInfoString();
    }

    public dicomInfoTools(DicomFile file)
    {
        dateFormat = "dd.MM.yyyy";
        timeFormat = "HH:mm:ss";

        if(file.Dataset.Contains(DicomTag.Columns))
        {
            imageWidth = file.Dataset.Get<int>(DicomTag.Columns);
            //Debug.Log($"Image Width found in dataset: {imageWidth}.");
        }
        else
        {
            imageWidth = 512;
            Debug.Log($"Image Width NOT found in dataset. Defaulting to: {imageWidth}.");
        }

        imageTransferSyntax = file.Dataset.InternalTransferSyntax.UID.ToString();

        if(file.Dataset.Contains(DicomTag.Rows))
        {
            imageHeight = file.Dataset.Get<int>(DicomTag.Rows);
            //Debug.Log($"Image Height found in dataset: {imageHeight}.");
        }
        else
        {
            imageHeight = 512;
            Debug.Log($"Image Height NOT found in dataset. Defaulting to: {imageHeight}.");
        }

        if(file.Dataset.Contains(DicomTag.RescaleSlope))
        {
            imageRescaleSlope = file.Dataset.Get<int>(DicomTag.RescaleSlope);
            //Debug.Log($"Image Rescale Slope found in dataset: {imageRescaleSlope}.");
        }
        else
        {
            imageRescaleSlope = 1;
            Debug.Log($"Image Rescale Slope NOT found in dataset. Defaulting to: {imageRescaleSlope}.");
        }

        if(file.Dataset.Contains(DicomTag.RescaleIntercept))
        {
            imageRescaleIntercept = file.Dataset.Get<int>(DicomTag.RescaleIntercept);
            //Debug.Log($"Image Rescale Intercept found in dataset: {imageRescaleIntercept}.");
        }
        else
        {
            imageRescaleIntercept = -1024;
            Debug.Log($"Image Rescale Intercept NOT found in dataset. Defaulting to: {imageRescaleIntercept}.");
        }

        if(file.Dataset.Contains(DicomTag.WindowWidth))
        {
            imageWindowWidth = file.Dataset.Get<int>(DicomTag.WindowWidth);
            //Debug.Log($"Image Window Width found in dataset: {imageWindowWidth}.");
        }
        else
        {
            imageWindowWidth = 300;
            Debug.Log($"Image Window Width NOT found in dataset. Defaulting to: {imageWindowWidth}.");
        }

        if(file.Dataset.Contains(DicomTag.WindowCenter))
        {
            imageWindowCenter = file.Dataset.Get<int>(DicomTag.WindowCenter);
            //Debug.Log($"Image Window Center found in dataset: {imageWindowCenter}.");
        }
        else
        {
            imageWindowCenter = 100;
            Debug.Log($"Image Window Center NOT found in dataset. Defaulting to: {imageWindowCenter}.");
        }

        if(file.Dataset.Contains(DicomTag.ImageOrientationPatient))
        {
            imageOrientationPatient = file.Dataset.Get<double[]>(DicomTag.ImageOrientationPatient);
            //Debug.Log($"Image Orientation of Patient found in dataset: {imageOrientationPatient}.");
        }
        else
        {
            imageOrientationPatient = null;
            Debug.Log($"Image Orientation of Patient NOT found in dataset. Defaulting to: {imageOrientationPatient}.");
        }


        if(file.Dataset.Contains(DicomTag.PatientID))
        {
            var id = file.Dataset.Get<string>(DicomTag.PatientID, "N/A");
            int num;
            bool isParsable = Int32.TryParse(id, out num);

            if(isParsable)
            {
                patientId = num;
            }
            else
            {
                patientId = 0;
            }
        }
        else
        {
            patientId = 0;
        }

        if(file.Dataset.Contains(DicomTag.PatientAge))
        {
            var age = file.Dataset.Get<string>(DicomTag.PatientAge, "N/A");
            age = age.Substring(0,3);
            int num;
            bool isParsable = Int32.TryParse(age, out num);

            if(isParsable)
            {
                patientAge = num;
            }
            else
            {
                patientAge = 0;
            }
        }
        else
        {
            patientAge = 0;
        }

        if(file.Dataset.Contains(DicomTag.PatientSex))
        {
            patientSex = file.Dataset.Get<string>(DicomTag.PatientSex, "N/A");
        }
        else
        {
            patientSex = "N/A";
        }

        if(file.Dataset.Contains(DicomTag.PatientName))
        {
            patientName = file.Dataset.Get<string>(DicomTag.PatientName, "N/A");
        }
        else
        {
            patientName = "N/A";
        }

        if(file.Dataset.Contains(DicomTag.PatientBirthDate))
        {
            var date = file.Dataset.Get<string>(DicomTag.PatientBirthDate, "N/A");

            string format = "yyyyMMdd";
            bool isFormattable = DateTime.TryParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newDate);

            if(isFormattable)
            {
                patientBd = newDate;
                //Debug.Log($"{date} converts to {patientBd.ToString(dateFormat)}.");
            }
            else
            {
                patientBd = DateTime.MinValue;
                //Debug.Log($"{date} is not in the correct format.");
            }
        }
        else
        {
            patientBd = DateTime.MinValue;
        }


        if(file.Dataset.Contains(DicomTag.StudyID))
        {
            var id = file.Dataset.Get<string>(DicomTag.StudyID, "N/A");
            int num;
            bool isParsable = Int32.TryParse(id, out num);

            if(isParsable)
            {
                studyId = num;
            }
            else
            {
                studyId = 0;
            }
        }
        else
        {
            studyId = 0;
        }

        if(file.Dataset.Contains(DicomTag.StudyDate))
        {
            var date = file.Dataset.Get<string>(DicomTag.StudyDate, "N/A");

            string format = "yyyyMMdd";
            bool isFormattable = DateTime.TryParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newDate);

            if(isFormattable)
            {
                studyDate = newDate;
                //Debug.Log($"{date} converts to {studyDate.ToString(dateFormat)}.");
            }
            else
            {
                studyDate = DateTime.MinValue;
                //Debug.Log($"{date} is not in the correct format.");
            }
        }
        else
        {
            studyDate = DateTime.MinValue;
        }

        if(file.Dataset.Contains(DicomTag.StudyTime))
        {
            var time = file.Dataset.Get<string>(DicomTag.StudyTime, "N/A");
            //time = time.Substring(0, 6);

            string format = "HHmmss.ffffff";
            bool isFormattable = DateTime.TryParseExact(time, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newTime);

            if(isFormattable)
            {
                studyTime = newTime;
                //Debug.Log($"{time} converts to {newTime.ToString(timeFormat)}.");
            }
            else
            {
                studyTime = DateTime.MinValue;
                //Debug.Log($"{time} is not in the correct format.");
            }
        }
        else
        {
            studyTime = DateTime.MinValue;
        }

        if(file.Dataset.Contains(DicomTag.StudyDescription))
        {
            studyDescription = file.Dataset.Get<string>(DicomTag.StudyDescription, "N/A");
            //Debug.Log(studyDescription.Length);
            studyDescription = studyDescription.Remove(studyDescription.Length - 1);
            //Debug.Log(studyDescription.Length);
        }
        else
        {
            studyDescription = "N/A";
        }

        if(file.Dataset.Contains(DicomTag.SeriesDescription))
        {
            studySeriesDescription = file.Dataset.Get<string>(DicomTag.SeriesDescription, "N/A");
        }
        else
        {
            studySeriesDescription = "N/A";
        }

        if(file.Dataset.Contains(DicomTag.ProtocolName))
        {
            studyProtocolName = file.Dataset.Get<string>(DicomTag.ProtocolName, "N/A");
        }
        else
        {
            studyProtocolName = "N/A";
        }

        if(file.Dataset.Contains(DicomTag.SliceThickness))
        {
            var thickness = file.Dataset.Get<string>(DicomTag.SliceThickness, "N/A");
            float num;
            bool isParsable = Single.TryParse(thickness, out num);

            if(isParsable)
            {
                studySliceThickness = num;
            }
            else
            {
                studySliceThickness = 0f;
            }
        }
        else
        {
            studySliceThickness = 0f;
        }

        if(file.Dataset.Contains(DicomTag.ReferringPhysicianName))
        {
            studyDoctorName = file.Dataset.Get<string>(DicomTag.ReferringPhysicianName, "N/A");
        }
        else
        {
            studyDoctorName = "N/A";
        }
        

        if(file.Dataset.Contains(DicomTag.Modality))
        {
            modality = file.Dataset.Get<string>(DicomTag.Modality, "N/A");
        }
        else
        {
            modality = "N/A";
        }

        if(file.Dataset.Contains(DicomTag.Manufacturer))
        {
            modalityManufacturer = file.Dataset.Get<string>(DicomTag.Manufacturer, "N/A"); 
        }
        else
        {
            modalityManufacturer = "N/A";
        }

        orientationPatient = GetPatientOrientationString(imageOrientationPatient);
        //Debug.Log($"Patient orientation: {orientationPatient}");

        GetDicomInfoString();

        //Debug.Log($"Image frame width: {imageWidth} pixels and frame height: {imageHeight} pixels.");
        //Debug.Log($"Image Transfer Syntax: {imageTransferSyntax.ToString()}. Applying Decompression Transfer Syntax: {defaultDicomTransferSyntax.ToString()}");
        //Debug.Log($"Image Rescale Slope: {imageRescaleSlope} and Rescale Intercept: {imageRescaleIntercept}");
    }

    public dicomInfoTools(SerializationInfo info, StreamingContext context)
    {
        dateFormat = (string)info.GetValue("DateFormat", typeof(string));
        timeFormat = (string)info.GetValue("TimeFormat", typeof(string));

        imageWidth = (int)info.GetValue("ImageWidth", typeof(int));
        imageHeight = (int)info.GetValue("ImageHeight", typeof(int));
        imageTransferSyntax = (string)info.GetValue("ImageTransferSyntax", typeof(string));
        defaultDicomTransferSyntax = (string)info.GetValue("DefaultDicomTransferSyntax", typeof(string));
        imageRescaleSlope = (int)info.GetValue("ImageRescaleSlope", typeof(int));
        imageRescaleIntercept = (int)info.GetValue("ImageRescaleIntercept", typeof(int));
        imageWindowWidth = (int)info.GetValue("ImageWindowWidth", typeof(int));
        imageWindowCenter = (int)info.GetValue("ImageWindowCenter", typeof(int));
        imageOrientationPatient = (double[])info.GetValue("ImageOrientationPatient", typeof(double[]));
        orientationPatient = (string)info.GetValue("OrientationPatient", typeof(string));

        patientId = (int)info.GetValue("PatientId", typeof(int));
        patientAge = (int)info.GetValue("PatientAge", typeof(int));
        patientBd = (DateTime)info.GetValue("PatientBd", typeof(DateTime));
        patientSex = (string)info.GetValue("PatientSex", typeof(string));
        patientName = (string)info.GetValue("PatientName", typeof(string));

        studyId = (int)info.GetValue("StudyId", typeof(int));
        studyTime = (DateTime)info.GetValue("StudyTime", typeof(DateTime));
        studyDate = (DateTime)info.GetValue("StudyDate", typeof(DateTime));
        studyDescription = (string)info.GetValue("StudyDescription", typeof(string));
        studySeriesDescription = (string)info.GetValue("StudySeriesDescription", typeof(string));
        studyProtocolName = (string)info.GetValue("StudyProtocolName", typeof(string));
        studySliceThickness = (float)info.GetValue("StudySliceThickness", typeof(float));
        studyDoctorName = (string)info.GetValue("StudyDoctorName", typeof(string));

        modality = (string)info.GetValue("Modality", typeof(string));
        modalityManufacturer = (string)info.GetValue("ModalityManufacturer", typeof(string));

        strings = (dicomInfoString)info.GetValue("Strings", typeof(dicomInfoString));
        GetDicomInfoString();
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        //throw new NotImplementedException();

        info.AddValue("DateFormat", DateFormat);
        info.AddValue("TimeFormat", TimeFormat);

        info.AddValue("ImageWidth", ImageWidth);
        info.AddValue("ImageHeight", ImageHeight);
        info.AddValue("ImageTransferSyntax", ImageTransferSyntax);
        info.AddValue("DefaultDicomTransferSyntax", DefaultDicomTransferSyntax);
        info.AddValue("ImageRescaleSlope", ImageRescaleSlope);
        info.AddValue("ImageRescaleIntercept", ImageRescaleIntercept);
        info.AddValue("ImageWindowWidth", ImageWindowWidth);
        info.AddValue("ImageWindowCenter", ImageWindowCenter);
        info.AddValue("ImageOrientationPatient", ImageOrientationPatient);
        info.AddValue("OrientationPatient", OrientationPatient);

        info.AddValue("PatientId", PatientId);
        info.AddValue("PatientAge", PatientAge);
        info.AddValue("PatientBd", PatientBd);
        info.AddValue("PatientSex", PatientSex);
        info.AddValue("PatientName", PatientName);

        info.AddValue("StudyId", StudyId);
        info.AddValue("StudyTime", StudyTime);
        info.AddValue("StudyDate", StudyDate);
        info.AddValue("StudyDescription", StudyDescription);
        info.AddValue("StudySeriesDescription", StudySeriesDescription);
        info.AddValue("StudyProtocolName", StudyProtocolName);
        info.AddValue("StudySliceThickness", StudySliceThickness);
        info.AddValue("StudyDoctorName", StudyDoctorName);

        info.AddValue("Modality", Modality);
        info.AddValue("ModalityManufacturer", ModalityManufacturer);

        info.AddValue("Strings", Strings);
    }

    /*public void SetDicomInfoFromFile(DicomFile file)
    {
        
    }*/

    public void GetDicomInfoString()
    {
        string patientIdString;
        string patientAgeString;
        string studyIdString;
        string studyDateString;
        string studyTimeString;
        string patientBdString;
        string studySliceThicknessString;

        if(patientId != 0)
        {
            patientIdString = patientId.ToString();
        }
        else
        {
            patientIdString = "N/A";
        }

        if(patientAge != 0)
        {
            patientAgeString = patientAge.ToString();
        }
        else
        {
            patientAgeString = "N/A";
        }

        if(studyId != 0)
        {
            studyIdString = studyId.ToString();
        }
        else
        {
            studyIdString = "N/A";
        }

        if(studyDate != DateTime.MinValue)
        {
            studyDateString = studyDate.ToString(dateFormat);
        }
        else
        {
            studyDateString = "N/A";
        }

        if(studyTime != DateTime.MinValue)
        {
            studyTimeString = studyTime.ToString(timeFormat);
        }
        else
        {
            studyTimeString = "N/A";
        }

        if(patientBd != DateTime.MinValue)
        {
            patientBdString = patientBd.ToString(dateFormat);
        }
        else
        {
            patientBdString = "N/A";
        }

        if(studySliceThickness != 0)
        {
            studySliceThicknessString = studySliceThickness.ToString();
        }
        else
        {
            studySliceThicknessString = "N/A";
        }

        string patInfo =   "<b>Patient Info:\n\n</b>" +
                            $"ID: {patientIdString}\nGender: {patientSex}\nAge: {patientAgeString}\n" + 
                            $"Name: {patientName}\nBirth Date: {patientBdString}\n";

        string styInfo =    "<b>Study Info:</b>\n\n" +
                            $"ID: {studyIdString}\nDescription: {studyDescription}\n" +
                            $"Series: {studySeriesDescription}\nThickness: {studySliceThicknessString} mm\nOrientation: {orientationPatient}\n" +
                            $"Date: {studyDateString}\nTime: {studyTimeString}\n" +
                            $"Referring Physician: {studyDoctorName}\n";

        string mdInfo =     "<b>Modality Info:</b>\n\n" +
                            $"Modality: {modality}\nManufacturer: {modalityManufacturer}\n";

        strings = new dicomInfoString(patInfo, styInfo, mdInfo);

        //Debug.Log(info);

        /*Debug.Log(dumpPatientId);
        Debug.Log(dumpPatientSex);
        Debug.Log(dumpPatientName);
        Debug.Log(dumpPatientBd);
        Debug.Log(dumpStudyId);
        Debug.Log(dumpStudyDate);
        Debug.Log(dumpStudyTime);
        Debug.Log(dumpDoctorName);
        Debug.Log(dumpModality);
        Debug.Log(dumpModalityManufacturer);*/

        return;
    }

    private string GetPatientOrientationString(double[] orientations)
    {
        string orientation = null;

        if(orientations != null)
        {
            //var orientations = file.Dataset.GetValues<double>(DicomTag.ImageOrientationPatient); // this array has length 6
            var rowDirection = new Dicom.Imaging.Mathematics.Vector3D(orientations, 0); // take 3 values starting from index 0
            var colDirection = new Dicom.Imaging.Mathematics.Vector3D(orientations, 3); // take 3 values starting from index 3
            var normalvector = colDirection.CrossProduct(rowDirection);
            var nearestAxis = normalvector.NearestAxis();
            var nearestAxisString = nearestAxis.ToString().Replace(" ", "");

            switch(nearestAxisString)
            {
                case ("(0,1,0)"):
                    orientation = "Coronal AP";
                    break;
                case ("(0,-1,0)"):
                    orientation = "Coronal PA";
                    break;
                case ("(0,0,1)"):
                    orientation = "Axial SI";
                    break;
                case ("(0,0,-1)"):
                    orientation = "Axial IS";
                    break;
                case ("(1,0,0)"):
                    orientation = "Saggital LR";
                    break;
                case ("(-1,0,0)"):
                    orientation = "Saggital RL";
                    break;
                default:
                    orientation = "N/A";
                    break;
            }
        }
        else
        {
            orientation = "N/A";
        }

        return orientation;
    }
}
