using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FreediverApp
{
    /**
     *  This dataclass represents a whole datarecord including all sensor values from the dive computer.
     *  While a user is diving, the arduino produces 10 Measurepoints per second and stores them into logfiles,
     *  that are transmitted to our app after the user has connected to his dive computer via bluetooth low energy.
     **/
    public class Measurepoint
    {
        public string accelerator_x;        // string (1)
        public string accelerator_y;        // string (2)
        public string accelerator_z;        // string (3)
        public string depth;                // float  (4)
        public string duration;             // float  (5)
        public string gyroscope_x;          // string (6)
        public string gyroscope_y;          // string (7)
        public string gyroscope_z;          // string (8)
        public string heart_freq;           // int    (9)
        public string heart_var;            // int    (10)
        public string luminance;            // int    (11)
        public string oxygen_saturation;    // int    (12)
        public string ref_dive;             // string (13)
        public string water_temp;           // float  (14)

        public Measurepoint(){}

        public Measurepoint(string _refDive) 
        {
            ref_dive = _refDive;
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

        /**
         *  This function creates a measurepoint based on a json string that was received by the dive computer via bluetooth.
         *  We are using the Newtonsoft.Json Library to achieve the conversion and build the measurepoint from the json map.
         *  We decided to enumerate all the parameters that are being sent to minimize the length of the payload of the bluetooth data package.
         *  So a received JSON measurepoint from arduino side can look like this for example: "{ \"1\": 20.0, \"2\": 0.04, \"3\": 34 ...}"
         */
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