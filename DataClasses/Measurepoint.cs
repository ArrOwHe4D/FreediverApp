namespace FreediverApp
{
    class Measurepoint
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

        public Measurepoint(object jsonObject)
        {
            int x = 5;
            //this.accelerator_x = saveData.Put("accelerator_x", obj.accelerator_x);
            //saveData.Put("accelerator_y", obj.accelerator_y);
            //saveData.Put("accelerator_z", obj.accelerator_z);
            //saveData.Put("depth", obj.depth);
            //saveData.Put("duration", obj.duration);
            //saveData.Put("gyroscope_x", obj.gyroscope_x);
            //saveData.Put("gyroscope_y", obj.gyroscope_y);
            //saveData.Put("gyroscope_z", obj.gyroscope_z);
            //saveData.Put("heart_freq", obj.heartFreq);
            //saveData.Put("heart_var", obj.heartVar);
            //saveData.Put("luminance", obj.luminance);
            //saveData.Put("oxygen_saturation", obj.oxygenSaturation);
            //saveData.Put("ref_dive", obj.refDive);
            //saveData.Put("water_temp", obj.waterTemperature);
        }
    }
}