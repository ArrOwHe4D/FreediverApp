using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FreediverApp
{
    public class Measurepoint
    {
        public string accelerator_x; // string
        public string accelerator_y; // string
        public string accelerator_z; // string
        public string depth; // float
        public string duration; // float
        public string gyroscope_x; // string
        public string gyroscope_y; // string
        public string gyroscope_z; // string
        public string heart_freq; // int
        public string heart_var; // int
        public string luminance; // int
        public string oxygen_saturation; // int
        public string ref_dive; // string (id)
        public string water_temp; // float

        public Measurepoint(string _refDive) 
        {
            ref_dive = _refDive;
        }

        public Measurepoint()
        {

        }

        public Measurepoint(string _accelerator_x, string _accelerator_y, string _accelerator_z, string _depth,
        string _duration, string _gyroscope_x, string _gyroscope_y, string _gyroscope_z, string _heart_freq,
        string _heart_var, string _luminance, string _oxygen_saturation, string _ref_dive, string _water_temp)
        {
            accelerator_x = _accelerator_x;
            accelerator_y = _accelerator_y; 
            accelerator_z = _accelerator_z; 
            depth = _depth; 
            duration = _duration; 
            gyroscope_x = _gyroscope_x; 
            gyroscope_y = _gyroscope_y; 
            gyroscope_z = _gyroscope_z; 
            heart_freq = _heart_freq;
            heart_var = _heart_var;
            luminance = _luminance;
            oxygen_saturation = _oxygen_saturation;
            ref_dive = _ref_dive;
            water_temp = _water_temp;
        }

        public static Measurepoint fromJson(string jsonString)
        {            
            try
            {
                var temp = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
                
                Measurepoint measurepoint = new Measurepoint(temp["1"].ToString(), temp["2"].ToString(), temp["3"].ToString(),
                    temp["4"].ToString(), temp["5"].ToString(), temp["6"].ToString(), temp["7"].ToString(), temp["8"].ToString(),
                    temp["9"].ToString(), temp["10"].ToString(), temp["11"].ToString(), temp["12"].ToString(), temp["13"].ToString(), temp["14"].ToString());
                return measurepoint;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }     
        }
    }
}