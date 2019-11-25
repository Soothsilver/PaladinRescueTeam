using System;
using System.Collections.Generic;
using System.Linq;
using Cother;
using Microsoft.Xna.Framework;
using Origin.Characters;
using Origin.Levels;

namespace Origin.Mission
{
    public class Character
    {
        public string Name;
        public string NPCDescription;
        public bool IsNPC;
        public Illustration Illustration;
        public Vector2 Position;
        public readonly TSession Session;
        public float HP;
        public int MaxHP;
        public MovementProgress MovementProgress;
        public Interaction Goal;
        public ImmediateActivity ImmediateActivity { get; set; }
        public bool Wounded;
        public bool Dead => HP <= 0;

        public Character(string name, bool isNpc, Illustration illustration, Vector2 position, TSession session)
        {
            Name = name;
            NPCDescription = "Civilian";
            IsNPC = isNpc;
            HP = 10;
            MaxHP = 10;
            Illustration = illustration;
            Position = position;
            this.Session = session;
        }

        private CharacterSheet sheet;

        public CharacterSheet Sheet
        {
            get => sheet;
            set
            {
                sheet = value;
                Powers = value.Powers.Select(PowerDb.GetPower).ToList();
            }
        }
        public Tile Occupies => Session.Map[(int) Position.X, (int) Position.Y];
        public bool IsLivingPaladin => HP > 0 && !this.IsNPC;
        public List<string> Tags { get; set; } = new List<string>();
        public List<HeldItem> HeldItems { get; set; } = new List<HeldItem>();
        public bool Hostile { get; set; }
        public bool Idle => MovementProgress == null && ImmediateActivity == null;

        public List<Power> Powers = new List<Power>();

        public string DescribeSelf()
        {
            if (Sheet != null)
            {
                return Sheet.DescribeSelf() + "\n" +
                       (Wounded ? "(Wounded and bleeding!)" : "") +
                       string.Join("\n", Powers.Select(p => "Has power:  {b}" + p.Name + "{/b}"));
            }
            else
            {
                return "{b}" + Name + "{/b}\n" + NPCDescription;
            }
        }

        public IEnumerable<Interaction> GetInteractionsBy(Character actor)
        {
            if (this.IsNPC)
            {
                if (!this.Hostile && this.HP > 0)
                {
                    if (this.HP >= 5 && (this.HP < this.MaxHP || this.Wounded))
                    {
                        yield return new EvacuateActivity(actor, this).ToInteraction();
                    }

                    if (this.HP < this.MaxHP)
                    {
                        yield return new TendWoundsActivity(actor, this).ToInteraction();
                    }

                    yield return new TalkActivity(actor, this).ToInteraction();
                }

                if (this.Hostile && this.HP > 0)
                {
                    yield return new AttackActivity(actor, this).ToInteraction();
                }
            }
        }

        public void ProgressMove(float elapsedSeconds)
        {

            Tile movedTo = null;
            if (MovementProgress.CurrentlyMovingTo != null)
            {
                this.Position += MovementProgress.CurrentSpeed * elapsedSeconds;
                this.MovementProgress.TimeRemaining -= elapsedSeconds;
                if (this.MovementProgress.TimeRemaining <= 0)
                {
                    movedTo = MovementProgress.CurrentlyMovingTo;
                    MovementProgress.CurrentlyMovingTo = null;
                }
            }
            if (MovementProgress.CurrentlyMovingTo == null)
            {
                ImmediateActivity theGoal = MovementProgress.TheGoal;
                if (theGoal != null && theGoal.WithinRange())
                {
                    MovementProgress = null;
                    if (movedTo != null)
                    {
                        this.Position = new Vector2(movedTo.X, movedTo.Y);
                    }

                    this.ImmediateActivity = theGoal;
                    theGoal.Commence();
                }
                else if (MovementProgress.Path.Count > 0)
                {
                    var firstNode = MovementProgress.Path.First;
                    MovementProgress.Path.RemoveFirst();
                    float TILES_PER_SECOND = 2;
                    float timeToGetThere = 1f / TILES_PER_SECOND;
                    this.MovementProgress.TimeRemaining = timeToGetThere;
                    this.MovementProgress.CurrentlyMovingTo = firstNode.Value;
                    float xSpeed = (firstNode.Value.X - this.Position.X) / timeToGetThere;
                    float ySpeed = (firstNode.Value.Y - this.Position.Y) / timeToGetThere;
                    this.MovementProgress.CurrentSpeed = new Vector2(xSpeed, ySpeed);
                }
                else
                {
                    this.Position = new Vector2(MovementProgress.UltimateTarget.X, MovementProgress.UltimateTarget.Y);
                    this.MovementProgress = null;
                }
            }
        }

