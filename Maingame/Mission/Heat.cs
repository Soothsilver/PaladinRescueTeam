namespace Origin.Mission
{
    public class HeatThings
    {
        public static float HeavyFire = 5;

        public static float GetFuel(Illustration illustration)
        {
            switch (illustration)
            {
                case Illustration.Ash:
                case Illustration.Evac:
                    return 1000;
                case Illustration.Rock:
                case Illustration.Sidewalk:
                case Illustration.BrownBrick:
                case Illustration.BrownDoor:
                    return 25;
                case Illustration.Weed:
                case Illustration.AutumnTree:
                case Illustration.Grass:
                case Illustration.Grass2:
                case Illustration.Grass3:
                case Illustration.Bush:
                    return 20;
                case Illustration.Cobweb:
                    return 2;
                default:
                    return 5;
            }
        }

        public static float GetFuelConsumptionPerSecond(Illustration illustration)
        {
            switch (illustration)
            {
                case Illustration.Ash:
                case Illustration.Evac:
                    return 0;
                case Illustration.Rock:
                case Illustration.Sidewalk:
                case Illustration.BrownBrick:
                case Illustration.BrownDoor:
                    return 0.5f;
                case Illustration.Weed:
                case Illustration.AutumnTree:
                case Illustration.Grass:
                case Illustration.Grass2:
                case Illustration.Grass3:
                case Illustration.Bush:
                    return 3;
                case Illustration.Cobweb:
                    return 10;
                default:
                    return 2;
            }
        }
    }
}