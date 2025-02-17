using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Api
{
    public string? deviceId { get; set; }
    public string? slug { get; set; }
    public string stationType { get; set;}
    public string? type {get; set;}
    public DateTime? lastReadingAt { get; set; }
    public DateTime? createdAt { get; set; }
    public DateTime? installedDate { get; set; }
    public string? deviceStatus { get; set; }
    public float? waterLevel {get; set;}
    public string? warningStatus { get; set; }

// Nested objects for different station readings
    public ArrLastReading? arrLastReading { get; set; }    // For "arr" station type
    public AwlrLastReading? awlrLastReading { get; set; }  // For "awlr" station type
    public AwsLastReading? awsLastReading { get; set; }    // For "aws" station type
    public AwlrArrLastReading? awlrArrLastReading { get; set; }  // For "awlrarr" station type

    public string? unitDisplay {get; set;}
    public string? unitSensor {get; set;}
    public string? intensityLastHour {get; set;}
    public string? subDomainOld {get; set;}
    public float? rainfallLastHour {get; set;}
    public float? rainfall {get; set;}

    [Required]
    public string balaiName { get; set; }
    public string? organizationCode { get; set; }
    public string subDomain { get; set;}
    public string name { get; set;}

    // Adjust the properties below based on the sorting logic used
    public int nomor { get; set; }
    public int jumlahPos { get; set; }
    public int jumlahPosOnline { get; set; }
    public int jumlahPosOffline { get; set; }
}

public class DataTableResult<T>
{
    public int draw { get; set; }
    public int recordsTotal { get; set; }
    public int recordsFiltered { get; set; }
    public List<T> data { get; set; }
}

public class ArrLastReading
{
    public string deviceId { get; set; }
    public string deviceStatus { get; set; }
    public DateTime? readingAt { get; set; }
}
// Model for AwlrLastReading (similar to ArrLastReading)
public class AwlrLastReading
{
    public string deviceId { get; set; }
    public string deviceStatus { get; set; }
    public DateTime? readingAt { get; set; }
}

// Model for AwsLastReading (similar to ArrLastReading)
public class AwsLastReading
{
    public string deviceId { get; set; }
    public string deviceStatus { get; set; }
    public DateTime? readingAt { get; set; }
}

// Model for AwlrArrLastReading (combination of Awlr and Arr)
public class AwlrArrLastReading
{
    public string deviceId { get; set; }
    public string deviceStatus { get; set; }
    public DateTime? readingAt { get; set; }
}

    public class Pic
    {
       
        public Guid id { get; set; } // Pastikan tipe datanya Guid jika menggunakan UUID
        public string orgCode { get; set; }
        public string no_pic { get; set; }
        public int pic { get; set; }
        
    }