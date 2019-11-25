using System.Collections.Generic;
using System.Linq;
using Auxiliary;
using Cother;
using Humanizer;
using Microsoft.Xna.Framework;
using Origin.Characters;
using Origin.Display;
using Origin.Levels;
using Origin.Mission.Interactions;
using Priority_Queue;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Origin.Mission
{
    public class Tile : PathingVertex
    {
        public TSession Session;
        public int X { get; }
        public int Y { get; }
        public List<Tile> Neighbours { get; set; } = new List<Tile>();
        public List<string> Tags { get; set; } = new List<string>();
        public List<Overhead> Overheads = new List<Overhead>();

        public List<Illustration> Illustrations = new List<Illustration>();
        public bool BlocksMovement;
        public bool BlocksLineOfSight;
        public bool Blackened = true;
        public int LastClosedBySearch = -1;
        public TrapId Trap { get; set; }
        public Tile(int x, int y, TSession session)
        {
            Session = session;
            X = x;
            Y = y;
            Heat = new HeatInfo(this);
        }

        public void AddIllustration(Illustration illus, TSession session, List<CharacterSheet> remainingPaladins)
        {
            switch (illus)
            {
                case Illustration.Grass:
                    int r = R.Next(3);
                    switch (r)
                    {
                        case 0: illus = Illustration.Grass2;
                            break;
                        case 1:
                            illus = Illustration.Grass3;
                            break;
                    }
                    break;
                case Illustration.Chest:
                case Illustration.Chest2:
                case Illustration.Cobweb:
                case Illustration.Grave:
                case Illustration.Grave2:
                case Illustration.WoodenGrave:
                case Illustration.Rock:
                case Illustration.AutumnTree: 
                case Illustration.Bush:
                case Illustration.Bag:
                    BlocksMovement = true;
                    break;
                case Illustration.BrownBrick:
                case Illustration.BrownDoor:
                    BlocksLineOfSight = true;
                    BlocksMovement = true;
                    break;
                case Illustration.BlueCivilian:
                case Illustration.GreenCivilian:
                case Illustration.PurpleCivilian:
                    session.Characters.Add(new Character("Civilian " + RandomNameGenerator.Generate(), true, illus, new Vector2(this.X, this.Y), session));
                    illus = Illustration.None;
                    break;
                case Illustration.Ghoul:
                    session.Characters.Add(new Character("Undead Ghoul", true, illus, new Vector2(this.X, this.Y), session)
                    {
                        Hostile = true,
                        NPCDescription = "Undead abomination",
                        HP = 5,
                        MaxHP = 5
                    });
                    illus = Illustration.None;
                    break;
                case Illustration.WarriorPaladin:
                    illus = Illustration.None;
                    SpawnPaladinHereIfAble(remainingPaladins, session);
                    break;
                case Illustration.Evac:
                    session.EvacuationTile = this;
                    break;
                case Illustration.Fire32:
                    illus = Illustration.None;
                    this.Heat.StartHeavyFire();
                    break;
            }

            if (illus != Illustration.None)
            {
                this.Illustrations.Add(illus);
                ResetFuel();
            }
        }

        private void SpawnPaladinHereIfAble(List<CharacterSheet> remainingPaladins, TSession session)
        {
            if (remainingPaladins.Count > 0)
            {
                var paladin = remainingPaladins[0];
                remainingPaladins.RemoveAt(0);
                session.Characters.Add(new Character(paladin.Name, false, paladin.Class == CharacterClass.Warrior ? Illustration.WarriorPaladin : Illustration.WaterPaladin,
                    new Vector2(this.X, this.Y), session)
                {
                    Sheet = paladin
                });
            }
        }

        public IEnumerable<Interaction> GetInteractionsBy(Character actor)
        {
            if (Is(Illustration.BrownDoor) || Is(Illustration.Chest) || Is(Illustration.Chest2))
            {
                yield return new UnlockActivity(actor, this).ToInteraction();
            }

            if (this.Heat.Burning || this.Heat.HeatCount >= 0.5f)
            {
                if (actor.Powers.Any(pow => pow.Id == PowerName.CastWater))
                {
                    yield return new CastWaterActivity(actor, this).ToInteraction();
                }
                else
                {
                    yield return new DouseActivity(actor, this).ToInteraction();
                }
            }
            
            if (Is(Illustration.Cobweb))
            {
                yield return new CleanCobwebActivity(actor, this).ToInteraction();
            }

            if (actor.HeldItems.Any(hi => hi.ItemId == HeldItemId.ScrollSparkFire) && this.BlocksMovement)
            {
                yield return new UseScrollActivity(actor,
                    actor.HeldItems.First(hi => hi.ItemId == HeldItemId.ScrollSparkFire), this).ToInteraction();
            }

            if (Trap != TrapId.NotTrapped && actor.HeldItems.Any(hi => hi.ItemId == HeldItemId.ScrollDisarmTrap))
            {
                yield return new UseScrollActivity(actor,
                    actor.HeldItems.First(hi => hi.ItemId == HeldItemId.ScrollDisarmTrap), this).ToInteraction();
            }
            yield return new MoveInteraction(actor, this);
        }

        public bool Is(Illustration illustration)
        {
            return Illustrations.Contains(illustration);
        }

        public void Speak(string hello)
        {
            var overhead = new Overhead(hello);
            Overheads.Add(overhead);
        }

        public bool HasTag(string tag)
        {
            return Tags.Contains(tag);
        }

        public void AddTag(string text)
        {
            this.Tags.Add(text);
            if (text == HauntedHouseTags.Bomb)
            {
                this.Trap = TrapId.EmpoweredFireball;
            }
            else if (text == MonasteryTags.FireballTrapHere)
            {
                this.Trap = TrapId.Fireball;
            }
            else if (text == HauntedHouseTags.MainDoor)
            {
                this.Trap = TrapId.Fireball;
            }
            else if (text == HauntedHouseTags.MechantChest)
            {
                this.Trap = TrapId.Fireball;
            }
            else if (text == MonasteryTags.InfiniFire)
            {
                this.Heat.FuelRemaining = float.MaxValue;
            }
        }


        public void Draw(Rectangle real)
        {
            foreach (var illustration in Illustrations)
            {
                Primitives.DrawImage(Assets.TextureFromCard(illustration), real);
                if (Treasure.Instance.ShowFireMode)
                {
                    Writer.DrawNumberInRectangle(Heat.DescribeSelf(), real.Extend(-4, -4));
                }
                else
                {

                    if (Heat.Burning)
                    {
                        Illustration fireIllus = Illustration.Fire16;
                        float diff = Heat.MaximumIntensity - Heat.FireStartAt;
                        if (Heat.HeatCount >= Heat.FireStartAt + diff / 4)
                        {
                            fireIllus = Illustration.Fire20;
                        }

                        if (Heat.HeatCount >= Heat.FireStartAt + diff * 2 / 4)
                        {
                            fireIllus = Illustration.Fire24;
                        }

                        if (Heat.HeatCount >= Heat.FireStartAt + diff * 3 / 4)
                        {
                            fireIllus = Illustration.Fire32;
                        }

                        Primitives.DrawImage(Assets.TextureFromCard(fireIllus), real);
                    }
                    else if (Heat.HeatCount >= 0.5f)
                    {
                        Primitives.DrawImage(Assets.TextureFromCard(Illustration.Fire16), real);
                    }
                }
            }
        }

        public HeatInfo Heat;

        public void ResetFuel()
        {
            Heat.ResetFuel();
        }

        public void HeatUpdate(float elapsedSeconds)
        {
            Heat.Update(elapsedSeconds);
        }

        public void Fireball(TSession session)
        {
            Heat.StartHeavyFire();
            foreach (var chara in session.Characters)
            {
                if (chara.Occupies == this)
                {
                    chara.TakeDamage(10);
                }
            }
        }

        public void TrapGoesOff()
        {
            this.Speak("Trap: " + (this.Trap.Humanize(LetterCasing.Title)) + "!");
            TSession session = this.Session;
            if (this.Trap == TrapId.Fireball)
            {
                this.Fireball(session);
                foreach (var neighbour in this.Neighbours)
                {
                    neighbour.Fireball(session);
                }
            }
            else if (this.Trap == TrapId.EmpoweredFireball)
            {
                this.Fireball(session);
                foreach (var neighbour in this.Neighbours)
                {
                    neighbour.Fireball(session);
                    foreach (var neighbour2 in neighbour.Neighbours)
                    {
                        neighbour2.Fireball(session);
                    }
                }
            }

            this.Trap = TrapId.NotTrapped;
        }
    }

    public class DouseActivity : ImmediateActivity
    {
        public DouseActivity(Character actor, Tile tile) : base(actor, 0.1f, "Dousing fire")
        {
            this.Tile = tile;
        }

        public override Tile Tile { get; }

        public override bool WithinRange()
        {
            return Actor.DistanceTo(Tile) <= 3;
        }

        public override void Commence()
        {
            base.Commence();
            Actor.Session.Particles.Add(new WaterParticle(Actor.Occupies, Tile));
            SFX.StartWater();
        }

        public override void Abort()
        {
            SFX.StopWater();
        }
        public override void Complete()
        {
            SFX.StopWater();
            Tile.Heat.HeatCount -= (Tile.Heat.HeatCount < 0 ? 1 : 0.2f);
            Tile.Heat.MaybeStopBurning();
            if (Tile.Heat.HeatCount >= -7)
            {
                Actor.GainNewGoal(new DouseActivity(Actor, Tile));
            }
            else
            {
                List<Tile> neighbours = Tile.Neighbours.Where(nei => nei.Heat.Burning || nei.Heat.HeatCount >= 0.5f).ToList();
                if (neighbours.Count > 0)
                {
                    Tile target = neighbours.GetRandom();
                    Actor.GainNewGoal(new DouseActivity(Actor, target));
                }
            }
        }

        public ActivityInteraction ToInteraction()
        {
            return new ActivityInteraction("Douse fire", "Fight the fire using close-range water spells", this);
        }
    }
    public class CastWaterActivity : ImmediateActivity
    {
        public CastWaterActivity(Character actor, Tile tile) : base(actor, 0.1f, "Casting water")
        {
            this.Tile = tile;
        }

        public override Tile Tile { get; }

        public override bool WithinRange()
        {
            return Actor.DistanceTo(Tile) <= 8;
        }

        public override void Commence()
        {
            base.Commence();
            Actor.Session.Particles.Add(new WaterParticle(Actor.Occupies, Tile));;
            SFX.StartWater();
        }

        public override void Abort()
        {
            SFX.StopWater();
        }

        public override void Complete()
        {
            SFX.StopWater();
            Tile.Heat.HeatCount -= (Tile.Heat.HeatCount < 0 ? 2 : 0.5f);
            Tile.Heat.MaybeStopBurning();
            if (Tile.Heat.HeatCount >= -7)
            {
                Actor.GainNewGoal(new CastWaterActivity(Actor, Tile));
            }
            else
            {
                List<Tile> neighbours = Tile.Neighbours.Where(nei => nei.Heat.Burning|| nei.Heat.HeatCount >= 0.5f).ToList();
                if (neighbours.Count > 0)
                {
                    Tile target = neighbours.GetRandom();
                    Actor.GainNewGoal(new CastWaterActivity(Actor, target));
                }
            }
        }

        public ActivityInteraction ToInteraction()
        {
            return new ActivityInteraction("Cast water", "Fight the fire using a long-range water spell", this);
        }
    }

    public class WaterParticle
    {
        public Vector2 Position;
        public WaterParticle(Tile source, Tile target)
        {
            Position = new Vector2(source.X + 0.5f, source.Y + 0.5f);
            TimeLeft = 2;
            float rx = R.NextFloat();
            float ry = R.NextFloat();
            Vector2 randomTarget = new Vector2(target.X + rx, target.Y + ry);
            Vector2 distance = (randomTarget - Position);
            Speed = distance / TimeLeft;
        }

        public float TimeLeft { get; set; }
        public Vector2 Speed { get; set; }
    }

    public class CleanCobwebActivity : ImmediateActivity
    {
        public CleanCobwebActivity(Character actor, Tile tile) 
        : base (actor, 7, "Cleaning cobwebs")
        {
            Tile = tile;
        }

        public override Tile Tile { get; }

        public override void Complete()
        {
            Tile.Illustrations.Remove(Illustration.Cobweb);
            Tile.ResetFuel();
            Tile.BlocksMovement = false;
            foreach (var nei in Tile.Neighbours)
            {
                if (nei.Is(Illustration.Cobweb))
                {
                    Actor.GainNewGoal(new CleanCobwebActivity(Actor, nei));
                    return;
                }
            }
            
        }

        public ActivityInteraction ToInteraction()
        {
            return new ActivityInteraction("Clean cobwebs", "Remove the giant cobwebs from blocking your path", this);
        }
    }


    public class PathingVertex : FastPriorityQueueNode
    {

        public int Pathfinding_EncounteredDuringSearch;
        public bool Pathfinding_Closed;
        public int Pathfinding_F;
        public int Pathfinding_G;
        public int Pathfinding_IsTargetDuringThisSearch;
        public Vector2 Pathfinding_TargetPreciseLocation;
        public Tile Pathfinding_Parent;
    }
    
}