        public float DistanceTo(Character target)
        {
            return Math.Max(Math.Abs(target.Position.X - this.Position.X),
                Math.Abs(target.Position.Y - this.Position.Y));

        }
        public float DistanceTo(Tile targetTile)
        {
            return Math.Max(Math.Abs(targetTile.X - this.Position.X),
                Math.Abs(targetTile.Y - this.Position.Y));
        }

        public string GetConversationTopic()
        {
            if (this.HasTag(HauntedHouseTags.Civ1))
            {
                return "Thank the gods you're here. You must hurry, my children are trapped inside this house with a {b}ghoul{/b}!";
            }
            else if (this.HasTag(HauntedHouseTags.Civ2))
            {
                return "I wouldn't advise going through the front door. It's booby-trapped.";
            }
            else if (this.HasTag(HauntedHouseTags.Civ4))
            {
                return "I don't like this door. But we don't have much time!";
            }
            else if (this.HasTag(HauntedHouseTags.Mechant))
            {
                this.Tags.Remove(HauntedHouseTags.Mechant);
                // TODO unlock the chest
                this.Tags.Add(HauntedHouseTags.MerchantSpokenTo);
                Tile chestTile = this.Session.AllTiles.FirstOrDefault(tl => tl.HasTag(HauntedHouseTags.MechantChest));
                if (chestTile != null)
                {
                    this.GainNewGoal(new CastSpellActivity(SpellName.DisarmTraps, this, chestTile));
                    return
                        "I'll help you! Wait while I disarm the trap on my chest, then take what's inside. I use it for cobweb clearing.";
                }
                else
                {
                    return "Get out, burglars!";
                }
            }
            else if (this.HasTag(HauntedHouseTags.MerchantSpokenTo))
            {
                return "Use the scroll in my chest to burn through the cobwebs.";
            }
            else if (this.HasTag(HauntedHouseTags.BackdoorPerson))
            {
                return "This path is safe, I think, ...but there are a lot of cobwebs.";
            }
            else if (this.HasTag(HauntedHouseTags.IntroMan))
            {
                return "The haunted house is just up ahead, to your left. Hurry! The children may be dying...";
            }
            else if (this.HasTag(HauntedHouseTags.Inj1) || this.HasTag(HauntedHouseTags.Inj2) ||
                     this.HasTag(HauntedHouseTags.Inj3))
            {
                return "Please get me to the evacuation point, mister! Quickly!!";
            }
            else
            {
                return GetFromDictionary(this.Tags);
            }
        }

        private string GetFromDictionary(List<string> tags)
        {
            foreach (var tag in tags)
            {
                string dd = GetFromDictionary(tag);
                if (dd != null)
                {
                    return dd;
                }
            }

            return "Please help us!";
        }

