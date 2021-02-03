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
        public string heartFreq; // int
        public string heartVar; // int
        public string luminance; // int
        public string oxygenSaturation; // int
        public string refDive; // string (id)
        public string waterTemperature; // float

        public Measurepoint(string _refDive) 
        {
            refDive = _refDive;
        }

        public Measurepoint()
        {

        }

        public Measurepoint(object jsonOject)
        {
            int x = 5;
        }
    }
}