using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Cother;
using Origin.Characters;
using Origin.Levels;

namespace Origin.Mission
{
    public class TSession
    {
        public string LevelId { get; }
        public Difficulty ChosenDifficulty { get; }
        public Tile[,] Map;
        public int MapWidth;
        public int MapHeight;
        public int TilesOnFire;
        public int TilesBurntOut;
        public List<Character> Characters = new List<Character>();
        public List<Tile> AllTiles = new List<Tile>();

        public TSession(string lsMapFileName, List<CharacterSheet> chosenPaladins, string levelId,
            Difficulty chosenDifficulty)
        {
            LevelId = levelId;
            ChosenDifficulty = chosenDifficulty;
            XDocument xdoc = XDocument.Load(lsMapFileName);
            XElement xMap = xdoc.Root;
            MapWidth = Convert.ToInt32(xMap.Attribute("width").Value);
            MapHeight = Convert.ToInt32(xMap.Attribute("height").Value);
            XElement xTileset = xMap.Element("tileset");
            int firstGid = Convert.ToInt32(xTileset.Attribute("firstgid").Value);
            Dictionary<int, Illustration> tiles = new Dictionary<int, Illustration>();
            foreach (XElement xTile in xTileset.Elements("tile"))
            {
                int id = Convert.ToInt32(xTile.Attribute("id").Value) + firstGid;
                string sourceStr = xTile.Element("image").Attribute("source").Value;
                Illustration illustration = Assets.IllustrationFromPath(sourceStr);
                tiles.Add(id, illustration);
            }

            Map = new Tile[MapWidth, MapHeight];
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    var tile = new Tile(x, y, this);
                    Map[x, y] = tile;
                    AllTiles.Add(tile);
                }
            }

            foreach (XElement xLayer in xMap.Elements("layer"))
            {
                XElement xData = xLayer.Element("data");
                string xCsv = xData.Value.Trim();
                string[] rows = xCsv.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
                for (int y = 0; y < MapHeight; y++)
                {
                    string[] columns = rows[y].Trim().Split(',');
                    for (int x = 0; x < MapWidth; x++)
                    {
                        int what = Convert.ToInt32(columns[x]);
                        if (what != 0)
                        {
                            Map[x, y].AddIllustration(tiles[what], this, chosenPaladins);
                        }
                    }
                }
            }

            foreach (XElement xObjectGroup in xMap.Elements("objectgroup"))
            {
                foreach (XElement xObject in xObjectGroup.Elements("object"))
                {
                    int xPixels = Convert.ToInt32(xObject.Attribute("x").Value);
                    int yPixels = Convert.ToInt32(xObject.Attribute("y").Value);
                    string text = xObject.Element("text").Value;
                    int x = xPixels / 32;
                    int y = yPixels / 32;
                    Map[x, y].AddTag(text);
                    Character anyChar =
                        Characters.FirstOrDefault(ch => (int) ch.Position.X == x && (int) ch.Position.Y == y);
                    if (anyChar != null)
                    {
                        anyChar.AddTag(text);
                    }
                }
            }
            
