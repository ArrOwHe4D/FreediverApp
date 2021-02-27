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
    }
}