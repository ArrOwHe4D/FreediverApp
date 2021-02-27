using Newtonsoft.Json;

namespace FreediverApp.BluetoothCommunication
{
    class DiveDataConverter
    {
        private string receivedData { get; set; }
        private object jsonObject { get; set; }

        public DiveDataConverter() 
        {

        }

        public DiveDataConverter(string receivedData) 
        {
            this.receivedData = receivedData;
            this.jsonObject = toJsonObject();
        }

        public bool isJson(string receivedData)
        {
            return receivedData[0] == '{';
        }

        public Measurepoint toJsonObject() 
        {
            return JsonConvert.DeserializeObject<Measurepoint>(receivedData);
        }

        public string toJsonString() 
        {
            return JsonConvert.SerializeObject(jsonObject);
        }
    }
}