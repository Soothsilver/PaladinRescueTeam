using System.Linq;
using Humanizer;
using Microsoft.Win32;
using Origin.Levels;

namespace Origin.Mission
{
    public class UnlockActivity: ImmediateActivity
    {
        public override Tile Tile { get; }
        

        public UnlockActivity(Character actor, Tile tile) : base(actor, 8, "Opening")
        {
            Tile = tile;
        }

        public override void Commence()
        {
            Actor.Occupies.Speak("**click,click**");
        }

        public override void Complete()
        {
            SFX.Play(SFX.UnlockDoor);
            if (Tile.Trap != TrapId.NotTrapped)
            {
                Tile.TrapGoesOff();
            }
            else if (Tile.HasTag("MonTrappedDoor"))
            {
                Actor.Occupies.Speak("unlocked");
                Tile.Illustrations.Add(Illustration.Sidewalk);
                Tile.ResetFuel();
                Tile.BlocksMovement = false;
                Tile.BlocksLineOfSight = false;
                var ftile = Actor.Session.AllTiles.FirstOrDefault(tl => tl.HasTag(MonasteryTags.FireballTrapHere));
                ftile?.TrapGoesOff();
            }
            else if (Tile.Illustrations.Remove(Illustration.BrownDoor))
            {
                Actor.Occupies.Speak("unlocked");
                Tile.Illustrations.Add(Illustration.Sidewalk);
                Tile.ResetFuel();
                Tile.BlocksMovement = false;
                Tile.BlocksLineOfSight = false;
            }
            else if (Tile.HasTag(HauntedHouseTags.ScrollChest))
            {
                Tile.Tags.Remove(HauntedHouseTags.ScrollChest);
                Actor.Occupies.Speak("There was a scroll of Disarm Trap. I can maybe use that somewhere...");
                Actor.HeldItems.Add(new HeldItem(HeldItemId.ScrollDisarmTrap, "Scroll of Disarm Trap"));
            }
            else if (Tile.HasTag(HauntedHouseTags.MechantChest))
            {
                Tile.Tags.Remove(HauntedHouseTags.MechantChest);
                Actor.Occupies.Speak("There was a scroll of Spark Fire. Maybe for cobwebs?");
                Actor.HeldItems.Add(new HeldItem(HeldItemId.ScrollSparkFire, "Scroll of Spark Fire"));
            }
            else if (Tile.HasTag(MonasteryTags.ScrF))
            {
                Tile.Tags.Remove(MonasteryTags.ScrF);
                Actor.HeldItems.Add(new HeldItem(HeldItemId.ScrollSparkFire, "Scroll of Spark Fire"));
                Actor.Occupies.Speak("There was a scroll of Spark Fire. I can use it to set fire to the cobwebs.");
            }
            else
            {
                Actor.Occupies.Speak("It's empty...");
            }
        }

        public Interaction ToInteraction()
        {
            if (Tile.Is(Illustration.BrownDoor))
            {
                return new ActivityInteraction("Unlock this door",
                    "Break the lock on this door so that you can see inside and enter", this);
            }
            else
            {
                return new ActivityInteraction("Unlock/open chest",
                    "Unlock this chest (or open it, if it's unlocked) and grab the contents", this);
            }
        }
    }

    public enum TrapId
    {
        NotTrapped,
        Fireball,
        EmpoweredFireball
    }

    public enum HeldItemId
    {
        ScrollDisarmTrap,
        ScrollSparkFire
    }
}