        private string GetFromDictionary(string tag)
        {
            switch (tag)
            {
                case MonasteryTags.M1:
                    return "This is our monastery, Commander. Start by unlocking the door to the next chamber.";
                case MonasteryTags.M2:
                    return "That civilian is injured! Heal them, then when they have 50% HP, tell them to evacuate to the evac point!";
                case MonasteryTags.M3:
                    if (this.HP < 5)
                    {
                        return "Please heal me to at least 50%!";
                    }
                    else
                    {
                        return "Tell me to evacuate or I'll {b}die{/b} here!";
                    }
                case MonasteryTags.M4:
                    return "When you open the door, the {b}ghoul{/b} will attack you. Your class is {b}warrior{/b} so you should be able to defeat him!";
                case MonasteryTags.M5:
                    return "Clear these giant cobwebs blocking your path!";
                case MonasteryTags.M6:
                    return
                        "These chests contain scrolls of Spark Fire. Take them, then use them on the cobwebs. The fire will clear the cobwebs out for you!";
                case MonasteryTags.M7:
                    Session.Characters.Where(ch => ch.IsNPC && ch.HasTag(MonasteryTags.Conv)).ToList().ForEach(ch =>
                    {
                             ch.Name = "Conscripted Blue Mage";
                            ch.IsNPC = false;
                            ch.Sheet = CharacterSheet.CreateBlueMage();
                            ch.Illustration = Illustration.WaterPaladin;
                    });
                    return
                        "These {b}blue mages{/b} are now under your command. They extinguish fires. Your last challenge is behind that door... extinguish all fires!";
                case "ExampleFire":
                    return "This is an example fire. You must extinguish it as well.";
                case MonasteryTags.Conv:
                    return "Talk to our leader to take command of us blue mages.";
                default:
                    return null;
            }
        }


        public bool HasTag(string tag)
        {
            return this.Tags.Contains(tag);
        }

        public void GainNewGoal(ImmediateActivity activity)
        {
            if (this.Dead)
            {
                return;
            }
            if (activity.WithinRange())
            {
                activity.Actor.ImmediateActivity = activity;
                activity.Commence();
            }
            else
            {
                LinkedList<Tile> path = Pathfinding.AStar(activity.Actor, activity.Tile, activity.Actor.Session, PathfindingMode.FindClosestIfDirectIsImpossible);
                if (path != null)
                {
                    activity.Actor.MovementProgress = new MovementProgress(path, activity);
                }
            }
        }

        public void MoveToTileIfPossible(Tile target)
        {
            LinkedList<Tile> path = Pathfinding.AStar(this, target, this.Session, PathfindingMode.FindClosestIfDirectIsImpossible);
            if (path != null)
            {
                this.MovementProgress = new MovementProgress(path);
            }
        }

        public void AddTag(string text)
        {
            this.Tags.Add(text);
            if (text == HauntedHouseTags.Inj1 || text == HauntedHouseTags.Inj2 || text == HauntedHouseTags.Inj3)
            {
                if (text == HauntedHouseTags.Inj1)
                {
                    this.HP = 8;
                }
                if (text == HauntedHouseTags.Inj2)
                {
                    this.HP = 5;
                }
                if (text == HauntedHouseTags.Inj3)
                {
                    this.HP = 3;
                }
                this.Wounded = true;
            }

            if (text == MonasteryTags.M3)
            {
                this.Wounded = true;
                this.HP = 4.7f;
            }
        }

        public string GetAttackCommencement()
        {
            if (this.IsNPC)
            {
                if (R.Coin())
                {
                    return "Grr";
                }
                else
                {
                    return "Harr";
                }
            }
            else
            {
                if (R.Coin())
                {
                    return "Attack!";
                }
                else
                {
                    return "(huff)";
                }
            }
        }

        public void TakeDamage(float attackDamage)
        {
            bool wasDead = this.Dead;
            this.HP -= attackDamage;
            if (this.HP <= 0 && !wasDead)
            {
                this.HP = 0;
                this.Wounded = false;
                if (this.HasTag(HauntedHouseTags.TrappedGhoul))
                {
                    Tile bombTile = Session.AllTiles.FirstOrDefault(tt => tt.HasTag(HauntedHouseTags.Bomb));
                    if (bombTile != null)
                    {
                        bombTile.TrapGoesOff();
                    }
                }
                Session.CheckVictoryCondition();
            }
        }
    }

    public enum SpellName
    {
        DisarmTraps,
        CastWater,
        SparkFire
    }
}