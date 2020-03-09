namespace lab4
{
    public class HomeAutomationReadings
    {
        public TemperatureReading TemperatureReading { get; set; }
        public HumidityReading HumidityReading { get; set; }
        public GarageDoorReading GarageDoorReading { get; set; }
        public PressureReading PressureReading { get; set; }

        public HomeAutomationReadings()
        {
            TemperatureReading = new TemperatureReading { Quadrant = Quadrant.TopLeft };
            HumidityReading = new HumidityReading { Quadrant = Quadrant.TopRight };
            GarageDoorReading = new GarageDoorReading { Quadrant = Quadrant.BottomRight };
            PressureReading = new PressureReading { Quadrant = Quadrant.BottomLeft };
        }
    }
}
