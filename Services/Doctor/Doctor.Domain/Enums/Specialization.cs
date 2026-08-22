using System;
using System.Collections.Generic;
using System.Text;

namespace Doctor.Domain.Enums
{
    public enum Specialization
    {
        Cardiology = 1,
        Dermatology = 2,
        Endocrinology = 3,
        Gastroenterology = 4,
        Hematology = 5,
        InfectiousDisease = 6,
        InternalMedicine = 7,
        Nephrology = 8,
        Neurology = 9,
        ObstetricsGynecology = 10,
        Oncology = 11,
        Ophthalmology = 12,
        Orthopedics = 13,
        Otolaryngology = 14,
        Pediatrics = 15,
        Pulmonology = 16,
        Psychiatry = 17,
        Radiology = 18,
        Rheumatology = 19,
        Surgery = 20,
        Urology = 21
    }

    public static class SpecializationExtensions
    {
        public static string GetDisplayName(this Specialization specialization)
        {
            return specialization switch
            {
                Specialization.Cardiology => "Cardiology",
                Specialization.Dermatology => "Dermatology",
                Specialization.Endocrinology => "Endocrinology",
                Specialization.Gastroenterology => "Gastroenterology",
                Specialization.Hematology => "Hematology",
                Specialization.InfectiousDisease => "Infectious Disease",
                Specialization.InternalMedicine => "Internal Medicine",
                Specialization.Nephrology => "Nephrology",
                Specialization.Neurology => "Neurology",
                Specialization.ObstetricsGynecology => "Obstetrics & Gynecology",
                Specialization.Oncology => "Oncology",
                Specialization.Ophthalmology => "Ophthalmology",
                Specialization.Orthopedics => "Orthopedics",
                Specialization.Otolaryngology => "Otolaryngology",
                Specialization.Pediatrics => "Pediatrics",
                Specialization.Pulmonology => "Pulmonology",
                Specialization.Psychiatry => "Psychiatry",
                Specialization.Radiology => "Radiology",
                Specialization.Rheumatology => "Rheumatology",
                Specialization.Surgery => "Surgery",
                Specialization.Urology => "Urology",
                _ => specialization.ToString()
            };
        }
    }
}