            CalcNeighbours();
        }

        public Tile EvacuationTile { get; set; }
        public List<WaterParticle> Particles { get; set; } = new List<WaterParticle>();

        private void CalcNeighbours()
        {
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    if (y >= 1)
                    {
                        if (x >= 1)
                            Map[x, y].Neighbours.Add(Map[x - 1, y - 1]);
                        Map[x, y].Neighbours.Add(Map[x, y - 1]);
                        if (x < MapWidth - 1)
                            Map[x, y].Neighbours.Add(Map[x + 1, y - 1]);
                    }

                    if (x >= 1)
                        Map[x, y].Neighbours.Add(Map[x - 1, y]);
                    Map[x, y].Neighbours.Add(Map[x, y]);
                    if (x < MapWidth - 1)
                        Map[x, y].Neighbours.Add(Map[x + 1, y]);

                    if (y < MapHeight - 1)
                    {
                        if (x >= 1)
                            Map[x, y].Neighbours.Add(Map[x - 1, y + 1]);
                        Map[x, y].Neighbours.Add(Map[x, y + 1]);
                        if (x < MapWidth - 1)
                            Map[x, y].Neighbours.Add(Map[x + 1, y + 1]);
                    }
                }
            }
            Characters.Sort((ch1, ch2) =>
            {
                if (ch1.IsNPC && !ch2.IsNPC)
                {
                    return -1;
                }
                else if (!ch1.IsNPC && ch2.IsNPC)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            });
        }

        private int longRunningDiff = 10;

        private float BLEEDING_RATE = 1f / 50f;
        public void Update(float elapsedSeconds)
        {
            foreach (var tile in AllTiles)
            {
                tile.HeatUpdate(elapsedSeconds);
            }
            for (var ci = 0; ci < Characters.Count; ci++)
            {
                var character = Characters[ci];
                if (character.MovementProgress != null)
                {
                    character.ProgressMove(elapsedSeconds);
                }

                if (character.Occupies.Heat.HeatCount >= 0.5f)
                {
                    float damagePerSecond = character.Occupies.Heat.HeatCount >= 2
                        ? 1
                        : (character.Occupies.Heat.HeatCount >= 1 ? 0.5f : 0.25f);
                    damagePerSecond *= 0.2f;
                    character.TakeDamage(damagePerSecond * elapsedSeconds);
                }

                if (character.Wounded)
                {
                    float difficultyModifier = 1;
                    if (ChosenDifficulty == Difficulty.Easy)
                    {
                        difficultyModifier = 0;
                    }

                    if (ChosenDifficulty == Difficulty.Normal)
                    {
                        difficultyModifier = 0.2f;
                    }
                    character.TakeDamage(elapsedSeconds * BLEEDING_RATE * difficultyModifier);
                }
                character.ImmediateActivity?.MakeProgress(elapsedSeconds);
                if (character.Occupies == EvacuationTile && character.IsNPC)
                {
                    Characters.RemoveAt(ci);
                    CheckVictoryCondition();
                    ci--;
                }
            }

            longRunningDiff++;
            if (longRunningDiff >= 20)
            {
                longRunningDiff = 0;
                RecalcFogOFWar();
                foreach (var character in Characters)
                {
                    if (character.Hostile && !character.Dead)
                    {
                        foreach (var otherCharacter in Characters)
                        {
                            if (otherCharacter.IsLivingPaladin)
                            {
                                if (character.DistanceTo(otherCharacter) <= 6 && character.Idle)
                                {
                                    character.GainNewGoal(new AttackActivity(character, otherCharacter));
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool? Victory;
        public string VictoryText;
        public EndReason VictoryEndReason;

        public void CheckVictoryCondition()
        {
            bool noInjured = Characters.All(ch => !ch.Wounded || ch.Dead);
            bool noFires = TilesOnFire == 0;
            bool noEnemies = Characters.All(ch => !ch.Hostile || ch.Dead);
            if (noInjured && noFires && noEnemies)
            {
                Victory = true;
                VictoryText =
                    "All injured civilians are evacuated, all fires are extinguished and all hostiles are taken care of!\n\nWell done!\n\n";
                if (LevelId == MonasteryTags.MonasteryLevelId)
                {
                    if (Characters.Count(ch => ch.Dead && !ch.Hostile) == 0)
                    {
                        VictoryText +=
                            "You passed the final challenge with the unexpected exploding trap without loss of life. That is still commendable. You are now ready to take on an actual emergency, where your decisions save lives, or end them.";
                    }
                    else
                    {

                        VictoryText +=
                            "You passed the challenge, but life was lost along the way. Perhaps you were not fast enough or overextended when fighting ghouls or fires.\n\nEither way, the loss of life is on your conscience. Had you made a better decision, decades of life of innocent people would not have been cut away.\n\nLearn from this and do better in an actual emergency.";
                    }
                }
                else
                {
                    int paladinsLost = Characters.Count(ch => !ch.IsNPC && ch.Dead);
                    int civiliansLost = Characters.Count(ch => ch.Dead && ch.IsNPC && !ch.Hostile);
                    if (paladinsLost == 0 && civiliansLost == 0)
                    {
                        VictoryText +=
                            "{b}Congratulations!{/b} This is the best you could hope for! No deaths among the paladins, and no deaths among civilians either! It may seem just like 'doing your job' to you, but every life saved means a family kept intact or a paladin still working in defense of the realm. Too often in our job, we must make a decision on who to save and who     to let die. You proved that it does not always have to be that way.";
                    }
                    else if (paladinsLost == 0)
                    {
                        VictoryText +=
                            "You chose to proceed cautiously and not let your people die. That is a reasonable decision and earns you the respect of your paladins. However, standing by while horror happens to civilians is not what the Paladin Rescue Team is about. If you made a better decision somewhere along the way, you wonder, could you have saved more?";

                    }
                    else if (civiliansLost == 0)
                    {  
                        VictoryText +=
                            "You made the decision to sacrifice paladins to save civilians. Of course, such a decision is never easy but you likely made the right choice. After all, the paladin did sign up for this and will receive a well-deserved reward in the Seven Heavens. You wonder, though, how their family will feel about your decision...";
                    }
                    else
                    {
                        VictoryText +=
                            "The mission was a success, technically, but your decisions could have been better. They led to the death of both a paladin and a civilian, both of which you are supposed to protect. Yes, in an emergency, time flies fast and it is difficult to think. But we must do so nonetheless. That is, after all, a decision we made a long time ago when we joined the Team.";
                    }

                    if (ChosenDifficulty != Difficulty.Meaningful)
                    {
                        VictoryText +=
                            "\n\nThat said, you're not playing at the {b}meaningful difficulty level{/b}. All difficulty levels except 'meaningful' do not accurately represent reality. It is possible that your strategy would not have worked without the crutch of easy difficulty.";
                    }
                }

                return;
            }
            bool noPaladins = Characters.All(ch => ch.IsNPC || ch.Dead);
            if (noPaladins)
            {
                Victory = false;
                VictoryEndReason = EndReason.PaladinDeath;
                return;
            }

            if (ChosenDifficulty != Difficulty.Easy)
            {
                int numDead = Characters.Count(cnt => !cnt.Hostile && cnt.Dead);
                if (numDead >= 3)
                {
                    Victory = false;
                    VictoryEndReason = EndReason.CivilianDeath;
                }

                int fires = TilesOnFire + TilesBurntOut;
                if (fires >= 300)
                {
                    Victory = false;
                    VictoryEndReason = EndReason.FireOutOfControl;
                }
            }
        }

        private void RecalcFogOFWar()
        {
            foreach (var character in Characters)
            {
                if (!character.IsNPC)
                {
                    FloodFillToReveal(character.Occupies, 22);
                }
            }
        }

        private void FloodFillToReveal(Tile startFrom, int sightRemaining)
        {
            Pathfinding.FloodFillToReveal(startFrom, sightRemaining);
        }
    }

    public class AttackActivity : ImmediateActivity
    {
        private float DAMAGE = 1;
        public Character Defender { get; }

        public AttackActivity(Character attacker, Character defender) : base(attacker, 3, "Fighting" )
        {
            Defender = defender;
        }

        public override Tile Tile => Defender.Occupies;
        public override void Commence()
        {
            Actor.Occupies.Speak(Actor.GetAttackCommencement());
            if (Defender.Idle)
            {
                Defender.GainNewGoal(new AttackActivity(Defender, Actor));
            }
        }

        public override void Complete()
        {
            if (!Defender.IsNPC)
            {
                SFX.Play(R.Coin() ? SFX.Sword : SFX.Sword2);
            }

            float attackDamage = DAMAGE;
            if (Actor.Powers.Any(power => power.Id == PowerName.StrongBody))
            {
                attackDamage *= 2;
            }
            if (Defender.Powers.Any(power => power.Id == PowerName.StrongBody))
            {
                attackDamage /= 2;
            }

            Defender.TakeDamage(attackDamage);
            if (Defender.HP > 0)
            {
                Actor.GainNewGoal(new AttackActivity(Actor, Defender));
            }
        }

        public Interaction ToInteraction()
        {
            return new ActivityInteraction("Engage " + Defender.Name + " in combat", "Fight to the death with the creature", this);
        }
    }
}