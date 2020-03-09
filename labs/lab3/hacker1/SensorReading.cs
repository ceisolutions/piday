namespace lab3
{    public class SensorReading<T>
    {
        public string city {get;set;}
        public string room {get;set;}
        public string sensor {get;set;}
        public T value {get;set;}
    }
}