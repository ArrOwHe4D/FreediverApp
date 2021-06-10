using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreediverApp.UI.Fragments
{
    public class DatePickerFragment : DialogFragment, DatePickerDialog.IOnDateSetListener
    {
        // TAG can be any string of your choice.
        public static readonly string TAG = "X:" + typeof(DatePickerFragment).Name.ToUpper();

        // Initialize this value to prevent NullReferenceExceptions.
        Action<DateTime> dateSelectedHandler = delegate { };

        public static DatePickerFragment NewInstance(Action<DateTime> onDateSelected)
        {
            DatePickerFragment datePickerFragment = new DatePickerFragment();
            datePickerFragment.dateSelectedHandler = onDateSelected;
            return datePickerFragment;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime currentDateTime = DateTime.Now;
            DatePickerDialog datePickerDialog = new DatePickerDialog(Activity,
                                                           this,
                                                           currentDateTime.Year,
                                                           currentDateTime.Month - 1,
                                                           currentDateTime.Day);
            return datePickerDialog;
        }

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            // Note: monthOfYear is a value between 0 and 11, not 1 and 12!
            DateTime selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
            Log.Debug(TAG, selectedDate.ToLongDateString());
            dateSelectedHandler(selectedDate);
        }
    }
}