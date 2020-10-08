using System;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using Dicom;
using Dicom.Imaging;
using Dicom.Log;
using Dicom.Network;
using Dicom.Media;

using TMPro;

public class dicomInfoTools
{

    private string dateFormat = "N/A";
    private string timeFormat = "N/A";

    private int patientId = 0;
    private int patientAge = 0;
    private DateTime patientBd = DateTime.MinValue;
    private string patientSex = "N/A";
    private string patientName = "N/A";

    private int studyId = 0;
    private DateTime studyTime = DateTime.MinValue;
    private DateTime studyDate = DateTime.MinValue;
    private string studyDescription = "N/A";
    private string studySeriesDescription = "N/A";
    private string studyProtocolName = "N/A";
    private float  studySliceThickness = 0f;
    private string studyDoctorName = "N/A";

    private string modality = "N/A";
    private string modalityManufacturer = "N/A";

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

    public dicomInfoString Strings;

    public void setDicomInfo(DicomFile file)
    {
        dateFormat = "dd.MM.yyyy";
        timeFormat = "HH:mm:ss";

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

        Strings = getDicomInfoString();
    }

    private dicomInfoString getDicomInfoString()
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
                            $"Series: {studySeriesDescription}\nThickness: {studySliceThicknessString} mm\n" +
                            $"Date: {studyDateString}\nTime: {studyTimeString}\n" +
                            $"Referring Physician: {studyDoctorName}\n";

        string mdInfo =     "<b>Modality Info:</b>\n\n" +
                            $"Modality: {modality}\nManufacturer: {modalityManufacturer}\n";

        dicomInfoString info = new dicomInfoString(patInfo, styInfo, mdInfo);

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

        return info;
    }
}
