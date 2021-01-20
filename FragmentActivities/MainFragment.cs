using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Microcharts;
using Microcharts.Droid;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace FreediverApp.FragmentActivities
{
    [Obsolete]
    public class MainFragment : Fragment
    {
        private ChartView chartView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.content_main, container, false);

            chartView = view.FindViewById<ChartView>(Resource.Id.chartView);
            generateChart();

            Toast.MakeText(this.Context, "Login successful!", ToastLength.Long).Show();

            return view;
        }

        private void generateChart()
        {
            List<ChartEntry> dataList = new List<ChartEntry>();

            dataList.Add(new ChartEntry(0)
            {
                Label = "0:05",
                ValueLabel = "0",
                Color = SKColor.Parse("#5cf739")
            });

            dataList.Add(new ChartEntry(5)
            {
                Label = "0:10",
                ValueLabel = "5",
                Color = SKColor.Parse("#f7c139")
            });

            dataList.Add(new ChartEntry(8)
            {
                Label = "0:15",
                ValueLabel = "8",
                Color = SKColor.Parse("#f75939")
            });

            dataList.Add(new ChartEntry(8)
            {
                Label = "0:20",
                ValueLabel = "8",
                Color = SKColor.Parse("#f75939")
            });

            dataList.Add(new ChartEntry(9)
            {
                Label = "0:25",
                ValueLabel = "9",
                Color = SKColor.Parse("#f75939")
            });

            dataList.Add(new ChartEntry(8)
            {
                Label = "0:30",
                ValueLabel = "8",
                Color = SKColor.Parse("#f75939")
            });

            dataList.Add(new ChartEntry(4)
            {
                Label = "0:35",
                ValueLabel = "4",
                Color = SKColor.Parse("f7c139")
            });

            dataList.Add(new ChartEntry(1)
            {
                Label = "0:40",
                ValueLabel = "4",
                Color = SKColor.Parse("#5cf739")
            });

            var chart = new LineChart { Entries = dataList, LabelTextSize = 30f };
            chartView.Chart = chart;
        }
    }
}