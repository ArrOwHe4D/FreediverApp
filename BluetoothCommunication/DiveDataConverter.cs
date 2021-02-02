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

        public object toJsonObject() 
        {
            return JsonConvert.DeserializeObject(receivedData);
        }

        public string toJsonString() 
        {
            return JsonConvert.SerializeObject(jsonObject);
        }
    }
}