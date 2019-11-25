namespace Origin.Mission
{
    public class HeatMaterial
    {
        public float TotalFuel { get; set; }
        public float MaximumIntensity { get; set; }
        public float FuelBurningRate { get; set; }
        public float FireStartAt { get; set; }
        public float DissipationRate { get; }

        public HeatMaterial(float totalFuel, float maximumIntensity, float fuelBurningRate, float fireStartAt, float dissipationRate)
        {
            TotalFuel = totalFuel;
            MaximumIntensity = maximumIntensity;
            FuelBurningRate = fuelBurningRate;
            FireStartAt = fireStartAt;
            DissipationRate = dissipationRate;
        }
    }
}