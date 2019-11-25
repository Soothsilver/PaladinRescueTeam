using Cother;

namespace Origin.Mission
{
    public class HeatInfo
    {
        public Tile Tile { get; }
        public float HeatCount = 0;
        
        public float FuelRemaining = 9;
        public float MaximumIntensity = 3;
        public float FuelBurningRate = 1;
        
        public float FireStartAt = 1;
        public float FireSpreadAt = 2;
        
        public float FireStopAt = 0.5f;
        private bool burning;

        public bool Burning
        {
            get => burning;
            set
            {
                if (!burning && value)
                {
                    SFX.StartFire();
                }
                else if (burning && !value)
                {
                    SFX.StopFire();
                }
                burning = value;
                
            }
        }

        public void ResetFuel()
        {
            HeatMaterial material = GetMaterial(Tile.Illustrations[Tile.Illustrations.Count - 1]);
            FuelRemaining = material.TotalFuel;
            MaximumIntensity = material.MaximumIntensity;
            FuelBurningRate = material.FuelBurningRate;
            FireStartAt = material.FireStartAt;
            FireDissipationRate = material.DissipationRate;
            FireStopAt = FireStartAt / 2;
            FireSpreadAt = (MaximumIntensity - FireStartAt) / 2;
        }

        private HeatMaterial GetMaterial(Illustration illustration)
        {  
            switch (illustration)
            {
                case Illustration.Ash:
                case Illustration.Evac:
                    return new HeatMaterial(1000, 0, 0, 1000, 100);
                case Illustration.Rock:
                case Illustration.Sidewalk:
                case Illustration.BrownBrick:
                case Illustration.BrownDoor:
                    return new HeatMaterial(9, 2, 0.1f, 1.8f, 10);
                case Illustration.Weed:
                case Illustration.AutumnTree:
                case Illustration.Grass:
                case Illustration.Grass2:
                case Illustration.Grass3:
                case Illustration.Bush:
                    return new HeatMaterial(9, 3, 1, 1, 0.7f);
                case Illustration.Cobweb:
                    return new HeatMaterial(3, 1, 3, 0.05f, 0f);
                    //
                default:
                    return new HeatMaterial(9, 3, 1, 1, 1);
            }
        }

        public HeatInfo(Tile tile)
        {
            Tile = tile;
        }

        public void StartHeavyFire()
        {
            HeatCount = 2;
            if (!Burning)
            {
                Tile.Session.TilesOnFire++;
                Burning = true;
            }

        }

        public string DescribeSelf()
        {
            if (HeatCount == 0)
            {
                return "";
            }
            return (Burning ? "B " : "") + HeatCount.ToString("N1") + " / " + FuelRemaining.ToString("N1");
        }

        public float FUEL_BURNING_RATE = 0.1f;
        private float FIRE_SPREADING_RATE = 0.04f;
        private float FireDissipationRate = 1;
        private float FIRE_DISSIPATION_RATE = 0.001f;

        public void Update(float elapsedSeconds)
        {
            elapsedSeconds *= 8;
            if (this.FireDissipationRate > 0.01f && this.FireDissipationRate < 6)
            {
                // Not cobweb or stone
                if (Tile.Session.ChosenDifficulty == Difficulty.Normal)
                {
                    elapsedSeconds /= 3;
                }

                if (Tile.Session.ChosenDifficulty == Difficulty.Easy)
                {
                    elapsedSeconds /= 8;
                }
            }
            
            if (!Burning && HeatCount >= FireStartAt)
            {
                Tile.Session.TilesOnFire++;
                Burning = true;
            }

            if (HeatCount > 0)
            {
                HeatCount -= elapsedSeconds * FireDissipationRate * FIRE_DISSIPATION_RATE;
            }

            if (Burning)
            {
                if (FuelRemaining > 0 && HeatCount < MaximumIntensity)
                {
                    FuelRemaining -= elapsedSeconds * FUEL_BURNING_RATE * FuelBurningRate;
                    HeatCount += elapsedSeconds * FUEL_BURNING_RATE * FuelBurningRate;
                }

                if (FuelRemaining <= 0)
                {
                    this.Tile.Illustrations.RemoveAt(this.Tile.Illustrations.Count - 1);
                    if (this.Tile.Illustrations.Count == 0)
                    {
                        this.Tile.Illustrations.Add(Illustration.Ash);
                        Tile.Session.TilesBurntOut++;
                        HeatCount = 0;
                    }
                    Tile.BlocksMovement = false;
                    Tile.BlocksLineOfSight = false;
                    ResetFuel();
                }

                MaybeStopBurning();

                if (HeatCount >= FireSpreadAt)
                {
                    var spreadCount = elapsedSeconds * FIRE_SPREADING_RATE;
                    HeatCount -= spreadCount;
                    Tile neighbour = Tile.Neighbours.GetRandom();
                    neighbour.Heat.HeatCount += spreadCount;
                }
            }
        }

        public void MaybeStopBurning()
        {
            if (Burning && HeatCount <= FireStopAt)
            {
                Tile.Session.TilesOnFire--;
                Burning = false;
                Tile.Session.CheckVictoryCondition();
            }
        }
    }
}