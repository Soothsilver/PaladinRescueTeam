namespace Origin.Characters
{
    public enum CharacterClass
    {
        Warrior,
        BlueWizard
    }

    public static class CharacterClassExtensions
    {
        public static string Humanize(this CharacterClass klass)
        {
            switch (klass)
            {
                case CharacterClass.Warrior: return "Warrior";
                case CharacterClass.BlueWizard: return "Blue Wizard";
                default: return klass.ToString();
            }
        }
    }
}