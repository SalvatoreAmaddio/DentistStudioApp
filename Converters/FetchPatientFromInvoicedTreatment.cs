﻿using Backend.ExtensionMethods;
using DentistStudioApp.Model;
using FrontEnd.Converters;
using System.Globalization;

namespace DentistStudioApp.Converters
{
    public class FetchPatientFromInvoicedTreatment : AbstractFetchModel<Invoice, Patient>
    {
        protected override string Sql => new InvoicedTreatment()
        .Select("Patient.*")
        .From()
            .InnerJoin(nameof(Treatment), "TreatmentID")
            .InnerJoin(nameof(Treatment), nameof(Patient),"PatientID")
        .Where().EqualsTo("InvoicedTreatment.InvoiceID", "@id")
        .Limit().Statement();

        public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Record = (Invoice)value;
            para.Add(new("id", Record.InvoiceID));
            return (Patient?)Db?.Retrieve(Sql, para).FirstOrDefault();
        }

    }

    public class FetchPatientFromAppointmentTreatment : AbstractFetchModel<Appointment, Patient>
    {
        protected override string Sql => new Patient()
        .Select("Patient.*")
        .From()
            .InnerJoin(nameof(Treatment), "PatientID")
        .Where().EqualsTo("Treatment.TreatmentID", "@id")
        .Limit().Statement();

        public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Record = (Appointment?)value;
            para.Add(new("id", Record?.Treatment?.TreatmentID));
            return (Patient?)Db?.Retrieve(Sql, para).FirstOrDefault();
        }
    }

    public class FetchPatientFromTreatment : AbstractFetchModel<Treatment, Patient>
    {
        protected override string Sql =>
            new Treatment()
            .Select("Patient.*")
            .From()
            .InnerJoin(new Patient())
            .Where()
            .EqualsTo("Treatment.PatientID", "@id").Limit()
            .Statement();

        public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Record = (Treatment?)value;
            para.Add(new("id", Record?.Patient?.PatientID));
            return (Patient?)Db?.Retrieve(Sql, para).FirstOrDefault();
        }
    }

    public class FetchPaymentMethod : AbstractFetchModel<Invoice, PaymentType>
    {
        protected override string Sql => new PaymentType().Select().From().Where().This("id").Statement();

        public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Record = (Invoice)value;
            para.Add(new("id", Record?.PaymentType?.PaymentTypeID));
            return (PaymentType?)Db?.Retrieve(Sql, para).FirstOrDefault();
        }
    }
}
